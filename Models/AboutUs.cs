using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace webProgramming.Models
{
    public class AboutUs
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public string? Image { get; set; }

    }
}
