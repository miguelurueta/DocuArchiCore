using System.Collections.Concurrent;
using System.Threading;
using DocuArchi.Api.Controllers.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.Radicacion.Tramite;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.SessionHelper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RegistrarEntranteConcurrencyTests
{
    [Fact]
    public async Task RegistrarEntrante_CincuentaSolicitudesConcurrentes_GeneraConsecutivosUnicos()
    {
        var claimService = BuildClaimService("DA", "10");
        var ipHelper = BuildIpHelper();
        var registrar = new Mock<IRegistrarRadicacionEntranteService>();
        var secuencia = 0;

        registrar
            .Setup(s => s.RegistrarRadicacionEntranteAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 10, "DA", It.IsAny<string>(), 1))
            .ReturnsAsync(() =>
            {
                var correlativo = Interlocked.Increment(ref secuencia);
                return new AppResponses<RegistrarRadicacionEntranteResponseDto>
                {
                    success = true,
                    message = "OK",
                    data = new RegistrarRadicacionEntranteResponseDto
                    {
                        ConsecutivoRadicado = $"RAD-{correlativo:D4}"
                    },
                    errors = []
                };
            });

        var consecutivos = new ConcurrentBag<string>();
        var tareas = Enumerable.Range(1, 50).Select(async _ =>
        {
            var controller = new RadicacionController(
                claimService.Object,
                registrar.Object,
                Mock.Of<IValidarRadicacionEntranteService>(),
                Mock.Of<IFlujoInicialRadicacionService>(),
                ipHelper.Object);

            var result = await controller.RegistrarEntrante(new RegistrarRadicacionEntranteRequestDto { IdPlantilla = 67 }, 1);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var payload = Assert.IsType<AppResponses<RegistrarRadicacionEntranteResponseDto>>(ok.Value);
            consecutivos.Add(payload.data.ConsecutivoRadicado);
        });

        await Task.WhenAll(tareas);

        Assert.Equal(50, consecutivos.Count);
        Assert.Equal(50, consecutivos.Distinct(StringComparer.OrdinalIgnoreCase).Count());
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
