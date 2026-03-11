using DocuArchi.Api.Controllers.Radicacion.PlantillaRadicado;
using MiApp.DTOs.DTOs.General;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Usuario;
using MiApp.Services.Service.Radicacion.PlantillaRadicado;
using MiApp.Services.Service.Radicacion.PlantillaValidacion;
using MiApp.Services.Service.Radicacion.Tramite;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class PlantillaRadicacionControllerTokenExpedienteRadicadoContractTests
{
    [Fact]
    public async Task SolicitaAutoCompleteTokenExpedienteRadicado_CuandoClaimYServicioOk_RetornaOk()
    {
        var claimService = BuildClaimService("DA");
        var service = new Mock<ISolicitaAutoCompleteTokenExpedienteRadicadoService>();

        service
            .Setup(s => s.ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync(It.IsAny<ParameterAutoComplete>(), "DA"))
            .ReturnsAsync(new AppResponses<List<rowTomSelect>>
            {
                success = true,
                message = "OK",
                data =
                [
                    new rowTomSelect { idValue = 1, texValue = "EXP-001" }
                ],
                errors = []
            });

        var controller = BuildController(claimService.Object, service.Object);
        var result = await controller.SolicitaAutoCompleteTokenExpedienteRadicado(new ParameterAutoComplete { TextoBuscado = "EXP" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<List<rowTomSelect>>>(ok.Value);
        Assert.True(payload.success);
        Assert.Single(payload.data);
    }

    [Fact]
    public async Task SolicitaAutoCompleteTokenExpedienteRadicado_CuandoFaltaClaimAlias_RetornaBadRequest()
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

        var controller = BuildController(claimService.Object, Mock.Of<ISolicitaAutoCompleteTokenExpedienteRadicadoService>());
        var result = await controller.SolicitaAutoCompleteTokenExpedienteRadicado(new ParameterAutoComplete { TextoBuscado = "EXP" });

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

    private static PlantillaRadicacionController BuildController(
        IClaimValidationService claimValidationService,
        ISolicitaAutoCompleteTokenExpedienteRadicadoService expedienteService)
    {
        return new PlantillaRadicacionController(
            Mock.Of<ICurrentUserService>(),
            Mock.Of<IPlantillaRadicacionL>(),
            claimValidationService,
            Mock.Of<MiApp.Repository.Repositorio.Radicador.PlantillaValidacion.IPlantillaValidacionR>(),
            Mock.Of<IPlantillaValidacionL>(),
            Mock.Of<IUsuarioCaracterizacionService>(),
            Mock.Of<IAutoCompleteDestinatarioRestriccionService>(),
            Mock.Of<ICamposDinamicosPlantillaService>(),
            Mock.Of<ISolicitaAutoCompleteTokenRadicadoService>(),
            expedienteService);
    }
}
