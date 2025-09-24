namespace SudokuServer.Models.Dto;

public class SudokuSetValueDto
{
    /// <summary>
    /// 游戏id
    /// </summary>
    public required Guid GameId { get; set; }

    public required int I { get; set; }

    public required int J { get; set; }

    /// <summary>
    /// 值，设置为0则删除该值
    /// </summary>
    public required int Value { get; set; }
}
