using MiApp.DTOs.DTOs.Workflow.BandejaCorrespondencia;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class WorkflowRouteColumnConfigRepositoryTests
{
    [Fact]
    public async Task GetColumnsByRouteAsync_CuandoRequestEsNulo_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var logger = new Mock<ILogger<WorkflowRouteColumnConfigRepository>>();
        var repository = new WorkflowRouteColumnConfigRepository(dapper.Object, logger.Object);

        var result = await repository.GetColumnsByRouteAsync(null);

        Assert.False(result.success);
        Assert.Equal("Request requerido", result.message);
        Assert.Null(result.data);
        dapper.Verify(d => d.GetAllAsync<ConfiguracionListadoRutaDto>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task GetColumnsByRouteAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var logger = new Mock<ILogger<WorkflowRouteColumnConfigRepository>>();
        var repository = new WorkflowRouteColumnConfigRepository(dapper.Object, logger.Object);

        var result = await repository.GetColumnsByRouteAsync(new WorkflowRouteColumnConfigRequestDto
        {
            IdRutaWorkflow = 7,
            DefaultDbAlias = string.Empty
        });

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.Null(result.data);
        dapper.Verify(d => d.GetAllAsync<ConfiguracionListadoRutaDto>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task GetColumnsByRouteAsync_CuandoRutaEsInvalida_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var logger = new Mock<ILogger<WorkflowRouteColumnConfigRepository>>();
        var repository = new WorkflowRouteColumnConfigRepository(dapper.Object, logger.Object);

        var result = await repository.GetColumnsByRouteAsync(new WorkflowRouteColumnConfigRequestDto
        {
            IdRutaWorkflow = 0,
            DefaultDbAlias = "WF"
        });

        Assert.False(result.success);
        Assert.Equal("IdRutaWorkflow requerido", result.message);
        Assert.Null(result.data);
        dapper.Verify(d => d.GetAllAsync<ConfiguracionListadoRutaDto>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task GetColumnsByRouteAsync_CuandoHayColumnasListaTarea_RetornaMetadataNormalizada()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var logger = new Mock<ILogger<WorkflowRouteColumnConfigRepository>>();
        QueryOptions? capturedOptions = null;

        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRutaDto>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(options => capturedOptions = options)
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRutaDto>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new ConfiguracionListadoRutaDto
                    {
                        IdConfiguracion = 1,
                        RutasWorkflowIdRuta = 7,
                        NombreCampo = "FechaRadicado",
                        TipoCampo = "datetime",
                        ListaTarea = true,
                        OrdenaTarea = 2,
                        CampoRadicado = true
                    },
                    new ConfiguracionListadoRutaDto
                    {
                        IdConfiguracion = 2,
                        RutasWorkflowIdRuta = 7,
                        NombreCampo = "Consecutivo",
                        TipoCampo = "varchar",
                        ListaTarea = true,
                        OrdenaTarea = 1,
                        CampoTramite = true
                    }
                ]
            });

        var repository = new WorkflowRouteColumnConfigRepository(dapper.Object, logger.Object);
        var result = await repository.GetColumnsByRouteAsync(new WorkflowRouteColumnConfigRequestDto
        {
            IdRutaWorkflow = 7,
            DefaultDbAlias = " WF ",
            Mode = WorkflowColumnListMode.ListaTarea
        });

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(7, result.data!.IdRutaWorkflow);
        Assert.Equal("ListaTarea", result.data.Mode);
        Assert.Equal(2, result.data.Columns.Count);
        Assert.Equal("Consecutivo", result.data.Columns[0].Key);
        Assert.Equal("DAT.Consecutivo", result.data.Columns[0].SqlColumnName);
        Assert.Equal("text", result.data.Columns[0].DataType);
        Assert.True(result.data.Columns[0].IsTramiteField);
        Assert.Equal("FechaRadicado", result.data.Columns[1].Key);
        Assert.Equal("datetime", result.data.Columns[1].DataType);
        Assert.True(result.data.Columns[1].IsRadicadoField);
        Assert.NotNull(capturedOptions);
        Assert.Equal("configuracion_listado_ruta", capturedOptions!.TableName);
        Assert.Equal("WF", capturedOptions.DefaultAlias);
        Assert.Equal(7, capturedOptions.Filters["Rutas_Workflow_id_Ruta"]);
        Assert.Equal(1, capturedOptions.Filters["Lista_Tarea"]);
        Assert.DoesNotContain("Lista_gestion_tamite", capturedOptions.Filters.Keys);
        Assert.Equal("Ordena_Tarea", capturedOptions.OrderByFields[0].Column);
        Assert.Equal("Id_Configuracion", capturedOptions.OrderByFields[1].Column);
    }

    [Fact]
    public async Task GetColumnsByRouteAsync_CuandoHayColumnasListaGestion_RetornaSoloColumnasValidas()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var logger = new Mock<ILogger<WorkflowRouteColumnConfigRepository>>();
        QueryOptions? capturedOptions = null;

        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRutaDto>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(options => capturedOptions = options)
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRutaDto>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new ConfiguracionListadoRutaDto
                    {
                        NombreCampo = "codigo_barras",
                        TipoCampo = "tinyint",
                        ListaGestionTamite = true,
                        OrdenListaGestionTamite = 1,
                        CampoCodigoBarras = true
                    },
                    new ConfiguracionListadoRutaDto
                    {
                        NombreCampo = "Asunto principal",
                        TipoCampo = "text",
                        ListaGestionTamite = true,
                        OrdenListaGestionTamite = 2
                    }
                ]
            });

        var repository = new WorkflowRouteColumnConfigRepository(dapper.Object, logger.Object);
        var result = await repository.GetColumnsByRouteAsync(new WorkflowRouteColumnConfigRequestDto
        {
            IdRutaWorkflow = 9,
            DefaultDbAlias = "WF",
            Mode = WorkflowColumnListMode.ListaGestionTramite
        });

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Single(result.data!.Columns);
        Assert.Equal("codigo_barras", result.data.Columns[0].Key);
        Assert.Equal("number", result.data.Columns[0].DataType);
        Assert.True(result.data.Columns[0].IsCodigoBarrasField);
        Assert.NotNull(capturedOptions);
        Assert.Equal(1, capturedOptions!.Filters["Lista_gestion_tamite"]);
        Assert.DoesNotContain("Lista_Tarea", capturedOptions.Filters.Keys);
        Assert.Equal("Orden_lista_gestion_tamite", capturedOptions.OrderByFields[0].Column);
    }

    [Fact]
    public async Task GetColumnsByRouteAsync_CuandoNoHayRegistros_RetornaSinResultados()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var logger = new Mock<ILogger<WorkflowRouteColumnConfigRepository>>();
        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRutaDto>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRutaDto>
            {
                Success = true,
                Message = "YES",
                Data = []
            });

        var repository = new WorkflowRouteColumnConfigRepository(dapper.Object, logger.Object);
        var result = await repository.GetColumnsByRouteAsync(new WorkflowRouteColumnConfigRequestDto
        {
            IdRutaWorkflow = 7,
            DefaultDbAlias = "WF"
        });

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!.Columns);
    }

    [Fact]
    public async Task GetColumnsByRouteAsync_CuandoTodasLasColumnasSonInvalidas_RetornaSinResultados()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var logger = new Mock<ILogger<WorkflowRouteColumnConfigRepository>>();
        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRutaDto>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRutaDto>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new ConfiguracionListadoRutaDto
                    {
                        NombreCampo = " ",
                        TipoCampo = "varchar",
                        OrdenaTarea = 1
                    },
                    new ConfiguracionListadoRutaDto
                    {
                        NombreCampo = "campo-con-guion",
                        TipoCampo = "varchar",
                        OrdenaTarea = 2
                    }
                ]
            });

        var repository = new WorkflowRouteColumnConfigRepository(dapper.Object, logger.Object);
        var result = await repository.GetColumnsByRouteAsync(new WorkflowRouteColumnConfigRequestDto
        {
            IdRutaWorkflow = 7,
            DefaultDbAlias = "WF"
        });

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!.Columns);
    }

    [Fact]
    public async Task GetColumnsByRouteAsync_CuandoEngineRetornaError_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var logger = new Mock<ILogger<WorkflowRouteColumnConfigRepository>>();
        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRutaDto>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRutaDto>
            {
                Success = false,
                Message = "boom"
            });

        var repository = new WorkflowRouteColumnConfigRepository(dapper.Object, logger.Object);
        var result = await repository.GetColumnsByRouteAsync(new WorkflowRouteColumnConfigRequestDto
        {
            IdRutaWorkflow = 7,
            DefaultDbAlias = "WF"
        });

        Assert.False(result.success);
        Assert.Contains("Error consultando configuracion_listado_ruta", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task GetColumnsByRouteAsync_CuandoEngineLanzaExcepcion_LogueaYRetornaError()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var logger = new Mock<ILogger<WorkflowRouteColumnConfigRepository>>();
        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRutaDto>(It.IsAny<QueryOptions>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var repository = new WorkflowRouteColumnConfigRepository(dapper.Object, logger.Object);
        var result = await repository.GetColumnsByRouteAsync(new WorkflowRouteColumnConfigRequestDto
        {
            IdRutaWorkflow = 7,
            DefaultDbAlias = "WF"
        });

        Assert.False(result.success);
        Assert.Contains("Inconsistencia WorkflowRouteColumnConfigRepository", result.message);
        Assert.Null(result.data);
        logger.VerifyLog(LogLevel.Error, Times.Once());
    }
}

internal static class LoggerMoqExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, Times times)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
}
