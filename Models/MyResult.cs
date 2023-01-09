namespace MyWebSocket.Models
{
    [Serializable]
    public class MyMessage
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string? Message { get; set; }
    
        public string? ToUser { get; set; }

        public int StatusCode { get; set; } = 404;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    }
}

