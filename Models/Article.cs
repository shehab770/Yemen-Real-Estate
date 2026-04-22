using System.ComponentModel.DataAnnotations;

namespace webProgramming.Models
{
    public class Article
    {
        public int Id { get; set; }
        public required string SellerId { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; } // HTML content
        public required string Image { get; set; } // URL or base64
        public int LikeCounts { get; set; } = 0;
        public int ViewsCount { get; set; } = 0;
        public bool IsPublished { get; set; } = false; // Default false (0)
        public string? RefusedReason { get; set; } // Null if accepted
        public required string MetaTitle { get; set; }
        public required string MetaDescription { get; set; }
        public int CityId { get; set; }
        public int NeighbId { get; set; }
        public ArticleHouseType HouseType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
    }   // ← هنا سكرت الكلاس بشكل صحيح

    public enum ArticleHouseType
    {
        [Display(Name = "منزل")]
        House = 1,

        [Display(Name = "شقة")]
        Apartment = 2,

        [Display(Name = "فيلا")]
        Villa = 3,

        [Display(Name = "أرض")]
        Plotofland = 4,

        [Display(Name = "فندق")]
        Hotel = 5
    }
}