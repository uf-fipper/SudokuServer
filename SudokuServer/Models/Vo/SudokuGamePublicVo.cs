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

    public int BoardEmptyCount => Game.BoardEmptyCount;

    [JsonIgnore]
    public SudokuGameVo Game { get; set; }
}
