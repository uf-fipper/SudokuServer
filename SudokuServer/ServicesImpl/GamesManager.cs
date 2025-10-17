using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SudokuServer.Extensions;
using SudokuServer.Models.Dto;
using SudokuServer.Models.Vo;
using SudokuServer.Services;

namespace SudokuServer.ServicesImpl;

public class GamesManager(
    SudokuService sudokuService,
    IDistributedLock distributedLock,
    IOptions<JsonOptions> jsonOptions
)
{
    public const string GameLockKey = "Lock:WebSocketGameLock:";

    public JsonSerializerOptions JsonSerializerOptions => jsonOptions.Value.SerializerOptions;

    public static Dictionary<Guid, GameManager> Games { get; set; } = [];

    public async Task<GameManager?> Connect(WebSocket webSocket, Guid gameId)
    {
        await using var _ = await distributedLock.LockAsync(
            GameLockKey + gameId.ToString("N"),
            TimeSpan.FromSeconds(10)
        );
        Games.TryGetValue(gameId, out var gameManager);
        if (gameManager == null)
        {
            var game = await sudokuService.GetGameAsync(gameId);
            if (game == null)
                return null;
            gameManager = new GameManager { Game = game };
            Games[gameId] = gameManager;
        }
        var isAdded = gameManager.AddSocket(webSocket);
        if (!isAdded)
        {
            await webSocket.SendAsJsonAsync(BaseVo.Fail("400", "人数已满"));
            await webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "人数已满",
                CancellationToken.None
            );
            return null;
        }
        var resultGame = new SudokuGamePublicVo(gameManager.Game)
        {
            SetCorrectMap = gameManager.SendSolve,
        };
        await gameManager.SendAsJsonAsync(
            BaseVo.Success(SudokuWebSocketBaseVo.Game(resultGame, gameManager.MessageSeq)),
            JsonSerializerOptions
        );
        return gameManager;
    }

    public async Task ReadNextAsync(GameManager gameManager, WebSocket webSocket)
    {
        var newText = await webSocket.ReceiveAllTextAsync();
        await ReadOnceAsync(gameManager, webSocket, newText);
    }

    private async Task ReadOnceAsync(GameManager gameManager, WebSocket webSocket, string text)
    {
        await using var lockObj = await LockAsync(gameManager);
        if (text.IsNullOrEmpty())
        {
            return;
        }
        // todo: 解析并做处理
        var baseDto = JsonSerializer.Deserialize<SudokuWebSocketBaseDto>(
            text,
            jsonOptions.Value.SerializerOptions
        );
        var task = baseDto?.Type switch
        {
            "SetValue" => ReadSetValueAsync(gameManager, webSocket, baseDto.Data),
            "SetSendSolve" => ReadSetSendSolveAsync(gameManager, webSocket, baseDto.Data),
            _ => webSocket.SendAsJsonAsync(
                BaseVo.Fail("400", "不支持的操作"),
                jsonOptions.Value.SerializerOptions
            ),
        };
        await task;

        // 目前只是简单地广播收到的消息
        // await SendAsync(text);
    }

    private async Task ReadSetValueAsync(
        GameManager gameManager,
        WebSocket webSocket,
        JsonElement jsonElement
    )
    {
        if (!TryJsonDeserialize<SudokuSetValueDto>(jsonElement, out var setValueDto))
        {
            await webSocket.SendAsJsonAsync(
                BaseVo.Fail("400", "请求格式错误"),
                JsonSerializerOptions
            );
            return;
        }
        if (setValueDto.GameId != gameManager.GameId)
        {
            await webSocket.SendAsJsonAsync(BaseVo.Fail("403", "游戏错误"), JsonSerializerOptions);
            return;
        }
        var setValueResult =
            await sudokuService.SetValueAsync(setValueDto, true)
            ?? throw new NotSupportedException("游戏不存在");
        if (setValueResult.IsLocked)
        {
            await webSocket.SendAsJsonAsync(
                BaseVo.Fail("400", "网络繁忙，请稍后再试"),
                JsonSerializerOptions
            );
            return;
        }
        gameManager.Game = setValueResult.Game!.Game;
        if (gameManager.SendSolve)
        {
            setValueResult.Game.SetCorrectMap = true;
        }
        await gameManager.SendAsJsonAsync(
            BaseVo.Success(SudokuWebSocketBaseVo.SetValue(setValueResult, gameManager.MessageSeq)),
            JsonSerializerOptions
        );
    }

    private async Task ReadSetSendSolveAsync(
        GameManager gameManager,
        WebSocket webSocket,
        JsonElement jsonElement
    )
    {
        if (!TryJsonDeserialize<SudokuSetSendSolveDto>(jsonElement, out var setSendSolveDto))
        {
            await webSocket.SendAsJsonAsync(
                BaseVo.Fail("400", "请求格式错误"),
                JsonSerializerOptions
            );
            return;
        }
        gameManager.SendSolve = setSendSolveDto.SendSolve;
        var game = new SudokuGamePublicVo(gameManager.Game)
        {
            SetCorrectMap = gameManager.SendSolve,
        };
        await gameManager.SendAsJsonAsync(
            BaseVo.Success(SudokuWebSocketBaseVo.Game(game, gameManager.MessageSeq)),
            JsonSerializerOptions
        );
    }

    private bool TryJsonDeserialize<T>(JsonElement jsonElement, [NotNullWhen(true)] out T? result)
    {
        try
        {
            result = jsonElement.Deserialize<T>(JsonSerializerOptions);
            if (result == null)
            {
                return false;
            }
            return true;
        }
        catch (JsonException)
        {
            result = default;
            return false;
        }
    }

    public async Task<IDistributedLockObject> LockAsync(GameManager gameManager)
    {
        var start = DateTime.Now;
        IDistributedLockObject obj;
        do
        {
            obj = await distributedLock.LockAsync(gameManager.LockKey, TimeSpan.FromSeconds(10));
            if (obj.IsLocked)
                return obj;
            await Task.Delay(10);
            if (DateTime.Now - start > TimeSpan.FromSeconds(10))
            {
                throw new TimeoutException("获取锁超时");
            }
        } while (true);
    }
}

