using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProyectoCompilarNet.BussinesLogic.Services;
using ProyectoCompilarNet.Controllers;

namespace TestCompilarNet
{
    public class OrdenControllerUnitTest
    {
        ILogger<CuestionariosController>? _logger;
        
        public OrdenControllerUnitTest()
        {

        }

        [Fact]
        //public async Task RetrieveVal_SynchronousSucces_OrdenTest()
        public void RetrieveVal_SynchronousSucces_OrdenTest()
        {
            var LLAVE = "12345";
            var service = new Mock<IOrden>();
            service.Setup(x => x.GetSegmentos(It.IsAny<string>())).Returns(() => Task.FromResult(new List<Cuestionarios>()));

            var cuestionarioController = new CuestionariosController(_logger, new ContextoServer(), service.Object);

            var result = cuestionarioController.GetCuestionarioBySegmento(LLAVE);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task RetrieveVal_AsynchronousSucces_OrdenTest()
        {
            var LLAVE = "12345";
            var service = new Mock<IOrden>();
            service.Setup(x => x.GetSegmentos(It.IsAny<string>())).Returns(async () =>
            {
                //await Task.Delay(60000);
                await Task.Yield();
                return new List<Cuestionarios>();
            });

            var cuestionarioController = new CuestionariosController(_logger, new ContextoServer(), service.Object);

            var result = await cuestionarioController.GetCuestionarioBySegmento(LLAVE);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task VerifyVal_AsynchronousSucces_OrdenTest()
        {
            //arrange
            var LLAVE = "12345";
            var mockOrden = new Mock<IOrden>();
            mockOrden.Setup(x => x.GetSegmentos(It.IsAny<string>())).Returns(async () =>
            {
                await Task.Yield();
                return new List<Cuestionarios>() { new Cuestionarios()
                {
                     CodigoSegmento = String.Empty,
                     Llave = LLAVE
                } };
            });

            var cuestionarioController = new CuestionariosController(_logger, new ContextoServer(), mockOrden.Object);

            //Act
            var result = await cuestionarioController.GetCuestionarioBySegmento(LLAVE);
            
            mockOrden.Verify(m => m.GetSegmentos(It.IsAny<string>()), Times.Once());

            //Assert
            Assert.NotNull(result);

            var cuestionarioVal = Assert.IsType<OkObjectResult>(result);

            var listaCuestionario = cuestionarioVal.Value as List<Cuestionarios>;

            var llaveActual = listaCuestionario?.First().Llave;

            Assert.Equal(LLAVE, llaveActual);
        }

        [Fact]
        public async Task RetrieveVal_AsynchronousFailure_Orden_ThrowsTest()
        {
            //arrange
            var LLAVE = "12345";
            var mockOrden = new Mock<IOrden>();
            mockOrden.Setup(x => x.GetSegmentos(LLAVE)).Returns(async () =>
            {
                await Task.Yield();
                throw new Exception();
            });

            var cuestionarioController = new CuestionariosController(_logger, new ContextoServer(), mockOrden.Object);
            mockOrden.Verify(m => m.GetSegmentos(It.IsAny<string>()), Times.Never());

            //Act async
            var resulte = await cuestionarioController.GetCuestionarioBySegmento(LLAVE);

            //Assert
            Assert.NotNull(resulte);
            Assert.IsType<BadRequestObjectResult>(resulte);
        }

    }
}