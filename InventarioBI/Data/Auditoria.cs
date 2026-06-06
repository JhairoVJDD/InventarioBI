using System.ComponentModel.DataAnnotations;

public class Auditoria
{
    [Key]
    public int IdAuditoria { get; set; }

    public string TablaAfectada { get; set; } = string.Empty;
    public string Operacion { get; set; } = string.Empty; // CREATE, UPDATE, DELETE
    public string Usuario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Detalles { get; set; } = string.Empty;
}