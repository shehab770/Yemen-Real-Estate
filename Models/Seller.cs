using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace webProgramming.Models
{

    
    public class Seller
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailAvailable", controller: "Auth", ErrorMessage = "Email is already registered.")]
        public required string Email { get; set; }

        [Required]
        [Phone]
        [Remote(action: "IsPhoneAvailable", controller: "Auth", ErrorMessage = "Phone is already registered.")]
        public required string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }


        public IFormFile? Image { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile? OfficePhoto { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile? BusinessDocuments { get; set; }

        public bool IsVerified { get; set; }
    }
}
