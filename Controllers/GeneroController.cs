using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peliculas.Data.Context;
using Peliculas.Models;
using System.Threading.Tasks;

namespace Peliculas.Controllers
{
    public class GeneroMVCController : Controller
    {
        private readonly AppDBContext _context;

        public GeneroMVCController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var generos = await _context.Generos.ToListAsync();
            return View("Views/Genero/Index.cshtml", generos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var genero = await _context.Generos
                .Include(g => g.Peliculas).ThenInclude(p => p.Director)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (genero == null) return NotFound();
            return View("Views/Genero/Details.cshtml", genero);
        }

        public IActionResult Create()
        {
            return View("Views/Genero/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genero genero)
        {
            if (!ModelState.IsValid)
                return View("Views/Genero/Create.cshtml", genero);

            _context.Generos.Add(genero);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var genero = await _context.Generos.FindAsync(id);
            if (genero == null) return NotFound();
            return View("Views/Genero/Edit.cshtml", genero);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Genero genero)
        {
            if (id != genero.Id) return BadRequest();
            if (!ModelState.IsValid) return View("Views/Genero/Edit.cshtml", genero);

            _context.Update(genero);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var genero = await _context.Generos.FindAsync(id);
            if (genero == null) return NotFound();
            return View("Views/Genero/Delete.cshtml", genero);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genero = await _context.Generos.FindAsync(id);
            if (genero != null)
            {
                _context.Generos.Remove(genero);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
