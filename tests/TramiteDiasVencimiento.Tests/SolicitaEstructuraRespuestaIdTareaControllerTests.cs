using DocuArchi.Api.Controllers.GestionCorrespondencia;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestionCorrespondencia;
using MiApp.Services.Service.GestorDocumental;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaEstructuraRespuestaIdTareaControllerTests
{
    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTarea_CuandoClaimEsInvalido_RetornaBadRequest()
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
                    data = string.Empty,
                    errors = []
                }
            });

        var service = new Mock<IServiceSolicitaEstructuraRespuesta>();
        var logger = new Mock<ILogger<SolicitaEstructuraRespuestaIdTareaController>>();
        var controller = new SolicitaEstructuraRespuestaIdTareaController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var action = await controller.SolicitaEstructuraRespuestaIdTarea(10);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.Verify(s => s.SolicitaEstructuraRespuestaIdTareaAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTarea_CuandoIdEsInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "WF",
                Response = null
            });

        var service = new Mock<IServiceSolicitaEstructuraRespuesta>();
        var logger = new Mock<ILogger<SolicitaEstructuraRespuestaIdTareaController>>();
        var controller = new SolicitaEstructuraRespuestaIdTareaController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var action = await controller.SolicitaEstructuraRespuestaIdTarea(0);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.Verify(s => s.SolicitaEstructuraRespuestaIdTareaAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTarea_CuandoServiceRetornaError_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "WF",
                Response = null
            });

        var service = new Mock<IServiceSolicitaEstructuraRespuesta>();
        service.Setup(s => s.SolicitaEstructuraRespuestaIdTareaAsync(10, "WF"))
            .ReturnsAsync(new AppResponses<List<RaRespuestaRadicado>>
            {
                success = false,
                message = "Error",
                data = [],
                errors = []
            });

        var logger = new Mock<ILogger<SolicitaEstructuraRespuestaIdTareaController>>();
        var controller = new SolicitaEstructuraRespuestaIdTareaController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var action = await controller.SolicitaEstructuraRespuestaIdTarea(10);

        Assert.IsType<BadRequestObjectResult>(action.Result);
    }

    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTarea_CuandoServiceRetornaOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "WF",
                Response = null
            });

        var service = new Mock<IServiceSolicitaEstructuraRespuesta>();
        service.Setup(s => s.SolicitaEstructuraRespuestaIdTareaAsync(10, "WF"))
            .ReturnsAsync(new AppResponses<List<RaRespuestaRadicado>>
            {
                success = true,
                message = "YES",
                data = [],
                errors = []
            });

        var logger = new Mock<ILogger<SolicitaEstructuraRespuestaIdTareaController>>();
        var controller = new SolicitaEstructuraRespuestaIdTareaController(claimValidation.Object, service.Object, logger.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var action = await controller.SolicitaEstructuraRespuestaIdTarea(10);

        Assert.IsType<OkObjectResult>(action.Result);
    }
}
