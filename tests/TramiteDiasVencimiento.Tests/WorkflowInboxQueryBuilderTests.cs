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
    public void Build_CuandoTipoEsInvalido_UsaComportamientoLegacy()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(searchType: 99, search: "rad"), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Empty(result.RawConditions);
    }

    [Fact]
    public void Build_CuandoTipoEsDos_UsaSoloColumnasTextoVisiblesYFiltrables()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(searchType: 2, search: "O'Hara"), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Single(result.RawConditions);
        Assert.Equal(
            "AND (DAT.asunto LIKE '%O''Hara%' ESCAPE '\\' OR DAT.remitente LIKE '%O''Hara%' ESCAPE '\\')",
            result.RawConditions[0]);
    }

    [Fact]
    public void Build_CuandoTipoEsDosYSearchEsCorto_NoAgregaRawCondition()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(searchType: 2, search: " a "), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Empty(result.RawConditions);
    }

    [Fact]
    public void Build_CuandoTipoEsDosYSearchTieneComodines_LosEscapaComoLiterales()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(searchType: 2, search: "a%_b"), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Single(result.RawConditions);
        Assert.Equal(
            "AND (DAT.asunto LIKE '%a\\%\\_b%' ESCAPE '\\' OR DAT.remitente LIKE '%a\\%\\_b%' ESCAPE '\\')",
            result.RawConditions[0]);
    }

    [Fact]
    public void Build_CuandoTipoEsDosYSearchTieneCorchetes_LosEscapaComoLiterales()
    {
        var builder = new WorkflowInboxQueryBuilder();

        var result = builder.Build(CreateRequest(searchType: 2, search: "rad[12]"), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Single(result.RawConditions);
        Assert.Equal(
            "AND (DAT.asunto LIKE '%rad\\[12\\]%' ESCAPE '\\' OR DAT.remitente LIKE '%rad\\[12\\]%' ESCAPE '\\')",
            result.RawConditions[0]);
    }

    [Fact]
    public void Build_CuandoTipoEsDosYSearchSuperaMaximo_TruncaTermino()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var expectedSearch = new string('x', WorkflowInboxQueryPolicy.LikeSearchMaxLength);

        var result = builder.Build(CreateRequest(searchType: 2, search: new string('x', WorkflowInboxQueryPolicy.LikeSearchMaxLength + 5)), CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Single(result.RawConditions);
        Assert.Equal(
            $"AND (DAT.asunto LIKE '%{expectedSearch}%' ESCAPE '\\' OR DAT.remitente LIKE '%{expectedSearch}%' ESCAPE '\\')",
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
    public void Build_CuandoTipoEsDosIgnoraColumnasNoFiltrables()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var columns = CreateDynamicColumns();
        columns.Add(new WorkflowDynamicColumnDefinitionDto
        {
            Key = "observacion",
            ColumnName = "observacion",
            SqlColumnName = "DAT.observacion",
            DataType = "text",
            IsVisible = true,
            IsFilterable = false,
            IsSortable = true
        });

        var result = builder.Build(CreateRequest(searchType: 2, search: "rad"), CreateContext(), columns, "WF");

        Assert.Single(result.RawConditions);
        Assert.DoesNotContain("DAT.observacion", result.RawConditions[0], StringComparison.OrdinalIgnoreCase);
        Assert.Equal(
            "AND (DAT.asunto LIKE '%rad%' ESCAPE '\\' OR DAT.remitente LIKE '%rad%' ESCAPE '\\')",
            result.RawConditions[0]);
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
    public void Build_CuandoStructuredFiltersSonValidos_LosAgregaComoCondicionesAdicionales()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var request = CreateRequest();
        request.StructuredFilters =
        [
            new WorkflowStructuredFilterDto
            {
                Field = "asunto",
                Operator = "contains",
                Value = "contrato"
            },
            new WorkflowStructuredFilterDto
            {
                Field = "fecha_inicio",
                Operator = "between",
                ValueFrom = "2026-01-01",
                ValueTo = "2026-12-31"
            },
            new WorkflowStructuredFilterDto
            {
                Field = "id_tarea",
                Operator = "gte",
                Value = 50
            }
        ];

        var result = builder.Build(request, CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Equal(3, result.RawConditions.Count);
        Assert.Equal("AND (DAT.asunto LIKE '%contrato%')", result.RawConditions[0]);
        Assert.Equal("AND (etw.Fecha_Inicio BETWEEN '2026-01-01' AND '2026-12-31')", result.RawConditions[1]);
        Assert.Equal("AND (etw.Inicio_Tareas_Workflow_id_Tarea >= 50)", result.RawConditions[2]);
    }

    [Fact]
    public void Build_CuandoStructuredFiltersSeCombinanConSearchTypeDos_ConservaAmbasCapas()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var request = CreateRequest(searchType: 2, search: "rad");
        request.StructuredFilters =
        [
            new WorkflowStructuredFilterDto
            {
                Field = "remitente",
                Operator = "startsWith",
                Value = "ana"
            }
        ];

        var result = builder.Build(request, CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Equal(2, result.RawConditions.Count);
        Assert.Equal("AND (DAT.asunto LIKE '%rad%' ESCAPE '\\' OR DAT.remitente LIKE '%rad%' ESCAPE '\\')", result.RawConditions[0]);
        Assert.Equal("AND (DAT.remitente LIKE 'ana%')", result.RawConditions[1]);
    }

    [Fact]
    public void Build_CuandoStructuredFilterTieneCampoInvalido_LanzaErrorControlado()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var request = CreateRequest();
        request.StructuredFilters =
        [
            new WorkflowStructuredFilterDto
            {
                Field = "campo_no_permitido",
                Operator = "eq",
                Value = "x"
            }
        ];

        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.Build(request, CreateContext(), CreateDynamicColumns(), "WF"));

        Assert.Contains("campo no permitido", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Build_CuandoStructuredFilterTieneOperadorInvalido_LanzaErrorControlado()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var request = CreateRequest();
        request.StructuredFilters =
        [
            new WorkflowStructuredFilterDto
            {
                Field = "asunto",
                Operator = "hack",
                Value = "x"
            }
        ];

        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.Build(request, CreateContext(), CreateDynamicColumns(), "WF"));

        Assert.Contains("operador", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Build_CuandoStructuredFilterContainsRecibeCampoNoTexto_LanzaErrorControlado()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var request = CreateRequest();
        request.StructuredFilters =
        [
            new WorkflowStructuredFilterDto
            {
                Field = "fecha_radicado",
                Operator = "contains",
                Value = "2026"
            }
        ];

        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.Build(request, CreateContext(), CreateDynamicColumns(), "WF"));

        Assert.Contains("solo aplica a campos texto", exception.Message, StringComparison.OrdinalIgnoreCase);
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
    public void BuildCount_CuandoRequestEsValido_ReutilizaFiltrosSinOrderNiPaginacion()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var request = CreateRequest(searchType: 2, search: "rad", sortField: "id_tarea", sortDir: "DESC", page: 3, pageSize: 10, estadoTramite: "Abierto");
        request.StructuredFilters =
        [
            new WorkflowStructuredFilterDto
            {
                Field = "remitente",
                Operator = "startsWith",
                Value = "ana"
            }
        ];

        var result = builder.BuildCount(request, CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Equal("COUNT(1) AS total_count", result.RawSelect);
        Assert.Empty(result.Columns);
        Assert.Null(result.Limit);
        Assert.Null(result.Offset);
        Assert.Empty(result.OrderByFields);
        Assert.Equal("Abierto", result.Filters["estado_tramite"]);
        Assert.Equal(2, result.RawConditions.Count);
        Assert.Equal("AND (DAT.asunto LIKE '%rad%' ESCAPE '\\' OR DAT.remitente LIKE '%rad%' ESCAPE '\\')", result.RawConditions[0]);
        Assert.Equal("AND (DAT.remitente LIKE 'ana%')", result.RawConditions[1]);
        Assert.Single(result.Joins);
    }

    [Fact]
    public void BuildExport_CuandoExportModeEsAllMatching_ReutilizaFiltrosYOrdenSinPaginacion()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var request = CreateRequest(searchType: 2, search: "rad", sortField: "id_tarea", sortDir: "DESC", page: 8, pageSize: 5, estadoTramite: "Abierto");
        request.ExportMode = WorkflowInboxExportModes.AllMatching;
        request.StructuredFilters =
        [
            new WorkflowStructuredFilterDto
            {
                Field = "remitente",
                Operator = "startsWith",
                Value = "ana"
            }
        ];

        var result = builder.BuildExport(request, CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Null(result.Limit);
        Assert.Null(result.Offset);
        Assert.Single(result.OrderByFields);
        Assert.Equal("etw.Inicio_Tareas_Workflow_id_Tarea", result.OrderByFields[0].Column);
        Assert.Equal("DESC", result.OrderByFields[0].Direction);
        Assert.Equal("Abierto", result.Filters["estado_tramite"]);
        Assert.Equal(2, result.RawConditions.Count);
        Assert.Equal("AND (DAT.asunto LIKE '%rad%' ESCAPE '\\' OR DAT.remitente LIKE '%rad%' ESCAPE '\\')", result.RawConditions[0]);
        Assert.Equal("AND (DAT.remitente LIKE 'ana%')", result.RawConditions[1]);
    }

    [Fact]
    public void BuildExport_CuandoExportModeEsCurrentPage_RespetaPaginacionVisible()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var request = CreateRequest(searchType: 2, search: "rad", sortField: "id_tarea", sortDir: "DESC", page: 8, pageSize: 5, estadoTramite: "Abierto");
        request.ExportMode = WorkflowInboxExportModes.CurrentPage;
        request.StructuredFilters =
        [
            new WorkflowStructuredFilterDto
            {
                Field = "remitente",
                Operator = "startsWith",
                Value = "ana"
            }
        ];

        var result = builder.BuildExport(request, CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Equal(5, result.Limit);
        Assert.Equal(35, result.Offset);
        Assert.Single(result.OrderByFields);
        Assert.Equal("etw.Inicio_Tareas_Workflow_id_Tarea", result.OrderByFields[0].Column);
        Assert.Equal("DESC", result.OrderByFields[0].Direction);
        Assert.Equal("Abierto", result.Filters["estado_tramite"]);
        Assert.Equal(2, result.RawConditions.Count);
        Assert.Equal("AND (DAT.asunto LIKE '%rad%' ESCAPE '\\' OR DAT.remitente LIKE '%rad%' ESCAPE '\\')", result.RawConditions[0]);
        Assert.Equal("AND (DAT.remitente LIKE 'ana%')", result.RawConditions[1]);
    }

    [Fact]
    public void BuildBuildCountYBuildExport_UsanMismaCondicionLike()
    {
        var builder = new WorkflowInboxQueryBuilder();
        var request = CreateRequest(searchType: 2, search: " rad%_[x] ");

        var rows = builder.Build(request, CreateContext(), CreateDynamicColumns(), "WF");
        var count = builder.BuildCount(request, CreateContext(), CreateDynamicColumns(), "WF");
        var export = builder.BuildExport(request, CreateContext(), CreateDynamicColumns(), "WF");

        Assert.Single(rows.RawConditions);
        Assert.Equal(rows.RawConditions, count.RawConditions);
        Assert.Equal(rows.RawConditions, export.RawConditions);
        Assert.Equal(
            "AND (DAT.asunto LIKE '%rad\\%\\_\\[x\\]%' ESCAPE '\\' OR DAT.remitente LIKE '%rad\\%\\_\\[x\\]%' ESCAPE '\\')",
            rows.RawConditions[0]);
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
