using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.GestionCorrespondencia.GestionRespuesta;
using MiApp.Repository.Repositorio.GestionCorrespondencia.GestionRespuesta;
using MiApp.Services.Service.GestionCorrespondencia.GestionRespuesta;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSolicitaDocumentosAdjuntosRespuestaRadicadoTests
{
    [Fact]
    public async Task SolicitaDocumentosAdjuntosRespuestaRadicadoAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var repository = new Mock<ISolicitaDocumentosAdjuntosRespuestaRadicadoRepository>();
        var service = new ServiceSolicitaDocumentosAdjuntosRespuestaRadicado(repository.Object);

        var result = await service.SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(10, " ");

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        repository.Verify(r => r.SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaDocumentosAdjuntosRespuestaRadicadoAsync_CuandoSinDatos_RetornaEmpty()
    {
        var repository = new Mock<ISolicitaDocumentosAdjuntosRespuestaRadicadoRepository>();
        repository.Setup(r => r.SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(10, "WF"))
            .ReturnsAsync(new AppResponses<List<DocumentoAdjuntoRespuestaRadicadoDto>>
            {
                success = true,
                message = "Sin resultados",
                data = [],
                meta = new AppMeta { Status = "empty" },
                errors = []
            });

        var service = new ServiceSolicitaDocumentosAdjuntosRespuestaRadicado(repository.Object);
        var result = await service.SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(10, "WF");

        Assert.True(result.success);
        Assert.Empty(result.data);
        Assert.Equal("empty", result.meta?.Status);
    }

    [Fact]
    public async Task SolicitaDocumentosAdjuntosRespuestaRadicadoAsync_CuandoDuplicados_DeduplicaYLimita100()
    {
        var source = new List<DocumentoAdjuntoRespuestaRadicadoDto>();

        for (var i = 1; i <= 110; i++)
        {
            source.Add(new DocumentoAdjuntoRespuestaRadicadoDto
            {
                IdRespuestaRadicado = i,
                TipoAdjunto = "DocumentoPrincipal",
                IdImagen = i,
                Gabinete = "G"
            });

            source.Add(new DocumentoAdjuntoRespuestaRadicadoDto
            {
                IdRespuestaRadicado = i,
                TipoAdjunto = "DocumentoPrincipal",
                IdImagen = i,
                Gabinete = "G"
            });
        }

        var repository = new Mock<ISolicitaDocumentosAdjuntosRespuestaRadicadoRepository>();
        repository.Setup(r => r.SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(10, "WF"))
            .ReturnsAsync(new AppResponses<List<DocumentoAdjuntoRespuestaRadicadoDto>>
            {
                success = true,
                message = "YES",
                data = source,
                meta = new AppMeta { Status = "success" },
                errors = []
            });

        var service = new ServiceSolicitaDocumentosAdjuntosRespuestaRadicado(repository.Object);
        var result = await service.SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(10, "WF");

        Assert.True(result.success);
        Assert.Equal(100, result.data.Count);
        Assert.Equal("success", result.meta?.Status);
    }
}
