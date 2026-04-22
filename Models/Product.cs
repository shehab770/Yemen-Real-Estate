using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webProgramming.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public required string Title { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;
        public string YearBuilt { get; set; } = string.Empty;
        public required string SellerId { get; set; }
        public required int CountryId { get; set; }
        public required int CityId { get; set; }
        public required int NeighbsId { get; set; }
        public PropertyStatus isAccepted { get; set; } = PropertyStatus.Pending; // القيمة الافتراضية
        public string BlockedReason { get; set; } = string.Empty;
        public required string Address { get; set; } = string.Empty;
        public PropertyType Type { get; set; }
        public HouseType EstateType { get; set; }
        public PropertyCategoryType CategoryType { get; set; }
        public string Properties { get; set; } = string.Empty; // مخزنة كـ JSON
        public string Description { get; set; } = string.Empty; // محتوى HTML
        public string MainImage { get; set; } = string.Empty; // صورة رئيسية
        public string ListingImages { get; set; } = string.Empty; // صور متعددة
        public int FavorCount { get; set; } = 0;
        public bool IsFeatured { get; set; } = false; // هل العقار مميز

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties (Entity Framework)
        public User? Seller { get; set; }
        public Country? Country { get; set; }
        public City? City { get; set; }
        public Neighb? Neighbs { get; set; }
    }

    public enum PropertyType
    {
        [Display(Name = "بيع")]
        Sale = 1,

        [Display(Name = "إيجار")]
        Rent = 2,

        [Display(Name = "مباع")]
        Sold = 3,

        [Display(Name = "مؤجر")]
        Rented = 4
    }

    public enum PropertyCategoryType
    {
        [Display(Name = "بيع")]
        Selling = 0,

        [Display(Name = "يومي")]
        Daily = 1,

        [Display(Name = "شهري")]
        Monthly = 2,

        [Display(Name = "سنوي")]
        Yearly = 3,
    }

    public enum HouseType
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
        Hotel = 5,

         [Display(Name = "شاليه")]
        Chalet = 6
    }

    public enum PropertyStatus
    {
        [Display(Name = "قيد الانتظار")]
        Pending = 0,

        [Display(Name = "مقبول")]
        Approved = 1,

        [Display(Name = "مرفوض")]
        Rejected = 2,
    }
}