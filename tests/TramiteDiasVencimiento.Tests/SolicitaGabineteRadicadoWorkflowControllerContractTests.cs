using DocuArchi.Api.Controllers.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.RutaTrabajo;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.Workflow.RutaTrabajo;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaGabineteRadicadoWorkflowControllerContractTests
{
    [Fact]
    public async Task SolicitaPorRadicado_CuandoClaimYServicioOk_RetornaOk()
    {
        var claimService = BuildClaimService("WF", "DA");
        var service = new Mock<ISolicitaGabineteRadicadoWorkflowService>();

        service
            .Setup(s => s.SolicitaPorRadicadoAsync("2500466700035", "WF", "DA"))
            .ReturnsAsync(new AppResponses<RadicadoGabineteWorkflowDto>
            {
                success = true,
                message = "YES",
                data = new RadicadoGabineteWorkflowDto
                {
                    Radicado = "2500466700035",
                    IdTareaWorkflow = 10,
                    IdGabinete = 12,
                    NombreGabinete = "CORRESPO",
                    EstadoExistenciaRadicado = "YES"
                },
                errors = []
            });

        var controller = new SolicitaGabineteRadicadoWorkflowController(claimService.Object, service.Object);
        var result = await controller.SolicitaPorRadicado("2500466700035");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<RadicadoGabineteWorkflowDto>>(ok.Value);
        Assert.True(payload.success);
        Assert.Equal("YES", payload.data.EstadoExistenciaRadicado);
    }

    [Fact]
    public async Task SolicitaPorIdTareaWorkflow_CuandoFaltaClaimWorkflow_RetornaBadRequest()
    {
        var claimService = new Mock<IClaimValidationService>();
        claimService
            .Setup(c => c.ValidateClaim<string>("defaulaliaswf"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "sin alias wf", data = string.Empty }
            });

        var controller = new SolicitaGabineteRadicadoWorkflowController(
            claimService.Object,
            Mock.Of<ISolicitaGabineteRadicadoWorkflowService>());

        var result = await controller.SolicitaPorIdTareaWorkflow(10);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    private static Mock<IClaimValidationService> BuildClaimService(string aliasWorkflow, string aliasGestion)
    {
        var claimService = new Mock<IClaimValidationService>();
        claimService
            .Setup(c => c.ValidateClaim<string>("defaulaliaswf"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = aliasWorkflow });

        claimService
            .Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = aliasGestion });

        return claimService;
    }
}

