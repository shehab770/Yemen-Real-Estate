namespace webProgramming.Models
{
    public class Message
    {
        public int Id { get; set; }
        public required string ChatId { get; set; }
        public required string SenderId { get; set; }
        public required string ReceiverId { get; set; }
        public required string Content { get; set; }
        public int IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        // Navigation property
    }
}
