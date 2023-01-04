using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using MyTest.Models;
using Newtonsoft.Json;

namespace MyTest.Controllers
{
    public class WebSocketHandler 
    {
        private readonly ILogger<WebSocketHandler> logger;

        public WebSocketHandler(ILogger<WebSocketHandler> logger)
        {
            this.logger = logger;
        }
        //REF: https://radu-matei.com/blog/aspnet-core-websockets-middleware/
        ConcurrentDictionary<int, WebSocket> WebSockets = new ConcurrentDictionary<int, WebSocket>();

        public async Task ProcessWebSocket(WebSocket webSocket) 
        {
            WebSockets.TryAdd(webSocket.GetHashCode(), webSocket);
            var buffer = new byte[1024 * 4];
            var res = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var model = new MyMessage(){ UserId = "訪客" };
            while (!res.CloseStatus.HasValue) {
                var cmd = Encoding.UTF8.GetString(buffer, 0, res.Count);
                if (!string.IsNullOrEmpty(cmd)) {
                    logger.LogInformation(cmd);
                    model = JsonConvert.DeserializeObject<MyMessage>(cmd);
                    Broadcast(model!);
                }
                res = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(res.CloseStatus.Value, res.CloseStatusDescription, CancellationToken.None);
            WebSockets.TryRemove(webSocket.GetHashCode(), out var removed);
            model!.Message = $"{model.UserId} left the room.";
            Broadcast(model);
        }

        public void Broadcast(MyMessage model) 
        {
            var buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));
            var data = new ArraySegment<byte>(buff, 0, buff.Length);
            Parallel.ForEach(WebSockets.Values, async (webSocket) =>
            {
                if (webSocket.State == WebSocketState.Open)
                    await webSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
            });
        }
    }
}

   