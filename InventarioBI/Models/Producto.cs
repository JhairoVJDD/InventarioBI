using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Relación con Categoría
        [Required]
        public int Categoria { get; set; }   // ← Este es el FK real en tu BD

        [ForeignKey("Categoria")]
        public Categoria? CategoriaNavigation { get; set; }

        public string? Subcategoria { get; set; }

        // Relación con Marca
        [Required]
        public int Marca { get; set; }      

        [ForeignKey("Marca")]
        public Marca? MarcaNavigation { get; set; }

        [Required]
        public decimal PrecioCosto { get; set; }

        [Required]
        public decimal PrecioVenta { get; set; }

        public int StockActual { get; set; } = 0;

        public bool Activo { get; set; } = true;
    }
}