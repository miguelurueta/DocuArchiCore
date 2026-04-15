using MiApp.Models.Models.GestionCorrespondencia;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestionCorrespondencia;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaEstructuraRespuestaIdTareaRepositoryTests
{
    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTareaAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaEstructuraRespuestaIdTareaRepository(dapper.Object);

        var result = await repository.SolicitaEstructuraRespuestaIdTareaAsync(10, string.Empty);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data);
        dapper.Verify(d => d.GetAllAsync<RaRespuestaRadicado>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTareaAsync_CuandoIdEsInvalido_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaEstructuraRespuestaIdTareaRepository(dapper.Object);

        var result = await repository.SolicitaEstructuraRespuestaIdTareaAsync(0, "WF");

        Assert.False(result.success);
        Assert.Equal("IdTareaWf requerido", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data);
        dapper.Verify(d => d.GetAllAsync<RaRespuestaRadicado>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTareaAsync_CuandoNoHayFilas_RetornaSinResultadosYQueryOptionsSoloFiltraPorIdTareaWf()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        QueryOptions? capturedOptions = null;

        dapper.Setup(d => d.GetAllAsync<RaRespuestaRadicado>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(options => capturedOptions = options)
            .ReturnsAsync(new QueryResult<RaRespuestaRadicado>
            {
                Success = true,
                Message = "YES",
                Data = []
            });

        var repository = new SolicitaEstructuraRespuestaIdTareaRepository(dapper.Object);
        var result = await repository.SolicitaEstructuraRespuestaIdTareaAsync(123, " WF ");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data);

        Assert.NotNull(capturedOptions);
        Assert.Equal("ra_respuesta_radicado", capturedOptions!.TableName);
        Assert.Equal("WF", capturedOptions.DefaultAlias);
        Assert.Single(capturedOptions.Filters);
        Assert.True(capturedOptions.Filters.ContainsKey("ID_TAREA_WF"));
        Assert.Equal(123L, capturedOptions.Filters["ID_TAREA_WF"]);
    }

    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTareaAsync_CuandoHayFilas_RetornaLista()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<RaRespuestaRadicado>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RaRespuestaRadicado>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new RaRespuestaRadicado
                    {
                        IdRespuestaRadicado = 1,
                        IdRemitDestInt = 2,
                        IdArea = 3,
                        FechaRegistro = DateTime.UtcNow,
                        FechaVence = DateTime.UtcNow,
                        EstadoEnvioCorreo = 0,
                        TipoRespuestaElabUsuario = 0,
                        IdTipoDocRespuesta = 1,
                        TipoRespuesta = "R",
                        TipoRadicado = "T",
                        IdTipoRadicado = 1
                    }
                ]
            });

        var repository = new SolicitaEstructuraRespuestaIdTareaRepository(dapper.Object);
        var result = await repository.SolicitaEstructuraRespuestaIdTareaAsync(5, "WF");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Single(result.data);
    }

    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTareaAsync_CuandoEngineFalla_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<RaRespuestaRadicado>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RaRespuestaRadicado>
            {
                Success = false,
                Message = "DB error",
                Data = []
            });

        var repository = new SolicitaEstructuraRespuestaIdTareaRepository(dapper.Object);
        var result = await repository.SolicitaEstructuraRespuestaIdTareaAsync(5, "WF");

        Assert.False(result.success);
        Assert.Contains("Error consultando ra_respuesta_radicado", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data);
        Assert.NotEmpty(result.errors);
    }

    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTareaAsync_CuandoEngineLanzaExcepcion_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<RaRespuestaRadicado>(It.IsAny<QueryOptions>()))
            .ThrowsAsync(new InvalidOperationException("Boom"));

        var repository = new SolicitaEstructuraRespuestaIdTareaRepository(dapper.Object);
        var result = await repository.SolicitaEstructuraRespuestaIdTareaAsync(5, "WF");

        Assert.False(result.success);
        Assert.Contains("Inconsistencia general al consultar ra_respuesta_radicado", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data);
        Assert.NotEmpty(result.errors);
    }
}

