namespace SudokuServer.Models.Vo;

public class SudokuListVo
{
    public List<SudokuGamePublicVo> Games { get; set; } = [];

    public required PageDataVo PageData { get; set; }
}
