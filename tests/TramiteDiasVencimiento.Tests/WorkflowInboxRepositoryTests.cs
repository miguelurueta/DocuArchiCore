using System.Dynamic;
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
        var context = CreateContext();
        var request = CreateRequest();
        var columns = CreateColumns();

        builder
            .Setup(q => q.Build(request, context, columns, "WF"))
            .Returns(expectedQuery);

        dapper
            .Setup(engine => engine.GetAllAsync<ExpandoObject>(expectedQuery))
            .ReturnsAsync(new QueryResult<ExpandoObject>
            {
                Success = true,
                Message = "YES",
                Data = [CreateRow(10)],
                TotalRecords = 1
            });

        var repository = new WorkflowInboxRepository(dapper.Object, builder.Object);

        var result = await repository.GetInboxAsync(request, context, columns, "WF");

        Assert.True(result.success);
        Assert.Single(result.data);
        builder.Verify(q => q.Build(request, context, columns, "WF"), Times.Once);
    }

    [Fact]
    public async Task GetInboxAsync_CuandoEngineFalla_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var builder = new Mock<IWorkflowInboxQueryBuilder>();
        var expectedQuery = new QueryOptions { TableName = "tabla", DefaultAlias = "WF" };

        builder
            .Setup(q => q.Build(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), It.IsAny<WorkflowInboxResolvedContextDto>(), It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(), "WF"))
            .Returns(expectedQuery);

        dapper
            .Setup(engine => engine.GetAllAsync<ExpandoObject>(expectedQuery))
            .ReturnsAsync(new QueryResult<ExpandoObject>
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

    private static WorkflowInboxDynamicTableRequestDto CreateRequest() => new()
    {
        EstadoTramite = "Todos",
        Page = 1,
        PageSize = 25,
        SortDir = "ASC"
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

    private static ExpandoObject CreateRow(int idTarea)
    {
        dynamic row = new ExpandoObject();
        row.id_tarea = idTarea;
        row.asunto = "Caso";
        return row;
    }
}
