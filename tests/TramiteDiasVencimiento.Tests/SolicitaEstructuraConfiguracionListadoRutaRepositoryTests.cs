using MiApp.Models.Models.Workflow.RutaTrabajo;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaEstructuraConfiguracionListadoRutaRepositoryTests
{
    [Fact]
    public async Task SolicitaEstructuraConfiguracionListadoRutaRepository_CuandoIdRutaEsInvalido_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaEstructuraConfiguracionListadoRutaRepository(dapper.Object);

        var result = await repository.SolicitaEstructuraConfiguracionListadoRutaAsync(0, "WF");

        Assert.False(result.success);
        Assert.Equal("IdRuta requerido", result.message);
        Assert.Null(result.data);
        dapper.Verify(d => d.GetAllAsync<ConfiguracionListadoRuta>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaEstructuraConfiguracionListadoRutaRepository_CuandoHayDatos_RetornaConfiguracion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        QueryOptions? capturedOptions = null;

        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRuta>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(options => capturedOptions = options)
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRuta>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new ConfiguracionListadoRuta
                    {
                        Id_Configuracion = 1,
                        Rutas_Workflow_id_Ruta = 7,
                        id_Campo = 10,
                        Nombre_Campo = "Consecutivo",
                        Tipo_Campo = "text",
                        Lista_Tarea = 1,
                        Ordena_Tarea = 2,
                        Prioridad = 1,
                        campo_radicado = 1,
                        campo_tramite = 0,
                        campo_fecha_vence = 0,
                        Campo_beneficiario = 0,
                        Lista_gestion_tamite = 1,
                        Orden_lista_gestion_tamite = 2,
                        campo_codigo_barras = 0
                    }
                ]
            });

        var repository = new SolicitaEstructuraConfiguracionListadoRutaRepository(dapper.Object);
        var result = await repository.SolicitaEstructuraConfiguracionListadoRutaAsync(7, "WF");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Single(result.data!);
        Assert.NotNull(capturedOptions);
        Assert.Equal("configuracion_listado_ruta", capturedOptions!.TableName);
        Assert.Equal("WF", capturedOptions.DefaultAlias);
        Assert.Equal(7, capturedOptions.Filters["Rutas_Workflow_id_Ruta"]);
    }

    [Fact]
    public async Task SolicitaEstructuraConfiguracionListadoRutaRepository_CuandoNoHayRegistros_RetornaSinResultados()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRuta>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRuta>
            {
                Success = true,
                Message = "YES",
                Data = []
            });

        var repository = new SolicitaEstructuraConfiguracionListadoRutaRepository(dapper.Object);
        var result = await repository.SolicitaEstructuraConfiguracionListadoRutaAsync(7, "WF");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task SolicitaEstructuraConfiguracionListadoRutaRepository_CuandoDapperFalla_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRuta>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRuta>
            {
                Success = false,
                Message = "boom"
            });

        var repository = new SolicitaEstructuraConfiguracionListadoRutaRepository(dapper.Object);
        var result = await repository.SolicitaEstructuraConfiguracionListadoRutaAsync(7, "WF");

        Assert.False(result.success);
        Assert.Contains("Error consultando configuracion_listado_ruta", result.message);
        Assert.Null(result.data);
    }
}
