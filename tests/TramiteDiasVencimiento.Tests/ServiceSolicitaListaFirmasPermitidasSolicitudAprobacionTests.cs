using MiApp.Repository.Repositorio.GestionCorrespondencia.Firmas;
using MiApp.Services.Service.GestionCorrespondencia.Firmas;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSolicitaListaFirmasPermitidasSolicitudAprobacionTests
{
    [Fact]
    public async Task SolicitaListaFirmasPermitidasPorSolicitudAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var repository = new Mock<ISolicitaListaFirmasPermitidasSolicitudAprobacionRepository>();
        var service = new ServiceSolicitaListaFirmasPermitidasSolicitudAprobacion(repository.Object);

        var result = await service.SolicitaListaFirmasPermitidasPorSolicitudAsync(10, 1, " ");

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        repository.Verify(
            r => r.SolicitaListaFirmasPermitidasPorSolicitudAsync(It.IsAny<long>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task SolicitaListaFirmasPermitidasPorSolicitudAsync_CuandoSinDatos_RetornaEmpty()
    {
        var repository = new Mock<ISolicitaListaFirmasPermitidasSolicitudAprobacionRepository>();
        repository.Setup(r => r.SolicitaListaFirmasPermitidasPorSolicitudAsync(10, "WF"))
            .ReturnsAsync([]);

        var service = new ServiceSolicitaListaFirmasPermitidasSolicitudAprobacion(repository.Object);
        var result = await service.SolicitaListaFirmasPermitidasPorSolicitudAsync(10, 1, "WF");

        Assert.True(result.success);
        Assert.Empty(result.data);
        Assert.Equal("empty", result.meta?.Status);
    }

    [Fact]
    public async Task SolicitaListaFirmasPermitidasPorSolicitudAsync_CuandoDuplicados_DeduplicaYMapea()
    {
        var repository = new Mock<ISolicitaListaFirmasPermitidasSolicitudAprobacionRepository>();
        repository.Setup(r => r.SolicitaListaFirmasPermitidasPorSolicitudAsync(10, "WF"))
            .ReturnsAsync(
            [
                new FirmaPermitidaSolicitudRow { Id = 7, NombreRemitente = " Ana ", CargoRemite = " Lider " },
                new FirmaPermitidaSolicitudRow { Id = 7, NombreRemitente = "Ana", CargoRemite = "Lider" },
                new FirmaPermitidaSolicitudRow { Id = 8, NombreRemitente = null, CargoRemite = " " }
            ]);

        var service = new ServiceSolicitaListaFirmasPermitidasSolicitudAprobacion(repository.Object);
        var result = await service.SolicitaListaFirmasPermitidasPorSolicitudAsync(10, 1, "WF");

        Assert.True(result.success);
        Assert.Equal(2, result.data.Count);
        Assert.Equal("Ana - Lider", result.data[0].Descripcion);
        Assert.Equal("Sin nombre", result.data[1].Descripcion);
        Assert.Equal("success", result.meta?.Status);
    }
}
