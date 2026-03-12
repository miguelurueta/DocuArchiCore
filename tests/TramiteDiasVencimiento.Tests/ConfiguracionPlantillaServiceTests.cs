using AutoMapper;
using MiApp.DTOs.DTOs.Radicacion.Configuracion;
using MiApp.Models.Models.Radicacion.Configuracion;
using MiApp.Repository.Repositorio.Radicador.Configuracion;
using MiApp.Services.Service.Radicacion.Configuracion;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ConfiguracionPlantillaServiceTests
{
    [Fact]
    public async Task SolicitaConfiguracionPlantillaAsync_CuandoDatosValidos_RetornaOk()
    {
        var repository = new Mock<IConfiguracionPlantillaRepository>();
        repository
            .Setup(x => x.SolicitaConfiguracionPlantillaAsync(67, 1, "DA"))
            .ReturnsAsync(new MiApp.DTOs.DTOs.Utilidades.AppResponses<RaRadConfigPlantillaRadicacion?>
            {
                success = true,
                message = "OK",
                data = new RaRadConfigPlantillaRadicacion
                {
                    id_rad_config_plantilla_radicacion = 1,
                    system_plantilla_radicado_id_Plantilla = 67,
                    Tipo_radicacion_plantilla = 1,
                    Descripcion_tipo_radicacion = "Externa",
                    util_notificacion_remitente = 1,
                    util_notificacion_destinatario = 0,
                    util_valida_restriccion_radicacion = 1
                },
                errors = []
            });

        var mapper = new Mock<IMapper>();
        mapper
            .Setup(m => m.Map<RaRadConfigPlantillaRadicacionDto>(It.IsAny<RaRadConfigPlantillaRadicacion>()))
            .Returns(new RaRadConfigPlantillaRadicacionDto
            {
                id_rad_config_plantilla_radicacion = 1,
                system_plantilla_radicado_id_Plantilla = 67,
                Tipo_radicacion_plantilla = 1,
                Descripcion_tipo_radicacion = "Externa",
                util_notificacion_remitente = 1,
                util_notificacion_destinatario = 0,
                util_valida_restriccion_radicacion = 1
            });

        var service = new ConfiguracionPlantillaService(repository.Object, mapper.Object);
        var result = await service.SolicitaConfiguracionPlantillaAsync(67, 1, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(67, result.data!.system_plantilla_radicado_id_Plantilla);
    }

    [Fact]
    public async Task SolicitaConfiguracionPlantillaAsync_CuandoSinResultados_RetornaSinResultados()
    {
        var repository = new Mock<IConfiguracionPlantillaRepository>();
        repository
            .Setup(x => x.SolicitaConfiguracionPlantillaAsync(67, 1, "DA"))
            .ReturnsAsync(new MiApp.DTOs.DTOs.Utilidades.AppResponses<RaRadConfigPlantillaRadicacion?>
            {
                success = true,
                message = "Sin resultados",
                data = null,
                errors = []
            });

        var service = new ConfiguracionPlantillaService(repository.Object, Mock.Of<IMapper>());
        var result = await service.SolicitaConfiguracionPlantillaAsync(67, 1, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task SolicitaConfiguracionPlantillaAsync_CuandoExcepcion_RetornaErrorControlado()
    {
        var repository = new Mock<IConfiguracionPlantillaRepository>();
        repository
            .Setup(x => x.SolicitaConfiguracionPlantillaAsync(67, 1, "DA"))
            .ThrowsAsync(new Exception("fallo simulado"));

        var service = new ConfiguracionPlantillaService(repository.Object, Mock.Of<IMapper>());
        var result = await service.SolicitaConfiguracionPlantillaAsync(67, 1, "DA");

        Assert.False(result.success);
        Assert.NotNull(result.errors);
    }
}
