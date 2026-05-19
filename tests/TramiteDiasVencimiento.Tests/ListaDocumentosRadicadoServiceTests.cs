using MiApp.DTOs.DTOs.GestorDocumental.Documentos.ListaDocumentosRadicados;
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.Repository.Repositorio.GestorDocumental.Documentos.ListaDocumentosRadicados;
using MiApp.Services.Service.GestorDocumental.Documentos.ListaDocumentosRadicados;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ListaDocumentosRadicadoServiceTests
{
    [Fact]
    public async Task Query_IncludeConfigTrue_MapeaFilaYConfiguraAccionVerDocumento()
    {
        var repository = new Mock<IListaDocumentosRadicadosRepository>();
        repository
            .Setup(r => r.SolicitaDocumentosRelacionadosAsync(It.IsAny<ListaDocumentosRadicadosTreeQueryRequestDto>(), "DA"))
            .ReturnsAsync(new MiApp.DTOs.DTOs.Utilidades.AppResponses<List<ListaDocumentosRadicadosLegacyRowDto>>
            {
                success = true,
                message = "OK",
                data =
                [
                    new ListaDocumentosRadicadosLegacyRowDto
                    {
                        ID = 9921,
                        DBT = -2,
                        PAG = 1,
                        TIPODOCUMENTO = "",
                        ESTADO_FIRMA_DIGITAL = "0",
                        ENLASE = "2500466700035"
                    }
                ],
                errors = []
            });

        var service = new ListaDocumentosRadicadoService(repository.Object, Mock.Of<ILogger<ListaDocumentosRadicadoService>>());

        var result = await service.SolicitaListaDocumentosRadicadosTreeAsync(
            new ListaDocumentosRadicadosTreeQueryRequestDto
            {
                IncludeConfig = true,
                ViewMode = "flatDocuments",
                NombreGabinete = "CORRESPO"
            },
            141,
            "DA");

        Assert.True(result.success);
        var payload = Assert.IsType<DynamicUiTableDto>(result.data);
        Assert.Equal("InboxListaRadicados", payload.TableId);
        Assert.Single(payload.Rows);
        Assert.Equal("DOC 9921", payload.Rows[0].Values["TIPODOCUMENTO"]);
        Assert.Equal("CORRESPO", payload.Rows[0].Values["NOMBRE_GABINETE"]);

        var verDocumento = Assert.Single(payload.RowActions.Where(a => a.ActionId == "ver_documento"));
        Assert.Equal("client_event", verDocumento.Behavior);
        Assert.NotNull(verDocumento.Request);
        Assert.Equal("ID", verDocumento.Request!.RowIdField);
        Assert.Equal("ID", verDocumento.Request.PayloadFields!["IdDocumento"]);
        Assert.Equal("NOMBRE_GABINETE", verDocumento.Request.PayloadFields["NombreGabinete"]);
        Assert.Equal("/api/gestor-documental/documentos/visualizacion/resolve", verDocumento.BehaviorConfig!["endpoint"]);
    }

    [Fact]
    public async Task Query_IncludeConfigFalse_YPaginacionDeshabilitada_RetornaRowsOnlySinPaginacion()
    {
        var repository = new Mock<IListaDocumentosRadicadosRepository>();
        repository
            .Setup(r => r.SolicitaDocumentosRelacionadosAsync(It.IsAny<ListaDocumentosRadicadosTreeQueryRequestDto>(), "DA"))
            .ReturnsAsync(new MiApp.DTOs.DTOs.Utilidades.AppResponses<List<ListaDocumentosRadicadosLegacyRowDto>>
            {
                success = true,
                message = "OK",
                data = [new ListaDocumentosRadicadosLegacyRowDto { ID = 1, TIPODOCUMENTO = "Radicado" }],
                errors = []
            });

        var service = new ListaDocumentosRadicadoService(repository.Object, Mock.Of<ILogger<ListaDocumentosRadicadoService>>());

        var result = await service.SolicitaListaDocumentosRadicadosTreeAsync(
            new ListaDocumentosRadicadosTreeQueryRequestDto
            {
                IncludeConfig = false,
                EnablePagination = false,
                ViewMode = "flatDocuments",
                NombreGabinete = "CORRESPO"
            },
            141,
            "DA");

        Assert.True(result.success);
        var payload = Assert.IsType<DynamicUiRowsOnlyDto>(result.data);
        Assert.Null(payload.Pagination);
        Assert.Single(payload.Rows);
    }

    [Fact]
    public async Task Action_VerDocumento_AceptaDocumentIdLegacyYRetornaResolveRequest()
    {
        var service = new ListaDocumentosRadicadoService(
            Mock.Of<IListaDocumentosRadicadosRepository>(),
            Mock.Of<ILogger<ListaDocumentosRadicadoService>>());

        var result = await service.EjecutaAccionListaDocumentosRadicadosTreeAsync(
            new ListaDocumentosRadicadosTreeActionRequestDto
            {
                ActionId = "ver_documento",
                RowId = "doc-9921",
                Payload = new Dictionary<string, object?>
                {
                    ["DocumentId"] = 9921,
                    ["NombreGabinete"] = "CORRESPO"
                }
            },
            141,
            "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal("view", result.data!.Operation);
        Assert.Equal(9921, result.data.DocumentResolveRequest!.IdDocumento);
        Assert.Equal("CORRESPO", result.data.DocumentResolveRequest.NombreGabinete);
    }

    [Fact]
    public async Task Action_CuandoActionIdNoSoportado_RetornaErrorValidacion()
    {
        var service = new ListaDocumentosRadicadoService(
            Mock.Of<IListaDocumentosRadicadosRepository>(),
            Mock.Of<ILogger<ListaDocumentosRadicadoService>>());

        var result = await service.EjecutaAccionListaDocumentosRadicadosTreeAsync(
            new ListaDocumentosRadicadosTreeActionRequestDto { ActionId = "no_soportada" },
            141,
            "DA");

        Assert.False(result.success);
        Assert.Equal("validation", result.meta?.Status);
    }
}
