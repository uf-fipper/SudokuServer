namespace SudokuServer.Models.Vo;

public class SudokuWebSocketBaseVo
{
    public string Type { get; set; } = "";

    public static SudokuWebSocketBaseVo<SudokuSetValueVo> SetValue(SudokuSetValueVo data)
    {
        return new() { Type = "SetValue", Data = data };
    }

    public static SudokuWebSocketBaseVo<SudokuGamePublicVo> Game(SudokuGamePublicVo data)
    {
        return new() { Type = "Game", Data = data };
    }
}

public class SudokuWebSocketBaseVo<T> : SudokuWebSocketBaseVo
{
    public T Data { get; set; } = default!;
}
