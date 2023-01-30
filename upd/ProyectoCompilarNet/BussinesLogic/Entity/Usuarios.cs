using System.ComponentModel.DataAnnotations;

public class Usuarios
{

    [Key]
    public int Id { get; set; }
    [StringLength(1)]
    public string Rol { get; set; }
    [StringLength(4)]
    public string Region { get; set; }
    [StringLength(3)]
    public string Zona { get; set; }
    [StringLength(1)]
    public string Numeracion { get; set; }
    [Required]
    [StringLength(9)]
    public string Codigo { get; set; }
    [StringLength(6)]
    public string UserName { get; set; }
    public int? Superior { get; set; }
    [StringLength(50)]
    public string Nombre { get; set; }
    [StringLength(50)]
    public string Password { get; set; }
}