using DocuArchi.Api.Controllers.GestionCorrespondencia.Firmas;
using MiApp.DTOs.DTOs.Common;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestionCorrespondencia.Firmas;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaUsuarioPrincipalRespuestaControllerTests
{
    [Fact]
    public async Task Get_CuandoClaimAliasEsInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = false, ClaimValue = null, Response = new AppResponses<string>() });

        var service = new Mock<IServiceSolicitaUsuarioPrincipalRespuesta>();
        var logger = new Mock<ILogger<SolicitaUsuarioPrincipalRespuestaController>>();
        var controller = new SolicitaUsuarioPrincipalRespuestaController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await controller.Get(10);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        service.Verify(s => s.SolicitaUsuarioPrincipalRespuestaAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Get_CuandoIdUsuarioGestionEsInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "WF", Response = null });
        claimValidation.Setup(c => c.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "12", Response = null });

        var service = new Mock<IServiceSolicitaUsuarioPrincipalRespuesta>();
        var logger = new Mock<ILogger<SolicitaUsuarioPrincipalRespuestaController>>();
        var controller = new SolicitaUsuarioPrincipalRespuestaController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await controller.Get(0);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        service.Verify(s => s.SolicitaUsuarioPrincipalRespuestaAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Get_CuandoServiceRetornaSuccess_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "WF", Response = null });
        claimValidation.Setup(c => c.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "12", Response = null });

        var service = new Mock<IServiceSolicitaUsuarioPrincipalRespuesta>();
        service.Setup(s => s.SolicitaUsuarioPrincipalRespuestaAsync(10, 12, "WF"))
            .ReturnsAsync(new AppResponses<ResponseDropdownDto?>
            {
                success = true,
                message = "YES",
                data = new ResponseDropdownDto { Id = 10, Descripcion = "Ana - Analista" },
                errors = []
            });

        var logger = new Mock<ILogger<SolicitaUsuarioPrincipalRespuestaController>>();
        var controller = new SolicitaUsuarioPrincipalRespuestaController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await controller.Get(10);

        Assert.IsType<OkObjectResult>(result.Result);
    }
}
