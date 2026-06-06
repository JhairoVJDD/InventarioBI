using System.ComponentModel.DataAnnotations;
namespace InventarioBI.Models
{
    public class MovimientoInventario
    {
        [Key]
        public int IdMovimiento { get; set; }

        public int IdProducto { get; set; }
        public Producto? Producto { get; set; }

        public int IdTienda { get; set; }

        public string TipoMovimiento { get; set; } = string.Empty; // ENTRADA, SALIDA, AJUSTE, MERMA
        public decimal Cantidad { get; set; }
        public decimal StockAnterior { get; set; }
        public decimal StockNuevo { get; set; }

        public string Motivo { get; set; } = string.Empty;
        public string UsuarioResponsable { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}