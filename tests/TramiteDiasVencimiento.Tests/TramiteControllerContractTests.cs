using DocuArchi.Api.Controllers.Radicacion.Tramite;
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.Radicacion.Tramite;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class TramiteControllerContractTests
{
    [Fact]
    public async Task ApListaRadicadosPendientes_CuandoClaimsValidosYServiceOk_RetornaOkConTablaDinamica()
    {
        var claimService = BuildClaimService("DA", "10");
        var listaService = new Mock<IListaRadicadosPendientesService>();
        listaService
            .Setup(s => s.SolicitaListaRadicadosPendientes(10, "DA"))
            .ReturnsAsync(new AppResponses<DynamicUiTableDto>
            {
                success = true,
                message = "OK",
                data = new DynamicUiTableDto
                {
                    TableId = "lista-radicados-pendientes",
                    Rows =
                    [
                        new UiRowDto
                        {
                            Id = "1",
                            Values = new Dictionary<string, object?>
                            {
                                ["id_estado_radicado"] = 1L
                            }
                        }
                    ]
                },
                errors = []
            });

        var controller = BuildController(claimService.Object, listaService.Object);

        var result = await controller.ApListaRadicadosPendientes();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<DynamicUiTableDto>>(ok.Value);
        Assert.True(payload.success);
        Assert.NotNull(payload.data);
        Assert.Equal("lista-radicados-pendientes", payload.data.TableId);
    }

    [Fact]
    public async Task ApListaRadicadosPendientes_CuandoFaltaAliasClaim_RetornaBadRequest()
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

        var controller = BuildController(claimService.Object, Mock.Of<IListaRadicadosPendientesService>());

        var result = await controller.ApListaRadicadosPendientes();

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ApListaRadicadosPendientes_CuandoUsuarioIdNoEsEntero_Retorna500Controlado()
    {
        var claimService = BuildClaimService("DA", "abc");
        var controller = BuildController(claimService.Object, Mock.Of<IListaRadicadosPendientesService>());

        var result = await controller.ApListaRadicadosPendientes();

        var status = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, status.StatusCode);
        var payload = Assert.IsType<AppResponses<DynamicUiTableDto>>(status.Value);
        Assert.False(payload.success);
        Assert.Equal("Error inesperado al consultar radicados pendientes", payload.message);
    }

    private static TramiteController BuildController(
        IClaimValidationService claimService,
        IListaRadicadosPendientesService listaService)
        => new(
            claimService,
            Mock.Of<IFlujosRelacionadosTramiteService>(),
            Mock.Of<IRelacionTipoRestriccionService>(),
            Mock.Of<ITotalDiasVencimientoTramiteService>(),
            Mock.Of<IListaDiasFeriadosTramiteService>(),
            Mock.Of<IFechaLimiteRespuestaService>(),
            listaService);

    private static Mock<IClaimValidationService> BuildClaimService(string alias, string usuarioId)
    {
        var claimService = new Mock<IClaimValidationService>();
        claimService
            .Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = alias });
        claimService
            .Setup(c => c.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = usuarioId });
        return claimService;
    }
}
