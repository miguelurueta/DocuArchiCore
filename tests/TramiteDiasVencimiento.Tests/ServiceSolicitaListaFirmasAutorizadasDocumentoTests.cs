using MiApp.Repository.Repositorio.GestionCorrespondencia.Firmas;
using MiApp.Services.Service.GestionCorrespondencia.Firmas;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSolicitaListaFirmasAutorizadasDocumentoTests
{
    [Fact]
    public async Task SolicitaListaFirmasAutorizadasDocumentoAsync_CuandoNoAutorizado_RetornaValidacion()
    {
        var repository = new Mock<ISolicitaListaFirmasAutorizadasDocumentoRepository>();
        var logger = new Mock<ILogger<ServiceSolicitaListaFirmasAutorizadasDocumento>>();
        var service = new ServiceSolicitaListaFirmasAutorizadasDocumento(repository.Object, logger.Object);

        var result = await service.SolicitaListaFirmasAutorizadasDocumentoAsync(10, 9, "WF");

        Assert.False(result.success);
        Assert.Equal("No autorizado", result.message);
        repository.Verify(
            r => r.SolicitaListaFirmasAutorizadasDocumentoAsync(It.IsAny<long>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task SolicitaListaFirmasAutorizadasDocumentoAsync_CuandoSinDatos_RetornaEmpty()
    {
        var repository = new Mock<ISolicitaListaFirmasAutorizadasDocumentoRepository>();
        repository.Setup(r => r.SolicitaListaFirmasAutorizadasDocumentoAsync(10, "WF"))
            .ReturnsAsync([]);
        var logger = new Mock<ILogger<ServiceSolicitaListaFirmasAutorizadasDocumento>>();
        var service = new ServiceSolicitaListaFirmasAutorizadasDocumento(repository.Object, logger.Object);

        var result = await service.SolicitaListaFirmasAutorizadasDocumentoAsync(10, 10, "WF");

        Assert.True(result.success);
        Assert.Empty(result.data);
        Assert.Equal("empty", result.meta?.Status);
    }

    [Fact]
    public async Task SolicitaListaFirmasAutorizadasDocumentoAsync_CuandoDuplicados_DeduplicaYMapea()
    {
        var repository = new Mock<ISolicitaListaFirmasAutorizadasDocumentoRepository>();
        repository.Setup(r => r.SolicitaListaFirmasAutorizadasDocumentoAsync(10, "WF"))
            .ReturnsAsync(
            [
                new FirmaPermitidaSolicitudRow { Id = 7, NombreRemitente = " Ana ", CargoRemite = " Lider " },
                new FirmaPermitidaSolicitudRow { Id = 7, NombreRemitente = "Ana", CargoRemite = "Lider" },
                new FirmaPermitidaSolicitudRow { Id = 8, NombreRemitente = null, CargoRemite = " " }
            ]);
        var logger = new Mock<ILogger<ServiceSolicitaListaFirmasAutorizadasDocumento>>();
        var service = new ServiceSolicitaListaFirmasAutorizadasDocumento(repository.Object, logger.Object);

        var result = await service.SolicitaListaFirmasAutorizadasDocumentoAsync(10, 10, "WF");

        Assert.True(result.success);
        Assert.Equal(2, result.data.Count);
        Assert.Equal("Ana - Lider", result.data[0].Descripcion);
        Assert.Equal("Sin nombre", result.data[1].Descripcion);
        Assert.Equal("success", result.meta?.Status);
    }
}
