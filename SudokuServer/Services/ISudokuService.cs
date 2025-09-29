using SudokuServer.Models.DatabaseModels.Models;
using SudokuServer.Models.Dto;
using SudokuServer.Models.Vo;

namespace SudokuServer.Services;

public interface ISudokuService
{
    /// <summary>
    /// 创建一个新的数独游戏
    /// </summary>
    /// <param name="size">数独边长</param>
    /// <param name="seed">随机种子</param>
    /// <returns>游戏id</returns>
    /// <exception cref="ArgumentException">size 不是平方数</exception>
    Task<SudokuGameVo> NewGameAsync(int size, int? seed, SudokuGameType type);

    Task<SudokuGameVo> NewGameAsync(int size, int? seed) =>
        NewGameAsync(size, seed, SudokuGameType.Default);

    Task<SudokuGameVo> NewGameAsync(int size) => NewGameAsync(size, null);

    Task<SudokuGameVo> NewGameAsync() => NewGameAsync(9, null);

    /// <summary>
    /// 通过游戏id获取游戏信息
    /// </summary>
    /// <param name="gameId">游戏id</param>
    /// <returns>null为游戏不存在，否则返回游戏信息</returns>
    Task<SudokuGameVo?> GetGameAsync(Guid gameId);

    /// <summary>
    /// 获取游戏列表
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    Task<SudokuListVo> GetGameListAsync(int page);

    /// <summary>
    /// 设置游戏值
    /// </summary>
    /// <param name="dto">设置参数</param>
    /// <returns>null为游戏不存在或游戏已结束，否则返回设置成功模型</returns>
    /// <exception cref="ArgumentOutOfRangeException">i, j, value 参数越界</exception>
    Task<SudokuSetValueVo?> SetValueAsync(SudokuSetValueDto dto);
}
