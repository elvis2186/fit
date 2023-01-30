using Microsoft.EntityFrameworkCore;
using Moq;
using ProyectoCompilarNet.BussinesLogic.Services;
using ProyectoCompilarNet.BussinesLogic.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCompilarNet
{
    public class OrdenServiceUnitTest
    {
        private IOrden? _iorden;

        private async Task<ContextoServer> GetDatabaseContext_Cuestionarios()
        {
            var options = new DbContextOptionsBuilder<ContextoServer>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var contextoServer = new ContextoServer(options);

            contextoServer.Database.EnsureCreated();

            if (await contextoServer.Cuestionarios.CountAsync() <= 0)
            {
                for (int i = 1; i <= 2; i++)
                {
                    contextoServer.Cuestionarios.Add(new Cuestionarios()
                    {
                        CodigoSegmento = "09080101E0034",
                        Llave = "ABC8484812ETA"
                    });
                    await contextoServer.SaveChangesAsync();
                }
            }

            return contextoServer;
        }


        [Fact]
        public async Task ContextPass_asynchronus_Test()
        {
            //arrange
            var LLAVE = "ABC8484812ETA";
            
            var dbContext = await GetDatabaseContext_Cuestionarios();    

            _iorden = new Orden(dbContext);

            //Act async        
            var res = await _iorden.GetSegmentos(LLAVE);
   

            //Assert
            Assert.NotNull(res);
            Assert.True(res.Count == 2, "Existen 2 cuestionarios devueltos en la lista");
            Assert.Equal(2, res.Count);
        }


        [Fact]
        public async Task ContextNotFound_asynchronus_Test()
        {
            //arrange
            var LLAVE = "AS1233B44323";

            var dbContext = await GetDatabaseContext_Cuestionarios();

            _iorden = new Orden(dbContext);

            //Act async        
            var res = await _iorden.GetSegmentos(LLAVE);

            //Assert
            Assert.Null(res);            
        }

        [Fact]
        public async Task ContextFailureException_asynchronus_Test()
        {
            //arrange
            var LLAVE = "AS1233B44323";

            var mockContext = new Mock<ContextoServer>();
            mockContext.Setup(x => x.Cuestionarios).Throws(new Exception());

            _iorden = new Orden(mockContext.Object);

            //Act async        
            var res = await _iorden.GetSegmentos(LLAVE);

            //Assert
            Assert.Null(res);        
        }


        [Fact(Timeout = 6000)]
        public async Task ContextPassTimeout_asynchronus_Test()
        {
            //arrange
            var LLAVE = "ABC8484812ETA";

            var dbContext = await GetDatabaseContext_Cuestionarios();

            _iorden = new Orden(dbContext);

            //Act async        
            var res = await _iorden.GetSegmentos(LLAVE);
            await Task.Delay(5000);


            //Assert
            Assert.NotNull(res);
            Assert.True(res.Count == 2, "Existen 2 cuestionarios devueltos en la lista");
            Assert.Equal(2, res.Count);
        }

    }
}
