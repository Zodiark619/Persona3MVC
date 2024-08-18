using Microsoft.AspNetCore.Mvc;
using Persona3MVC.Models;
using Persona3MVC.Services;

namespace Persona3MVC.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 4;

        public StoreController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index(int pageIndex,string? search, string? arcana, int? level, string? sort)
        {
            IQueryable<Persona> query = context.Personas;



            if (search != null && search.Length > 0)
            {
                query = query.Where(p => p.Name.Contains(search));
            }


            // filter functionality
            if (arcana != null && arcana.Length > 0)
            {
                query = query.Where(p => p.Arcana.Contains(arcana));
            }

            if (level != null )
            {
                query = query.Where(p => p.Level<=level);
            }

            // sort functionality
            if (sort == "price_asc")
            {
                query = query.OrderBy(p => p.Price);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(p => p.Price);
            }
            else
            {
                // newest products first
                query = query.OrderByDescending(p => p.Id);
            }
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var personas = query.ToList();
            ViewBag.Personas = personas;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            var storeSearchModel = new StoreSearchModel()
            {
                Search = search,
                Arcana = arcana,
                Level = level,
                Sort = sort
            };
            return View(storeSearchModel);
        }


        public IActionResult Details(int id)
        {
            var persona = context.Personas.Find(id);
            if (persona == null)
            {
                return RedirectToAction("Index", "Store");
            }

            return View(persona);
        }
    }
}
