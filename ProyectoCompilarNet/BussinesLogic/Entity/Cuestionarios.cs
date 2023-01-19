
using System.ComponentModel.DataAnnotations;

public class Cuestionarios
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(16)]
    public string Llave { get; set; }
    [StringLength(12)]
    public string CodigoSegmento { get; set; }
    [StringLength(2)]
    public string ProvinciaId { get; set; }
    [StringLength(2)]
    public string DistritoId { get; set; }
    [StringLength(2)]
    public string CorregimientoId { get; set; }
    [StringLength(1)]
    public string SubZona { get; set; }
    [StringLength(4)]
    public string Segmento { get; set; }
}