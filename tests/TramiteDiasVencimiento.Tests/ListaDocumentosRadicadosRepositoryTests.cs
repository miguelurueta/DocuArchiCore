using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.GestorDocumental.Documentos.ListaDocumentosRadicados;
using MiApp.DTOs.DTOs.Workflow.BandejaCorrespondencia;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.Documentos.ListaDocumentosRadicados;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ListaDocumentosRadicadosRepositoryTests
{
    [Fact]
    public async Task SolicitaDocumentosRelacionadosAsync_CuandoRequestValido_ConstruyeQueryOptionsParametrizado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        QueryOptions? captured = null;

        dapper
            .Setup(engine => engine.GetAllAsync<ListaDocumentosRadicadosLegacyRowDto>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(options => captured = options)
            .ReturnsAsync(new QueryResult<ListaDocumentosRadicadosLegacyRowDto>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new ListaDocumentosRadicadosLegacyRowDto
                    {
                        ID = 15416,
                        DBT = 778,
                        PAG = 4,
                        TIPODOCUMENTO = "Factura",
                        ESTADO_FIRMA_DIGITAL = "PENDIENTE",
                        ENLASE = "2500466700035"
                    }
                ]
            });

        var repository = new ListaDocumentosRadicadosRepository(dapper.Object);
        var request = new ListaDocumentosRadicadosTreeQueryRequestDto
        {
            NombreGabinete = "WF_DOCS",
            CampoRadicado = "ENLASE",
            Search = "2500466700035",
            SortField = "NO_PERMITIDO",
            SortDir = "DESC",
            Page = 2,
            PageSize = 30,
            StructuredFilters =
            [
                new WorkflowStructuredFilterDto { Field = "TIPODOCUMENTO", Operator = "eq", Value = "Factura" },
                new WorkflowStructuredFilterDto { Field = "INYECCION_SQL", Operator = "=", Value = "X" },
                new WorkflowStructuredFilterDto { Field = "PAG", Operator = "like", Value = "4" }
            ]
        };

        var result = await repository.SolicitaDocumentosRelacionadosAsync(request, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.Single(result.data);
        Assert.NotNull(captured);
        Assert.Equal("WF_DOCS", captured!.TableName);
        Assert.Equal("DA", captured.DefaultAlias);
        Assert.Equal(30, captured.Limit);
        Assert.Equal(30, captured.Offset);
        Assert.Contains("ID", captured.Columns);
        Assert.Contains("ESTADO_FIRMA_DIGITAL", captured.Columns);
        Assert.Single(captured.OrderByFields);
        Assert.Equal("ID", captured.OrderByFields[0].Column);
        Assert.Equal("DESC", captured.OrderByFields[0].Direction);
        Assert.Equal("2500466700035", captured.Filters["ENLASE"]);
        Assert.Equal("Factura", captured.Filters["TIPODOCUMENTO"]);
        Assert.DoesNotContain("INYECCION_SQL", captured.Filters.Keys, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain("PAG", captured.Filters.Keys, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SolicitaDocumentosRelacionadosAsync_CuandoNombreGabineteEsInvalido_RetornaValidationYSinConsultarDapper()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new ListaDocumentosRadicadosRepository(dapper.Object);

        var result = await repository.SolicitaDocumentosRelacionadosAsync(
            new ListaDocumentosRadicadosTreeQueryRequestDto
            {
                NombreGabinete = "WF_DOCS;DROP TABLE X",
                Search = "ABC"
            },
            "DA");

        Assert.False(result.success);
        Assert.Equal("NombreGabinete invalido", result.message);
        Assert.Equal("validation", result.meta?.Status);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Field == "NombreGabinete");
        dapper.Verify(
            engine => engine.GetAllAsync<ListaDocumentosRadicadosLegacyRowDto>(It.IsAny<QueryOptions>()),
            Times.Never);
    }

    [Fact]
    public async Task SolicitaDocumentosRelacionadosAsync_CuandoEngineFalla_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper
            .Setup(engine => engine.GetAllAsync<ListaDocumentosRadicadosLegacyRowDto>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<ListaDocumentosRadicadosLegacyRowDto>
            {
                Success = false,
                Message = "boom"
            });

        var repository = new ListaDocumentosRadicadosRepository(dapper.Object);

        var result = await repository.SolicitaDocumentosRelacionadosAsync(
            new ListaDocumentosRadicadosTreeQueryRequestDto
            {
                NombreGabinete = "WF_DOCS",
                Search = "2500466700035"
            },
            "DA");

        Assert.False(result.success);
        Assert.Equal("Error al consultar documentos relacionados", result.message);
        Assert.Equal("error", result.meta?.Status);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Field == "SolicitaDocumentosRelacionadosAsync");
    }
}

