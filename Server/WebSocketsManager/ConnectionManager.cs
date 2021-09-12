using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server.WebSocketManager
{
    public class ConnectionManager
    {
        ConcurrentDictionary<string, WebSocket> _connections = new();

        public WebSocket GetSocketById(string id)
        {
            return _connections.FirstOrDefault(x => x.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAllConnections()
        {
            return _connections;
        }

        public string GetId(WebSocket webSocket)
        {
            return _connections.FirstOrDefault(x => x.Value == webSocket).Key;
        }

        public async Task RemoveSoketAsync(string id)
        {
            _connections.TryRemove(id, out var webSocket);
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
                "WebSocket connection closed", 
                CancellationToken.None);
        }

        public void AddSocket(WebSocket webSocket)
        {
            _connections.TryAdd(GetConnectionId(), webSocket);
        }

        private string GetConnectionId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
