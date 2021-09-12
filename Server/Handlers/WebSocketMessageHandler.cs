using Server.WebSocketManager;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handlers
{
    public class WebSocketMessageHandler : WebSocketHandler
    {
        public WebSocketMessageHandler(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public override async Task OnConnected(WebSocket webSocket)
        {
            await base.OnConnected(webSocket);
            var webSocketId = Connections.GetId(webSocket);
            await SendMessageToAll($"{webSocketId} just joined the party");
        }

        public override async Task Receive(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer)
        {
            var webSocketId = Connections.GetId(webSocket);
            var message = $"{webSocketId} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";
            await SendMessageToAll(message);
        }
    }
}
