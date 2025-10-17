using Sudoku;
using Sudoku.Default;
using SudokuServer.Models.DatabaseModels.Models;

namespace SudokuServer.Models.Vo;

public class SudokuGameVo
{
    public SudokuGameVo(ISudokuAsync sudoku, Guid gameId, int seed)
    {
        Sudoku = sudoku;
        GameId = gameId;
        Seed = seed;
        StartBoardEmptyCount = sudoku.Size * sudoku.Size - sudoku.BaseIndexs.Count;
    }

    public SudokuGameVo(SudokuGame game)
    {
        ISudokuAsync sudoku = game.Type switch
        {
            SudokuGameType.Default => new SudokuDefault(FromBoardString(game.StartBoard)),
            _ => throw new NotSupportedException(),
        };
        var nowBoard = FromBoardString(game.Board);
        int size = sudoku.Size;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (sudoku.IsBaseIndex(i, j))
                    continue;
                sudoku[i, j] = nowBoard[i][j];
            }
        }
        _gameModel = game;
        GameId = game.Id;
        Sudoku = sudoku;
        Seed = game.Seed;
        StartBoardEmptyCount = sudoku.Size * sudoku.Size - sudoku.BaseIndexs.Count;
    }

    private readonly SudokuGame? _gameModel;

    public Guid GameId { get; set; }

    public ISudokuAsync Sudoku { get; set; }

    public int Seed { get; set; }

    public bool IsWin => Sudoku.IsWin();

    public int GetBoardEmptyCount() => GetBoard().Sum(x => x.Where(y => y == 0).Count());

    public int StartBoardEmptyCount { get; set; }

    public int[][] GetWinBoard(bool syncSolve = false)
    {
        if (_gameModel?.WinBoard != null)
        {
            return FromBoardString(_gameModel.WinBoard);
        }
        if (!syncSolve)
        {
            throw new NotSupportedException("未获取到WinBoard");
        }
        var newSudoku = Sudoku.SolveNew() ?? throw new Exception("数独无解");
        var result = new int[Sudoku.Size][];
        for (int i = 0; i < Sudoku.Size; i++)
        {
            result[i] = new int[Sudoku.Size];
            for (int j = 0; j < Sudoku.Size; j++)
            {
                result[i][j] = newSudoku[i, j];
            }
        }
        return result;
    }

    public SudokuSetValueVo? SetValueStatus { get; set; }

    public int[][] GetBoard()
    {
        int size = Sudoku.Size;
        var Board = new int[size][];
        for (int i = 0; i < size; i++)
        {
            Board[i] = new int[size];
            for (int j = 0; j < size; j++)
            {
                Board[i][j] = Sudoku[i, j];
            }
        }
        return Board;
    }

    public static string ToBoardString(int[][] board)
    {
        var str = string.Join('\n', board.Select(row => string.Join(',', row)));
        return str;
    }

    public static int[][] FromBoardString(string boardString)
    {
        return boardString
            .Split('\n')
            .Select(row => row.Split(',').Select(int.Parse).ToArray())
            .ToArray();
    }

    public SudokuGame ToSudokuGame()
    {
        int size = Sudoku.Size;
        var startBoard = new int[size][];
        for (int i = 0; i < size; i++)
        {
            startBoard[i] = new int[size];
            for (int j = 0; j < size; j++)
            {
                if (Sudoku.IsBaseIndex(i, j))
                    startBoard[i][j] = Sudoku[i, j];
                else
                    startBoard[i][j] = 0;
            }
        }
        var game = new SudokuGame
        {
            Size = size,
            Board = ToBoardString(GetBoard()),
            StartBoard = ToBoardString(startBoard),
        };
        if (GameId != default)
        {
            game.Id = GameId;
        }

        return game;
    }
}