public class GameManager
{
    public Guid GameId => Game.GameId;

    public required SudokuGameVo Game { get; set; }

    public HashSet<WebSocket> WebSockets { get; } = [];

    private readonly object _lockAddRemove = new();

    public bool SendSolve = false;

    private int _messageSeq = 0;

    public int MessageSeq => _messageSeq++;

    /// <summary>
    /// 最大人数
    /// </summary>
    public const int MaxPersonCount = 16;

    public int PersonCount = 0;

    public string LockKey => $"Lock:WebSocketGameManagerLock:{GameId}";

    public bool AddSocket(WebSocket webSocket)
    {
        lock (_lockAddRemove)
        {
            if (PersonCount >= MaxPersonCount)
                return false;
            WebSockets.Add(webSocket);
            return true;
        }
    }

    public void RemoveSocket(WebSocket webSocket)
    {
        lock (_lockAddRemove)
        {
            WebSockets.Remove(webSocket);
            PersonCount--;
            webSocket.Dispose();
        }
    }

    public async Task SendAsync(string text)
    {
        await Task.WhenAll(
            WebSockets.Select(socket =>
            {
                if (socket.State != WebSocketState.Open)
                {
                    RemoveSocket(socket);
                    return Task.CompletedTask;
                }
                try
                {
                    return socket.SendTextAsync(text);
                }
                catch (WebSocketException)
                {
                    RemoveSocket(socket);
                    return Task.CompletedTask;
                }
            })
        );
    }

    public async Task SendAsJsonAsync<T>(T obj, JsonSerializerOptions? serializerOptions = null)
    {
        var text = JsonSerializer.Serialize(obj, serializerOptions);
        await SendAsync(text);
    }
}
