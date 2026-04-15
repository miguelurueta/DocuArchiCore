using MiApp.Models.Models.GestionCorrespondencia;
using MiApp.Repository.Repositorio.GestionCorrespondencia;
using MiApp.Services.Service.GestorDocumental;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSolicitaEstructuraRespuestaTests
{
    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTareaAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var repository = new Mock<ISolicitaEstructuraRespuestaIdTareaRepository>();
        var service = new ServiceSolicitaEstructuraRespuesta(repository.Object);

        var result = await service.SolicitaEstructuraRespuestaIdTareaAsync(1, " ");

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data);
        repository.Verify(r => r.SolicitaEstructuraRespuestaIdTareaAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaEstructuraRespuestaIdTareaAsync_CuandoRepositorioRetornaError_PropagaBadRequest()
    {
        var repository = new Mock<ISolicitaEstructuraRespuestaIdTareaRepository>();
        repository.Setup(r => r.SolicitaEstructuraRespuestaIdTareaAsync(1, "WF"))
            .ReturnsAsync(new MiApp.DTOs.DTOs.Utilidades.AppResponses<List<RaRespuestaRadicado>>
            {
                success = false,
                message = "Error",
                data = [],
                errors = []
            });

        var service = new ServiceSolicitaEstructuraRespuesta(repository.Object);
        var result = await service.SolicitaEstructuraRespuestaIdTareaAsync(1, "WF");

        Assert.False(result.success);
        Assert.Equal("Error", result.message);
    }
}

