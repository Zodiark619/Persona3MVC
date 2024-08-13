using Microsoft.EntityFrameworkCore;
using Persona3MVC.Models;

namespace Persona3MVC.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Persona> Personas { get; set; }
    }
}
