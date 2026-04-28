using MiApp.Repository.Repositorio.GestionCorrespondencia.GestionRespuesta;
using MiApp.Repository.Repositorio.DataAccess;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaEstructuraRespuestaIdTareaRepositoryTests
{
    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTareaAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var dbFactory = new Mock<IDbConnectionFactory>();
        var logger = new Mock<ILogger<SolicitaEstructuraRespuestaIdTareaRepository>>();
        var repository = new SolicitaEstructuraRespuestaIdTareaRepository(dbFactory.Object, logger.Object);

        var result = await repository.SolicitaEstructuraRespuestaIdTareaAsync(10, string.Empty);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data);
        dbFactory.Verify(f => f.GetOpenConnectionAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTareaAsync_CuandoIdEsInvalido_RetornaValidacion()
    {
        var dbFactory = new Mock<IDbConnectionFactory>();
        var logger = new Mock<ILogger<SolicitaEstructuraRespuestaIdTareaRepository>>();
        var repository = new SolicitaEstructuraRespuestaIdTareaRepository(dbFactory.Object, logger.Object);

        var result = await repository.SolicitaEstructuraRespuestaIdTareaAsync(0, "WF");

        Assert.False(result.success);
        Assert.Equal("IdTareaWf requerido", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data);
        dbFactory.Verify(f => f.GetOpenConnectionAsync(It.IsAny<string>()), Times.Never);
    }
}

