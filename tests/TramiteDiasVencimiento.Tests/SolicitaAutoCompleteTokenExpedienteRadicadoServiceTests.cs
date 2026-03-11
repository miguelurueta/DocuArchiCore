using MiApp.DTOs.DTOs.General;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Services.Service.Radicacion.PlantillaRadicado;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaAutoCompleteTokenExpedienteRadicadoServiceTests
{
    [Fact]
    public async Task ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync_CuandoDatosValidos_RetornaOk()
    {
        var repository = new Mock<ISolicitaAutoCompleteTokenExpedienteRadicadoRepository>();
        repository
            .Setup(x => x.SolicitaAutoCompleteTokenExpedienteRadicadoRepositoryAsync(
                It.IsAny<ParameterAutoComplete>(),
                "DA"))
            .ReturnsAsync(new AppResponses<List<rowTomSelect>>
            {
                success = true,
                message = "OK",
                data =
                [
                    new rowTomSelect { idValue = 1, texValue = "EXP-001" }
                ],
                errors = []
            });

        var service = new SolicitaAutoCompleteTokenExpedienteRadicadoService(repository.Object);
        var result = await service.ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync(
            new ParameterAutoComplete { TextoBuscado = "EXP" },
            "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Single(result.data);
    }

    [Fact]
    public async Task ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync_CuandoSinResultados_RetornaSinResultados()
    {
        var repository = new Mock<ISolicitaAutoCompleteTokenExpedienteRadicadoRepository>();
        repository
            .Setup(x => x.SolicitaAutoCompleteTokenExpedienteRadicadoRepositoryAsync(
                It.IsAny<ParameterAutoComplete>(),
                "DA"))
            .ReturnsAsync(new AppResponses<List<rowTomSelect>>
            {
                success = true,
                message = "Sin resultados",
                data = null!,
                errors = []
            });

        var service = new SolicitaAutoCompleteTokenExpedienteRadicadoService(repository.Object);
        var result = await service.ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync(
            new ParameterAutoComplete { TextoBuscado = "ZZZ" },
            "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync_CuandoExcepcion_RetornaErrorControlado()
    {
        var repository = new Mock<ISolicitaAutoCompleteTokenExpedienteRadicadoRepository>();
        repository
            .Setup(x => x.SolicitaAutoCompleteTokenExpedienteRadicadoRepositoryAsync(
                It.IsAny<ParameterAutoComplete>(),
                "DA"))
            .ThrowsAsync(new Exception("fallo simulado"));

        var service = new SolicitaAutoCompleteTokenExpedienteRadicadoService(repository.Object);
        var result = await service.ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync(
            new ParameterAutoComplete { TextoBuscado = "EXP" },
            "DA");

        Assert.False(result.success);
        Assert.NotNull(result.errors);
    }
}
