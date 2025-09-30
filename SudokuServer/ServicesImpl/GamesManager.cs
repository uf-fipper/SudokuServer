using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SudokuServer.Extensions;
using SudokuServer.Models.Vo;
using SudokuServer.Services;

namespace SudokuServer.ServicesImpl;

public class GamesManager(
    ISudokuService sudokuService,
    IDistributedLock distributedLock,
    IServiceProvider serviceProvider
)
{
    public const string GameLockKey = "Lock:WebSocketGameLock:";

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
            gameManager = serviceProvider.GetRequiredService<GameManager>();
            gameManager.Game = game;
            Games[gameId] = gameManager;
        }
        gameManager.AddSocket(webSocket);
        return gameManager;
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

public class GameManager(IDistributedLock distributedLock)
{
    public Guid GameId => Game.GameId;

    public SudokuGameVo Game { get; set; } = default!;

    public HashSet<WebSocket> WebSockets { get; } = [];

    public string LockKey => $"Lock:WebSocketGameManagerLock:{GameId}";

    public void AddSocket(WebSocket webSocket)
    {
        WebSockets.Add(webSocket);
    }

    public void RemoveSocket(WebSocket webSocket)
    {
        WebSockets.Remove(webSocket);
        webSocket.Dispose();
    }

    public async Task ReadNextAsync(WebSocket webSocket)
    {
        var newText = await webSocket.ReceiveAllTextAsync();
        await ReadOnceAsync(webSocket, newText);
    }

    private async Task ReadOnceAsync(WebSocket webSocket, string text)
    {
        await using var lockObj = await LockAsync();
        if (text.IsNullOrEmpty())
        {
            return;
        }
        // todo: 解析并做处理
        // 目前只是简单地广播收到的消息
        await SendAsync(text);
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

    public async Task<IDistributedLockObject> LockAsync()
    {
        var start = DateTime.Now;
        IDistributedLockObject obj;
        do
        {
            obj = await distributedLock.LockAsync(LockKey, TimeSpan.FromSeconds(10));
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
