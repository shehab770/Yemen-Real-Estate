namespace webProgramming.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Subject { get; set; } // Nullable
        public required string Message { get; set; }
        public bool SmtpStatus { get; set; } // True = sent successfully
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: If you want to track which user submitted (if logged in)
        public int? UserId { get; set; }
    }
}
