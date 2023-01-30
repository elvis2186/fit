namespace ProyectoCompilarNet.BussinesLogic.Models
{
    public class UserAuthenticate
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Codigo { get; set; }
        public string Rol { get; set; }
        public string Region { get; set; }
        public string AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }

    }
}
