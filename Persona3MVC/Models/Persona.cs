using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Persona3MVC.Models
{
    public class Persona
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = "";
        [MaxLength(100)]

        public string Arcana { get; set; } = "";

        [Range(1, 100)]
        public int Level { get; set; } 
        [Precision(16, 2)]
        public decimal Price { get; set; }
        public string Description { get; set; } = "";
        [MaxLength(100)]

        public string ImageFileName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
