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

public sealed class PlantillaRadicacionControllerTokenRadicadoContractTests
{
    [Fact]
    public async Task SolicitaAutoCompleteTokenRadicado_CuandoClaimYServicioOk_RetornaOk()
    {
        var claimService = BuildClaimService("DA");
        var autoCompleteTokenService = new Mock<ISolicitaAutoCompleteTokenRadicadoService>();

        autoCompleteTokenService
            .Setup(s => s.ServiceSolicitaAutoCompleteTokenRadicadoAsync(It.IsAny<ParameterAutoComplete>(), "DA"))
            .ReturnsAsync(new AppResponses<List<rowTomSelect>>
            {
                success = true,
                message = "OK",
                data =
                [
                    new rowTomSelect { texValue = "26000100010100001" }
                ],
                errors = []
            });

        var controller = BuildController(claimService.Object, autoCompleteTokenService.Object);
        var result = await controller.SolicitaAutoCompleteTokenRadicado(new ParameterAutoComplete { TextoBuscado = "260001" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<List<rowTomSelect>>>(ok.Value);
        Assert.True(payload.success);
        Assert.NotNull(payload.data);
        Assert.Single(payload.data);
    }

    [Fact]
    public async Task SolicitaAutoCompleteTokenRadicado_CuandoFaltaClaimAlias_RetornaBadRequest()
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

        var controller = BuildController(claimService.Object, Mock.Of<ISolicitaAutoCompleteTokenRadicadoService>());
        var result = await controller.SolicitaAutoCompleteTokenRadicado(new ParameterAutoComplete { TextoBuscado = "260001" });

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
        ISolicitaAutoCompleteTokenRadicadoService autoCompleteTokenService)
    {
        return new PlantillaRadicacionController(
            Mock.Of<MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim.ICurrentUserService>(),
            Mock.Of<IPlantillaRadicacionL>(),
            claimValidationService,
            Mock.Of<MiApp.Repository.Repositorio.Radicador.PlantillaValidacion.IPlantillaValidacionR>(),
            Mock.Of<IPlantillaValidacionL>(),
            Mock.Of<IUsuarioCaracterizacionService>(),
            Mock.Of<IAutoCompleteDestinatarioRestriccionService>(),
            Mock.Of<ICamposDinamicosPlantillaService>(),
            autoCompleteTokenService);
    }
}
