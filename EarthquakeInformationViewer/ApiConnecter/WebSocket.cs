using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketClient : IDisposable
{
    private ClientWebSocket _webSocket;
    private CancellationTokenSource _cts;

    public event Action Connected;
    public event Action<string> MessageReceived;
    public event Action Disconnected;
    public event Action<Exception> Error;

    public bool IsConnected => _webSocket != null && _webSocket.State == WebSocketState.Open;

    public async Task ConnectAsync(Uri uri)
    {
        _webSocket = new ClientWebSocket();
        _cts = new CancellationTokenSource();
        try
        {
            await _webSocket.ConnectAsync(uri, _cts.Token);
            Connected?.Invoke();
            _ = ReceiveLoop();
        }
        catch (Exception ex)
        {
            Error?.Invoke(ex);
            Dispose();
        }
    }

    public async Task SendAsync(string message)
    {
        if (_webSocket == null || _webSocket.State != WebSocketState.Open)
            throw new InvalidOperationException("WebSocket is not connected.");

        var buffer = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);
        try
        {
            await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, _cts.Token);
        }
        catch (Exception ex)
        {
            Error?.Invoke(ex);
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[4096];
        try
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    Disconnected?.Invoke();
                    break;
                }
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                MessageReceived?.Invoke(message);
            }
        }
        catch (Exception ex)
        {
            Error?.Invoke(ex);
            Disconnected?.Invoke();
        }
    }

    public async Task DisconnectAsync()
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            _cts.Cancel();
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
            Disconnected?.Invoke();
        }
        Dispose();
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _webSocket?.Dispose();
        _cts?.Dispose();
    }
}