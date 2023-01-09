namespace MyWebSocket.Models
{
    [Serializable]
    public class MyMeet
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public byte[]? Buff { get; set; }
        public int Count { get; set; }

        public string? ToUser { get; set; }

        public string StatusCode { get; set; } = "Offline";
    }
}

