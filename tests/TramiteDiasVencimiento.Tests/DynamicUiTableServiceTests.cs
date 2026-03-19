using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.Services.Service.UI.MuiTable;
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
        Assert.Equal("1", payload.Rows[0].Id);
        Assert.Equal("Prueba", payload.Rows[0].Values["asunto"]);
        Assert.Equal(1, payload.Pagination!.Page);
        Assert.Equal("asunto", payload.Sorting!.SortField);
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
