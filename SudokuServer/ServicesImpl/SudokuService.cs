using Microsoft.EntityFrameworkCore;
using Sudoku;
using Sudoku.Default;
using SudokuServer.Models.DatabaseModels.Context;
using SudokuServer.Models.DatabaseModels.Models;
using SudokuServer.Models.Dto;
using SudokuServer.Models.Vo;
using SudokuServer.Services;

namespace SudokuServer.ServicesImpl;

public class SudokuService(DatabaseContext db) : ISudokuService
{
    public async Task<SudokuGameVo> NewGameAsync(int size, int? seed, SudokuGameType type)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        seed ??= new Random().Next();
        ISudokuAsync sudoku = type switch
        {
            SudokuGameType.Default => await SudokuDefault.NewSudokuAsync(
                size,
                new Random(seed.Value)
            ),
            _ => throw new NotSupportedException(),
        };
        var game = new SudokuGameVo(sudoku, default, seed.Value);
        var gameModel = game.ToSudokuGame();
        gameModel.Seed = seed.Value;
        await db.SudokuGames.AddAsync(gameModel);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        game.GameId = gameModel.Id;
        return game;
    }

    public async Task<SudokuGameVo?> GetGameAsync(Guid gameId)
    {
        var gameModel = await db.SudokuGames.Where(x => x.Id == gameId).FirstOrDefaultAsync();
        if (gameModel == null)
            return null;
        var game = new SudokuGameVo(gameModel);
        return game;
    }

    public async Task<SudokuSetValueVo?> SetValueAsync(SudokuSetValueDto dto)
    {
        int i = dto.I;
        int j = dto.J;
        int value = dto.Value;
        await using var transaction = await db.Database.BeginTransactionAsync();
        var game = await GetGameAsync(dto.GameId);
        if (game == null)
            return null;
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
        await db
            .SudokuGames.Where(x => x.Id == game.GameId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.Board, newGame.Board));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return new SudokuSetValueVo(game) { IsSuccess = true };
    }
}
