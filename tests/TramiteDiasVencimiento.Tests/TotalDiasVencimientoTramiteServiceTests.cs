using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class TotalDiasVencimientoTramiteServiceTests
{
    [Fact]
    public async Task ServiceSolicitaTotalDiasVencimientoTramite_CuandoHayDatos_RetornaSuccess()
    {
        var repoMock = new Mock<ITotalDiasVencimientoTramiteRepository>();
        repoMock
            .Setup(r => r.SolicitaTotalDiasVencimientoTramite(10, 20, "DA"))
            .ReturnsAsync(new AppResponses<int>
            {
                success = true,
                message = "OK",
                data = 15,
                errors = []
            });

        var service = new TotalDiasVencimientoTramiteService(repoMock.Object);

        var result = await service.ServiceSolicitaTotalDiasVencimientoTramite(10, 20, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.Equal(15, result.data);
    }

    [Fact]
    public async Task ServiceSolicitaTotalDiasVencimientoTramite_CuandoNoHayRegistros_RetornaSinResultados()
    {
        var repoMock = new Mock<ITotalDiasVencimientoTramiteRepository>();
        repoMock
            .Setup(r => r.SolicitaTotalDiasVencimientoTramite(10, 999, "DA"))
            .ReturnsAsync(new AppResponses<int>
            {
                success = true,
                message = "Sin resultados",
                data = 0,
                errors = []
            });

        var service = new TotalDiasVencimientoTramiteService(repoMock.Object);

        var result = await service.ServiceSolicitaTotalDiasVencimientoTramite(10, 999, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Equal(0, result.data);
    }

    [Fact]
    public async Task ServiceSolicitaTotalDiasVencimientoTramite_CuandoHayExcepcion_RetornaErrorControlado()
    {
        var repoMock = new Mock<ITotalDiasVencimientoTramiteRepository>();
        repoMock
            .Setup(r => r.SolicitaTotalDiasVencimientoTramite(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("fallo simulado"));

        var service = new TotalDiasVencimientoTramiteService(repoMock.Object);

        var result = await service.ServiceSolicitaTotalDiasVencimientoTramite(10, 20, "DA");

        Assert.False(result.success);
        Assert.Equal(0, result.data);
        Assert.NotNull(result.errors);
        Assert.Contains(
            result.errors!.Cast<AppError>(),
            e => e.Field == "IdTipoTramite" && e.Message.Contains("fallo simulado"));
    }
}
