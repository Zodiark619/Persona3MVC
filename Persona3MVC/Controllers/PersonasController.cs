using Microsoft.AspNetCore.Mvc;
using Persona3MVC.Models;
using Persona3MVC.Services;
using System;

namespace Persona3MVC.Controllers
{
    [Route("/Admin/[controller]/{action=Index}/{id?}")]

    public class PersonasController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 3;

        public PersonasController(ApplicationDbContext context,IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index(int pageIndex,string? search, string? column, string? orderBy)
        {
            IQueryable<Persona> query = context.Personas;
            if (search != null)
            {
                query = query.Where(p => p.Name.Contains(search) || p.Arcana.Contains(search));
            }

            string[] validColumns = { "Id", "Name", "Level", "Arcana", "Price", "CreatedAt" };
            string[] validOrderBy = { "desc", "asc" };

            if (!validColumns.Contains(column))
            {
                column = "Id";
            }

            if (!validOrderBy.Contains(orderBy))
            {
                orderBy = "desc";
            }


            if (column == "Name")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(x => x.Name);
                }
                else
                {
                    query = query.OrderByDescending(x => x.Name);

                }
            }
            else if (column == "Level")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(x => x.Level);
                }
                else
                {
                    query = query.OrderByDescending(x => x.Level);

                }
            }
            else if (column == "Arcana")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(x => x.Arcana);
                }
                else
                {
                    query = query.OrderByDescending(x => x.Arcana);

                }
            }
            else if (column == "Price")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(x => x.Price);
                }
                else
                {
                    query = query.OrderByDescending(x => x.Price);

                }
            }
            else if (column == "CreatedAt")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(x => x.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(x => x.CreatedAt);

                }
            }
            else
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(x => x.Id);
                }
                else
                {
                    query = query.OrderByDescending(x => x.Id);

                }
            }

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var personas = query.ToList();
            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;
            ViewData["Search"] = search ?? "";
            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;
            return View(personas);
        }
        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Create(PersonaDto personaDto)
        {
            if (personaDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");

            }

            if (!ModelState.IsValid)
            {
                return View(personaDto);

            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(personaDto.ImageFile!.FileName);
            string imageFullPath = environment.WebRootPath + "/personas/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                personaDto.ImageFile.CopyTo(stream);

            }
            Persona persona = new Persona()
            {
                Name = personaDto.Name,
                Level = personaDto.Level,
                Arcana = personaDto.Arcana,
                Price = personaDto.Price,
                Description = personaDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };
            context.Personas.Add(persona);
            context.SaveChanges();
            return RedirectToAction("Index", "Personas");

        }


        public IActionResult Edit(int id)
        {
            var persona = context.Personas.Find(id);
            if (persona == null)
            {
                return RedirectToAction("Index", "Products");

            }
            var personaDto = new PersonaDto()
            {
                Name = persona.Name,
                Level = persona.Level,
                Arcana = persona.Arcana,
                Price = persona.Price,
                Description = persona.Description,

            };
            ViewData["PersonaId"] = persona.Id;
            ViewData["ImageFileName"] = persona.ImageFileName;
            ViewData["CreatedAt"] = persona.CreatedAt.ToString("MM/dd/yyyy");
            return View(personaDto);


        }

        [HttpPost]
        public IActionResult Edit(int id,PersonaDto personaDto)
        {
            var persona = context.Personas.Find(id);
            if (persona == null)
            {
                return RedirectToAction("Index", "Personas");

            }
            if (!ModelState.IsValid)
            {
                ViewData["PersonaId"] = persona.Id;
                ViewData["ImageFileName"] = persona.ImageFileName;
                ViewData["CreatedAt"] = persona.CreatedAt.ToString("MM/dd/yyyy");
                return View(personaDto);
            }
            string newFileName = persona.ImageFileName;
            if (personaDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(personaDto.ImageFile!.FileName);
                string imageFullPath = environment.WebRootPath + "/personas/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    personaDto.ImageFile.CopyTo(stream);

                }
                string oldImageFullPath = environment.WebRootPath + "/personas/" + persona.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }
            persona.Name = personaDto.Name;
            persona.Arcana = personaDto.Arcana;
            persona.Level = personaDto.Level;
            persona.Price = personaDto.Price;

            persona.Description = personaDto.Description;
            persona.ImageFileName = newFileName;

            context.SaveChanges();

            return RedirectToAction("Index", "Personas");

        }

        public IActionResult Delete(int id)
        {
            var persona = context.Personas.Find(id);
            if (persona == null)
            {
                return RedirectToAction("Index", "Personas");

            }
            string imageFullPath = environment.WebRootPath + "/personas/" + persona.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Personas.Remove(persona);
            context.SaveChanges();

            return RedirectToAction("Index", "Personas");

        }
    }
}
