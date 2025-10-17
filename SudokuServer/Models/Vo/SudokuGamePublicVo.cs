using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SudokuServer.Models.Vo;

public class SudokuGamePublicVo
{
    public SudokuGamePublicVo(SudokuGameVo game)
    {
        GameId = game.GameId;
        Board = game.GetBoard();
        Game = game;
        Seed = game.Seed;
        BaseIndexs = game.Sudoku.BaseIndexs.Select(x => new int[2] { x.i, x.j }).ToList();
    }

    public Guid GameId { get; set; }

    /// <summary>
    /// 数独版块
    /// </summary>
    public int[][] Board { get; set; }

    public List<int[]> BaseIndexs { get; set; }

    public int Seed { get; set; }

    public bool IsWin => Game.IsWin;

    public int BoardEmptyCount => Game.GetBoardEmptyCount();

    public int StartBoardEmptyCount => Game.StartBoardEmptyCount;

    public bool[][]? CorrectMap
    {
        get
        {
            if (!SetCorrectMap)
                return null;
            var winBoard = Game.GetWinBoard();
            var result = new bool[winBoard.Length][];
            for (int i = 0; i < winBoard.Length; i++)
            {
                result[i] = new bool[winBoard.Length];
                for (int j = 0; j < winBoard.Length; j++)
                {
                    result[i][j] = winBoard[i][j] == Game.Sudoku[i, j];
                }
            }
            return result;
        }
    }

    [JsonIgnore]
    public bool SetCorrectMap { get; set; } = false;

    [JsonIgnore]
    public SudokuGameVo Game { get; set; }
}
