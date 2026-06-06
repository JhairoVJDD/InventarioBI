using InventarioBI.Data;
using InventarioBI.Models;
using InventarioBI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioBI.Controllers
{
    [Authorize]
    public class MovimientoInventarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ExcelService _excelService;

        // ← UN SOLO CONSTRUCTOR
        public MovimientoInventarioController(ApplicationDbContext context, ExcelService excelService)
        {
            _context = context;
            _excelService = excelService;
        }

        // GET: Lista de Movimientos
        public async Task<IActionResult> Index()
        {
            var movimientos = await _context.MovimientosInventario
                .Include(m => m.Producto)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            return View(movimientos);
        }

        // GET: Crear Movimiento
        public async Task<IActionResult> Create()
        {
            ViewBag.Productos = await _context.Productos
                .Where(p => p.Activo)
                .Select(p => new { p.IdProducto, p.Descripcion, p.StockActual })
                .ToListAsync();

            return View();
        }

        // POST: Crear Movimiento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovimientoInventario movimiento)
        {
            if (ModelState.IsValid)
            {
                var producto = await _context.Productos.FindAsync(movimiento.IdProducto);
                if (producto == null) return NotFound();

                movimiento.StockAnterior = producto.StockActual;
                movimiento.Fecha = DateTime.Now;
                movimiento.UsuarioResponsable = User.Identity?.Name ?? "Sistema";

                switch (movimiento.TipoMovimiento.ToUpper())
                {
                    case "ENTRADA":
                        producto.StockActual += (int)movimiento.Cantidad;
                        break;
                    case "SALIDA":
                    case "MERMA":
                        producto.StockActual -= (int)movimiento.Cantidad;
                        break;
                    case "AJUSTE":
                        producto.StockActual = (int)movimiento.StockNuevo;
                        break;
                }

                movimiento.StockNuevo = producto.StockActual;

                _context.Add(movimiento);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(movimiento);
        }

        // Exportar a Excel
        public async Task<IActionResult> ExportarExcel()
        {
            var movimientos = await _context.MovimientosInventario
                .Include(m => m.Producto)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            var bytes = _excelService.ExportarMovimientos(movimientos);

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Movimientos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }
    }
}