using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.WebSocketManager
{
    public abstract class WebSocketHandler
    {
        public ConnectionManager Connections { get; set; }

        public WebSocketHandler(ConnectionManager connections)
        {
            Connections = connections;
        }

        public virtual async Task OnConnected(WebSocket webSocket)
        {
            await Task.Run(() => { Connections.AddSocket(webSocket); });
        }

        public virtual async Task OnDisconnected(WebSocket webSocket) 
        { 
            await Connections.RemoveSoketAsync(Connections.GetId(webSocket));
        }

        public async Task SendMessage(WebSocket webSocket, string message)
        {
            if (webSocket.State != WebSocketState.Open) return;

            if (string.IsNullOrEmpty(message)) return;

            await webSocket.SendAsync(new System.ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length), 
                WebSocketMessageType.Text, 
                true,
                CancellationToken.None);
        }

        public async Task SendMessage(string id, string message)
        {
            await SendMessage(Connections.GetSocketById(id), message);
        }

        public async Task SendMessageToAll(string message)
        {
            foreach (var item in Connections.GetAllConnections())
            {
                await SendMessage(item.Value, message);
            }
        }

        public abstract Task Receive(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer);
    }
}
