using System.ComponentModel.DataAnnotations;

namespace InventarioBI.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activa { get; set; } = true;

        // Relación inversa
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}