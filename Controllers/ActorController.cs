using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peliculas.Data.Context;
using Peliculas.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Peliculas.Controllers
{
    public class ActorController : Controller
    {
        private readonly AppDBContext _context;

        public ActorController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? mensaje = null)
        {
            var actores = await _context.Actores.ToListAsync();
            ViewBag.Mensaje = mensaje;
            return View("Views/Actor/Index.cshtml", actores);
        }

        public IActionResult Create()
        {
            return View("Views/Actor/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor)
        {
            Console.WriteLine("🟢 Entró al método POST de Create");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("⚠️ Modelo inválido, regresando a la vista Create");
                return View("Views/Actor/Create.cshtml", actor);
            }

            try
            {
                _context.Actores.Add(actor);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Actor guardado correctamente: {actor.Nombre} ({actor.FechaNacimiento:yyyy-MM-dd})");

                return RedirectToAction(nameof(Index), new { mensaje = "Actor guardado correctamente ✅" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al guardar el actor.");
                return View("Views/Actor/Create.cshtml", actor);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var actor = await _context.Actores
                .Include(a => a.Peliculas)
                    .ThenInclude(ap => ap.Pelicula)
                        .ThenInclude(p => p.Genero)
                .Include(a => a.Peliculas)
                    .ThenInclude(ap => ap.Pelicula)
                        .ThenInclude(p => p.Director)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            return View("Views/Actor/Details.cshtml", actor);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _context.Actores.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View("Views/Actor/Edit.cshtml", actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Actor actor)
        {
            if (id != actor.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                Console.WriteLine("⚠️ Modelo inválido al editar");
                return View("Views/Actor/Edit.cshtml", actor);
            }

            try
            {
                _context.Update(actor);
                await _context.SaveChangesAsync();
                Console.WriteLine("✏️ Actor actualizado correctamente.");
                return RedirectToAction(nameof(Index), new { mensaje = "Actor actualizado ✅" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Actores.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al actualizar: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al actualizar el actor.");
                return View("Views/Actor/Edit.cshtml", actor);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _context.Actores.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            return View("Views/Actor/Delete.cshtml", actor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actores.FindAsync(id);
            if (actor != null)
            {
                _context.Actores.Remove(actor);
                await _context.SaveChangesAsync();
                Console.WriteLine("🗑️ Actor eliminado correctamente.");
            }

            return RedirectToAction(nameof(Index), new { mensaje = "Actor eliminado 🗑️" });
        }
    }
}
