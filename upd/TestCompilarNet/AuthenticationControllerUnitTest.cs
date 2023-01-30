using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using ProyectoCompilarNet.BussinesLogic.Models;
using ProyectoCompilarNet.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestCompilarNet
{
    public class AuthenticationControllerUnitTest
    {

        private async Task<ContextoServer> GetDatabaseContext_User()
        {
            var options = new DbContextOptionsBuilder<ContextoServer>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var contextoServer = new ContextoServer(options);

            contextoServer.Database.EnsureCreated();

            if(await contextoServer.Usuarios.CountAsync() <= 0)
            {
                for(int i=1; i<=1; i++)
                {
                    contextoServer.Usuarios.Add(new Usuarios()
                    {
                          UserName = "S00001",
                          Codigo = "S00001",
                          Password = "0-000-0000",
                          Rol = "S",
                          Region = "SANTIAGO"
                    });
                    await contextoServer.SaveChangesAsync();
                }
            }

            return contextoServer;
        }

        [Fact]
        public async Task LoginSuccesAsyncronousTest()
        {
            var USUARIO = new LoginUser()
            {
                UserName = "S00001",
                Password = "0-000-0000",
                Rol = "S"
            };

            var ACCESS_TOKEN = "abc28342384784adad";

            var REFRESH_TOKEN = "nmp12383248324dfg";

            var contextoServer = await GetDatabaseContext_User();            
            
            var mockJwtManager = new Mock<IJwtAuthManager>();
            
            mockJwtManager
                .Setup(x => x.GenerateTokens(It.IsAny<string>(), It.IsAny<Claim[]>(), It.IsAny<DateTime>()))
                .ReturnsAsync(
                     new JwtAuthResult()
                     {
                         AccessToken = ACCESS_TOKEN,
                         RefreshToken = new RefreshToken()
                         {
                             ExpireAt = DateTime.UtcNow,
                             TokenString = REFRESH_TOKEN,
                             Username = USUARIO.UserName
                         }
                     }
                );

            ////
            /*mockJwtManager
                .Setup(x => x.GenerateTokens(It.IsAny<string>(), It.IsAny<Claim[]>(), It.IsAny<DateTime>()))
                .Returns(async () => { 
                  await Task.Yield();
                    new JwtAuthResult()
                    {
                        AccessToken = "abc28342384784adad",
                        RefreshToken = new RefreshToken()
                        {
                            ExpireAt = DateTime.UtcNow,
                            TokenString = "nmp12383248324dfg",
                            Username = "S00001"
                        } 
                    };
                });*/

            var authController = new AuthController(null, contextoServer, mockJwtManager.Object,null);


            var result = await authController.PostLogin(USUARIO);

            mockJwtManager.Verify(m => m.GenerateTokens(It.IsAny<string>(), It.IsAny<Claim[]>(), It.IsAny<DateTime>()), Times.Once());

            
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);                      

            var jsonString = JsonConvert.SerializeObject(okResult.Value);

            var usuarioLogeado = JsonConvert.DeserializeObject<UserAuthenticate>(jsonString);

            var tokenActual = usuarioLogeado?.AccessToken;

            Assert.Equal(ACCESS_TOKEN, tokenActual);
        }

        [Fact]
        public async Task LogoutPassSuccessAsyncronousTest()
        {
            //arrange
            var contextoServer = await GetDatabaseContext_User();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "S00001")
            }, "mock"));

            var mockJwtAuthManager = new Mock<IJwtAuthManager>();

            mockJwtAuthManager
                .Setup(m => m.RemoveRefreshTokenByUsername(It.IsAny<string>()))
                .Returns(async () =>
                  {
                      await Task.Yield();
                  }
                );

            var authController = new AuthController(null, contextoServer, mockJwtAuthManager.Object, null);

            authController.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            //Act async
            var result = await authController.Logout();

            //Assert
            mockJwtAuthManager.Verify(m => m.RemoveRefreshTokenByUsername(It.IsAny<string>()), Times.Once);

            Assert.NotNull(result);

            Assert.IsType<OkResult>(result);

        }

        [Fact]
        public async Task RefreshSuccessAsynchronousTest()
        {
            //arrange
            var USUARIO = new LoginUser()
            {
                UserName = "S00001",
                Password = "0-000-0000",
                Rol = "S"
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name,"S00001"),
                new Claim(ClaimTypes.GivenName,"S00001"),
                new Claim(ClaimTypes.Role,"S"),
                new Claim(ClaimTypes.Country,"SANTIAGO")
            }, "mock"));            

            var ACCESS_TOKEN = "abc28342384784adad";
            var REFRESH_TOKEN = "nmp12383248324dfg";

            var REQUEST = new RefreshTokenRequest()
            {
                RefreshToken = REFRESH_TOKEN
            };

            var contextoServer = await GetDatabaseContext_User();

            var mockJwtManager = new Mock<IJwtAuthManager>();

            mockJwtManager
                .Setup(m => m.Refresh(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(
                  new JwtAuthResult()
                  {
                       AccessToken = ACCESS_TOKEN,
                       RefreshToken = new RefreshToken()
                       {
                          ExpireAt = DateTime.UtcNow,
                          TokenString = REFRESH_TOKEN,
                          Username = USUARIO.UserName
                       }
                  }
                );

            var authController = new AuthController(null, contextoServer, mockJwtManager.Object, null);

            authController.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            //Act async
            var result = await authController.Refresh(REQUEST);

            //Assert
            Assert.NotNull(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);

            var jsonString = JsonConvert.SerializeObject(okObjectResult.Value);

            var usuarioAutenticado = JsonConvert.DeserializeObject<dynamic>(jsonString);

            string refreshTokenActual = usuarioAutenticado?.RefreshToken;

            Assert.Equal(REFRESH_TOKEN, refreshTokenActual);
                
        }

        [Fact]
        public async Task LoginNotFoundAsynchronousTest()
        {
            var context = await GetDatabaseContext_User();

            var USUARIO = "EYU123301";

            var PASSWD = "321445";

            var mockJWTManager = new Mock<IJwtAuthManager>();

            mockJWTManager.Setup(m => m.GenerateTokens(It.IsAny<string>(), It.IsAny<Claim[]>(), It.IsAny<DateTime>()))
                .ReturnsAsync(
                  new JwtAuthResult()
                  {
                      AccessToken = "abc1321"
                  });


            var authController = new AuthController(null, context, mockJWTManager.Object, null);

            var result =  await authController.PostLogin(new LoginUser { UserName = USUARIO, Password = PASSWD });

            mockJWTManager.Verify(m=> m.GenerateTokens(It.IsAny<string>(),It.IsAny<Claim[]>(), It.IsAny<DateTime>()), Times.Never);

            
            Xunit.Assert.NotNull(result);
            Xunit.Assert.IsType<UnauthorizedResult>(result);

        }

        [Fact]
        public async Task LogoutUserNotFoundAsyncronousTest()
        {
            var contexto = await GetDatabaseContext_User();

            var mockJwtManager = new Mock<IJwtAuthManager>();
            mockJwtManager.Setup(m => m.RemoveRefreshTokenByUsername(It.IsAny<string>()))
                .Returns(async () =>
                {
                    await Task.Yield();
                });

            var authController = new AuthController(null, contexto, mockJwtManager.Object, null);

            var result = await authController.Logout();

            mockJwtManager.Verify(m => m.RemoveRefreshTokenByUsername(It.IsAny<string>()), Times.Never());

            Xunit.Assert.NotNull(result);
            Xunit.Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task LogoutUserBadRequestAsyncronousTest()
        {
            var contexto = await GetDatabaseContext_User();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {               
            }, "mock"));

            var mockJwtManager = new Mock<IJwtAuthManager>();
            mockJwtManager.Setup(m => m.RemoveRefreshTokenByUsername(It.IsAny<string>()))
                .Returns(async () =>
                {
                    await Task.Yield();
                });

            var authController = new AuthController(null, contexto, mockJwtManager.Object, null);

            authController.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            var result = await authController.Logout();

            mockJwtManager.Verify(m => m.RemoveRefreshTokenByUsername(It.IsAny<string>()), Times.Never());

            Xunit.Assert.NotNull(result);
            Xunit.Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RefreshTokenNotFoundAsyncronousTest()
        {
            var mockJwtAuthManager = new Mock<IJwtAuthManager>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name,"S00001"),
                new Claim(ClaimTypes.GivenName,"S00001"),
                new Claim(ClaimTypes.Role,"S"),
                new Claim(ClaimTypes.Country,"SANTIAGO")
            }, "mock"));

            mockJwtAuthManager.Setup(m => m.Refresh(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new JwtAuthResult()
                {
                    AccessToken = "13432432",
                    RefreshToken = new RefreshToken()
                    {
                         TokenString = "3284hfuhfh"
                    }
                });

            var contexto = await GetDatabaseContext_User();

            var authController = new AuthController(null, contexto, mockJwtAuthManager.Object, null);

            authController.ControllerContext.HttpContext = new DefaultHttpContext() { User = user};


            var resultado = await authController.Refresh(new RefreshTokenRequest());

            mockJwtAuthManager.Verify(m => m.Refresh(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);

            Xunit.Assert.NotNull(resultado);
            Xunit.Assert.IsType<UnauthorizedObjectResult>(resultado);

        }

        [Fact]
        public async Task RefreshRetrieveFailAsyncronousTest()
        {
            var mockJwtAuthManager = new Mock<IJwtAuthManager>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {}));

            mockJwtAuthManager.Setup(m => m.Refresh(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new JwtAuthResult()
                {
                      AccessToken = "8324832742348h",
                      RefreshToken = new RefreshToken()
                      {
                           TokenString = "8234hhs834823"
                      }
                });

            var contexto = await GetDatabaseContext_User();

            var authController = new AuthController(null, contexto, mockJwtAuthManager.Object, null);

            authController.ControllerContext.HttpContext = new DefaultHttpContext() { User = user};

            var resultado = await authController.Refresh(new RefreshTokenRequest() { RefreshToken = "abc1223123123" });

            Xunit.Assert.NotNull(resultado);
            Xunit.Assert.IsType<BadRequestObjectResult>(resultado);
        }
    }
}
