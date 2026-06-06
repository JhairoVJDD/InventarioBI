using InventarioBI.Data;
using InventarioBI.Models;
using InventarioBI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioBI.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalProductos = await _context.Productos.CountAsync(p => p.Activo),
                StockTotalValorizado = await _context.Productos.SumAsync(p => p.StockActual * p.PrecioCosto),
                AnomaliasPendientes = await _context.ConteosFisicos.CountAsync(c => c.Estado == "Pendiente"),
                TotalMovimientos = await _context.MovimientosInventario.CountAsync(),

                // Datos para gráficos
                MovimientosPorTipo = await _context.MovimientosInventario
                    .GroupBy(m => m.TipoMovimiento)
                    .Select(g => new MovimientoPorTipoViewModel{ Tipo = g.Key, Cantidad = g.Count() })
                    .ToListAsync(),

                TopProductosMovimiento = await _context.MovimientosInventario
                    .GroupBy(m => m.Producto!.Descripcion)
                    .Select(g => new TopProductoViewModel { Producto = g.Key, TotalMovimientos = g.Count() })
                    .OrderByDescending(g => g.TotalMovimientos)
                    .Take(5)
                    .ToListAsync(),

                UltimosConteos = await _context.ConteosFisicos
                    .Include(c => c.Producto)
                    .OrderByDescending(c => c.FechaConteo)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}