using System.Text.Json;

namespace SudokuServer.Models.Dto;

public class SudokuWebSocketBaseDto
{
    public string Type { get; set; } = "";

    public JsonElement Data { get; set; }
}
