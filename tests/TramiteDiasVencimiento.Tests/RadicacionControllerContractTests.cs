using DocuArchi.Api.Controllers.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.Radicacion.Tramite;
using MiApp.Services.Service.SessionHelper;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RadicacionControllerContractTests
{
    [Fact]
    public async Task RegistrarEntrante_CuandoClaimsValidosYServiceOk_RetornaOkConContrato()
    {
        var claimService = BuildClaimService("DA", "10");
        var registrar = new Mock<IRegistrarRadicacionEntranteService>();
        var validar = new Mock<IValidarRadicacionEntranteService>();
        var flujo = new Mock<IFlujoInicialRadicacionService>();

        registrar
            .Setup(s => s.RegistrarRadicacionEntranteAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 10, "DA", It.IsAny<string>(), 1))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto>
            {
                success = true,
                message = "OK",
                data = new RegistrarRadicacionEntranteResponseDto
                {
                    ConsecutivoRadicado = "RAD-001"
                },
                errors = []
            });

        var controller = new RadicacionController(claimService.Object, registrar.Object, validar.Object, flujo.Object, BuildIpHelper().Object);
        var result = await controller.RegistrarEntrante(new RegistrarRadicacionEntranteRequestDto { IdPlantilla = 100 }, 1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<RegistrarRadicacionEntranteResponseDto>>(ok.Value);
        Assert.True(payload.success);
        Assert.Equal("RAD-001", payload.data.ConsecutivoRadicado);
    }

    [Fact]
    public async Task RegistrarEntrante_CuandoRecibeTipoModuloRadicacion_LoPropagaAlServicio()
    {
        var claimService = BuildClaimService("DA", "10");
        var registrar = new Mock<IRegistrarRadicacionEntranteService>();

        registrar
            .Setup(s => s.RegistrarRadicacionEntranteAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 10, "DA", It.IsAny<string>(), 2))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto>
            {
                success = true,
                message = "OK",
                data = new RegistrarRadicacionEntranteResponseDto()
            });

        var controller = new RadicacionController(
            claimService.Object,
            registrar.Object,
            Mock.Of<IValidarRadicacionEntranteService>(),
            Mock.Of<IFlujoInicialRadicacionService>(),
            BuildIpHelper().Object);

        var result = await controller.RegistrarEntrante(new RegistrarRadicacionEntranteRequestDto { IdPlantilla = 100 }, 2);

        Assert.IsType<OkObjectResult>(result.Result);
        registrar.Verify(s => s.RegistrarRadicacionEntranteAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 10, "DA", It.IsAny<string>(), 2), Times.Once);
    }

    [Fact]
    public async Task RegistrarEntrante_CuandoFaltaAliasClaim_RetornaBadRequest()
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

        var controller = new RadicacionController(
            claimService.Object,
            Mock.Of<IRegistrarRadicacionEntranteService>(),
            Mock.Of<IValidarRadicacionEntranteService>(),
            Mock.Of<IFlujoInicialRadicacionService>(),
            BuildIpHelper().Object);

        var result = await controller.RegistrarEntrante(new RegistrarRadicacionEntranteRequestDto { IdPlantilla = 100 }, 1);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task RegistrarEntrante_CuandoFaltaUsuarioIdClaim_RetornaBadRequest()
    {
        var claimService = new Mock<IClaimValidationService>();
        claimService
            .Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "DA" });
        claimService
            .Setup(c => c.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "sin usuarioid", data = string.Empty }
            });

        var controller = new RadicacionController(
            claimService.Object,
            Mock.Of<IRegistrarRadicacionEntranteService>(),
            Mock.Of<IValidarRadicacionEntranteService>(),
            Mock.Of<IFlujoInicialRadicacionService>(),
            BuildIpHelper().Object);

        var result = await controller.RegistrarEntrante(new RegistrarRadicacionEntranteRequestDto { IdPlantilla = 100 }, 1);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task RegistrarEntrante_CuandoServiceSinResultados_RetornaOkConDataNull()
    {
        var claimService = BuildClaimService("DA", "10");
        var registrar = new Mock<IRegistrarRadicacionEntranteService>();

        registrar
            .Setup(s => s.RegistrarRadicacionEntranteAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 10, "DA", It.IsAny<string>(), 1))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto>
            {
                success = true,
                message = "Sin resultados",
                data = null!,
                errors = []
            });

        var controller = new RadicacionController(
            claimService.Object,
            registrar.Object,
            Mock.Of<IValidarRadicacionEntranteService>(),
            Mock.Of<IFlujoInicialRadicacionService>(),
            BuildIpHelper().Object);

        var result = await controller.RegistrarEntrante(new RegistrarRadicacionEntranteRequestDto { IdPlantilla = 100 }, 1);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<RegistrarRadicacionEntranteResponseDto>>(ok.Value);

        Assert.True(payload.success);
        Assert.Equal("Sin resultados", payload.message);
        Assert.Null(payload.data);
    }

    [Fact]
    public async Task RegistrarEntrante_CuandoServiceLanzaExcepcion_Retorna500Controlado()
    {
        var claimService = BuildClaimService("DA", "10");
        var registrar = new Mock<IRegistrarRadicacionEntranteService>();
        registrar
            .Setup(s => s.RegistrarRadicacionEntranteAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 10, "DA", It.IsAny<string>(), 1))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var controller = new RadicacionController(
            claimService.Object,
            registrar.Object,
            Mock.Of<IValidarRadicacionEntranteService>(),
            Mock.Of<IFlujoInicialRadicacionService>(),
            BuildIpHelper().Object);

        var result = await controller.RegistrarEntrante(new RegistrarRadicacionEntranteRequestDto { IdPlantilla = 100 }, 1);
        var status = Assert.IsType<ObjectResult>(result.Result);

        Assert.Equal(500, status.StatusCode);
    }

    [Fact]
    public async Task FlujoInicial_CuandoServiceOk_RetornaOkConContrato()
    {
        var claimService = BuildClaimService("DA", "10");
        var flujo = new Mock<IFlujoInicialRadicacionService>();
        flujo
            .Setup(s => s.ObtenerFlujoInicialAsync(5, "DA"))
            .ReturnsAsync(new AppResponses<FlujoInicialDto>
            {
                success = true,
                message = "OK",
                data = new FlujoInicialDto { IdTipoTramite = 5, CodigoFlujo = "FLUJO-5", ActividadInicial = "Radicar" },
                errors = []
            });

        var controller = new RadicacionController(
            claimService.Object,
            Mock.Of<IRegistrarRadicacionEntranteService>(),
            Mock.Of<IValidarRadicacionEntranteService>(),
            flujo.Object,
            BuildIpHelper().Object);

        var result = await controller.FlujoInicial(5);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<FlujoInicialDto>>(ok.Value);
        Assert.True(payload.success);
        Assert.Equal("FLUJO-5", payload.data.CodigoFlujo);
    }

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

    private static Mock<IIpHelper> BuildIpHelper()
    {
        var ipHelper = new Mock<IIpHelper>();
        ipHelper
            .Setup(i => i.ObtenerDireccionIP(It.IsAny<Microsoft.AspNetCore.Http.HttpContext>()))
            .Returns("127.0.0.1");
        return ipHelper;
    }
}
