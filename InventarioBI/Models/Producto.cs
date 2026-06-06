using System.ComponentModel.DataAnnotations;

namespace InventarioBI.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required]
        public string CodigoBarras { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;
        public string Subcategoria { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;

        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }

        public int StockActual { get; set; } = 0;
        public bool Activo { get; set; } = true;
    }
}
