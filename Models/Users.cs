using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace webProgramming.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserId { get; set; }= string.Empty;
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string? Phone { get; set; }
        public string? Image { get; set; }
        public string? OfficePhoto { get; set; }
        public string? BusinessDocuments { get; set; }
        public UserRole Role { get; set; }
        public bool IsBlocked { get; set; } = false;
        public bool IsVerified { get; set; }
        public int? BlockedBy { get; set; } // Reference to another user
        public string? BlockReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum UserRole
    {
        Admin = 1,
        Seller = 2,
        Buyer = 3
    }
}
