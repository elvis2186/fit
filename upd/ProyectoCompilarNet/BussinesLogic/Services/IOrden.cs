namespace ProyectoCompilarNet.BussinesLogic.Services
{
    public interface IOrden
    {
        public Task<List<Cuestionarios>> GetSegmentos(string llave);
    }
}
