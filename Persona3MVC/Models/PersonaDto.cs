using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Persona3MVC.Models
{
    public class PersonaDto
    {


        [Required,MaxLength(100)]
        public string Name { get; set; } = "";
        [Required, MaxLength(100)]

        public string Arcana { get; set; } = "";

        [Required,Range(1,100)]
        public int Level { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; } = "";
       
        public IFormFile? ImageFile { get; set; } 






    }
}
