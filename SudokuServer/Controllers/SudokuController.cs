using Microsoft.AspNetCore.Mvc;
using Sudoku.Default;
using SudokuServer.Models.Vo;

namespace SudokuServer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SudokuController : Controller
{
    [HttpGet]
    public async Task<IActionResult> CreateGame()
    {
        await Task.CompletedTask;
        var gameId = Guid.NewGuid().ToString();
        var game = await SudokuDefault.NewSudokuAsync(9, null);
        var board = new int[9][];
        for (int i = 0; i < 9; i++)
        {
            board[i] = new int[9];
            for (int j = 0; j < 9; j++)
            {
                board[i][j] = game[i, j];
            }
        }
        var sudokuGameVo = new SudokuGameVo { Board = board };
        return Ok(BaseVo.Success(sudokuGameVo));
    }
}
