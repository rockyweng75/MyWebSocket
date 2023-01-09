using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using MyWebSocket.Models;
using Newtonsoft.Json;

namespace MyWebSocket.Services
{
    public class SignalingHandler 
    {
        private readonly ILogger<SignalingHandler> logger;

        public SignalingHandler(ILogger<SignalingHandler> logger)
        {
            this.logger = logger;
        }
        //REF: https://radu-matei.com/blog/aspnet-core-websockets-middleware/
        ConcurrentDictionary<string, WebSocket> WebSockets = new ConcurrentDictionary<string, WebSocket>();

        public async Task ProcessWebSocket(string user, WebSocket webSocket) 
        {
            WebSockets.TryAdd(user, webSocket);
            var buffer = new byte[1024 * 10];
            var res = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!res.CloseStatus.HasValue) {
                var cmd = Encoding.UTF8.GetString(buffer, 0, res.Count);
                if (!string.IsNullOrEmpty(cmd)) {
                    var model = JsonConvert.DeserializeObject(cmd);
                    Middleware(user, model!);
                }
                res = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(res.CloseStatus.Value, res.CloseStatusDescription, CancellationToken.None);
            WebSockets.TryRemove(user, out var removed);
            Disconnection(user);

        }
        public void Middleware(string user, dynamic _data) 
        {
            var buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_data));
            var data = new ArraySegment<byte>(buff, 0, buff.Length);
            Parallel.ForEach(WebSockets.Where(o => o.Key != user).ToList(), async (keyValue) =>
            {
                WebSocket webSocket = keyValue.Value;
                if (webSocket.State == WebSocketState.Open)
                    await webSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
            });
        }

        public async void Disconnection(string user) 
        {
            var buff = Encoding.UTF8.GetBytes("disconnection");
            var data = new ArraySegment<byte>(buff, 0, buff.Length);
            Parallel.ForEach(WebSockets.Where(o => o.Key != user).ToList(), async (keyValue) =>
            {
                WebSocket webSocket = keyValue.Value;
                if (webSocket.State == WebSocketState.Open)
                    await webSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
            });
        }
    }
}

   