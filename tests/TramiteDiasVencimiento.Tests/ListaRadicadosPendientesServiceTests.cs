using AutoMapper;
using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.usuario;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Models.Models.Radicacion.Tramite;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using MiApp.Services.Service.Usuario;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class ListaRadicadosPendientesServiceTests
{
    [Fact]
    public async Task SolicitaListaRadicadosPendientes_CuandoHayDatos_RetornaSuccess()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var pendientesRepo = new Mock<IListaRadicadosPendientesRepository>();
        var mapper = new Mock<IMapper>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "YES",
                data = new RemitDestInterno
                {
                    Id_Remit_Dest_Int = 10,
                    Relacion_Id_Usuario_Radicacion = 55,
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
                },
                errors = []
            });

        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado>
            {
                success = true,
                message = "YES",
                data = new SystemPlantillaRadicado
                {
                    id_Plantilla = 100,
                    Nombre_Plantilla_Radicado = "Default",
                    Tipo_Plantilla = "GEN"
                },
                errors = []
            });

        var row = new raradestadosmoduloradicacion
        {
            id_estado_radicado = 1,
            consecutivo_radicado = "RAD-1",
            remitente = "Juan",
            fecha_registro = new DateTime(2026, 3, 2),
            system_plantilla_radicado_id_Plantilla = 100,
            id_usuario_radicado = 55,
            estado = 1
        };

        pendientesRepo
            .Setup(r => r.SolicitaListaRadicadosPendientes(100, 55, "DA"))
            .ReturnsAsync(new AppResponses<List<raradestadosmoduloradicacion>>
            {
                success = true,
                message = "OK",
                data = [row],
                errors = []
            });

        mapper
            .Setup(m => m.Map<List<ListaRadicadosPendientesDto>>(It.IsAny<List<raradestadosmoduloradicacion>>()))
            .Returns(
            [
                new ListaRadicadosPendientesDto
                {
                    id_estado_radicado = 1,
                    consecutivo_radicado = "RAD-1",
                    remitente = "Juan",
                    fecha_registro = "2026-03-02"
                }
            ]);

        var service = new ListaRadicadosPendientesService(
            remitRepo.Object,
            plantillaRepo.Object,
            pendientesRepo.Object,
            mapper.Object);

        var result = await service.SolicitaListaRadicadosPendientes(10, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Single(result.data);
        Assert.Equal("RAD-1", result.data[0].consecutivo_radicado);
    }

    [Fact]
    public async Task SolicitaListaRadicadosPendientes_CuandoNoHayRegistros_RetornaSinResultados()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var pendientesRepo = new Mock<IListaRadicadosPendientesRepository>();
        var mapper = new Mock<IMapper>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "YES",
                data = new RemitDestInterno
                {
                    Id_Remit_Dest_Int = 10,
                    Relacion_Id_Usuario_Radicacion = 55,
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
                },
                errors = []
            });

        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado>
            {
                success = true,
                message = "YES",
                data = new SystemPlantillaRadicado
                {
                    id_Plantilla = 100,
                    Nombre_Plantilla_Radicado = "Default",
                    Tipo_Plantilla = "GEN"
                },
                errors = []
            });

        pendientesRepo
            .Setup(r => r.SolicitaListaRadicadosPendientes(100, 55, "DA"))
            .ReturnsAsync(new AppResponses<List<raradestadosmoduloradicacion>>
            {
                success = true,
                message = "Sin resultados",
                data = null!,
                errors = []
            });

        var service = new ListaRadicadosPendientesService(
            remitRepo.Object,
            plantillaRepo.Object,
            pendientesRepo.Object,
            mapper.Object);

        var result = await service.SolicitaListaRadicadosPendientes(10, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task SolicitaListaRadicadosPendientes_CuandoUsuarioRadicadorRelacionadoEsInvalido_RetornaErrorValidacion()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var pendientesRepo = new Mock<IListaRadicadosPendientesRepository>();
        var mapper = new Mock<IMapper>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "YES",
                data = new RemitDestInterno
                {
                    Id_Remit_Dest_Int = 77,
                    Relacion_Id_Usuario_Radicacion = 0,
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
                },
                errors = []
            });

        var service = new ListaRadicadosPendientesService(
            remitRepo.Object,
            plantillaRepo.Object,
            pendientesRepo.Object,
            mapper.Object);

        var result = await service.SolicitaListaRadicadosPendientes(10, "DA");

        Assert.False(result.success);
        Assert.Equal("El usuario radicador relacionado al usuario de gestión es null o 0", result.message);
        Assert.NotNull(result.errors);
        var validationErrors = result.errors!.OfType<AppError>().ToList();
        Assert.Contains(validationErrors, e => e.Field == "usuarioGestion.data.Relacion_Id_Usuario_Radicacion");
        Assert.Empty(result.data);

        plantillaRepo.Verify(
            p => p.SolicitaEstructuraPlantillaRadicacionDefault(It.IsAny<string>()),
            Times.Never);
        pendientesRepo.Verify(
            p => p.SolicitaListaRadicadosPendientes(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
    }
}
