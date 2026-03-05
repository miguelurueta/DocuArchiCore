using MiApp.DTOs.DTOs.Errors;
using MiApp.Services.Service.Radicacion.Tramite;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class FlujoInicialRadicacionServiceTests
{
    [Fact]
    public async Task ObtenerFlujoInicialAsync_CuandoParametrosValidos_RetornaSuccess()
    {
        var service = new FlujoInicialRadicacionService();

        var result = await service.ObtenerFlujoInicialAsync(7, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(7, result.data.IdTipoTramite);
        Assert.Equal("FLUJO-7", result.data.CodigoFlujo);
        Assert.Equal("Radicar", result.data.ActividadInicial);
    }

    [Fact]
    public async Task ObtenerFlujoInicialAsync_CuandoIdTipoTramiteInvalido_RetornaErrorValidacion()
    {
        var service = new FlujoInicialRadicacionService();

        var result = await service.ObtenerFlujoInicialAsync(0, "DA");

        Assert.False(result.success);
        Assert.Equal("IdTipoTramite requerido", result.message);
        Assert.NotNull(result.errors);
        var errors = result.errors!.OfType<AppError>().ToList();
        Assert.Contains(errors, e => e.Field == "idTipoTramite");
    }

    [Fact]
    public async Task ObtenerFlujoInicialAsync_CuandoAliasVacio_RetornaErrorValidacion()
    {
        var service = new FlujoInicialRadicacionService();

        var result = await service.ObtenerFlujoInicialAsync(7, "");

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.NotNull(result.errors);
        var errors = result.errors!.OfType<AppError>().ToList();
        Assert.Contains(errors, e => e.Field == "defaultDbAlias");
    }
}
