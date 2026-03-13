using DocuArchi.Api.Controllers.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.RutaTrabajo;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.Workflow.RutaTrabajo;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaExistenciaRadicadoRutaWorkflowControllerContractTests
{
    [Fact]
    public async Task SolicitaExistenciaRadicadoRutaWorkflow_CuandoClaimYServicioOk_RetornaOk()
    {
        var claimService = BuildClaimService("WF");
        var service = new Mock<ISolicitaExistenciaRadicadoRutaWorkflowService>();

        service
            .Setup(s => s.SolicitaExistenciaRadicadoRutaWorkflowAsync("260001", "01", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>
            {
                success = true,
                message = "YES",
                data = new SolicitaExistenciaRadicadoRutaWorkflowDto
                {
                    Radicado = "260001",
                    IdTareaWorkflow = 100,
                    EstadoExistenciaRadicado = "YES"
                },
                errors = []
            });

        var controller = new SolicitaExistenciaRadicadoRutaWorkflowController(claimService.Object, service.Object);
        var result = await controller.SolicitaExistenciaRadicadoRutaWorkflow("260001", "01");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>>(ok.Value);
        Assert.True(payload.success);
        Assert.Equal("YES", payload.data.EstadoExistenciaRadicado);
    }

    [Fact]
    public async Task SolicitaExistenciaRadicadoRutaWorkflow_CuandoFaltaClaim_RetornaBadRequest()
    {
        var claimService = new Mock<IClaimValidationService>();
        claimService
            .Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "sin alias", data = string.Empty }
            });

        var controller = new SolicitaExistenciaRadicadoRutaWorkflowController(
            claimService.Object,
            Mock.Of<ISolicitaExistenciaRadicadoRutaWorkflowService>());

        var result = await controller.SolicitaExistenciaRadicadoRutaWorkflow("260001", "01");
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    private static Mock<IClaimValidationService> BuildClaimService(string alias)
    {
        var claimService = new Mock<IClaimValidationService>();
        claimService
            .Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = alias });
        return claimService;
    }
}
