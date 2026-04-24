using MiApp.Repository.Repositorio.GestionCorrespondencia.PlantillaValidacion.SolicitaCorreoElectronicoRemitente;
using MiApp.Services.Service.GestionCorrespondencia.PlantillaValidacion.SolicitaCorreoElectronicoRemitente;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSolicitaCorreoElectronicoRemitenteTests
{
    [Fact]
    public async Task SolicitaCorreoElectronicoRemitenteAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var repository = new Mock<ISolicitaCorreoElectronicoRemitenteRepository>();
        var service = new ServiceSolicitaCorreoElectronicoRemitente(repository.Object);

        var result = await service.SolicitaCorreoElectronicoRemitenteAsync(1, 1, " ");

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.Equal(string.Empty, result.data);
        repository.Verify(
            r => r.SolicitaCorreoElectronicoRemitenteAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task SolicitaCorreoElectronicoRemitenteAsync_CuandoRepositorioRetornaEmpty_PropagaOkEmpty()
    {
        var repository = new Mock<ISolicitaCorreoElectronicoRemitenteRepository>();
        repository.Setup(r => r.SolicitaCorreoElectronicoRemitenteAsync(10, 450, "WF"))
            .ReturnsAsync(new MiApp.DTOs.DTOs.Utilidades.AppResponses<string>
            {
                success = true,
                message = "Sin resultados",
                data = "",
                meta = new MiApp.DTOs.DTOs.Utilidades.AppMeta { Status = "empty" },
                errors = []
            });

        var service = new ServiceSolicitaCorreoElectronicoRemitente(repository.Object);
        var result = await service.SolicitaCorreoElectronicoRemitenteAsync(10, 450, "WF");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Equal("", result.data);
        Assert.NotNull(result.meta);
        Assert.Equal("empty", result.meta?.Status);
    }

    [Fact]
    public async Task SolicitaCorreoElectronicoRemitenteAsync_CuandoRepositorioRetornaError_PropagaBadRequest()
    {
        var repository = new Mock<ISolicitaCorreoElectronicoRemitenteRepository>();
        repository.Setup(r => r.SolicitaCorreoElectronicoRemitenteAsync(10, 450, "WF"))
            .ReturnsAsync(new MiApp.DTOs.DTOs.Utilidades.AppResponses<string>
            {
                success = false,
                message = "Error",
                data = "",
                errors = []
            });

        var service = new ServiceSolicitaCorreoElectronicoRemitente(repository.Object);
        var result = await service.SolicitaCorreoElectronicoRemitenteAsync(10, 450, "WF");

        Assert.False(result.success);
        Assert.Equal("Error", result.message);
        Assert.Equal(string.Empty, result.data);
        Assert.NotNull(result.meta);
        Assert.Equal("error", result.meta?.Status);
    }
}

