using System.ComponentModel.DataAnnotations;

public class Tienda
{
    [Key]
    public int IdTienda { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string Distrito { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public decimal AreaM2 { get; set; }
}