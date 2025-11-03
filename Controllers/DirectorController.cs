using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peliculas.Data.Context;
using Peliculas.Models;
using System.Threading.Tasks;

namespace Peliculas.Controllers
{
    public class DirectorMVCController : Controller
    {
        private readonly AppDBContext _context;

        public DirectorMVCController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var directores = await _context.Directores.ToListAsync();
            return View("Views/Director/Index.cshtml", directores);
        }

        public async Task<IActionResult> Details(int id)
        {
            var director = await _context.Directores
                .Include(d => d.Peliculas).ThenInclude(p => p.Genero)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (director == null) return NotFound();
            return View("Views/Director/Details.cshtml", director);
        }

        public IActionResult Create()
        {
            return View("Views/Director/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Director director)
        {
            if (!ModelState.IsValid)
                return View("Views/Director/Create.cshtml", director);

            _context.Directores.Add(director);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var director = await _context.Directores.FindAsync(id);
            if (director == null) return NotFound();
            return View("Views/Director/Edit.cshtml", director);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Director director)
        {
            if (id != director.Id) return BadRequest();
            if (!ModelState.IsValid) return View("Views/Director/Edit.cshtml", director);

            _context.Update(director);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var director = await _context.Directores.FindAsync(id);
            if (director == null) return NotFound();
            return View("Views/Director/Delete.cshtml", director);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var director = await _context.Directores.FindAsync(id);
            if (director != null)
            {
                _context.Directores.Remove(director);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
