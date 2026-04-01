using MiApp.DTOs.DTOs.Workflow.BandejaCorrespondencia;
using MiApp.Repository.DataAccess;
using MiApp.Services.Service.Workflow.BandejaCorrespondencia;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class WorkflowInboxQueryBuilderTests
{
    [Fact]
    public void Build_CuandoTipoEsUno_NoAgregaBusquedaAdicional()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(searchType: 1, search: "rad"), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Equal("dat_adic_tarRUTA_A DAT", result.TableName);
        Assert.Equal("WF", result.DefaultAlias);
        Assert.Empty(result.RawConditions);
        Assert.Equal(2, result.AdvancedFilters.Count);
    }

    [Fact]
    public void Build_CuandoTipoEsDos_UsaSoloColumnasTextoVisiblesYFiltrables()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(searchType: 2, search: "O'Hara"), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Single(result.RawConditions);
        Assert.Equal(
            "AND (DAT.asunto LIKE '%O''Hara%' OR DAT.remitente LIKE '%O''Hara%')",
            result.RawConditions[0]);
    }

    [Fact]
    public void Build_CuandoTipoEsDosYNoHayColumnasTexto_NoAgregaRawCondition()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var columns = new List<WorkflowDynamicColumnDefinitionDto>
        {
            new()
            {
                Key = "fecha_radicado",
                ColumnName = "fecha_radicado",
                SqlColumnName = "DAT.fecha_radicado",
                DataType = "datetime",
                IsVisible = true,
                IsFilterable = true
            }
        };

        var result = builder.Build(CreateRequest(searchType: 2, search: "rad"), CreateContext(), columns, "WF");

        Assert.Empty(result.RawConditions);
    }

    [Fact]
    public void Build_CuandoTipoEsTresYExpresionEsValida_AplicaWhitelist()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(
            CreateRequest(searchType: 3, search: "asunto LIKE '%2026%' AND fecha_inicio >= '2026-01-01'"),
            CreateContext(),
            CreateDynamicColumns(),
            "WF");

        Assert.Single(result.RawConditions);
        Assert.Equal(
            "AND (DAT.asunto LIKE '%2026%' AND etw.Fecha_Inicio >= '2026-01-01')",
            result.RawConditions[0]);
    }

    [Fact]
    public void Build_CuandoTipoEsTresYExpresionEsPeligrosa_LaBloquea()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(
            CreateRequest(searchType: 3, search: "asunto LIKE '%x%' ; DROP TABLE x"),
            CreateContext(),
            CreateDynamicColumns(),
            "WF");

        Assert.Empty(result.RawConditions);
    }

    [Fact]
    public void Build_CuandoSortEsValido_ResuelveColumnaSolicitada()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(sortField: "id_tarea", sortDir: "DESC"), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Single(result.OrderByFields);
        Assert.Equal("etw.Inicio_Tareas_Workflow_id_Tarea", result.OrderByFields[0].Column);
        Assert.Equal("DESC", result.OrderByFields[0].Direction);
    }

    [Fact]
    public void Build_CuandoSortEsInvalido_UsaFechaInicioComoFallback()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(sortField: "no_existe"), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Single(result.OrderByFields);
        Assert.Equal("etw.Fecha_Inicio", result.OrderByFields[0].Column);
        Assert.Equal("ASC", result.OrderByFields[0].Direction);
    }

    [Fact]
    public void Build_CuandoPaginacionEsInvalida_AplicaDefaults()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(page: 0, pageSize: 0), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Equal(1000, result.Limit);
        Assert.Equal(0, result.Offset);
    }

    [Fact]
    public void Build_CuandoContextoIncluyeNumeroTareaLista_PriorizaEseLimit()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var context = CreateContext();
        context.NumeroTareaLista = 40;

        var result = builder.Build(
            CreateRequest(page: 3, pageSize: 10),
            context,
            CreateDynamicColumns(),
            "WF");

        Assert.Equal(40, result.Limit);
        Assert.Equal(80, result.Offset);
    }

    [Fact]
    public void Build_CuandoNumeroTareaListaEsInvalido_UsaFallbackDePageSize()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var context = CreateContext();
        context.NumeroTareaLista = 0;

        var result = builder.Build(
            CreateRequest(page: 2, pageSize: 10),
            context,
            CreateDynamicColumns(),
            "WF");

        Assert.Equal(10, result.Limit);
        Assert.Equal(10, result.Offset);
    }

    [Fact]
    public void Build_CuandoRequestEsValido_IncluyeColumnasDinamicasYFiltrosBase()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(estadoTramite: "Abierto", page: 3, pageSize: 10), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Contains("etw.Inicio_Tareas_Workflow_id_Tarea AS id_tarea", result.Columns);
        Assert.Contains("DAT.ESTADO_TRAMITE AS ESTADO", result.Columns);
        Assert.Contains("DAT.asunto AS asunto", result.Columns);
        Assert.Contains("DAT.remitente AS remitente", result.Columns);
        Assert.Equal(10, result.Limit);
        Assert.Equal(20, result.Offset);
        Assert.Equal(7, result.Filters["Id_Actividad"]);
        Assert.Equal(99, result.Filters["Id_Usuario"]);
        Assert.Equal(0, result.Filters["Estado_Tarea"]);
        Assert.Equal(0, result.Filters["ESTADO_ACTIVIDA_MODULO_RAD"]);
        Assert.Equal(1, result.Filters["estado_modulo_radicado"]);
        Assert.Equal("Abierto", result.Filters["estado_tramite"]);
        Assert.Single(result.Joins);
    }

    [Fact]
    public void Build_CuandoContextoDifiereDelRequest_UsaValoresDelContexto()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var request = CreateRequest();
        request.NombreRuta = "RUTA_LEGACY";
        request.IdActividad = 1;
        request.IdUsuarioWorkflow = 2;

        var context = new WorkflowInboxResolvedContextDto
        {
            IdUsuarioWorkflow = 99,
            IdGrupoWorkflow = 14,
            IdRutaWorkflow = 7,
            NombreRuta = "RUTA_REAL",
            IdActividad = 77
        };

        var result = builder.Build(request, context, CreateDynamicColumns(), "WF_CTX");

        Assert.Equal("dat_adic_tarRUTA_REAL DAT", result.TableName);
        Assert.Equal("WF_CTX", result.DefaultAlias);
        Assert.Equal(77, result.Filters["Id_Actividad"]);
        Assert.Equal(99, result.Filters["Id_Usuario"]);
    }

    private static WorkflowInboxDynamicTableRequestDto CreateRequest(
        int searchType = 1,
        string search = "",
        string sortField = "",
        string sortDir = "ASC",
        int page = 1,
        int pageSize = 25,
        string estadoTramite = "Todos")
    {
        return new WorkflowInboxDynamicTableRequestDto
        {
            TableId = "workflowInboxgestion",
            IdUsuarioGestion = 10,
            IdRutaWorkflow = 7,
            NombreRuta = "RUTA_A",
            DefaultDbAlias = "WF",
            IdActividad = 7,
            IdUsuarioWorkflow = 99,
            EstadoTramite = estadoTramite,
            TipoConsulta = searchType,
            Search = search,
            SortField = sortField,
            SortDir = sortDir,
            Page = page,
            PageSize = pageSize,
            StructuredFilters = []
        };
    }

    private static List<WorkflowDynamicColumnDefinitionDto> CreateDynamicColumns()
    {
        return
        [
            new WorkflowDynamicColumnDefinitionDto
            {
                Key = "asunto",
                ColumnName = "asunto",
                SqlColumnName = "DAT.asunto",
                DataType = "text",
                IsVisible = true,
                IsFilterable = true,
                IsSortable = true
            },
            new WorkflowDynamicColumnDefinitionDto
            {
                Key = "remitente",
                ColumnName = "remitente",
                SqlColumnName = "DAT.remitente",
                DataType = "text",
                IsVisible = true,
                IsFilterable = true,
                IsSortable = true
            },
            new WorkflowDynamicColumnDefinitionDto
            {
                Key = "fecha_radicado",
                ColumnName = "fecha_radicado",
                SqlColumnName = "DAT.fecha_radicado",
                DataType = "datetime",
                IsVisible = true,
                IsFilterable = true,
                IsSortable = true
            },
            new WorkflowDynamicColumnDefinitionDto
            {
                Key = "oculta",
                ColumnName = "oculta",
                SqlColumnName = "DAT.oculta",
                DataType = "text",
                IsVisible = false,
                IsFilterable = true,
                IsSortable = true
            }
        ];
    }

    private static WorkflowInboxResolvedContextDto CreateContext()
    {
        return new WorkflowInboxResolvedContextDto
        {
            IdUsuarioWorkflow = 99,
            IdGrupoWorkflow = 14,
            IdRutaWorkflow = 7,
            NombreRuta = "RUTA_A",
            IdActividad = 7
        };
    }
}
