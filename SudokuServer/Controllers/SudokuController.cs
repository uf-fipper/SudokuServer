using Microsoft.AspNetCore.Mvc;
using Sudoku.Default;
using SudokuServer.Models.Dto;
using SudokuServer.Models.Vo;
using SudokuServer.Services;

namespace SudokuServer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SudokuController(ISudokuService sudokuService) : Controller
{
    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> NewGame()
    {
        var game = await sudokuService.NewGameAsync();
        var gameResult = new SudokuGamePublicVo(game);
        return Ok(BaseVo.Success(gameResult));
    }

    [HttpGet]
    public async Task<IActionResult> GetGame([FromQuery] Guid gameId)
    {
        var game = await sudokuService.GetGameAsync(gameId);
        if (game == null)
            return GameNotFound();
        var gameResult = new SudokuGamePublicVo(game);
        return Ok(BaseVo.Success(gameResult));
    }

    [HttpPost]
    public async Task<IActionResult> SetValue([FromBody] SudokuSetValueDto dto)
    {
        var result = await sudokuService.SetValueAsync(dto);
        if (result == null)
            return GameNotFound();
        return Ok(BaseVo.Success(result));
    }

    [HttpGet]
    public async Task<IActionResult> GetGameList([FromQuery] int page)
    {
        var result = await sudokuService.GetGameListAsync(page);
        return Ok(BaseVo.Success(result));
    }

    public IActionResult GameNotFound()
    {
        return Ok(BaseVo.Fail("404", "游戏不存在"));
    }
}
