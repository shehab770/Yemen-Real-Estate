using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace webProgramming.Models
{
    public class City
    {
        public int Id { get; set; }
        public int CountryId{get;set;}

        public required string Name { get; set; }
    }
}
