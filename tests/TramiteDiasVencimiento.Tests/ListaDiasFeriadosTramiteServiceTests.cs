using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class ListaDiasFeriadosTramiteServiceTests
{
    [Fact]
    public async Task ServiceSolicitaListaDiasFeriados_CuandoHayDatos_RetornaSuccess()
    {
        var repoMock = new Mock<IListaDiasFeriadosTramiteRepository>();
        repoMock
            .Setup(r => r.SolicitaListaDiasFeriados("DA"))
            .ReturnsAsync(new AppResponses<List<string>>
            {
                success = true,
                message = "OK",
                data = ["2026-01-01", "2026-01-06"],
                errors = []
            });

        var service = new ListaDiasFeriadosTramiteService(repoMock.Object);

        var result = await service.ServiceSolicitaListaDiasFeriados("DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.Equal(2, result.data.Count);
        Assert.Equal("2026-01-01", result.data[0]);
    }

    [Fact]
    public async Task ServiceSolicitaListaDiasFeriados_CuandoNoHayRegistros_RetornaSinResultados()
    {
        var repoMock = new Mock<IListaDiasFeriadosTramiteRepository>();
        repoMock
            .Setup(r => r.SolicitaListaDiasFeriados("DA"))
            .ReturnsAsync(new AppResponses<List<string>>
            {
                success = true,
                message = "Sin resultados",
                data = [],
                errors = []
            });

        var service = new ListaDiasFeriadosTramiteService(repoMock.Object);

        var result = await service.ServiceSolicitaListaDiasFeriados("DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Empty(result.data);
    }

    [Fact]
    public async Task ServiceSolicitaListaDiasFeriados_CuandoHayExcepcion_RetornaErrorControlado()
    {
        var repoMock = new Mock<IListaDiasFeriadosTramiteRepository>();
        repoMock
            .Setup(r => r.SolicitaListaDiasFeriados(It.IsAny<string>()))
            .ThrowsAsync(new Exception("fallo simulado"));

        var service = new ListaDiasFeriadosTramiteService(repoMock.Object);

        var result = await service.ServiceSolicitaListaDiasFeriados("DA");

        Assert.False(result.success);
        Assert.Empty(result.data);
        Assert.NotNull(result.errors);
        Assert.Contains(
            result.errors!.Cast<AppError>(),
            e => e.Field == "IdTipoTramite" && e.Message.Contains("fallo simulado"));
    }

    [Fact]
    public async Task ServiceSolicitaListaDiasFeriados_CuandoDefaultDbAliasEsNulo_RetornaErrorControlado()
    {
        var repoMock = new Mock<IListaDiasFeriadosTramiteRepository>();
        var service = new ListaDiasFeriadosTramiteService(repoMock.Object);

        var result = await service.ServiceSolicitaListaDiasFeriados(null!);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.Empty(result.data);
        Assert.NotNull(result.errors);
        Assert.Contains(
            result.errors!.Cast<AppError>(),
            e => e.Field == "defaultDbAlias" && e.Message == "DefaultDbAlias requerido");
    }
}
