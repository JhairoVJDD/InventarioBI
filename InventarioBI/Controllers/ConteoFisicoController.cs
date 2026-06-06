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

            return View(conteos);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Productos = await _context.Productos
                .Where(p => p.Activo)
                .Select(p => new { p.IdProducto, p.Descripcion, p.StockActual })
                .ToListAsync();

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
                conteoFisico.Estado = Math.Abs(conteoFisico.Diferencia) > 0 ? "Pendiente" : "Resuelto";

                _context.Add(conteoFisico);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
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

            return View(anomalias);
        }

        // GET: Gestionar Anomalía (mostrar detalles)
        public async Task<IActionResult> Gestionar(int id)
        {
            var conteo = await _context.ConteosFisicos
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(c => c.IdConteo == id);

            if (conteo == null) return NotFound();

            return View(conteo);
        }

        // POST: Gestionar Anomalía
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gestionar(int id, string nuevoEstado)
        {
            if (string.IsNullOrEmpty(nuevoEstado))
            {
                ModelState.AddModelError("", "Debe seleccionar un estado.");
                return RedirectToAction(nameof(Gestionar), new { id });
            }

            var conteo = await _context.ConteosFisicos.FindAsync(id);
            if (conteo == null)
            {
                return NotFound();
            }

            conteo.Estado = nuevoEstado;

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

            return View(conteo);
        }
    }
}