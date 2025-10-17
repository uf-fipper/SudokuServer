namespace SudokuServer.Models.Vo;

public class SudokuWebSocketBaseVo
{
    public required string Type { get; set; }

    public required int MessageSeq { get; set; }

    public static SudokuWebSocketBaseVo<SudokuSetValueVo> SetValue(
        SudokuSetValueVo data,
        int messageSeq
    )
    {
        return new()
        {
            Type = "SetValue",
            MessageSeq = messageSeq,
            Data = data,
        };
    }

    public static SudokuWebSocketBaseVo<SudokuGamePublicVo> Game(
        SudokuGamePublicVo data,
        int messageSeq
    )
    {
        return new()
        {
            Type = "Game",
            MessageSeq = messageSeq,
            Data = data,
        };
    }
}

public class SudokuWebSocketBaseVo<T> : SudokuWebSocketBaseVo
{
    public T Data { get; set; } = default!;
}
