using AutoMapper;
using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.usuario;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Models.Models.Radicacion.Tramite;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using MiApp.Services.Service.UI.MuiTable;
using MiApp.Services.Service.Usuario;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class ListaRadicadosPendientesServiceTests
{
    [Fact]
    public async Task SolicitaListaRadicadosPendientes_CuandoHayDatos_RetornaTablaDinamica()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var pendientesRepo = new Mock<IListaRadicadosPendientesRepository>();
        var builder = new Mock<IDynamicUiTableBuilder>();
        var mapper = new Mock<IMapper>();
        DynamicUiTableBuildInput? capturedInput = null;

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(BuildUsuarioGestionResponse(55));

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
                message = "OK",
                data =
                [
                    new raradestadosmoduloradicacion
                    {
                        id_estado_radicado = 1,
                        consecutivo_radicado = "RAD-1",
                        remitente = "Juan",
                        fecha_registro = new DateTime(2026, 3, 2),
                        system_plantilla_radicado_id_Plantilla = 100,
                        id_usuario_radicado = 55,
                        estado = 1
                    }
                ],
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

        builder
            .Setup(b => b.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()))
            .Callback<DynamicUiTableBuildInput>(input => capturedInput = input)
            .ReturnsAsync(new DynamicUiTableDto
            {
                TableId = "lista-radicados-pendientes",
                Rows =
                [
                    new UiRowDto
                    {
                        Id = "1",
                        Values = new Dictionary<string, object?>
                        {
                            ["id_estado_radicado"] = 1L,
                            ["consecutivo_radicado"] = "RAD-1"
                        }
                    }
                ],
                CellActions =
                [
                    new UiCellActionDto
                    {
                        ColumnKey = "actions",
                        Action = new UiActionDto
                        {
                            ActionId = "asignacion-tarea",
                            Request = new DynamicUiActionRequestDto
                            {
                                RowIdField = "id_estado_radicado"
                            }
                        }
                    }
                ]
            });

        var service = new ListaRadicadosPendientesService(
            remitRepo.Object,
            plantillaRepo.Object,
            pendientesRepo.Object,
            builder.Object,
            mapper.Object);

        var result = await service.SolicitaListaRadicadosPendientes(10, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Equal("lista-radicados-pendientes", result.data.TableId);
        Assert.NotNull(capturedInput);
        Assert.Equal("lista-radicados-pendientes", capturedInput!.Request.TableId);
        Assert.Single(capturedInput.CellActions);
        Assert.Equal("actions", capturedInput.CellActions[0].ColumnKey);
        var actionRequest = Assert.IsType<DynamicUiActionRequestDto>(capturedInput.CellActions[0].Action.Request);
        Assert.Equal("id_estado_radicado", actionRequest.RowIdField);
        Assert.Equal("id_estado_radicado", actionRequest.PayloadFields!["id_estado_radicado"]);
    }

    [Fact]
    public async Task SolicitaListaRadicadosPendientes_CuandoNoHayRegistros_RetornaSinResultados()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var pendientesRepo = new Mock<IListaRadicadosPendientesRepository>();
        var builder = new Mock<IDynamicUiTableBuilder>();
        var mapper = new Mock<IMapper>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(BuildUsuarioGestionResponse(55));

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
            builder.Object,
            mapper.Object);

        var result = await service.SolicitaListaRadicadosPendientes(10, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
        builder.Verify(b => b.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaListaRadicadosPendientes_CuandoUsuarioRadicadorRelacionadoEsInvalido_RetornaErrorValidacion()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var pendientesRepo = new Mock<IListaRadicadosPendientesRepository>();
        var builder = new Mock<IDynamicUiTableBuilder>();
        var mapper = new Mock<IMapper>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(BuildUsuarioGestionResponse(0));

        var service = new ListaRadicadosPendientesService(
            remitRepo.Object,
            plantillaRepo.Object,
            pendientesRepo.Object,
            builder.Object,
            mapper.Object);

        var result = await service.SolicitaListaRadicadosPendientes(10, "DA");

        Assert.False(result.success);
        Assert.Equal("El usuario radicador relacionado al usuario de gestión es null o 0", result.message);
        Assert.NotNull(result.errors);
        Assert.Contains(result.errors!.OfType<AppError>(), e => e.Field == "usuarioGestion.data.Relacion_Id_Usuario_Radicacion");
        Assert.Null(result.data);

        plantillaRepo.Verify(
            p => p.SolicitaEstructuraPlantillaRadicacionDefault(It.IsAny<string>()),
            Times.Never);
        pendientesRepo.Verify(
            p => p.SolicitaListaRadicadosPendientes(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
        builder.Verify(b => b.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaListaRadicadosPendientes_CuandoBuilderLanzaExcepcion_RetornaErrorControlado()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var pendientesRepo = new Mock<IListaRadicadosPendientesRepository>();
        var builder = new Mock<IDynamicUiTableBuilder>();
        var mapper = new Mock<IMapper>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(BuildUsuarioGestionResponse(55));

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
                message = "OK",
                data =
                [
                    new raradestadosmoduloradicacion
                    {
                        id_estado_radicado = 1,
                        consecutivo_radicado = "RAD-1",
                        remitente = "Juan",
                        fecha_registro = new DateTime(2026, 3, 2),
                        system_plantilla_radicado_id_Plantilla = 100,
                        id_usuario_radicado = 55,
                        estado = 1
                    }
                ],
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

        builder
            .Setup(b => b.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var service = new ListaRadicadosPendientesService(
            remitRepo.Object,
            plantillaRepo.Object,
            pendientesRepo.Object,
            builder.Object,
            mapper.Object);

        var result = await service.SolicitaListaRadicadosPendientes(10, "DA");

        Assert.False(result.success);
        Assert.Equal("Error al obtener radicados pendientes", result.message);
        Assert.NotNull(result.errors);
        Assert.Contains(result.errors!.OfType<AppError>(), e => e.Type == "Exception" && e.Message == "boom");
        Assert.Null(result.data);
    }

    private static AppResponses<RemitDestInterno> BuildUsuarioGestionResponse(int idUsuarioRadicacion) =>
        new()
        {
            success = true,
            message = "YES",
            data = new RemitDestInterno
            {
                Id_Remit_Dest_Int = 10,
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
            },
            errors = []
        };
}
