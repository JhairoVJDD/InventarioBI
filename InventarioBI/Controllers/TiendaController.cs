using InventarioBI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InventarioBI.Controllers
{
    [Authorize]
    public class TiendaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TiendaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tienda
        public async Task<IActionResult> Index()
        {
            var tiendas = await _context.Tiendas.ToListAsync();
            return View(tiendas);
        }

        // GET: Tienda/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tienda/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tienda tienda)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tienda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tienda);
        }

        // GET: Tienda/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tienda = await _context.Tiendas.FindAsync(id);
            if (tienda == null) return NotFound();
            return View(tienda);
        }

        // POST: Tienda/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tienda tienda)
        {
            if (id != tienda.IdTienda) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tienda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TiendaExists(tienda.IdTienda)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tienda);
        }

        // GET: Tienda/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var tienda = await _context.Tiendas.FindAsync(id);
            if (tienda == null) return NotFound();
            return View(tienda);
        }

        // POST: Tienda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tienda = await _context.Tiendas.FindAsync(id);
            if (tienda != null)
            {
                _context.Tiendas.Remove(tienda);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TiendaExists(int id)
        {
            return _context.Tiendas.Any(e => e.IdTienda == id);
        }
    }
}
