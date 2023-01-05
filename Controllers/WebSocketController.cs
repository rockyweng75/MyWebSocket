using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await webSocketHandler.ProcessWebSocket(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
