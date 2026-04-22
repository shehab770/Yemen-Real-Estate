using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace webProgramming.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailAvailable", controller: "Auth", ErrorMessage = "Email is already registered.")]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
