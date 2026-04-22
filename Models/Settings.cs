using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace webProgramming.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public required string SiteAddress { get; set; }
        public required string SiteEmail { get; set; }

        public required string SitePhone { get; set; }

        public string? LogoImage { get; set; }
        
        public string? FooterLogoImage { get; set; }

        public required string SocialMediaLink { get; set; }
        public required string CopyrightText { get; set; }

    }
}
