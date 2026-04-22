namespace webProgramming.Models
{
    public class Favor
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string ProductId { get; set; }

        // Navigation properties
    }
}
