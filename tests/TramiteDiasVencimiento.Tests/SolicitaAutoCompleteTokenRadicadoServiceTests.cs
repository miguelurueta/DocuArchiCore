using MiApp.DTOs.DTOs.General;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Services.Service.Radicacion.PlantillaRadicado;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaAutoCompleteTokenRadicadoServiceTests
{
    [Fact]
    public async Task ServiceSolicitaAutoCompleteTokenRadicadoAsync_CuandoDatosValidos_RetornaOk()
    {
        var systemPlantilla = new Mock<ISystemPlantillaRadicadoR>();
        var repository = new Mock<ISolicitaAutoCompleteTokenRadicadoRepository>();

        systemPlantilla
            .Setup(x => x.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado>
            {
                success = true,
                message = "OK",
                data = new SystemPlantillaRadicado
                {
                    Nombre_Plantilla_Radicado = "ra_test_radicado",
                    Tipo_Plantilla = "RADICACION"
                },
                errors = []
            });

        repository
            .Setup(x => x.SolicitaAutoCompleteTokenRadicadoRepositoryAsync(
                It.IsAny<ParameterAutoComplete>(),
                "ra_test_radicado",
                "DA"))
            .ReturnsAsync(new AppResponses<List<rowTomSelect>>
            {
                success = true,
                message = "OK",
                data =
                [
                    new rowTomSelect { texValue = "26000100010100001" }
                ],
                errors = []
            });

        var service = new SolicitaAutoCompleteTokenRadicadoService(systemPlantilla.Object, repository.Object);
        var result = await service.ServiceSolicitaAutoCompleteTokenRadicadoAsync(
            new ParameterAutoComplete { TextoBuscado = "260001" },
            "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Single(result.data);
    }

    [Fact]
    public async Task ServiceSolicitaAutoCompleteTokenRadicadoAsync_CuandoNoHayPlantillaDefault_RetornaSinResultados()
    {
        var systemPlantilla = new Mock<ISystemPlantillaRadicadoR>();
        var repository = new Mock<ISolicitaAutoCompleteTokenRadicadoRepository>();

        systemPlantilla
            .Setup(x => x.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado>
            {
                success = false,
                message = "NO_DATA",
                data = null!,
                errors = []
            });

        var service = new SolicitaAutoCompleteTokenRadicadoService(systemPlantilla.Object, repository.Object);
        var result = await service.ServiceSolicitaAutoCompleteTokenRadicadoAsync(
            new ParameterAutoComplete { TextoBuscado = "260001" },
            "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task ServiceSolicitaAutoCompleteTokenRadicadoAsync_CuandoExcepcion_RetornaErrorControlado()
    {
        var systemPlantilla = new Mock<ISystemPlantillaRadicadoR>();
        var repository = new Mock<ISolicitaAutoCompleteTokenRadicadoRepository>();

        systemPlantilla
            .Setup(x => x.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ThrowsAsync(new Exception("fallo simulado"));

        var service = new SolicitaAutoCompleteTokenRadicadoService(systemPlantilla.Object, repository.Object);
        var result = await service.ServiceSolicitaAutoCompleteTokenRadicadoAsync(
            new ParameterAutoComplete { TextoBuscado = "260001" },
            "DA");

        Assert.False(result.success);
        Assert.NotNull(result.errors);
    }
}
