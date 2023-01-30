
using System.ComponentModel.DataAnnotations;

public class LoginUser
{
    [Required(ErrorMessage = "La propiedad UserName es requerida")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "La propiedad Password es requerida")]
    public string Password { get; set; }
    public string Rol { get; set; }
}