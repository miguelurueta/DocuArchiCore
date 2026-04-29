using MiApp.DTOs.DTOs.Common;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestionCorrespondencia.Firmas;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSolicitaFirmasDocumentoRespuestaOrquestadoTests
{
    [Fact]
    public async Task SolicitaFirmasDocumentoRespuestaOrquestadoAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var principal = new Mock<IServiceSolicitaUsuarioPrincipalRespuesta>();
        var autorizadas = new Mock<IServiceSolicitaListaFirmasAutorizadasDocumento>();
        var service = new ServiceSolicitaFirmasDocumentoRespuestaOrquestado(principal.Object, autorizadas.Object);

        var result = await service.SolicitaFirmasDocumentoRespuestaOrquestadoAsync(10, 10, " ");

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        principal.Verify(x => x.SolicitaUsuarioPrincipalRespuestaAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaFirmasDocumentoRespuestaOrquestadoAsync_CuandoSinDatos_RetornaEmpty()
    {
        var principal = new Mock<IServiceSolicitaUsuarioPrincipalRespuesta>();
        principal.Setup(x => x.SolicitaUsuarioPrincipalRespuestaAsync(10, 10, "WF"))
            .ReturnsAsync(new AppResponses<ResponseDropdownDto?> { success = true, message = "Usuario no encontrado", data = null, errors = [] });

        var autorizadas = new Mock<IServiceSolicitaListaFirmasAutorizadasDocumento>();
        autorizadas.Setup(x => x.SolicitaListaFirmasAutorizadasDocumentoAsync(10, 10, "WF"))
            .ReturnsAsync(new AppResponses<List<ResponseDropdownDto>> { success = true, message = "Sin resultados", data = [], errors = [] });

        var service = new ServiceSolicitaFirmasDocumentoRespuestaOrquestado(principal.Object, autorizadas.Object);
        var result = await service.SolicitaFirmasDocumentoRespuestaOrquestadoAsync(10, 10, "WF");

        Assert.True(result.success);
        Assert.Empty(result.data);
        Assert.Equal("empty", result.meta?.Status);
    }

    [Fact]
    public async Task SolicitaFirmasDocumentoRespuestaOrquestadoAsync_CuandoDatos_CombineYDeduplica()
    {
        var principal = new Mock<IServiceSolicitaUsuarioPrincipalRespuesta>();
        principal.Setup(x => x.SolicitaUsuarioPrincipalRespuestaAsync(10, 10, "WF"))
            .ReturnsAsync(new AppResponses<ResponseDropdownDto?>
            {
                success = true,
                message = "YES",
                data = new ResponseDropdownDto { Id = 7, Descripcion = "Ana - Lider" },
                errors = []
            });

        var autorizadas = new Mock<IServiceSolicitaListaFirmasAutorizadasDocumento>();
        autorizadas.Setup(x => x.SolicitaListaFirmasAutorizadasDocumentoAsync(10, 10, "WF"))
            .ReturnsAsync(new AppResponses<List<ResponseDropdownDto>>
            {
                success = true,
                message = "YES",
                data =
                [
                    new ResponseDropdownDto { Id = 7, Descripcion = "Ana - Lider" },
                    new ResponseDropdownDto { Id = 8, Descripcion = "Luis - Analista" }
                ],
                errors = []
            });

        var service = new ServiceSolicitaFirmasDocumentoRespuestaOrquestado(principal.Object, autorizadas.Object);
        var result = await service.SolicitaFirmasDocumentoRespuestaOrquestadoAsync(10, 10, "WF");

        Assert.True(result.success);
        Assert.Equal(2, result.data.Count);
        Assert.Equal(7, result.data[0].Id);
        Assert.Equal(8, result.data[1].Id);
        Assert.Equal("success", result.meta?.Status);
    }
}
