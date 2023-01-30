using System.ComponentModel.DataAnnotations;

namespace ProyectoCompilarNet.BussinesLogic.Models
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "La propiedad RefreshToken es requerida")]
        public string RefreshToken { get; set; }
    }
}
