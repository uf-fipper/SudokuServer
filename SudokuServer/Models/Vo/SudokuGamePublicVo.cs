using System.Text.Json.Serialization;

namespace SudokuServer.Models.Vo;

public class SudokuGamePublicVo
{
    public SudokuGamePublicVo(SudokuGameVo game)
    {
        GameId = game.GameId;
        Board = game.GetBoard();
        IsWin = game.IsWin;
        Game = game;
        Seed = game.Seed;
        BaseIndexs = game.Sudoku.BaseIndexs.ToList();
    }

    public Guid GameId { get; set; }

    /// <summary>
    /// 数独版块
    /// </summary>
    public int[][] Board { get; set; }

    public List<(int, int)> BaseIndexs { get; set; }

    public int Seed { get; set; }

    public bool IsWin { get; set; }

    [JsonIgnore]
    public SudokuGameVo Game { get; set; }
}
