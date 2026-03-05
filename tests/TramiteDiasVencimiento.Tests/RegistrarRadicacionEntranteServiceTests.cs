using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.usuario;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using MiApp.Services.Service.Usuario;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RegistrarRadicacionEntranteServiceTests
{
    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoRequestValido_RetornaSuccess()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var registrarRepo = new Mock<IRegistrarRadicacionEntranteRepository>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "OK",
                data = BuildRemitDestInterno(55)
            });

        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacion(100, "DA"))
            .ReturnsAsync(new AppResponse<SystemPlantillaRadicado>
            {
                Success = true,
                Message = "YES",
                Data = BuildPlantilla(100)
            });

        registrarRepo
            .Setup(r => r.RegistrarRadicacionEntranteAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                55,
                "DA",
                It.IsAny<SystemPlantillaRadicado>()))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto>
            {
                success = true,
                message = "OK",
                data = new RegistrarRadicacionEntranteResponseDto { ConsecutivoRadicado = "RAD-TEST-1" }
            });

        var service = new RegistrarRadicacionEntranteService(remitRepo.Object, plantillaRepo.Object, registrarRepo.Object);

        var result = await service.RegistrarRadicacionEntranteAsync(new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            TipoRadicacion = "ENTRANTE",
            Remitente = "R",
            Asunto = "A"
        }, 10, "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal("RAD-TEST-1", result.data!.ConsecutivoRadicado);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoAliasVacio_RetornaValidacion()
    {
        var service = new RegistrarRadicacionEntranteService(
            Mock.Of<IRemitDestInternoR>(),
            Mock.Of<ISystemPlantillaRadicadoR>(),
            Mock.Of<IRegistrarRadicacionEntranteRepository>());

        var result = await service.RegistrarRadicacionEntranteAsync(
            new RegistrarRadicacionEntranteRequestDto { IdPlantilla = 100 },
            10,
            string.Empty);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_PropagaAliasYUsuarioRadicadorAlRepositorio()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var registrarRepo = new Mock<IRegistrarRadicacionEntranteRepository>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(25, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "OK",
                data = BuildRemitDestInterno(77)
            });

        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacion(10, "DA"))
            .ReturnsAsync(new AppResponse<SystemPlantillaRadicado>
            {
                Success = true,
                Message = "YES",
                Data = BuildPlantilla(10)
            });

        registrarRepo
            .Setup(r => r.RegistrarRadicacionEntranteAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                77,
                "DA",
                It.IsAny<SystemPlantillaRadicado>()))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto>
            {
                success = true,
                message = "OK",
                data = new RegistrarRadicacionEntranteResponseDto()
            });

        var service = new RegistrarRadicacionEntranteService(remitRepo.Object, plantillaRepo.Object, registrarRepo.Object);
        var request = new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 10,
            TipoRadicacion = "ENTRANTE",
            Remitente = "R",
            Asunto = "A"
        };

        var result = await service.RegistrarRadicacionEntranteAsync(request, 25, "DA");

        Assert.True(result.success);
        registrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(request, 77, "DA", It.IsAny<SystemPlantillaRadicado>()), Times.Once);
    }

    private static SystemPlantillaRadicado BuildPlantilla(int idPlantilla)
    {
        return new SystemPlantillaRadicado
        {
            id_Plantilla = idPlantilla,
            Nombre_Plantilla_Radicado = "ra_plantilla_100",
            Tipo_Plantilla = "RAD",
            Fecha_Creacion = DateTime.UtcNow,
            Estado_Plantilla = 1,
            Consecutivo_Rad = 10,
            Consecutivo_CodBarra = 10,
            util_activo_plantilla_flujo = 1,
            Util_activo_plantilla_default_rad_interno = 0,
            util_evalua_periodo_general = 0,
            utill_evalua_periodo_pqr = 0,
            util_evalua_periodo_interno = 0,
            util_evalua_festivo = 0,
            util_tipo_modulo_envio = 0,
            util_consecutivo_general = 0,
            util_default_simple = 0,
            util_estado_pendiente_rad = 0,
            utilIncuyeConsecutivoArea = 0,
            utilConseTipoRad = 0,
            util_default_radicado = 1
        };
    }

    private static RemitDestInterno BuildRemitDestInterno(int idUsuarioRadicacion)
    {
        return new RemitDestInterno
        {
            Id_Remit_Dest_Int = 1,
            Relacion_Id_Usuario_Radicacion = idUsuarioRadicacion,
            Nombre_Remitente = string.Empty,
            Cargo_Remite = string.Empty,
            Login_Usuario = string.Empty,
            Pasw_Usuario = string.Empty,
            Correo_Electronico = string.Empty,
            Telefono_Usuario = string.Empty,
            Firma_Usuario = [],
            Pasw_Encript = string.Empty,
            Relacion_Workflow_Login = string.Empty,
            Relacion_Da_Login = string.Empty,
            Relacion_Login_Radicacion = string.Empty,
            Identificacion = string.Empty,
            Direccion = string.Empty,
            Relacion_Workflow_Login_Extend = string.Empty
        };
    }
}
