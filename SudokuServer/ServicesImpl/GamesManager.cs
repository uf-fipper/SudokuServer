using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SudokuServer.Extensions;
using SudokuServer.Models.Vo;
using SudokuServer.Services;

namespace SudokuServer.ServicesImpl;

public class GamesManager(ISudokuService sudokuService, IDistributedLock distributedLock)
{
    public const string GameLockKey = "Lock:WebSocketGameLock:";

    public static Dictionary<Guid, GameManager> Games { get; set; } = [];

    public async Task<bool> Connect(WebSocket webSocket, Guid gameId)
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
                return false;
            gameManager = new GameManager(game);
            Games[gameId] = gameManager;
        }
        gameManager.AddSocket(webSocket);
        return true;
    }

    public async Task<bool> SendAsync(Guid gameId, string text)
    {
        await using var _ = await distributedLock.LockAsync(
            GameLockKey + gameId.ToString("N"),
            TimeSpan.FromSeconds(10)
        );
        Games.TryGetValue(gameId, out var gameManager);
        if (gameManager == null)
            return false;
        await gameManager.SendAsync(text);
        return true;
    }

    public async Task<bool> SendAsJsonAsync<T>(Guid gameId, T obj)
    {
        await using var _ = await distributedLock.LockAsync(
            GameLockKey + gameId.ToString("N"),
            TimeSpan.FromSeconds(10)
        );
        Games.TryGetValue(gameId, out var gameManager);
        if (gameManager == null)
            return false;
        await gameManager.SendAsJsonAsync(obj);
        return true;
    }
}

public class GameManager(SudokuGameVo game)
{
    public Guid GameId => Game.GameId;

    public SudokuGameVo Game { get; } = game;

    public HashSet<WebSocket> WebSockets { get; } = [];

    public void AddSocket(WebSocket webSocket)
    {
        WebSockets.Add(webSocket);
        _ = Task.Run(async () => await ReadNextAsync(webSocket));
    }

    public void RemoveSocket(WebSocket webSocket)
    {
        WebSockets.Remove(webSocket);
        webSocket.Dispose();
    }

    private async Task ReadNextAsync(WebSocket webSocket)
    {
        string newText;
        try
        {
            newText = await webSocket.ReceiveAllTextAsync();
        }
        catch (WebSocketException)
        {
            RemoveSocket(webSocket);
            return;
        }
        await ReadOnceAsync(webSocket, newText);
    }

    private async Task ReadOnceAsync(WebSocket webSocket, string text)
    {
        if (text.IsNullOrEmpty())
        {
            RemoveSocket(webSocket);
            return;
        }
        // todo: 解析并做处理
        // 目前只是简单地广播收到的消息
        await SendAsync(text);
        // 等待下一次收到消息
        _ = Task.Run(async () => await ReadNextAsync(webSocket));
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

    public async Task SendAsJsonAsync<T>(T obj)
    {
        var text = System.Text.Json.JsonSerializer.Serialize(obj);
        await SendAsync(text);
    }
}
