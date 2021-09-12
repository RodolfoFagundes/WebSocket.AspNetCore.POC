using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server.WebSocketManager
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler Handler { get; set; }

        public WebSocketMiddleware(RequestDelegate next, WebSocketHandler handler)
        {
            _next = next;
            Handler = handler;
        }

        // Método chamado em WebSocketExtension.MapWebSockets
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (!httpContext.WebSockets.IsWebSocketRequest) return;

            var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

            await Handler.OnConnected(webSocket);

            await Receive(webSocket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await Handler.Receive(webSocket, result, buffer);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await Handler.OnDisconnected(webSocket);
                }
            });
        }

        private async Task Receive(WebSocket webSocket, Action<WebSocketReceiveResult, byte[]> messageToHandle)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                messageToHandle(result, buffer);
            }
        }
    }
}