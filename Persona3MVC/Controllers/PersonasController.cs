using Microsoft.AspNetCore.Mvc;
using Persona3MVC.Models;
using Persona3MVC.Services;
using System;

namespace Persona3MVC.Controllers
{
    public class PersonasController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public PersonasController(ApplicationDbContext context,IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var personas=context.Personas.OrderByDescending(x=>x.Id).ToList();
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
