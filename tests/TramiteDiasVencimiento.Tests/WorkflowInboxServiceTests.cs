using System.Dynamic;
using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.BandejaCorrespondencia;
using MiApp.Repository.Repositorio.Workflow.BandejaCorrespondencia;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.UI.MuiTable;
using MiApp.Services.Service.Workflow.BandejaCorrespondencia;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class WorkflowInboxServiceTests
{
    [Fact]
    public async Task SolicitaBandejaWorkflowAsync_CuandoClaimWorkflowNoExiste_RetornaValidacion()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(service => service.GetClaimValue("defaulaliaswf")).Returns((string?)null);

        var service = CreateService(currentUserService: currentUser);

        var result = await service.SolicitaBandejaWorkflowAsync(CreateRequest(), 10, "DA");

        Assert.False(result.success);
        Assert.Equal("Claim defaulaliaswf requerido para consultar bandeja workflow", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Field == "defaulaliaswf");
    }

    [Fact]
    public async Task SolicitaBandejaWorkflowAsync_CuandoRepositorioNoRetornaFilas_PropagaSinResultados()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var context = CreateContext();
        var request = CreateRequest();

        contextResolver
            .Setup(service => service.ResolveAsync(10))
            .ReturnsAsync(new AppResponses<WorkflowInboxResolvedContextDto>
            {
                success = true,
                message = "OK",
                data = context,
                errors = []
            });

        metadataRepository
            .Setup(repo => repo.GetColumnsByRouteAsync(It.IsAny<WorkflowRouteColumnConfigRequestDto>()))
            .ReturnsAsync(new AppResponses<WorkflowRouteColumnConfigResultDto?>
            {
                success = true,
                message = "OK",
                data = new WorkflowRouteColumnConfigResultDto
                {
                    IdRutaWorkflow = context.IdRutaWorkflow,
                    Mode = request.ColumnMode.ToString(),
                    Columns = CreateColumns()
                },
                errors = []
            });

        inboxRepository
            .Setup(repo => repo.GetInboxAsync(request, context, It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(), "WF"))
            .ReturnsAsync(new AppResponses<List<ExpandoObject>>
            {
                success = true,
                message = "Sin resultados",
                data = [],
                errors = []
            });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository);

        var result = await service.SolicitaBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
    }

    [Fact]
    public async Task SolicitaBandejaWorkflowAsync_CuandoContextoYMetadataSonValidos_ConstruyeTabla()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var dynamicColumnsBuilder = new Mock<IWorkflowDynamicUiColumnBuilder>();
        var tableBuilder = new Mock<IDynamicUiTableBuilder>();
        var context = CreateContext();
        var request = CreateRequest();
        request.IdUsuarioWorkflow = 1;
        request.IdActividad = 2;
        request.NombreRuta = "LEGACY";

        var columns = CreateColumns();

        contextResolver
            .Setup(service => service.ResolveAsync(10))
            .ReturnsAsync(new AppResponses<WorkflowInboxResolvedContextDto>
            {
                success = true,
                message = "OK",
                data = context,
                errors = []
            });

        metadataRepository
            .Setup(repo => repo.GetColumnsByRouteAsync(It.IsAny<WorkflowRouteColumnConfigRequestDto>()))
            .ReturnsAsync(new AppResponses<WorkflowRouteColumnConfigResultDto?>
            {
                success = true,
                message = "OK",
                data = new WorkflowRouteColumnConfigResultDto
                {
                    IdRutaWorkflow = context.IdRutaWorkflow,
                    Mode = request.ColumnMode.ToString(),
                    Columns = columns
                },
                errors = []
            });

        inboxRepository
            .Setup(repo => repo.GetInboxAsync(request, context, columns, "WF"))
            .ReturnsAsync(new AppResponses<List<ExpandoObject>>
            {
                success = true,
                message = "OK",
                data = [CreateRow(55)],
                errors = []
            });

        dynamicColumnsBuilder
            .Setup(builder => builder.Build(columns))
            .Returns(
            [
                new UiColumnDto
                {
                    Key = "acciones",
                    ColumnName = "acciones",
                    HeaderName = "Acciones",
                    Visible = true,
                    RenderType = "grid_actions",
                    IsActionColumn = true
                }
            ]);

        tableBuilder
            .Setup(builder => builder.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()))
            .ReturnsAsync(new DynamicUiTableDto
            {
                TableId = "workflowInboxgestion",
                Rows = [],
                Columns = []
            });

        DynamicUiTableBuildInput? capturedInput = null;
        tableBuilder
            .Setup(builder => builder.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()))
            .Callback<DynamicUiTableBuildInput>(input => capturedInput = input)
            .ReturnsAsync(new DynamicUiTableDto
            {
                TableId = "workflowInboxgestion",
                Rows = [],
                Columns = []
            });

        var service = CreateService(
            contextResolver,
            metadataRepository,
            inboxRepository,
            dynamicColumnsBuilder,
            tableBuilder);

        var result = await service.SolicitaBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.NotNull(capturedInput);
        metadataRepository.Verify(repo => repo.GetColumnsByRouteAsync(
            It.Is<WorkflowRouteColumnConfigRequestDto>(dto =>
                dto.IdRutaWorkflow == context.IdRutaWorkflow &&
                dto.DefaultDbAlias == "WF" &&
                dto.Mode == request.ColumnMode)), Times.Once);
        inboxRepository.Verify(repo => repo.GetInboxAsync(request, context, columns, "WF"), Times.Once);
        Assert.Single(capturedInput!.CellActions);
        Assert.Equal("acciones", capturedInput.CellActions[0].ColumnKey);
        var actionRequest = Assert.IsType<DynamicUiActionRequestDto>(capturedInput.CellActions[0].Action.Request);
        Assert.Equal("id_tarea", actionRequest.RowIdField);
        var payload = Assert.IsType<Dictionary<string, object?>>(capturedInput.Rows[0]);
        Assert.Equal(55, payload["id"]);
        Assert.Equal(55, payload["id_tarea"]);
    }

    private static WorkflowInboxService CreateService(
        Mock<IWorkflowInboxContextResolverService>? contextResolver = null,
        Mock<IWorkflowRouteColumnConfigRepository>? metadataRepository = null,
        Mock<IWorkflowInboxRepository>? inboxRepository = null,
        Mock<IWorkflowDynamicUiColumnBuilder>? dynamicColumnsBuilder = null,
        Mock<IDynamicUiTableBuilder>? tableBuilder = null,
        Mock<ICurrentUserService>? currentUserService = null)
    {
        if (currentUserService == null)
        {
            currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(service => service.GetClaimValue("defaulaliaswf")).Returns("WF");
        }

        if (dynamicColumnsBuilder == null)
        {
            dynamicColumnsBuilder = new Mock<IWorkflowDynamicUiColumnBuilder>();
            dynamicColumnsBuilder.Setup(builder => builder.Build(It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>())).Returns([]);
        }

        if (tableBuilder == null)
        {
            tableBuilder = new Mock<IDynamicUiTableBuilder>();
            tableBuilder.Setup(builder => builder.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()))
                .ReturnsAsync(new DynamicUiTableDto { TableId = "workflowInboxgestion" });
        }

        return new WorkflowInboxService(
            (contextResolver ?? new Mock<IWorkflowInboxContextResolverService>()).Object,
            (metadataRepository ?? new Mock<IWorkflowRouteColumnConfigRepository>()).Object,
            (inboxRepository ?? new Mock<IWorkflowInboxRepository>()).Object,
            dynamicColumnsBuilder.Object,
            tableBuilder.Object,
            currentUserService.Object);
    }

    private static WorkflowInboxDynamicTableRequestDto CreateRequest() => new()
    {
        ColumnMode = WorkflowColumnListMode.ListaGestionTramite,
        Page = 1,
        PageSize = 25,
        SortDir = "ASC",
        EstadoTramite = "Todos"
    };

    private static WorkflowInboxResolvedContextDto CreateContext() => new()
    {
        IdUsuarioWorkflow = 99,
        IdGrupoWorkflow = 14,
        IdRutaWorkflow = 7,
        NombreRuta = "RUTA_REAL",
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
