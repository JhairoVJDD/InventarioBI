using System.ComponentModel.DataAnnotations;

namespace InventarioBI.Models
{
    public class ConteoFisico
    {
        [Key]
        public int IdConteo { get; set; }

        public int IdProducto { get; set; }
        public Producto? Producto { get; set; }

        public int IdTienda { get; set; }
        public decimal StockSistema { get; set; }
        public decimal StockFisico { get; set; }
        public decimal Diferencia => StockFisico - StockSistema;

        public string Estado { get; set; } = "Pendiente"; 

        public string Comentario { get; set; } = string.Empty; 
        public DateTime FechaConteo { get; set; } = DateTime.Now;
    }
}
