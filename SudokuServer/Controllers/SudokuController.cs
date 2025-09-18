using Microsoft.AspNetCore.Mvc;
using Sudoku.Default;
using SudokuServer.Models.Vo;

namespace SudokuServer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SudokuController : Controller
{
    [HttpGet]
    public async Task<BaseVo<string>> CreateGame()
    {
        await Task.CompletedTask;
        var gameId = Guid.NewGuid().ToString();
        var game = await SudokuDefault.NewSudokuAsync(9, null);
        return BaseVo.Success(gameId);
    }
}
