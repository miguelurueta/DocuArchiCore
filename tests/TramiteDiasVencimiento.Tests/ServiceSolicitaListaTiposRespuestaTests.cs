using MiApp.DTOs.DTOs.Common;
using MiApp.Repository.Repositorio.GestionCorrespondencia.TiposRespuesta;
using MiApp.Services.Service.GestionCorrespondencia.TiposRespuesta;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSolicitaListaTiposRespuestaTests
{
    [Fact]
    public async Task SolicitaListaTiposRespuestaAsync_CuandoAliasVacio_RetornaValidacion()
    {
        var service = new ServiceSolicitaListaTiposRespuesta(
            Mock.Of<ISolicitaListaTiposRespuestaRepository>(),
            Mock.Of<ILogger<ServiceSolicitaListaTiposRespuesta>>());

        var result = await service.SolicitaListaTiposRespuestaAsync(string.Empty, "req-1");

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
    }

    [Fact]
    public async Task SolicitaListaTiposRespuestaAsync_CuandoCacheMissYHayDatos_RetornaSuccess()
    {
        var repository = new Mock<ISolicitaListaTiposRespuestaRepository>();
        repository
            .Setup(r => r.SolicitaListaTiposRespuestaAsync("DA"))
            .ReturnsAsync(
            [
                new ResponseDropdownDto { Id = 1, Descripcion = "Positiva" },
                new ResponseDropdownDto { Id = 2, Descripcion = "Negativa" }
            ]);

        var service = new ServiceSolicitaListaTiposRespuesta(
            repository.Object,
            Mock.Of<ILogger<ServiceSolicitaListaTiposRespuesta>>());

        var result = await service.SolicitaListaTiposRespuestaAsync("DA", "req-2");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(2, result.data.Count);
    }
}
