using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Workflow.BandejaCorrespondencia;
using MiApp.Repository.DataAccess;
using MiApp.Services.Service.Workflow.BandejaCorrespondencia;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class WorkflowInboxRepositoryTests
{
    [Fact]
    public async Task GetInboxAsync_CuandoContextoEsValido_UsaQueryBuilderYRetornaDatos()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var builder = new Mock<IWorkflowInboxQueryBuilder>();
        var expectedQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF" };
        var expectedCountQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF", RawSelect = "COUNT(1) AS total_count" };
        var context = CreateContext();
        var request = CreateRequest();
        var columns = CreateColumns();

        builder
            .Setup(q => q.Build(request, context, columns, "WF"))
            .Returns(expectedQuery);
        builder
            .Setup(q => q.BuildCount(request, context, columns, "WF"))
            .Returns(expectedCountQuery);

        dapper
            .Setup(engine => engine.GetAllAsync<object>(expectedQuery))
            .ReturnsAsync(new QueryResult<object>
            {
                Success = true,
                Message = "YES",
                Data = [CreateRow(10)]
            });
        dapper
            .Setup(engine => engine.GetAllAsync<WorkflowInboxCountRowDto>(expectedCountQuery))
            .ReturnsAsync(new QueryResult<WorkflowInboxCountRowDto>
            {
                Success = true,
                Message = "YES",
                Data = [new WorkflowInboxCountRowDto { Total_Count = 50 }]
            });

        var repository = new WorkflowInboxRepository(dapper.Object, builder.Object);

        var result = await repository.GetInboxAsync(request, context, columns, "WF");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(50, result.data!.TotalRecords);
        Assert.Single(result.data.Rows);
        Assert.IsType<Dictionary<string, object?>>(result.data.Rows[0]);
        Assert.Equal(10, result.data.Rows[0]["id_tarea"]);
        Assert.Equal(10, result.data.Rows[0]["id"]);
        builder.Verify(q => q.Build(request, context, columns, "WF"), Times.Once);
        builder.Verify(q => q.BuildCount(request, context, columns, "WF"), Times.Once);
    }

    [Fact]
    public async Task GetInboxAsync_CuandoEngineFalla_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var builder = new Mock<IWorkflowInboxQueryBuilder>();
        var expectedQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF" };
        var expectedCountQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF", RawSelect = "COUNT(1) AS total_count" };

        builder
            .Setup(q => q.Build(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), It.IsAny<WorkflowInboxResolvedContextDto>(), It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(), "WF"))
            .Returns(expectedQuery);
        builder
            .Setup(q => q.BuildCount(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), It.IsAny<WorkflowInboxResolvedContextDto>(), It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(), "WF"))
            .Returns(expectedCountQuery);

        dapper
            .Setup(engine => engine.GetAllAsync<object>(expectedQuery))
            .ReturnsAsync(new QueryResult<object>
            {
                Success = false,
                Message = "boom"
            });

        var repository = new WorkflowInboxRepository(dapper.Object, builder.Object);

        var result = await repository.GetInboxAsync(CreateRequest(), CreateContext(), CreateColumns(), "WF");

        Assert.False(result.success);
        Assert.Equal("Error consultando bandeja workflow", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Field == "GetInboxAsync");
    }

    [Fact]
    public async Task GetInboxAsync_CuandoFilaNoTieneIdTarea_NoAgregaId()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var builder = new Mock<IWorkflowInboxQueryBuilder>();
        var expectedQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF" };
        var expectedCountQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF", RawSelect = "COUNT(1) AS total_count" };

        builder
            .Setup(q => q.Build(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), It.IsAny<WorkflowInboxResolvedContextDto>(), It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(), "WF"))
            .Returns(expectedQuery);
        builder
            .Setup(q => q.BuildCount(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), It.IsAny<WorkflowInboxResolvedContextDto>(), It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(), "WF"))
            .Returns(expectedCountQuery);

        dapper
            .Setup(engine => engine.GetAllAsync<object>(expectedQuery))
            .ReturnsAsync(new QueryResult<object>
            {
                Success = true,
                Message = "YES",
                Data = [CreateRowWithoutId()]
            });
        dapper
            .Setup(engine => engine.GetAllAsync<WorkflowInboxCountRowDto>(expectedCountQuery))
            .ReturnsAsync(new QueryResult<WorkflowInboxCountRowDto>
            {
                Success = true,
                Message = "YES",
                Data = [new WorkflowInboxCountRowDto { Total_Count = 1 }]
            });

        var repository = new WorkflowInboxRepository(dapper.Object, builder.Object);

        var result = await repository.GetInboxAsync(CreateRequest(), CreateContext(), CreateColumns(), "WF");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Single(result.data!.Rows);
        Assert.DoesNotContain("id", result.data.Rows[0].Keys, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetInboxAsync_CuandoConteoFalla_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var builder = new Mock<IWorkflowInboxQueryBuilder>();
        var expectedQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF" };
        var expectedCountQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF", RawSelect = "COUNT(1) AS total_count" };

        builder
            .Setup(q => q.Build(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), It.IsAny<WorkflowInboxResolvedContextDto>(), It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(), "WF"))
            .Returns(expectedQuery);
        builder
            .Setup(q => q.BuildCount(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), It.IsAny<WorkflowInboxResolvedContextDto>(), It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(), "WF"))
            .Returns(expectedCountQuery);

        dapper
            .Setup(engine => engine.GetAllAsync<object>(expectedQuery))
            .ReturnsAsync(new QueryResult<object>
            {
                Success = true,
                Message = "YES",
                Data = []
            });
        dapper
            .Setup(engine => engine.GetAllAsync<WorkflowInboxCountRowDto>(expectedCountQuery))
            .ReturnsAsync(new QueryResult<WorkflowInboxCountRowDto>
            {
                Success = false,
                Message = "count boom"
            });

        var repository = new WorkflowInboxRepository(dapper.Object, builder.Object);

        var result = await repository.GetInboxAsync(CreateRequest(), CreateContext(), CreateColumns(), "WF");

        Assert.False(result.success);
        Assert.Equal("Error consultando total de bandeja workflow", result.message);
    }

    [Fact]
    public async Task GetAutocompleteAsync_CuandoContextoEsValido_DeduplicaYRespetaLimite()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var builder = new Mock<IWorkflowInboxQueryBuilder>();
        var firstQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF" };
        var secondQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF" };
        var context = CreateContext();
        var request = CreateRequest();
        var columns = CreateColumns();

        builder
            .Setup(q => q.BuildAutocomplete("ABC", 2, request, context, columns, "WF"))
            .Returns([firstQuery, secondQuery]);

        dapper
            .Setup(engine => engine.GetAllAsync<WorkflowInboxAutocompleteItemDto>(firstQuery))
            .ReturnsAsync(new QueryResult<WorkflowInboxAutocompleteItemDto>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new WorkflowInboxAutocompleteItemDto { Value = "ABC-123", Field = "asunto" },
                    new WorkflowInboxAutocompleteItemDto { Value = "ABC-123", Field = "asunto" }
                ]
            });
        dapper
            .Setup(engine => engine.GetAllAsync<WorkflowInboxAutocompleteItemDto>(secondQuery))
            .ReturnsAsync(new QueryResult<WorkflowInboxAutocompleteItemDto>
            {
                Success = true,
                Message = "YES",
                Data = [new WorkflowInboxAutocompleteItemDto { Value = "ABC-456", Label = "ABC-456", Field = "asunto" }]
            });

        var repository = new WorkflowInboxRepository(dapper.Object, builder.Object);

        var result = await repository.GetAutocompleteAsync(request, "ABC", 2, context, columns, "WF");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(2, result.data!.Items.Count);
        Assert.Equal("ABC-123", result.data.Items[0].Value);
        Assert.Equal("ABC-123", result.data.Items[0].Label);
        Assert.Equal("asunto", result.data.Items[0].Field);
        Assert.Equal("ABC-456", result.data.Items[1].Value);
    }

    private static WorkflowInboxDynamicTableRequestDto CreateRequest() => new()
    {
        TableId = "workflowInboxgestion",
        IdUsuarioGestion = 10,
        IdRutaWorkflow = 7,
        NombreRuta = "RUTA_A",
        DefaultDbAlias = "DA",
        IdActividad = 77,
        IdUsuarioWorkflow = 99,
        EstadoTramite = "Todos",
        TipoConsulta = 1,
        Page = 1,
        PageSize = 25,
        SortDir = "ASC",
        StructuredFilters = []
    };

    private static WorkflowInboxResolvedContextDto CreateContext() => new()
    {
        IdUsuarioWorkflow = 99,
        IdGrupoWorkflow = 14,
        IdRutaWorkflow = 7,
        NombreRuta = "RUTA_A",
        IdActividad = 77
    };

    private static List<WorkflowDynamicColumnDefinitionDto> CreateColumns() =>
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
        }
    ];

    private static Dictionary<string, object?> CreateRow(int idTarea)
    {
        return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["id_tarea"] = idTarea,
            ["asunto"] = "Caso"
        };
    }

    private static Dictionary<string, object?> CreateRowWithoutId()
    {
        return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["asunto"] = "Caso"
        };
    }
}
