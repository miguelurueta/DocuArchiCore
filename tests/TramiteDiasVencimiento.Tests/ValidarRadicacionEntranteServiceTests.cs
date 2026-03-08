using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ValidarRadicacionEntranteServiceTests
{
    [Fact]
    public async Task ValidarRadicacionEntranteAsync_CuandoRequestValido_RetornaSuccess()
    {
        var service = new ValidarRadicacionEntranteService();

        var result = await service.ValidarRadicacionEntranteAsync(new ValidarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            TipoRadicacion = "ENTRANTE",
            Asunto = "Asunto",
            Remitente = new RemitenteRadicacionDto
            {
                Nombre = "Remitente"
            }
        });

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.True(result.data.EsValido);
    }

    [Fact]
    public async Task ValidarRadicacionEntranteAsync_CuandoRequestInvalido_RetornaErroresDeValidacion()
    {
        var service = new ValidarRadicacionEntranteService();

        var result = await service.ValidarRadicacionEntranteAsync(new ValidarRadicacionEntranteRequestDto
        {
            IdPlantilla = 0,
            TipoRadicacion = "ENTRANTE",
            Asunto = "",
            Remitente = new RemitenteRadicacionDto()
        });

        Assert.False(result.success);
        Assert.Equal("Validacion fallida", result.message);
        Assert.NotNull(result.errors);

        var errors = result.errors!.OfType<AppError>().ToList();
        Assert.Contains(errors, e => e.Field == "IdPlantilla");
        Assert.Contains(errors, e => e.Field == "Remitente");
        Assert.Contains(errors, e => e.Field == "Asunto");
    }
}
