namespace SudokuServer.Models.Vo;

public class SudokuSetValueVo(SudokuGameVo? game)
{
    public SudokuGamePublicVo? Game { get; set; } = game is null ? null : new(game);

    /// <summary>
    /// 设置值是否成功
    /// </summary>
    public bool IsSuccess { get; set; } = false;

    /// <summary>
    /// 如果失败，是否是基本元素
    /// </summary>
    public bool IsBase { get; set; } = false;

    /// <summary>
    /// 如果失败，是否被锁定
    /// </summary>
    public bool IsLocked { get; set; } = false;

    public bool IsWin => Game?.IsWin ?? false;
}
