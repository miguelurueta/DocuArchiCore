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
    public async Task ExportBandejaWorkflowAsync_CuandoClaimWorkflowNoExiste_RetornaValidacion()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(service => service.GetClaimValue("defaulaliaswf")).Returns((string?)null);

        var service = CreateService(currentUserService: currentUser);

        var result = await service.ExportBandejaWorkflowAsync(CreateExportRequest(), 10, "DA");

        Assert.False(result.success);
        Assert.Equal("Claim defaulaliaswf requerido para exportar bandeja workflow", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Field == "defaulaliaswf");
    }

    [Fact]
    public async Task AutocompleteBandejaWorkflowAsync_CuandoBusquedaEsCorta_NoConsultaRepositorios()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var service = CreateService(contextResolver, metadataRepository, inboxRepository);

        var result = await service.AutocompleteBandejaWorkflowAsync(
            new WorkflowInboxAutocompleteRequestDto { Search = "ab", Limit = 5 },
            10,
            "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!.Items);
        contextResolver.Verify(repo => repo.ResolveAsync(It.IsAny<int>()), Times.Never);
        metadataRepository.Verify(repo => repo.GetColumnsByRouteAsync(It.IsAny<WorkflowRouteColumnConfigRequestDto>()), Times.Never);
        inboxRepository.Verify(repo => repo.GetAutocompleteAsync(
            It.IsAny<WorkflowInboxDynamicTableRequestDto>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<WorkflowInboxResolvedContextDto>(),
            It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AutocompleteBandejaWorkflowAsync_CuandoEsValido_CapaLimiteYFiltraColumnas()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var context = CreateContext();
        var columns = CreateColumns();
        columns.Add(new WorkflowDynamicColumnDefinitionDto
        {
            Key = "oculto",
            ColumnName = "oculto",
            SqlColumnName = "DAT.oculto",
            DataType = "text",
            IsVisible = false,
            IsFilterable = true
        });
        columns.Add(new WorkflowDynamicColumnDefinitionDto
        {
            Key = "numero",
            ColumnName = "numero",
            SqlColumnName = "DAT.numero",
            DataType = "number",
            IsVisible = true,
            IsFilterable = true
        });

        contextResolver
            .Setup(service => service.ResolveAsync(10))
            .ReturnsAsync(new AppResponses<WorkflowInboxResolvedContextDto> { success = true, message = "OK", data = context, errors = [] });
        metadataRepository
            .Setup(repo => repo.GetColumnsByRouteAsync(It.IsAny<WorkflowRouteColumnConfigRequestDto>()))
            .ReturnsAsync(new AppResponses<WorkflowRouteColumnConfigResultDto?>
            {
                success = true,
                message = "OK",
                data = new WorkflowRouteColumnConfigResultDto { IdRutaWorkflow = context.IdRutaWorkflow, Mode = WorkflowColumnListMode.ListaGestionTramite.ToString(), Columns = columns },
                errors = []
            });

        List<WorkflowDynamicColumnDefinitionDto>? capturedColumns = null;
        inboxRepository
            .Setup(repo => repo.GetAutocompleteAsync(
                It.IsAny<WorkflowInboxDynamicTableRequestDto>(),
                "ABC",
                10,
                context,
                It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(),
                "WF"))
            .Callback<WorkflowInboxDynamicTableRequestDto, string, int, WorkflowInboxResolvedContextDto, List<WorkflowDynamicColumnDefinitionDto>, string>(
                (_, _, _, _, cols, _) => capturedColumns = cols)
            .ReturnsAsync(new AppResponses<WorkflowInboxAutocompleteResponseDto>
            {
                success = true,
                message = "OK",
                data = new WorkflowInboxAutocompleteResponseDto
                {
                    Items = [new WorkflowInboxAutocompleteItemDto { Value = "ABC-123", Label = "ABC-123", Field = "asunto" }]
                },
                errors = []
            });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository);

        var result = await service.AutocompleteBandejaWorkflowAsync(
            new WorkflowInboxAutocompleteRequestDto { Search = " ABC ", Limit = 50 },
            10,
            "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Single(result.data!.Items);
        Assert.NotNull(capturedColumns);
        Assert.Single(capturedColumns!);
        Assert.Equal("asunto", capturedColumns![0].Key);
    }

    [Fact]
    public async Task ExportBandejaWorkflowAsync_CuandoTotalSuperaLimite_RetornaErrorControlado()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var context = CreateContext();
        var request = CreateExportRequest();
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
            .Setup(repo => repo.GetInboxCountAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .ReturnsAsync(new AppResponses<int>
            {
                success = true,
                message = "OK",
                data = 50001,
                errors = []
            });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository);

        var result = await service.ExportBandejaWorkflowAsync(request, 10, "DA");

        Assert.False(result.success);
        Assert.Contains("50000", result.message);
        inboxRepository.Verify(repo => repo.ExportInboxAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), It.IsAny<WorkflowInboxResolvedContextDto>(), It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExportBandejaWorkflowAsync_CuandoEsValido_RetornaCsvYMantieneDatasetAllMatching()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var context = CreateContext();
        var request = CreateExportRequest();
        request.Page = 9;
        request.PageSize = 3;
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

        WorkflowInboxDynamicTableRequestDto? capturedExportRequest = null;
        inboxRepository
            .Setup(repo => repo.GetInboxCountAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .ReturnsAsync(new AppResponses<int>
            {
                success = true,
                message = "OK",
                data = 1,
                errors = []
            });

        inboxRepository
            .Setup(repo => repo.ExportInboxAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .Callback<WorkflowInboxDynamicTableRequestDto, WorkflowInboxResolvedContextDto, List<WorkflowDynamicColumnDefinitionDto>, string>((dto, _, _, _) => capturedExportRequest = dto)
            .ReturnsAsync(new AppResponses<List<Dictionary<string, object?>>>
            {
                success = true,
                message = "OK",
                data =
                [
                    new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id_tarea"] = 55,
                        ["fecha_inicio"] = new DateTime(2026, 4, 5, 12, 30, 0, DateTimeKind.Utc),
                        ["ESTADO"] = "Abierto",
                        ["asunto"] = "Caso, prioridad alta"
                    }
                ],
                errors = []
            });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository);

        var result = await service.ExportBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal("text/csv", result.data!.ContentType);
        Assert.Equal(1, result.data.TotalRecords);
        Assert.NotNull(capturedExportRequest);
        Assert.Equal(9, capturedExportRequest!.Page);
        Assert.Equal(3, capturedExportRequest.PageSize);
        var csv = System.Text.Encoding.UTF8.GetString(result.data.FileBytes);
        Assert.Contains("asunto,fecha_inicio,ESTADO", csv);
        Assert.Contains("\"Caso, prioridad alta\"", csv);
    }

    [Fact]
    public async Task ExportBandejaWorkflowAsync_CuandoFormatoEsInvalido_RetornaValidacion()
    {
        var service = CreateService();
        var request = CreateExportRequest();
        request.Format = "xml";

        var result = await service.ExportBandejaWorkflowAsync(request, 10, "DA");

        Assert.False(result.success);
        Assert.Equal("Formato de exportacion no soportado", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Field == "format");
    }

    [Fact]
    public async Task ExportBandejaWorkflowAsync_CuandoExportModeEsInvalido_RetornaValidacion()
    {
        var service = CreateService();
        var request = CreateExportRequest();
        request.ExportMode = "allLoaded";

        var result = await service.ExportBandejaWorkflowAsync(request, 10, "DA");

        Assert.False(result.success);
        Assert.Equal("Modo de exportacion no soportado", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Field == "exportMode");
    }

    [Fact]
    public async Task ExportBandejaWorkflowAsync_CuandoExportModeEsCurrentPage_UsaSliceVisibleYSaltaConteoGlobal()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var context = CreateContext();
        var request = CreateExportRequest();
        request.ExportMode = WorkflowInboxExportModes.CurrentPage;
        request.Page = 4;
        request.PageSize = 2;
        var columns = CreateColumns();

        contextResolver
            .Setup(service => service.ResolveAsync(10))
            .ReturnsAsync(new AppResponses<WorkflowInboxResolvedContextDto> { success = true, message = "OK", data = context, errors = [] });
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

        WorkflowInboxDynamicTableRequestDto? capturedExportRequest = null;
        inboxRepository
            .Setup(repo => repo.ExportInboxAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .Callback<WorkflowInboxDynamicTableRequestDto, WorkflowInboxResolvedContextDto, List<WorkflowDynamicColumnDefinitionDto>, string>((dto, _, _, _) => capturedExportRequest = dto)
            .ReturnsAsync(new AppResponses<List<Dictionary<string, object?>>>
            {
                success = true,
                message = "OK",
                data = [CreateExportRow(), CreateSecondExportRow()],
                errors = []
            });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository);

        var result = await service.ExportBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(2, result.data!.TotalRecords);
        Assert.NotNull(capturedExportRequest);
        Assert.Equal(4, capturedExportRequest!.Page);
        Assert.Equal(2, capturedExportRequest.PageSize);
        Assert.Equal(WorkflowInboxExportModes.CurrentPage, capturedExportRequest.ExportMode);
        inboxRepository.Verify(repo => repo.GetInboxCountAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), It.IsAny<WorkflowInboxResolvedContextDto>(), It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExportBandejaWorkflowAsync_CuandoFormatoEsXlsx_RetornaArchivoValido()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var context = CreateContext();
        var request = CreateExportRequest();
        request.Format = WorkflowInboxExportFormats.Xlsx;
        var columns = CreateColumns();

        contextResolver
            .Setup(service => service.ResolveAsync(10))
            .ReturnsAsync(new AppResponses<WorkflowInboxResolvedContextDto> { success = true, message = "OK", data = context, errors = [] });
        metadataRepository
            .Setup(repo => repo.GetColumnsByRouteAsync(It.IsAny<WorkflowRouteColumnConfigRequestDto>()))
            .ReturnsAsync(new AppResponses<WorkflowRouteColumnConfigResultDto?> { success = true, message = "OK", data = new WorkflowRouteColumnConfigResultDto { IdRutaWorkflow = context.IdRutaWorkflow, Mode = request.ColumnMode.ToString(), Columns = columns }, errors = [] });
        inboxRepository
            .Setup(repo => repo.GetInboxCountAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .ReturnsAsync(new AppResponses<int> { success = true, message = "OK", data = 1, errors = [] });
        inboxRepository
            .Setup(repo => repo.ExportInboxAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .ReturnsAsync(new AppResponses<List<Dictionary<string, object?>>> { success = true, message = "OK", data = [CreateExportRow()], errors = [] });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository);

        var result = await service.ExportBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(WorkflowInboxExportFormats.Xlsx, result.data!.Format);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.data.ContentType);
        Assert.EndsWith(".xlsx", result.data.FileName, StringComparison.OrdinalIgnoreCase);
        Assert.NotEmpty(result.data.FileBytes);
    }

    [Fact]
    public async Task ExportBandejaWorkflowAsync_CuandoFormatoEsPdf_RetornaArchivoValido()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var context = CreateContext();
        var request = CreateExportRequest();
        request.Format = WorkflowInboxExportFormats.Pdf;
        var columns = CreateColumns();

        contextResolver
            .Setup(service => service.ResolveAsync(10))
            .ReturnsAsync(new AppResponses<WorkflowInboxResolvedContextDto> { success = true, message = "OK", data = context, errors = [] });
        metadataRepository
            .Setup(repo => repo.GetColumnsByRouteAsync(It.IsAny<WorkflowRouteColumnConfigRequestDto>()))
            .ReturnsAsync(new AppResponses<WorkflowRouteColumnConfigResultDto?> { success = true, message = "OK", data = new WorkflowRouteColumnConfigResultDto { IdRutaWorkflow = context.IdRutaWorkflow, Mode = request.ColumnMode.ToString(), Columns = columns }, errors = [] });
        inboxRepository
            .Setup(repo => repo.GetInboxCountAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .ReturnsAsync(new AppResponses<int> { success = true, message = "OK", data = 1, errors = [] });
        inboxRepository
            .Setup(repo => repo.ExportInboxAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .ReturnsAsync(new AppResponses<List<Dictionary<string, object?>>> { success = true, message = "OK", data = [CreateExportRow()], errors = [] });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository);

        var result = await service.ExportBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(WorkflowInboxExportFormats.Pdf, result.data!.Format);
        Assert.Equal("application/pdf", result.data.ContentType);
        Assert.EndsWith(".pdf", result.data.FileName, StringComparison.OrdinalIgnoreCase);
        Assert.True(result.data.FileBytes.Length > 4);
        Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(result.data.FileBytes, 0, 4));
    }

    [Fact]
    public async Task SolicitaBandejaWorkflowAsync_CuandoRepositorioNoRetornaFilas_RetornaTablaConsistente()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var dynamicColumnsBuilder = new Mock<IWorkflowDynamicUiColumnBuilder>();
        var tableBuilder = new Mock<IDynamicUiTableBuilder>();
        var currentUser = new Mock<ICurrentUserService>();
        var context = CreateContext();
        var request = CreateRequest();
        currentUser.Setup(service => service.GetClaimValue("defaulaliaswf")).Returns("WF");
        currentUser.SetupGet(service => service.Permisos).Returns(["wf.ver", "wf.gestionar"]);

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
            .Setup(repo => repo.GetInboxAsync(
                It.Is<WorkflowInboxDynamicTableRequestDto>(dto =>
                    dto.TableId == "workflowInboxgestion" &&
                    dto.IdUsuarioGestion == 10 &&
                    dto.IdRutaWorkflow == context.IdRutaWorkflow &&
                    dto.NombreRuta == context.NombreRuta &&
                    dto.DefaultDbAlias == "DA" &&
                    dto.IdActividad == context.IdActividad &&
                    dto.IdUsuarioWorkflow == context.IdUsuarioWorkflow &&
                    dto.TipoConsulta == request.SearchType),
                context,
                It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>(),
                "WF"))
            .ReturnsAsync(new AppResponses<WorkflowInboxRowsResultDto>
            {
                success = true,
                message = "Sin resultados",
                data = new WorkflowInboxRowsResultDto
                {
                    Rows = [],
                    TotalRecords = 0
                },
                errors = []
            });

        dynamicColumnsBuilder
            .Setup(builder => builder.Build(It.IsAny<List<WorkflowDynamicColumnDefinitionDto>>()))
            .Returns(
            [
                new UiColumnDto
                {
                    Key = "asunto",
                    ColumnName = "asunto",
                    HeaderName = "Asunto",
                    Visible = true,
                    RenderType = "grid_text"
                }
            ]);

        DynamicUiTableBuildInput? capturedInput = null;
        tableBuilder
            .Setup(builder => builder.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()))
            .Callback<DynamicUiTableBuildInput>(input => capturedInput = input)
            .ReturnsAsync((DynamicUiTableBuildInput input) => new DynamicUiTableDto
            {
                TableId = "workflowInboxgestion",
                Columns = input.Columns,
                Rows = [],
                UserClaims = input.Request.UserClaims,
                Pagination = new PaginationDto { Page = input.Request.Page, PageSize = input.Request.PageSize, Total = input.Total }
            });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository, dynamicColumnsBuilder, tableBuilder, currentUser);

        var result = await service.SolicitaBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        var payload = Assert.IsType<DynamicUiTableDto>(result.data);
        Assert.Equal("workflowInboxgestion", payload.TableId);
        Assert.Empty(payload.Rows);
        Assert.Equal(["wf.ver", "wf.gestionar"], payload.UserClaims);
        Assert.NotNull(capturedInput);
        Assert.Equal(25, capturedInput!.Request.PageSize);
        Assert.Equal(0, capturedInput.Total);
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
            .Setup(repo => repo.GetInboxAsync(
                It.Is<WorkflowInboxDynamicTableRequestDto>(dto =>
                    dto.TableId == "workflowInboxgestion" &&
                    dto.IdUsuarioGestion == 10 &&
                    dto.IdRutaWorkflow == context.IdRutaWorkflow &&
                    dto.NombreRuta == context.NombreRuta &&
                    dto.DefaultDbAlias == "DA" &&
                    dto.IdActividad == context.IdActividad &&
                    dto.IdUsuarioWorkflow == context.IdUsuarioWorkflow &&
                    dto.TipoConsulta == request.SearchType &&
                    dto.ColumnMode == request.ColumnMode),
                context,
                columns,
                "WF"))
            .ReturnsAsync(new AppResponses<WorkflowInboxRowsResultDto>
            {
                success = true,
                message = "OK",
                data = new WorkflowInboxRowsResultDto
                {
                    Rows = [CreateRow(55)],
                    TotalRecords = 88
                },
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
        inboxRepository.Verify(repo => repo.GetInboxAsync(
            It.Is<WorkflowInboxDynamicTableRequestDto>(dto =>
                dto.TableId == "workflowInboxgestion" &&
                dto.IdUsuarioGestion == 10 &&
                dto.IdRutaWorkflow == context.IdRutaWorkflow &&
                dto.NombreRuta == context.NombreRuta &&
                dto.DefaultDbAlias == "DA" &&
                dto.IdActividad == context.IdActividad &&
                dto.IdUsuarioWorkflow == context.IdUsuarioWorkflow &&
                dto.TipoConsulta == request.SearchType),
            context,
            columns,
            "WF"), Times.Once);
        Assert.Single(capturedInput!.CellActions);
        Assert.Equal("acciones", capturedInput.CellActions[0].ColumnKey);
        Assert.Equal(3, capturedInput.MenuActions.Count);
        Assert.Equal(["gestionar_tramite", "reasignar_tramite", "acciones_avanzadas"], capturedInput.MenuActions.Select(action => action.ActionId).ToArray());
        Assert.NotNull(capturedInput.MenuActions[2].Children);
        Assert.Equal("redireccionar_externo", capturedInput.MenuActions[2].Children![0].ActionId);
        Assert.True(capturedInput.MenuActions[2].Children![1].IsDivider);
        Assert.Equal(string.Empty, capturedInput.MenuActions[2].Children![1].Behavior);
        Assert.Equal("archivar_tramite", capturedInput.MenuActions[2].Children![2].ActionId);
        Assert.Equal(88, capturedInput.Total);
        Assert.Equal(["wf.ver"], capturedInput.Request.UserClaims);
        var actionRequest = Assert.IsType<DynamicUiActionRequestDto>(capturedInput.CellActions[0].Action.Request);
        Assert.Equal("id_tarea", actionRequest.RowIdField);
        var menuItems = Assert.IsType<string[]>(capturedInput.CellActions[0].Action.BehaviorConfig!["menuItems"]);
        Assert.Equal(["gestionar_tramite", "reasignar_tramite", "acciones_avanzadas"], menuItems);
        var payload = Assert.IsType<Dictionary<string, object?>>(capturedInput.Rows[0]);
        Assert.Equal(55, payload["id"]);
        Assert.Equal(55, payload["id_tarea"]);
    }

    [Fact]
    public async Task SolicitaBandejaWorkflowAsync_CuandoSearchTypeEsInvalido_NormalizaALegacy()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var context = CreateContext();
        var columns = CreateColumns();
        var request = CreateRequest();
        request.SearchType = 99;
        WorkflowInboxDynamicTableRequestDto? capturedRequest = null;

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
            .Setup(repo => repo.GetInboxAsync(
                It.IsAny<WorkflowInboxDynamicTableRequestDto>(),
                context,
                columns,
                "WF"))
            .Callback<WorkflowInboxDynamicTableRequestDto, WorkflowInboxResolvedContextDto, List<WorkflowDynamicColumnDefinitionDto>, string>(
                (dto, _, _, _) => capturedRequest = dto)
            .ReturnsAsync(new AppResponses<WorkflowInboxRowsResultDto>
            {
                success = true,
                message = "OK",
                data = new WorkflowInboxRowsResultDto
                {
                    Rows = [],
                    TotalRecords = 0
                },
                errors = []
            });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository);

        var result = await service.SolicitaBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.NotNull(capturedRequest);
        Assert.Equal(1, capturedRequest!.TipoConsulta);
    }

    [Fact]
    public async Task SolicitaBandejaWorkflowAsync_CuandoContextResolverFalla_PropagaErrorControlado()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        contextResolver
            .Setup(service => service.ResolveAsync(10))
            .ReturnsAsync(new AppResponses<WorkflowInboxResolvedContextDto>
            {
                success = false,
                message = "Contexto invalido",
                data = null!,
                errors = [new AppError { Field = "context", Message = "Contexto invalido", Type = "Functional" }]
            });

        var service = CreateService(contextResolver: contextResolver);

        var result = await service.SolicitaBandejaWorkflowAsync(CreateRequest(), 10, "DA");

        Assert.False(result.success);
        Assert.Equal("Contexto invalido", result.message);
    }

    [Fact]
    public async Task SolicitaBandejaWorkflowAsync_CuandoRepositoryRetornaDiccionario_NoUsaCastingDinamico()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var context = CreateContext();
        var request = CreateRequest();
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
            .Setup(repo => repo.GetInboxAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .ReturnsAsync(new AppResponses<WorkflowInboxRowsResultDto>
            {
                success = true,
                message = "OK",
                data = new WorkflowInboxRowsResultDto
                {
                    Rows =
                    [
                        new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["id_tarea"] = 80,
                            ["id"] = 80,
                            ["asunto"] = "Caso"
                        }
                    ],
                    TotalRecords = 1
                },
                errors = []
            });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository);

        var result = await service.SolicitaBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
    }

    [Fact]
    public async Task SolicitaBandejaWorkflowAsync_CuandoNumeroTareaListaExiste_NormalizaPageSizeConEseValor()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var tableBuilder = new Mock<IDynamicUiTableBuilder>();
        var context = CreateContext();
        context.NumeroTareaLista = 40;
        var request = CreateRequest();
        request.Page = 0;
        request.PageSize = 5;
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
            .Setup(repo => repo.GetInboxAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .ReturnsAsync(new AppResponses<WorkflowInboxRowsResultDto>
            {
                success = true,
                message = "OK",
                data = new WorkflowInboxRowsResultDto
                {
                    Rows = [CreateRow(55)],
                    TotalRecords = 55
                },
                errors = []
            });

        DynamicUiTableBuildInput? capturedInput = null;
        tableBuilder
            .Setup(builder => builder.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()))
            .Callback<DynamicUiTableBuildInput>(input => capturedInput = input)
            .ReturnsAsync(new DynamicUiTableDto { TableId = "workflowInboxgestion" });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository, tableBuilder: tableBuilder);

        var result = await service.SolicitaBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.NotNull(capturedInput);
        Assert.Equal(1, capturedInput!.Request.Page);
        Assert.Equal(40, capturedInput.Request.PageSize);
    }

    [Fact]
    public async Task SolicitaBandejaWorkflowAsync_CuandoPaginaEstaFueraDeRango_MantieneTotalCorrectoConRowsVacias()
    {
        var contextResolver = new Mock<IWorkflowInboxContextResolverService>();
        var metadataRepository = new Mock<IWorkflowRouteColumnConfigRepository>();
        var inboxRepository = new Mock<IWorkflowInboxRepository>();
        var tableBuilder = new Mock<IDynamicUiTableBuilder>();
        var context = CreateContext();
        var request = CreateRequest();
        request.Page = 5;
        request.PageSize = 25;
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
            .Setup(repo => repo.GetInboxAsync(It.IsAny<WorkflowInboxDynamicTableRequestDto>(), context, columns, "WF"))
            .ReturnsAsync(new AppResponses<WorkflowInboxRowsResultDto>
            {
                success = true,
                message = "Sin resultados",
                data = new WorkflowInboxRowsResultDto
                {
                    Rows = [],
                    TotalRecords = 88
                },
                errors = []
            });

        DynamicUiTableBuildInput? capturedInput = null;
        tableBuilder
            .Setup(builder => builder.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()))
            .Callback<DynamicUiTableBuildInput>(input => capturedInput = input)
            .ReturnsAsync((DynamicUiTableBuildInput input) => new DynamicUiTableDto
            {
                TableId = "workflowInboxgestion",
                Rows = [],
                Pagination = new PaginationDto { Page = input.Request.Page, PageSize = input.Request.PageSize, Total = input.Total }
            });

        var service = CreateService(contextResolver, metadataRepository, inboxRepository, tableBuilder: tableBuilder);

        var result = await service.SolicitaBandejaWorkflowAsync(request, 10, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.NotNull(capturedInput);
        Assert.Empty(capturedInput!.Rows);
        Assert.Equal(88, capturedInput.Total);
        Assert.Equal(5, capturedInput.Request.Page);
        Assert.Equal(25, capturedInput.Request.PageSize);
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
            currentUserService.SetupGet(service => service.Permisos).Returns(["wf.ver"]);
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

    private static WorkflowInboxApiRequestDto CreateRequest() => new()
    {
        ColumnMode = WorkflowColumnListMode.ListaGestionTramite,
        Page = 1,
        PageSize = 25,
        SortDir = "ASC",
        EstadoTramite = "Todos",
        SearchType = 1,
        StructuredFilters = []
    };

    private static WorkflowInboxExportRequestDto CreateExportRequest() => new()
    {
        ColumnMode = WorkflowColumnListMode.ListaGestionTramite,
        EstadoTramite = "Todos",
        SearchType = 2,
        Search = "radicado",
        SortField = "fecha_inicio",
        SortDir = "DESC",
        Page = 1,
        PageSize = 25,
        Format = "csv",
        ExportMode = WorkflowInboxExportModes.AllMatching,
        ReportTitle = "workflow inbox",
        StructuredFilters = []
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

    private static Dictionary<string, object?> CreateRow(int idTarea)
    {
        return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["id_tarea"] = idTarea,
            ["id"] = idTarea,
            ["asunto"] = "Caso"
        };
    }

    private static Dictionary<string, object?> CreateExportRow()
    {
        return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["id_tarea"] = 55,
            ["fecha_inicio"] = new DateTime(2026, 4, 5, 12, 30, 0, DateTimeKind.Utc),
            ["ESTADO"] = "Abierto",
            ["asunto"] = "Caso, prioridad alta"
        };
    }

    private static Dictionary<string, object?> CreateSecondExportRow()
    {
        return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["id_tarea"] = 56,
            ["fecha_inicio"] = new DateTime(2026, 4, 5, 12, 45, 0, DateTimeKind.Utc),
            ["ESTADO"] = "Pendiente",
            ["asunto"] = "Caso secundario"
        };
    }
}
