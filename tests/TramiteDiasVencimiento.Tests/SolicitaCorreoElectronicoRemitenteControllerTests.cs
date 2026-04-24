using DocuArchi.Api.Controllers.GestionCorrespondencia.PlantillaValidacion;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestionCorrespondencia.PlantillaValidacion.SolicitaCorreoElectronicoRemitente;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaCorreoElectronicoRemitenteControllerTests
{
    [Fact]
    public async Task Get_CuandoClaimEsInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string>
                {
                    success = false,
                    message = "No claim",
                    data = "",
                    errors = []
                }
            });

        var service = new Mock<IServiceSolicitaCorreoElectronicoRemitente>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<SolicitaCorreoElectronicoRemitenteController>>();
        var controller = new SolicitaCorreoElectronicoRemitenteController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var action = await controller.Get(10, 450);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.Verify(s => s.SolicitaCorreoElectronicoRemitenteAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Get_CuandoIdPlantillaEsInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "WF", Response = null });

        var service = new Mock<IServiceSolicitaCorreoElectronicoRemitente>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<SolicitaCorreoElectronicoRemitenteController>>();
        var controller = new SolicitaCorreoElectronicoRemitenteController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var action = await controller.Get(0, 450);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.Verify(s => s.SolicitaCorreoElectronicoRemitenteAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Get_CuandoServiceRetornaError_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "WF", Response = null });

        var service = new Mock<IServiceSolicitaCorreoElectronicoRemitente>();
        service.Setup(s => s.SolicitaCorreoElectronicoRemitenteAsync(10, 450, "WF"))
            .ReturnsAsync(new AppResponses<string> { success = false, message = "Error", data = "", errors = [] });

        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<SolicitaCorreoElectronicoRemitenteController>>();
        var controller = new SolicitaCorreoElectronicoRemitenteController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var action = await controller.Get(10, 450);

        Assert.IsType<BadRequestObjectResult>(action.Result);
    }

    [Fact]
    public async Task Get_CuandoServiceRetornaOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "WF", Response = null });

        var service = new Mock<IServiceSolicitaCorreoElectronicoRemitente>();
        service.Setup(s => s.SolicitaCorreoElectronicoRemitenteAsync(10, 450, "WF"))
            .ReturnsAsync(new AppResponses<string> { success = true, message = "YES", data = "remitente@dominio.com", errors = [] });

        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<SolicitaCorreoElectronicoRemitenteController>>();
        var controller = new SolicitaCorreoElectronicoRemitenteController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var action = await controller.Get(10, 450);

        Assert.IsType<OkObjectResult>(action.Result);
    }
}
