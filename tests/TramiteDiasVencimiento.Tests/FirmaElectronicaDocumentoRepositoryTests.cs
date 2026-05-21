using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.Documentos.FirmaElectronica;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class FirmaElectronicaDocumentoRepositoryTests
{
    [Fact]
    public async Task GetLatestFirmaRegistroAsync_CuandoInputValido_ConstruyeQueryOptionsEsperado()
    {
        var engine = new Mock<IDapperCrudEngine>();
        QueryOptions? captured = null;

        engine.Setup(e => e.GetAllAsync<FirmaElectronicaDocumentoRepositoryRow>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(q => captured = q)
            .ReturnsAsync(new QueryResult<FirmaElectronicaDocumentoRepositoryRow>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new FirmaElectronicaDocumentoRepositoryRow
                    {
                        IdRegistroCertificadoArchivo = 22,
                        IdCertificado = 999
                    }
                ]
            });

        var repository = new FirmaElectronicaDocumentoRepository(engine.Object);
        var row = await repository.GetLatestFirmaRegistroAsync(123, "WF_DOCS", "DA");

        Assert.NotNull(row);
        Assert.Equal(999, row!.IdCertificado);
        Assert.NotNull(captured);
        Assert.Equal("ra_cert_registro_certificado_archivo", captured!.TableName);
        Assert.Equal("DA", captured.DefaultAlias);
        Assert.Equal(1, captured.Limit);
        Assert.Equal(123L, captured.Filters["id_archivo"]);
        Assert.Equal("WF_DOCS", captured.Filters["nombre_gabinete"]);
        Assert.Single(captured.OrderByFields);
        Assert.Equal("id_registro_certificado_archivo", captured.OrderByFields[0].Column);
        Assert.Equal("DESC", captured.OrderByFields[0].Direction);
    }

    [Fact]
    public async Task GetLatestFirmaRegistroAsync_CuandoNombreGabineteInvalido_LanzaArgumentException()
    {
        var engine = new Mock<IDapperCrudEngine>();
        var repository = new FirmaElectronicaDocumentoRepository(engine.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            repository.GetLatestFirmaRegistroAsync(123, "WF_DOCS;DROP", "DA"));

        engine.Verify(e => e.GetAllAsync<FirmaElectronicaDocumentoRepositoryRow>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task GetLatestFirmaRegistroAsync_CuandoMotorFalla_LanzaInvalidOperationException()
    {
        var engine = new Mock<IDapperCrudEngine>();
        engine.Setup(e => e.GetAllAsync<FirmaElectronicaDocumentoRepositoryRow>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<FirmaElectronicaDocumentoRepositoryRow>
            {
                Success = false,
                ErrorMessage = "boom"
            });

        var repository = new FirmaElectronicaDocumentoRepository(engine.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repository.GetLatestFirmaRegistroAsync(123, "WF_DOCS", "DA"));
    }
}
