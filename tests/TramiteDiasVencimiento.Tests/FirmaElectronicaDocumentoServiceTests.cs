using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.Errors;
using MiApp.Repository.Repositorio.GestorDocumental.Documentos.FirmaElectronica;
using MiApp.Services.Service.GestorDocumental.Documentos.FirmaElectronica;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class FirmaElectronicaDocumentoServiceTests
{
    [Fact]
    public async Task GetFirmaElectronicaDocumentoAsync_CuandoExisteRegistro_RetornaFirmado()
    {
        var repo = new Mock<IFirmaElectronicaDocumentoRepository>();
        repo.Setup(r => r.GetLatestFirmaRegistroAsync(123, "WF_DOCS", "DA"))
            .ReturnsAsync(new FirmaElectronicaDocumentoRepositoryRow
            {
                IdRegistroCertificadoArchivo = 20,
                IdCertificado = 456
            });

        var service = new FirmaElectronicaDocumentoService(repo.Object, Mock.Of<ILogger<FirmaElectronicaDocumentoService>>());
        var result = await service.GetFirmaElectronicaDocumentoAsync(123, "WF_DOCS", "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.True(result.data.FirmadoElectronico);
        Assert.Equal(456, result.data.IdCertificado);
    }

    [Fact]
    public async Task GetFirmaElectronicaDocumentoAsync_CuandoNoExisteRegistro_RetornaNoFirmado()
    {
        var repo = new Mock<IFirmaElectronicaDocumentoRepository>();
        repo.Setup(r => r.GetLatestFirmaRegistroAsync(123, "WF_DOCS", "DA"))
            .ReturnsAsync((FirmaElectronicaDocumentoRepositoryRow?)null);

        var service = new FirmaElectronicaDocumentoService(repo.Object, Mock.Of<ILogger<FirmaElectronicaDocumentoService>>());
        var result = await service.GetFirmaElectronicaDocumentoAsync(123, "WF_DOCS", "DA");

        Assert.True(result.success);
        Assert.False(result.data.FirmadoElectronico);
        Assert.Equal(0, result.data.IdCertificado);
    }

    [Fact]
    public async Task GetFirmaElectronicaDocumentoAsync_CuandoNombreGabineteInvalido_RetornaValidation()
    {
        var service = new FirmaElectronicaDocumentoService(
            Mock.Of<IFirmaElectronicaDocumentoRepository>(),
            Mock.Of<ILogger<FirmaElectronicaDocumentoService>>());

        var result = await service.GetFirmaElectronicaDocumentoAsync(123, "WF_DOCS;DROP", "DA");

        Assert.False(result.success);
        Assert.Equal("validation", result.meta?.Status);
        Assert.Contains(result.errors!.OfType<AppError>(), e => e.Field == "nombreGabinete");
    }

    [Fact]
    public async Task GetFirmaElectronicaDocumentoAsync_CuandoRepositoryFalla_RetornaErrorControlado()
    {
        var repo = new Mock<IFirmaElectronicaDocumentoRepository>();
        repo.Setup(r => r.GetLatestFirmaRegistroAsync(123, "WF_DOCS", "DA"))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var service = new FirmaElectronicaDocumentoService(repo.Object, Mock.Of<ILogger<FirmaElectronicaDocumentoService>>());
        var result = await service.GetFirmaElectronicaDocumentoAsync(123, "WF_DOCS", "DA");

        Assert.False(result.success);
        Assert.Equal("error", result.meta?.Status);
        Assert.Contains(result.errors!.OfType<AppError>(), e => e.Field == "FirmaElectronicaDocumento");
    }
}
