using Microsoft.EntityFrameworkCore;

namespace ProyectoCompilarNet.BussinesLogic.Services.Implementation
{
    public class Orden : IOrden
    {
        public readonly ContextoServer _contexto;

        public Orden (ContextoServer contexto)
        {
            _contexto = contexto;
        }
        public async Task<List<Cuestionarios>> GetSegmentos(string llave)
        {
            try
            {
                var cuestionarios = await _contexto.Cuestionarios.Where(c => c.Llave.StartsWith(llave)).ToListAsync();

                if(cuestionarios.Count == 0)
                {
                    return null;
                }

                return (cuestionarios);

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }            
        }
    }
}
