namespace webProgramming.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public required string ChatId { get; set; }
        public required string SellerId { get; set; }
        public required string BuyerId { get; set; }
        public required string ProductId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        // Navigation property
    }
}
