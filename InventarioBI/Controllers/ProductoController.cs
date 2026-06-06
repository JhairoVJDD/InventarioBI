using InventarioBI.Data;
using InventarioBI.Models;
using InventarioBI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioBI.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ExcelService _excelService;

        public ProductoController(ApplicationDbContext context, ExcelService excelService)
        {
            _context = context;
            _excelService = excelService;

        }

        // GET: Lista de Productos
        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Descripcion)
                .ToListAsync();

            return View(productos);
        }

        // GET: Crear Producto
        public IActionResult Create()
        {
            return View();
        }

        // POST: Crear Producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.Activo = true;
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Editar Producto
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        // POST: Editar Producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.IdProducto) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.IdProducto)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Eliminar (Desactivar)
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        // POST: Eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                producto.Activo = false;  // Soft Delete
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }

        // Exportar Productos a Excel
        public async Task<IActionResult> ExportarExcel()
        {
            var productos = await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Descripcion)
                .ToListAsync();

            var bytes = _excelService.ExportarProductos(productos);

            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Productos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }
    }
}