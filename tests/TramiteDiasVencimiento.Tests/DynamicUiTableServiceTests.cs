using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.DTOs.DTOs.Workflow.BandejaCorrespondencia;
using MiApp.Repository.Repositorio.UI.MuiTable;
using MiApp.Services.Service.UI.MuiTable;
using MiApp.Services.Service.Workflow.BandejaCorrespondencia;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class DynamicUiTableServiceTests
{
    [Fact]
    public async Task QueryAsync_CuandoTableIdEsVacio_RetornaErrorValidacion()
    {
        var builder = new Mock<IDynamicUiTableBuilder>();
        var service = new DynamicUiTableService(builder.Object, []);

        var result = await service.QueryAsync(new DynamicUiTableQueryRequestDto
        {
            TableId = "",
            Page = 1,
            PageSize = 25
        });

        Assert.False(result.success);
        Assert.Equal("TableId requerido", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), e => e.Field == "TableId");
    }

    [Fact]
    public async Task QueryAsync_CuandoSortFieldNoEstaPermitido_RetornaErrorValidacion()
    {
        var builder = new Mock<IDynamicUiTableBuilder>();
        var handler = new FakeHandler(
            "radicados",
            rows: [],
            total: 0,
            fixedColumns:
            [
                new UiColumnDto { Key = "id", ColumnName = "id", HeaderName = "Id", Order = 1 }
            ]);
        var service = new DynamicUiTableService(builder.Object, [handler]);

        var result = await service.QueryAsync(new DynamicUiTableQueryRequestDto
        {
            TableId = "radicados",
            Page = 1,
            PageSize = 25,
            SortField = "fecha"
        });

        Assert.False(result.success);
        Assert.Equal("SortField no permitido para TableId", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), e => e.Field == "SortField");
    }

    [Fact]
    public async Task QueryAsync_CuandoIncludeConfigEsFalse_RetornaRowsOnly()
    {
        var builder = new Mock<IDynamicUiTableBuilder>();
        builder
            .Setup(b => b.BuildRowsOnlyAsync(It.IsAny<DynamicUiTableBuildInput>()))
            .ReturnsAsync(new DynamicUiRowsOnlyDto
            {
                TableId = "radicados",
                Rows =
                [
                    new UiRowDto
                    {
                        Id = "1",
                        Values = new Dictionary<string, object?> { ["id"] = "1", ["asunto"] = "Prueba" }
                    }
                ],
                Pagination = new PaginationDto { Page = 1, PageSize = 25, Total = 1 }
            });

        var handler = new FakeHandler(
            "radicados",
            rows:
            [
                new Dictionary<string, object?> { ["id"] = "1", ["asunto"] = "Prueba" }
            ],
            total: 1,
            fixedColumns:
            [
                new UiColumnDto { Key = "id", ColumnName = "id", HeaderName = "Id", Order = 1 },
                new UiColumnDto { Key = "asunto", ColumnName = "asunto", HeaderName = "Asunto", Order = 2 }
            ]);

        var service = new DynamicUiTableService(builder.Object, [handler]);

        var result = await service.QueryAsync(new DynamicUiTableQueryRequestDto
        {
            TableId = "radicados",
            Page = 1,
            PageSize = 25,
            IncludeConfig = false,
            SortField = "id"
        });

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        var payload = Assert.IsType<DynamicUiRowsOnlyDto>(result.data);
        Assert.Equal("radicados", payload.TableId);
        Assert.Single(payload.Rows);
        builder.Verify(b => b.BuildRowsOnlyAsync(It.IsAny<DynamicUiTableBuildInput>()), Times.Once);
        builder.Verify(b => b.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteActionAsync_CuandoActionIdEsVacio_RetornaErrorValidacion()
    {
        var builder = new Mock<IDynamicUiTableBuilder>();
        var service = new DynamicUiTableService(builder.Object, []);

        var result = await service.ExecuteActionAsync(new ExecuteUiActionRequestDto
        {
            TableId = "radicados",
            ActionId = ""
        });

        Assert.False(result.success);
        Assert.Equal("TableId y ActionId requeridos", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), e => e.Field == "TableId/ActionId");
    }

    [Fact]
    public async Task QueryAsync_CuandoIncludeConfigEsTrue_RetornaPayloadCompatibleConAdapterAntD()
    {
        var builder = new Mock<IDynamicUiTableBuilder>();
        builder
            .Setup(b => b.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()))
            .ReturnsAsync(new DynamicUiTableDto
            {
                TableId = "radicados",
                Columns =
                [
                    new UiColumnDto
                    {
                        Key = "asunto",
                        ColumnName = "asunto",
                        HeaderName = "Asunto",
                        RenderType = "grid_text",
                        Order = 1
                    },
                    new UiColumnDto
                    {
                        Key = "actions",
                        ColumnName = "actions",
                        HeaderName = "Opciones",
                        RenderType = "custom",
                        IsActionColumn = true,
                        Order = 2
                    }
                ],
                Rows =
                [
                    new UiRowDto
                    {
                        Id = "1",
                        Values = new Dictionary<string, object?> { ["asunto"] = "Prueba" }
                    }
                ],
                ToolbarActions = [new UiActionDto { ActionId = "refresh", Placement = "toolbar", Presentation = "button" }],
                RowActions = [new UiActionDto { ActionId = "open", Placement = "row", Presentation = "menu_item" }],
                MenuActions =
                [
                    new UiActionDto
                    {
                        ActionId = "open",
                        Placement = "row",
                        Presentation = "menu_item"
                    },
                    new UiActionDto
                    {
                        ActionId = "advanced",
                        Placement = "row",
                        Presentation = "menu_item",
                        Children =
                        [
                            new UiActionDto { ActionId = "archive", Placement = "row", Presentation = "menu_item" },
                            new UiActionDto { IsDivider = true, Behavior = string.Empty }
                        ]
                    }
                ],
                Pagination = new PaginationDto { Page = 1, PageSize = 25, Total = 1 },
                Sorting = new SortingDto { SortField = "asunto", SortDir = "asc" }
            });

        var handler = new FakeHandler(
            "radicados",
            rows:
            [
                new Dictionary<string, object?> { ["id"] = "1", ["asunto"] = "Prueba" }
            ],
            total: 1,
            fixedColumns:
            [
                new UiColumnDto { Key = "asunto", ColumnName = "asunto", HeaderName = "Asunto", Order = 1 }
            ]);

        var service = new DynamicUiTableService(builder.Object, [handler]);

        var result = await service.QueryAsync(new DynamicUiTableQueryRequestDto
        {
            TableId = "radicados",
            Page = 1,
            PageSize = 25,
            IncludeConfig = true,
            SortField = "asunto"
        });

        Assert.True(result.success);
        var payload = Assert.IsType<DynamicUiTableDto>(result.data);
        Assert.Equal("asunto", payload.Columns[0].ColumnName);
        Assert.Equal("Asunto", payload.Columns[0].HeaderName);
        Assert.Equal("grid_text", payload.Columns[0].RenderType);
        Assert.True(payload.Columns[1].IsActionColumn);
        Assert.Equal("button", payload.ToolbarActions[0].Presentation);
        Assert.Equal("menu_item", payload.RowActions[0].Presentation);
        Assert.Equal("advanced", payload.MenuActions[1].ActionId);
        Assert.True(payload.MenuActions[1].Children![1].IsDivider);
        Assert.Equal("1", payload.Rows[0].Id);
        Assert.Equal("Prueba", payload.Rows[0].Values["asunto"]);
        Assert.Equal(1, payload.Pagination!.Page);
        Assert.Equal("asunto", payload.Sorting!.SortField);
    }

    [Fact]
    public async Task BuildAsync_NormalizaAliasesAntDesignYFiltros()
    {
        var repository = new Mock<IUiTableConfigRepository>();
        var builder = new DynamicUiTableBuilder(repository.Object);

        var payload = await builder.BuildAsync(new DynamicUiTableBuildInput
        {
            Request = new DynamicUiTableQueryRequestDto
            {
                TableId = "radicados",
                DefaultDbAlias = "DA",
                Page = 1,
                PageSize = 25,
                UseColumnConfigFromDb = false
            },
            Rows =
            [
                new Dictionary<string, object?> { ["id"] = "1", ["asunto"] = "Prueba", ["fecha_registro"] = "2026-03-20" }
            ],
            Total = 1,
            Actions = [],
            CellActions = [],
            MenuActions =
            [
                new UiActionDto
                {
                    ActionId = "open",
                    Placement = "row",
                    Presentation = "menu_item",
                    Children =
                    [
                        new UiActionDto { ActionId = "archive", Placement = "row", Presentation = "menu_item" },
                        new UiActionDto { IsDivider = true, Behavior = string.Empty }
                    ]
                }
            ],
            Columns =
            [
                new UiColumnDto
                {
                    Key = "asunto",
                    ColumnName = "asunto",
                    HeaderName = "Asunto",
                    DataType = "text",
                    RenderType = "grid_text",
                    Order = 1
                },
                new UiColumnDto
                {
                    Key = "fecha_registro",
                    ColumnName = "fecha_registro",
                    HeaderName = "Fecha",
                    DataType = "date",
                    RenderType = "grid_date",
                    Order = 2
                }
            ]
        });

        Assert.Equal("asunto", payload.Columns[0].DataIndex);
        Assert.Equal("asunto", payload.Columns[0].Field);
        Assert.Equal("Asunto", payload.Columns[0].Title);
        Assert.Equal("text", payload.Columns[0].FilterType);
        Assert.Equal("agTextColumnFilter", payload.Columns[0].AgGridFilterType);
        Assert.True(payload.Columns[0].Filterable);

        Assert.Equal("fecha_registro", payload.Columns[1].DataIndex);
        Assert.Equal("fecha_registro", payload.Columns[1].Field);
        Assert.Equal("Fecha", payload.Columns[1].Title);
        Assert.Equal("date", payload.Columns[1].FilterType);
        Assert.Equal("agDateColumnFilter", payload.Columns[1].AgGridFilterType);
        Assert.Equal("open", payload.MenuActions[0].ActionId);
        Assert.True(payload.MenuActions[0].Children![1].IsDivider);

        Assert.True(payload.Columns[2].IsActionColumn);
        Assert.Equal("actions", payload.Columns[2].Field);
        Assert.Equal("actions", payload.Columns[2].DataIndex);
        Assert.Equal("Opciones", payload.Columns[2].Title);
        Assert.False(payload.Columns[2].Filterable);
        Assert.Equal("none", payload.Columns[2].FilterType);
        Assert.Equal("none", payload.Columns[2].AgGridFilterType);
    }

    [Fact]
    public async Task BuildAsync_CuandoColumnasVienenDeRepositorio_NormalizaAliasesAntD()
    {
        var repository = new Mock<IUiTableConfigRepository>();
        repository
            .Setup(r => r.GetColumnsAsync("DA", "radicados"))
            .ReturnsAsync(
            [
                new UiColumnDto
                {
                    Key = "estado",
                    ColumnName = "estado",
                    HeaderName = "Estado",
                    DataType = "number",
                    RenderType = "grid_chip",
                    Order = 1
                }
            ]);

        var builder = new DynamicUiTableBuilder(repository.Object);

        var payload = await builder.BuildAsync(new DynamicUiTableBuildInput
        {
            Request = new DynamicUiTableQueryRequestDto
            {
                TableId = "radicados",
                DefaultDbAlias = "DA",
                Page = 1,
                PageSize = 25,
                UseColumnConfigFromDb = true
            },
            Rows =
            [
                new Dictionary<string, object?> { ["id"] = "1", ["estado"] = 1 }
            ],
            Total = 1,
            Actions = [],
            CellActions = [],
            Columns = []
        });

        Assert.Equal("estado", payload.Columns[0].DataIndex);
        Assert.Equal("estado", payload.Columns[0].Field);
        Assert.Equal("Estado", payload.Columns[0].Title);
        Assert.Equal("select", payload.Columns[0].FilterType);
        Assert.Equal("agSetColumnFilter", payload.Columns[0].AgGridFilterType);
        repository.Verify(r => r.GetColumnsAsync("DA", "radicados"), Times.Once);
    }

    [Fact]
    public void WorkflowDynamicUiColumnBuilder_CuandoListaEsVacia_RetornaSoloColumnasEstaticas()
    {
        var builder = new WorkflowDynamicUiColumnBuilder();

        var result = builder.Build([]);

        Assert.Collection(
            result,
            column =>
            {
                Assert.Equal("id_tarea", column.Key);
                Assert.False(column.Visible);
            },
            column =>
            {
                Assert.Equal("fecha_inicio", column.Key);
                Assert.True(column.Visible);
            },
            column => Assert.Equal("ESTADO", column.Key),
            column =>
            {
                Assert.Equal("acciones", column.Key);
                Assert.True(column.IsActionColumn);
                Assert.Equal("grid_actions", column.RenderType);
            });
    }

    [Fact]
    public void WorkflowDynamicUiColumnBuilder_RespetaOrdenYNormalizaMetadatos()
    {
        var builder = new WorkflowDynamicUiColumnBuilder();

        var result = builder.Build(
        [
            new WorkflowDynamicColumnDefinitionDto
            {
                Key = "fechaVencimiento",
                ColumnName = "fecha_vencimiento",
                DataType = "datetime",
                Order = 20,
                IsVisible = true,
                IsSortable = true,
                IsFilterable = true
            },
            new WorkflowDynamicColumnDefinitionDto
            {
                Key = "asunto",
                ColumnName = "asunto",
                DataType = "text",
                Order = 10,
                IsVisible = true,
                IsSortable = true,
                IsFilterable = true
            },
            new WorkflowDynamicColumnDefinitionDto
            {
                Key = "dias_vencidos",
                ColumnName = "dias_vencidos",
                DataType = "number",
                Order = 30,
                IsVisible = true,
                IsSortable = true,
                IsFilterable = true
            }
        ]);

        Assert.Equal("asunto", result[0].Key);
        Assert.Equal("Asunto", result[0].HeaderName);
        Assert.Equal("grid_text", result[0].RenderType);
        Assert.Equal("text", result[0].FilterType);

        Assert.Equal("fechaVencimiento", result[1].Key);
        Assert.Equal("Fecha Vencimiento", result[1].Title);
        Assert.Equal("grid_datetime", result[1].RenderType);
        Assert.Equal("date", result[1].FilterType);
        Assert.Equal("agDateColumnFilter", result[1].AgGridFilterType);

        Assert.Equal("dias_vencidos", result[2].Key);
        Assert.Equal("right", result[2].Align);
        Assert.Equal("number", result[2].FilterType);
        Assert.Equal("agNumberColumnFilter", result[2].AgGridFilterType);

        Assert.Equal(["asunto", "fechaVencimiento", "dias_vencidos", "id_tarea", "fecha_inicio", "ESTADO", "acciones"], result.Select(x => x.Key).ToArray());
    }

    [Fact]
    public void WorkflowDynamicUiColumnBuilder_NoDuplicaKeysEstaticasNiDinamicas()
    {
        var builder = new WorkflowDynamicUiColumnBuilder();

        var result = builder.Build(
        [
            new WorkflowDynamicColumnDefinitionDto
            {
                Key = "acciones",
                ColumnName = "acciones_custom",
                DataType = "text",
                Order = 1,
                IsVisible = true,
                IsSortable = false,
                IsFilterable = false
            },
            new WorkflowDynamicColumnDefinitionDto
            {
                Key = "asunto",
                ColumnName = "asunto",
                DataType = "text",
                Order = 2,
                IsVisible = true,
                IsSortable = true,
                IsFilterable = true
            },
            new WorkflowDynamicColumnDefinitionDto
            {
                Key = "asunto",
                ColumnName = "asunto_duplicado",
                DataType = "text",
                Order = 3,
                IsVisible = true,
                IsSortable = true,
                IsFilterable = true
            }
        ]);

        Assert.Equal(1, result.Count(column => column.Key == "acciones"));
        Assert.Equal(1, result.Count(column => column.Key == "asunto"));
    }

    private sealed class FakeHandler : IDynamicUiTableHandler
    {
        private readonly List<object> _rows;
        private readonly int _total;
        private readonly List<UiColumnDto>? _fixedColumns;

        public FakeHandler(string tableId, List<object> rows, int total, List<UiColumnDto>? fixedColumns = null)
        {
            TableId = tableId;
            _rows = rows;
            _total = total;
            _fixedColumns = fixedColumns;
        }

        public string TableId { get; }

        public Task<(List<object> rows, int total)> GetRowsAsync(DynamicUiTableQueryRequestDto req)
            => Task.FromResult((_rows, _total));

        public List<UiActionDto> GetActions(DynamicUiTableQueryRequestDto req) => [];

        public List<UiCellActionDto> GetCellActions(DynamicUiTableQueryRequestDto req) => [];

        public Task<MiApp.DTOs.DTOs.Utilidades.AppResponses<object>> ExecuteActionAsync(ExecuteUiActionRequestDto req)
            => Task.FromResult(new MiApp.DTOs.DTOs.Utilidades.AppResponses<object>
            {
                success = true,
                message = "OK",
                errors = [],
                data = new { ok = true }
            });

        public List<UiColumnDto>? GetFixedColumns() => _fixedColumns;
    }
}
