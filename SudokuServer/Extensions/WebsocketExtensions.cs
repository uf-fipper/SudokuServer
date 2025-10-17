using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace SudokuServer.Extensions;

public static class WebsocketExtensions
{
    public static async Task<string> ReceiveAllTextAsync(
        this WebSocket webSocket,
        int onceLength = 1024 * 4,
        int? maxLength = null
    )
    {
        maxLength ??= onceLength;
        var buffer = new byte[onceLength];
        var sb = new StringBuilder();
        int count = 0;
        WebSocketReceiveResult receiveResult;
        do
        {
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );
            var text = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            count += receiveResult.Count;
            if (count > maxLength.Value)
                throw new WebSocketException("消息过长");
            sb.Append(text);
        } while (!receiveResult.EndOfMessage);
        return sb.ToString();
    }

    public static async Task<T> ReceiveAsJsonAsync<T>(
        this WebSocket webSocket,
        int onceLength = 1024 * 4,
        int? maxLength = null
    )
    {
        var text = await webSocket.ReceiveAllTextAsync(onceLength, maxLength);
        return JsonSerializer.Deserialize<T>(text)!;
    }

    public static async Task SendTextAsync(this WebSocket webSocket, string text)
    {
        var buffer = Encoding.UTF8.GetBytes(text);
        await webSocket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }

    public static async Task SendAsJsonAsync<T>(
        this WebSocket webSocket,
        T obj,
        JsonSerializerOptions? serializerOptions = null
    )
    {
        var text = JsonSerializer.Serialize(obj, serializerOptions);
        await webSocket.SendTextAsync(text);
    }
}
