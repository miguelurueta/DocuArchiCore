using DocuArchi.Api.Controllers.Workflow.UsuarioWorkflow;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.Usuario;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.Workflow.Usuario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class FirmaTemporalUsuarioWorkflowControllerTests
{
    [Fact]
    public async Task Get_CuandoFaltaClaimAlias_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulaliaswf"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "missing claim" }
            });

        var service = new Mock<IServiceFirmaTemporalUsuarioWorkflow>();
        var controller = BuildController(claimValidation.Object, service.Object);

        var result = await controller.Get();

        Assert.IsType<BadRequestObjectResult>(result.Result);
        service.Verify(
            s => s.SolicitaFirmaTemporalAsync(It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Get_CuandoClaimsValidosYServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulaliaswf"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "WF", Response = null });
        claimValidation.Setup(c => c.ValidateClaim<string>("IdUsuarioWorkflow"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "77", Response = null });

        var service = new Mock<IServiceFirmaTemporalUsuarioWorkflow>();
        service.Setup(s => s.SolicitaFirmaTemporalAsync(77, "WF"))
            .ReturnsAsync(new AppResponses<FirmaTemporalUsuarioWorkflowDto?>
            {
                success = true,
                message = "YES",
                data = new FirmaTemporalUsuarioWorkflowDto
                {
                    IdUsuarioWorkflow = 77,
                    FileName = "firma.png",
                    ContentType = "image/png",
                    RelativePath = "signatures/firma.png",
                    UrlTemporal = "/api/workflow/usuarios/firma-temporal/download/abc",
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10)
                },
                errors = []
            });

        var controller = BuildController(claimValidation.Object, service.Object);
        var result = await controller.Get();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public void Download_CuandoTokenNoExiste_RetornaNotFound()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("IdUsuarioWorkflow"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "77", Response = null });

        var service = new Mock<IServiceFirmaTemporalUsuarioWorkflow>();
        var outPath = string.Empty;
        var outContentType = string.Empty;
        var outFileName = string.Empty;
        service.Setup(s => s.TryResolveFirmaTemporal(
                "token-1",
                77,
                out outPath,
                out outContentType,
                out outFileName))
            .Returns(false);

        var controller = BuildController(claimValidation.Object, service.Object);
        var result = controller.Download("token-1");

        Assert.IsType<NotFoundResult>(result);
    }

    private static FirmaTemporalUsuarioWorkflowController BuildController(
        IClaimValidationService claimValidationService,
        IServiceFirmaTemporalUsuarioWorkflow service)
    {
        var controller = new FirmaTemporalUsuarioWorkflowController(claimValidationService, service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }
}
