using DocuArchi.Api.Controllers.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.RutaTrabajo;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.Workflow.RutaTrabajo;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaEstructuraRutaWorkflowControllerContractTests
{
    [Fact]
    public async Task SolicitaEstructuraRutaWorkflow_CuandoClaimYServicioOk_RetornaOk()
    {
        var claimService = BuildClaimService("WF");
        var service = new Mock<ISolicitaEstructuraRutaWorkflowService>();

        service
            .Setup(s => s.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(new AppResponses<List<SolicitaEstructuraRutaWorkflowDto>?>
            {
                success = true,
                message = "YES",
                data =
                [
                    new SolicitaEstructuraRutaWorkflowDto
                    {
                        id_Ruta = 1,
                        Nombre_Ruta = "ENTRADA",
                        Descripcion_Ruta = "Ruta de entrada",
                        Fecha_Creacion = new DateTime(2026, 3, 16),
                        Estado_Ruta = 1,
                        Archivo_Plantilla = [1],
                        Ruta_Archivo_Server = "/tmp/ruta",
                        Archivo_Plantilla_Mindifucion = "mind"
                    }
                ],
                errors = []
            });

        var controller = new SolicitaEstructuraRutaWorkflowController(claimService.Object, service.Object);
        var result = await controller.SolicitaEstructuraRutaWorkflow();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<List<SolicitaEstructuraRutaWorkflowDto>?>>(ok.Value);
        Assert.True(payload.success);
        Assert.NotNull(payload.data);
        Assert.Single(payload.data!);
    }

    [Fact]
    public async Task SolicitaEstructuraRutaWorkflow_CuandoFaltaClaim_RetornaBadRequest()
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

        var controller = new SolicitaEstructuraRutaWorkflowController(
            claimService.Object,
            Mock.Of<ISolicitaEstructuraRutaWorkflowService>());

        var result = await controller.SolicitaEstructuraRutaWorkflow();
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact(Skip = "Requiere MySQL Testcontainers/Docker para validacion de integracion real de rutas_workflow.")]
    public void SolicitaEstructuraRutaWorkflow_Integracion_MySqlTestcontainers_Pendiente()
    {
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
