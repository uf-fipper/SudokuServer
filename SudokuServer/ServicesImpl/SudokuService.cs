using Microsoft.EntityFrameworkCore;
using Sudoku;
using Sudoku.Default;
using SudokuServer.Models.DatabaseModels.Context;
using SudokuServer.Models.DatabaseModels.Models;
using SudokuServer.Models.Dto;
using SudokuServer.Models.Vo;
using SudokuServer.Services;

namespace SudokuServer.ServicesImpl;

public class SudokuService(DatabaseContext db, IDistributedLock distributedLock)
{
    public async Task<SudokuGameVo> NewGameAsync(
        int size = 9,
        int? seed = null,
        SudokuGameType type = SudokuGameType.Default,
        int maxCount = int.MaxValue
    )
    {
        // await using var transaction = await db.Database.BeginTransactionAsync();
        seed ??= new Random().Next();
        ISudokuAsync sudoku = type switch
        {
            SudokuGameType.Default => await SudokuDefault.NewSudokuAsync(
                size,
                new Random(seed.Value),
                maxCount
            ),
            _ => throw new NotSupportedException(),
        };
        var winSudoku = await sudoku.SolveNewAsync();
        var winBoard = SudokuGameVo.ToBoardString(winSudoku!.GetBoard());
        var game = new SudokuGameVo(sudoku, default, seed.Value);
        var gameModel = game.ToSudokuGame();
        gameModel.Seed = seed.Value;
        gameModel.WinBoard = winBoard;
        await db.SudokuGames.AddAsync(gameModel);
        await db.SaveChangesAsync();
        // await transaction.CommitAsync();
        game.GameId = gameModel.Id;
        return game;
    }

    public async Task<SudokuGameVo?> GetGameAsync(Guid gameId)
    {
        var gameModel = await GetGameInternalAsync(gameId);
        if (gameModel == null)
            return null;
        var game = new SudokuGameVo(gameModel);
        return game;
    }

    public async Task<SudokuGame?> GetGameInternalAsync(Guid gameId, bool asNoTracking = false)
    {
        var queryable = db.SudokuGames.Where(x => x.Id == gameId);
        if (asNoTracking)
            queryable = queryable.AsNoTracking();
        var gameModel = await queryable.FirstOrDefaultAsync();
        return gameModel;
    }

    public async Task<SudokuSetValueVo?> SetValueAsync(
        SudokuSetValueDto dto,
        bool asNoTracking = false
    )
    {
        // 加锁
        await using var lockObj = await LockGameAsync(dto.GameId);
        if (!lockObj.IsLocked)
        {
            return new SudokuSetValueVo(null) { IsSuccess = false, IsLocked = true };
        }
        int i = dto.I;
        int j = dto.J;
        int value = dto.Value;
        var gameModel = await GetGameInternalAsync(dto.GameId, asNoTracking);
        if (gameModel == null)
            return null;
        var game = new SudokuGameVo(gameModel);
        int size = game.Sudoku.Size;
        if (i < 0 || i >= size)
            throw new ArgumentOutOfRangeException(nameof(dto.I));
        if (j < 0 || j >= size)
            throw new ArgumentOutOfRangeException(nameof(dto.J));
        if (value < 0 || value > size)
            throw new ArgumentOutOfRangeException(nameof(dto.Value));
        if (game.IsWin)
            return new SudokuSetValueVo(game);
        if (game.Sudoku.IsBaseIndex(i, j))
        {
            return new SudokuSetValueVo(game) { IsBase = true };
        }
        game.Sudoku[i, j] = value;
        var newGame = game.ToSudokuGame();
        if (asNoTracking)
        {
            await db
                .SudokuGames.Where(x => x.Id == game.GameId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.Board, newGame.Board));
        }
        else
        {
            gameModel.Board = newGame.Board;
        }
        await db.SaveChangesAsync();
        // await transaction.CommitAsync();
        var result = new SudokuSetValueVo(game) { IsSuccess = true };
        return result;
    }

    public async Task<SudokuListVo> GetGameListAsync(int page)
    {
        const int pageSize = 10;
        var gameModels = await db
            .SudokuGames.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var count = await db.SudokuGames.CountAsync();
        return new SudokuListVo
        {
            Games = gameModels.Select(x => new SudokuGamePublicVo(new SudokuGameVo(x))).ToList(),
            PageData = new PageDataVo
            {
                Page = page,
                PageSize = pageSize,
                Total = count,
                TotalPage = (int)Math.Ceiling(count / (double)pageSize),
            },
        };
    }

    public string LockGameKey(Guid gameId) => $"lock:sudoku_game_{gameId}";

    public async Task<IDistributedLockObject> LockGameAsync(
        Guid gameId,
        TimeSpan? timeout = null,
        TimeSpan? waitTime = null
    )
    {
        timeout ??= TimeSpan.FromSeconds(10);
        waitTime ??= TimeSpan.FromSeconds(1);
        string key = LockGameKey(gameId);
        var startTime = DateTime.Now;
        do
        {
            var @lock = await distributedLock.LockAsync(key, timeout.Value);
            if (@lock.IsLocked)
                return @lock;
            await Task.Delay(100);
        } while (DateTime.Now - startTime < waitTime);
        // 最后再尝试一次
        return await distributedLock.LockAsync(gameId.ToString(), timeout.Value);
    }
}
