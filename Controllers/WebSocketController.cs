using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebSocket.Services;
using Newtonsoft.Json;

namespace MyWebSocket.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WebSocketController : ControllerBase
    {

        private WebSocketHandler webSocketHandler;
        private SignalingHandler teleMeetHandler;
        public WebSocketController(WebSocketHandler webSocketHandler, SignalingHandler teleMeetHandler)
        {
            this.webSocketHandler = webSocketHandler;
            this.teleMeetHandler = teleMeetHandler;
        }

        [Route("/ws")]
        [Authorize]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var user = User.Claims.Where(o => o.Type == "user").First();
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await webSocketHandler.ProcessWebSocket(user.Value, webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [Route("/telemeet")]
        public async Task GetTelemeet()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var user = User.Claims.Where(o => o.Type == "user").First();
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await teleMeetHandler.ProcessWebSocket(user.Value, webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
