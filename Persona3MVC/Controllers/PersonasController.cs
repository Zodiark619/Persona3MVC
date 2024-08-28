using Microsoft.AspNetCore.Mvc;
using Persona3MVC.Models;
using Persona3MVC.Services;
using System.Linq;

namespace Persona3MVC.Controllers
{
    public class PersonasController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 5;
        public PersonasController(ApplicationDbContext context,IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index(int pageIndex,string? search,string? column,string? orderBy)
        {
            IQueryable<Persona> query = context.Personas;
            if (search != null)
            {
                query = query.Where(p=> p.Name.Contains(search)||p.Arcana.Contains(search));

            }

            string[] validColumns = { "Id", "Name", "Arcana", "Level", "Price", "CreatedAt" };
            string[] validOrderBy = { "desc","asc" };

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
                    query = query.OrderBy(p => p.Name);

                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);

                }
            }
           else if (column == "Arcana")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Arcana);

                }
                else
                {
                    query = query.OrderByDescending(p => p.Arcana);

                }
            }
           else if (column == "Level")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Level);

                }
                else
                {
                    query = query.OrderByDescending(p => p.Level);

                }
            }
            else if (column == "Price")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Price);

                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);

                }
            }
            else if (column == "CreatedAt")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);

                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);

                }
            }
            else 
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Id);

                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);

                }
            }







            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count=query.Count();
            int totalPages=(int)Math.Ceiling(count/pageSize);
            query=query.Skip((pageIndex-1)*pageSize).Take(pageSize);


            var personas=query.ToList();


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
                ModelState.AddModelError("ImageFile", "The image file is required.");
            }
            if (!ModelState.IsValid)
            {
                return View(personaDto);

            }


            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(personaDto.ImageFile!.FileName);

            string imageFullPath=environment.WebRootPath+"/personas/"+newFileName;
            using (var stream =System.IO.File.Create(imageFullPath))
            {
                personaDto.ImageFile.CopyTo(stream);
            }
            Persona persona = new Persona()
            {
                Name = personaDto.Name,
                Arcana=personaDto.Arcana,
                Level=personaDto.Level,
                Price=personaDto.Price,
                CreatedAt=DateTime.Now,
                ImageFileName=newFileName,
                Description=personaDto.Description,
            };
            context.Personas.Add(persona);
            context.SaveChanges();
            return RedirectToAction("Index");
        }



        public IActionResult Edit(int id)
        {
            var persona = context.Personas.Find(id);
            if (persona == null)
            {
                return RedirectToAction("Index");
            }
            var personaDto = new PersonaDto
            {
                Name=persona.Name,
                Arcana=persona.Arcana,
                Level=persona.Level,
                Price=persona.Price,
                Description=persona.Description,
                
            };

            ViewData["PersonaId"] = persona.Id;
            ViewData["ImageFileName"] = persona.ImageFileName;
            ViewData["CreatedAt"] = persona.CreatedAt.ToString("MM/dd/yyyy");
            return View(personaDto);

        }
        [HttpPost]
        public IActionResult Edit(int id, PersonaDto personaDto)
        {
            var persona = context.Personas.Find(id);
            if (persona == null)
            {
                return RedirectToAction("Index");
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
            persona.Level = personaDto.Level;
            persona.Arcana = personaDto.Arcana;
            persona.Description = personaDto.Description;
            persona.Price = personaDto.Price;
            persona.ImageFileName = newFileName;

            context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var persona = context.Personas.Find(id);
            if (persona == null)
            {
                return RedirectToAction("Index");
            }
            string imageFullPath= environment.WebRootPath +"/personas/"+persona.ImageFileName;  
            System.IO.File.Delete(imageFullPath);


            context.Personas.Remove(persona);
            context.SaveChanges();
            return RedirectToAction("Index");


        }

    }
}
