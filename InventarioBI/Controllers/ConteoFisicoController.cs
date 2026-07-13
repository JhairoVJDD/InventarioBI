using InventarioBI.Data;
using InventarioBI.Models;
using InventarioBI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioBI.Controllers
{
    [Authorize]
    public class ConteoFisicoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ExcelService _ex;

        public ConteoFisicoController(ApplicationDbContext context, ExcelService excelService)
        {
            _context = context;
            _ex = excelService;
        }

        public async Task<IActionResult> ExportarExcel()
        {
            var conteos = await _context.ConteosFisicos
                .Include(c => c.Producto)
                .OrderByDescending(c => c.FechaConteo)
                .ToListAsync();
            var excelData = _ex.ExportarConteos(conteos);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ConteosFisicos{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        public async Task<IActionResult> Index()
        {
            var conteos = await _context.ConteosFisicos
                .Include(c => c.Producto)
                .OrderByDescending(c => c.FechaConteo)
                .ToListAsync();

            ViewBag.Tiendas = await _context.Tiendas.ToDictionaryAsync(t => t.IdTienda, t => t.Nombre);
            return View(conteos);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Productos = await _context.Productos
                .Where(p => p.Activo)
                .Select(p => new { p.IdProducto, p.Descripcion, p.StockActual })
                .ToListAsync();

            ViewBag.Tiendas = await _context.Tiendas.OrderBy(t => t.Nombre).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConteoFisico conteoFisico)
        {
            if (ModelState.IsValid)
            {
                var producto = await _context.Productos.FindAsync(conteoFisico.IdProducto);
                if (producto == null) return NotFound();

                conteoFisico.StockSistema = producto.StockActual;
                conteoFisico.FechaConteo = DateTime.Now;
                // Determinar estado basado en si hay diferencia entre stock físico y sistema
                conteoFisico.Estado = conteoFisico.Diferencia != 0 ? "Pendiente" : "Resuelto";

                _context.Add(conteoFisico);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Productos = await _context.Productos
                .Where(p => p.Activo)
                .Select(p => new { p.IdProducto, p.Descripcion, p.StockActual })
                .ToListAsync();
            ViewBag.Tiendas = await _context.Tiendas.OrderBy(t => t.Nombre).ToListAsync();
            return View(conteoFisico);
        }

        // GET: Ver todas las anomalías pendientes
        public async Task<IActionResult> Anomalias()
        {
            var anomalias = await _context.ConteosFisicos
                .Include(c => c.Producto)
                .Where(c => c.Estado == "Pendiente" || c.Estado == "Investigado")
                .OrderByDescending(c => c.FechaConteo)
                .ToListAsync();

            ViewBag.Tiendas = await _context.Tiendas.ToDictionaryAsync(t => t.IdTienda, t => t.Nombre);
            return View(anomalias);
        }

        // GET: Gestionar Anomalía (mostrar detalles)
        public async Task<IActionResult> Gestionar(int id)
        {
            var conteo = await _context.ConteosFisicos
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(c => c.IdConteo == id);

            if (conteo == null) return NotFound();

            ViewBag.Tiendas = await _context.Tiendas.ToDictionaryAsync(t => t.IdTienda, t => t.Nombre);
            return View(conteo);
        }

        // POST: Gestionar Anomalía
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gestionar(int id, string Estado, string Comentario)
        {
            if (string.IsNullOrEmpty(Estado))
            {
                ModelState.AddModelError("", "Debe seleccionar un estado.");
                return RedirectToAction(nameof(Gestionar), new { id });
            }

            var conteo = await _context.ConteosFisicos.FindAsync(id);
            if (conteo == null)
            {
                return NotFound();
            }

            conteo.Estado = Estado;
            conteo.Comentario = Comentario ?? string.Empty;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Estado actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                return RedirectToAction(nameof(Gestionar), new { id });
            }
        }
        // GET: Ver Detalle de un Conteo
        public async Task<IActionResult> Details(int id)
        {
            var conteo = await _context.ConteosFisicos
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(c => c.IdConteo == id);

            if (conteo == null)
            {
                return NotFound();
            }

            ViewBag.Tiendas = await _context.Tiendas.ToDictionaryAsync(t => t.IdTienda, t => t.Nombre);
            return View(conteo);
        }
    }
}