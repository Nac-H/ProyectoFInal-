using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peliculas.Data.Context;
using Peliculas.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Controllers
{
    public class PeliculaMVCController : Controller
    {
        private readonly AppDBContext _context;

        public PeliculaMVCController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q = null, int? generoId = null, int? directorId = null, string? mensaje = null)
        {
            var query = _context.Peliculas
                .Include(p => p.Genero)
                .Include(p => p.Director)
                .Include(p => p.Actores).ThenInclude(ap => ap.Actor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => p.Titulo.Contains(q));
            if (generoId.HasValue && generoId.Value > 0)
                query = query.Where(p => p.GeneroId == generoId.Value);
            if (directorId.HasValue && directorId.Value > 0)
                query = query.Where(p => p.DirectorId == directorId.Value);

            ViewBag.Generos = await _context.Generos.ToListAsync();
            ViewBag.Directores = await _context.Directores.ToListAsync();
            ViewBag.FiltroTexto = q;
            ViewBag.FiltroGeneroId = generoId ?? 0;
            ViewBag.FiltroDirectorId = directorId ?? 0;
            ViewBag.Mensaje = mensaje;

            var peliculas = await query.ToListAsync();
            return View("Views/Pelicula/Index.cshtml", peliculas);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pelicula = await _context.Peliculas
                .Include(p => p.Genero)
                .Include(p => p.Director)
                .Include(p => p.Actores).ThenInclude(ap => ap.Actor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula == null) return NotFound();
            return View("Views/Pelicula/Details.cshtml", pelicula);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Generos = await _context.Generos.ToListAsync();
            ViewBag.Directores = await _context.Directores.ToListAsync();
            ViewBag.Actores = await _context.Actores.ToListAsync();
            ViewBag.SelectedActorIds = new List<int>();
            return View("Views/Pelicula/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pelicula pelicula, List<int> SelectedActorIds)
        {
            ModelState.Remove(nameof(Pelicula.Poster));
            ModelState.Remove(nameof(Pelicula.Actores));
            ModelState.Remove(nameof(Pelicula.Genero));
            ModelState.Remove(nameof(Pelicula.Director));

            if (pelicula.GeneroId <= 0)
                ModelState.AddModelError(nameof(pelicula.GeneroId), "Selecciona un género válido.");
            if (pelicula.DirectorId <= 0)
                ModelState.AddModelError(nameof(pelicula.DirectorId), "Selecciona un director válido.");

            if (!ModelState.IsValid)
            {
                ViewBag.Generos = await _context.Generos.ToListAsync();
                ViewBag.Directores = await _context.Directores.ToListAsync();
                ViewBag.Actores = await _context.Actores.ToListAsync();
                ViewBag.SelectedActorIds = SelectedActorIds ?? new List<int>();
                return View("Views/Pelicula/Create.cshtml", pelicula);
            }

            if (pelicula.PosterFile is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await pelicula.PosterFile.CopyToAsync(ms);
                pelicula.Poster = ms.ToArray();
            }

            pelicula.Actores ??= new List<ActorPelicula>();
            foreach (var actorId in (SelectedActorIds ?? new List<int>()).Distinct())
                pelicula.Actores.Add(new ActorPelicula { ActorId = actorId });

            _context.Peliculas.Add(pelicula);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { mensaje = "Película creada ✅" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pelicula = await _context.Peliculas
                .Include(p => p.Actores)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pelicula == null) return NotFound();

            ViewBag.Generos = await _context.Generos.ToListAsync();
            ViewBag.Directores = await _context.Directores.ToListAsync();
            ViewBag.Actores = await _context.Actores.ToListAsync();
            ViewBag.SelectedActorIds = pelicula.Actores.Select(ap => ap.ActorId).ToList();

            return View("Views/Pelicula/Edit.cshtml", pelicula);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pelicula formData, List<int> SelectedActorIds)
        {
            if (id != formData.Id) return BadRequest();

            var pelicula = await _context.Peliculas
                .Include(p => p.Actores)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pelicula == null) return NotFound();

            ModelState.Remove(nameof(Pelicula.Poster));
            ModelState.Remove(nameof(Pelicula.Actores));
            ModelState.Remove(nameof(Pelicula.Genero));
            ModelState.Remove(nameof(Pelicula.Director));

            if (formData.GeneroId <= 0)
                ModelState.AddModelError(nameof(formData.GeneroId), "Selecciona un género válido.");
            if (formData.DirectorId <= 0)
                ModelState.AddModelError(nameof(formData.DirectorId), "Selecciona un director válido.");

            if (!ModelState.IsValid)
            {
                ViewBag.Generos = await _context.Generos.ToListAsync();
                ViewBag.Directores = await _context.Directores.ToListAsync();
                ViewBag.Actores = await _context.Actores.ToListAsync();
                ViewBag.SelectedActorIds = SelectedActorIds ?? new List<int>();
                return View("Views/Pelicula/Edit.cshtml", formData);
            }

            pelicula.Titulo = formData.Titulo;
            pelicula.Sinopsis = formData.Sinopsis;
            pelicula.DuracionMinutos = formData.DuracionMinutos;
            pelicula.FechaEstreno = formData.FechaEstreno;
            pelicula.GeneroId = formData.GeneroId;
            pelicula.DirectorId = formData.DirectorId;

            if (formData.PosterFile is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await formData.PosterFile.CopyToAsync(ms);
                pelicula.Poster = ms.ToArray();
            }

            var nuevos = (SelectedActorIds ?? new List<int>()).Distinct().ToHashSet();
            var actuales = pelicula.Actores.Select(ap => ap.ActorId).ToHashSet();

            pelicula.Actores = pelicula.Actores.Where(ap => nuevos.Contains(ap.ActorId)).ToList();
            foreach (var actorId in nuevos.Except(actuales))
                pelicula.Actores.Add(new ActorPelicula { PeliculaId = pelicula.Id, ActorId = actorId });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { mensaje = "Película actualizada ✏️✅" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var pelicula = await _context.Peliculas
                .Include(p => p.Genero)
                .Include(p => p.Director)
                .Include(p => p.Actores).ThenInclude(ap => ap.Actor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula == null) return NotFound();
            return View("Views/Pelicula/Delete.cshtml", pelicula);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula != null)
            {
                _context.Peliculas.Remove(pelicula);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { mensaje = "Película eliminada 🗑️" });
        }
    }
}
