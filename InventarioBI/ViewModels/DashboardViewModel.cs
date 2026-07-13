    using InventarioBI.Models;

namespace InventarioBI.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProductos { get; set; }
        public decimal StockTotalValorizado { get; set; }
        public int AnomaliasPendientes { get; set; }
        public int TotalMovimientos { get; set; }
        public int TotalTiendas { get; set; }
        public int TotalCategorias { get; set; }

        public List<MovimientoPorTipoViewModel> MovimientosPorTipo { get; set; } = new();
        public List<TopProductoViewModel> TopProductosMovimiento { get; set; } = new();
        public List<ConteoFisico> UltimosConteos { get; set; } = new();
    }

    public class MovimientoPorTipoViewModel
    {
        public string Tipo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class TopProductoViewModel
    {
        public string Producto { get; set; } = string.Empty;
        public int TotalMovimientos { get; set; }
    }
}