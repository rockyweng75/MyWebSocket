using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTest.Services;
using Newtonsoft.Json;

namespace MyTest.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WebSocketController : ControllerBase
    {

        private WebSocketHandler webSocketHandler;
        public WebSocketController(WebSocketHandler webSocketHandler)
        {
            this.webSocketHandler = webSocketHandler;
        }

        [Route("/ws")]
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
    }
}
