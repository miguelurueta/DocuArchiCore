using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Radicacion.Configuracion;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.RutaTrabajo;
using MiApp.Models.Models.GestorDocumental.usuario;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Models.Models.Radicacion.TipoTramite;
using MiApp.Models.Models.Workflow.Flujo;
using MiApp.Models.Models.Workflow.Grupo;
using MiApp.Models.Models.Workflow.Usuario;
using MiApp.Repository.ErrorController;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Repository.Repositorio.Workflow.Flujo;
using MiApp.Repository.Repositorio.Workflow.Grupo;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using MiApp.Repository.Repositorio.Workflow.usuario;
using MiApp.Services.Service.Radicacion.Configuracion;
using MiApp.Services.Service.Radicacion.PlantillaRadicado;
using MiApp.Services.Service.Radicacion.RelacionCamposRutaWorklflow;
using MiApp.Services.Service.Radicacion.Tramite;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.Usuario;
using MiApp.Services.Service.Workflow.RutaTrabajo;
using Moq;
using System.Linq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RegistrarRadicacionEntranteServiceTests
{
    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoRequestEsNull_RetornaValidacionControlada()
    {
        var fx = BuildFixture();

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(null!, 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("Request requerido", result.message);
        Assert.Contains(result.errors.Cast<AppError>(), error => error.Field == "request");
        fx.RegistrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(), It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoRequestValidoNoWorkflow_ConsultaRutaYOmiteUsuarioWorkflow()
    {
        var fx = BuildFixture();

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.True(result.success);
        fx.SolicitaEstructuraRutaWorkflowService.Verify(r => r.SolicitaEstructuraRutaWorkflowAsync("WF"), Times.Once);
        fx.UsuarioWorkflowRepository.Verify(r => r.SolicitaEstructuraIdUsuarioWorkflowId(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        fx.RegistrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 55, 10, "DA", It.IsAny<string>(), 1, "RADICACION",
            It.IsAny<SystemPlantillaRadicado>(), It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
            It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoTipoModuloEnvioRequiereActividadRelacionada_ConsultaGrupoWorkflowAntesDeRegistrar()
    {
        var fx = BuildFixture();
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow()
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(18) });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.True(result.success);
        fx.GruposWorkflowRepository.Verify(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"), Times.Once);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoTipoModuloEsWorkflowPeroTipoEnvioNoRequiereWorkflow_ConsultaRutaInternaYOmitePreRegistro()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 2);

        Assert.True(result.success);
        fx.ValidaPreRegistroWorkflowService.Verify(r => r.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<string>(), It.IsAny<TipoDocEntrante>()), Times.Never);
        fx.UsuarioWorkflowRepository.Verify(r => r.SolicitaEstructuraIdUsuarioWorkflowId(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        fx.SolicitaEstructuraRutaWorkflowService.Verify(r => r.SolicitaEstructuraRutaWorkflowAsync("WF"), Times.Once);
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Verify(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoWorkflowYTipoModuloEnvioRequiereActividad_ConsultaUsuarioYGrupoAntesDeRegistrar()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow()
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(18) });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 2);

        Assert.True(result.success);
        fx.UsuarioWorkflowRepository.Verify(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"), Times.Once);
        fx.GruposWorkflowRepository.Verify(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"), Times.Once);
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Verify(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"), Times.Once);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoExistenciaWorkflowFalla_RetornaErrorControlado()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow()
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(18) });
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>
            {
                success = false,
                message = "existencia workflow failed",
                errors = [new AppError { Field = "workflow", Message = "existencia workflow failed", Type = "Workflow" }],
                data = new SolicitaExistenciaRadicadoRutaWorkflowDto()
            });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 2);

        Assert.False(result.success);
        Assert.Equal("existencia workflow failed", result.message);
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Verify(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"), Times.Once);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoExistenciaWorkflowRetornaSuccessSinDatos_ContinuaSinCancelar()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow()
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(18) });
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>
            {
                success = true,
                message = "OK",
                errors = [],
                data = null
            });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 2);

        Assert.True(result.success);
        Assert.Equal("RAD-TEST-1", result.data?.ConsecutivoRadicado);
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Verify(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"), Times.Once);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoPreRegistroWorkflowFalla_RetornaErrorYNoRegistra()
    {
        var fx = BuildFixture();
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = false,
                message = "pre-registro workflow failed",
                errors = [new AppError { Field = "workflow", Message = "pre-registro workflow failed", Type = "Workflow" }],
                data = null
            });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.SolicitaDatosActividadInicioFlujoRepository.Setup(r => r.SolicitaDatosActividadInicioFlujoAsync(99, "WF"))
            .ReturnsAsync(new AppResponses<SolicitaDatosActividadInicioFlujo> { success = true, data = BuildActividadInicioFlujo(), errors = [] });

        var request = BuildRequest();
        request.RE_flujo_trabajo = new FlujoTrabajoRadicacionDto { id_tipo_flujo_workflow = 99 };

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(request, 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("pre-registro workflow failed", result.message);
        fx.RegistrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(), It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoNoExisteTareaWorkflow_RegistrarTareaWorkflowAsyncSeEjecuta()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow(),
                    Relaciones = []
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(18) });
        fx.SolicitaDatosActividadInicioFlujoRepository.Setup(r => r.SolicitaDatosActividadInicioFlujoAsync(99, "WF"))
            .ReturnsAsync(new AppResponses<SolicitaDatosActividadInicioFlujo> { success = true, data = BuildActividadInicioFlujo(), errors = [] });
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>
            {
                success = true,
                message = "OK",
                errors = [],
                data = new SolicitaExistenciaRadicadoRutaWorkflowDto { IdTareaWorkflow = 0 }
            });

        var request = BuildRequest();
        request.RE_flujo_trabajo = new FlujoTrabajoRadicacionDto { id_tipo_flujo_workflow = 99 };

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(request, 10, "DA", "127.0.0.1", 1);

        Assert.True(result.success);
        fx.RegistroTareaWorkflowRepository.Verify(r => r.RegistrarTareaWorkflowAsync(
            1,
            "RUTA-1",
            1,
            null,
            18,
            901,
            99,
            77,
            88,
            0,
            1,
            0,
            It.IsAny<IReadOnlyCollection<RelacionCamposRutaWorklflowDto>>(),
            It.IsAny<DateTime>(),
            "WF"), Times.Once);
        fx.RaRadEstadosModuloRadicacionRepository.Verify(r => r.ActualizaEstadoModuloRadicacio("DA", 444, 1, 321), Times.Once);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoNoHayRepositorioRegistroTareaWorkflow_RetornaErrorControlado()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.Service = new RegistrarRadicacionEntranteService(
            fx.DetalleRepo.Object, fx.RemitRepo.Object, fx.PlantillaRepo.Object, fx.RegistrarRepo.Object,
            fx.ConfiguracionPlantillaService.Object, fx.ParametrosService.Object, fx.TipoDocEntranteRepo.Object,
            fx.ValidaCamposRadicacionService.Object, fx.ValidaPreRegistroWorkflowService.Object,
            fx.SolicitaDatosActividadInicioFlujoRepository.Object, fx.RaRadEstadosModuloRadicacionRepository.Object, fx.CurrentUserService.Object,
            fx.SolicitaEstructuraRutaWorkflowService.Object, fx.SolicitaExistenciaRadicadoRutaWorkflowService.Object,
            null,
            fx.UsuarioWorkflowRepository.Object, fx.GruposWorkflowRepository.Object);
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow(),
                    Relaciones = []
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(18) });
        fx.SolicitaDatosActividadInicioFlujoRepository.Setup(r => r.SolicitaDatosActividadInicioFlujoAsync(99, "WF"))
            .ReturnsAsync(new AppResponses<SolicitaDatosActividadInicioFlujo> { success = true, data = BuildActividadInicioFlujo(), errors = [] });
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>
            {
                success = true,
                message = "OK",
                errors = [],
                data = new SolicitaExistenciaRadicadoRutaWorkflowDto { IdTareaWorkflow = 0 }
            });

        var request = BuildRequest();
        request.RE_flujo_trabajo = new FlujoTrabajoRadicacionDto { id_tipo_flujo_workflow = 99 };

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(request, 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("Repositorio RegistrarTareaWorkflowAsync requerido para registrar tarea workflow", result.message);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoActualizaEstadoModuloFalla_RetornaErrorControlado()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow(),
                    Relaciones = []
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(18) });
        fx.SolicitaDatosActividadInicioFlujoRepository.Setup(r => r.SolicitaDatosActividadInicioFlujoAsync(99, "WF"))
            .ReturnsAsync(new AppResponses<SolicitaDatosActividadInicioFlujo> { success = true, data = BuildActividadInicioFlujo(), errors = [] });
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>
            {
                success = true,
                message = "OK",
                errors = [],
                data = new SolicitaExistenciaRadicadoRutaWorkflowDto { IdTareaWorkflow = 0 }
            });
        fx.RaRadEstadosModuloRadicacionRepository.Setup(r => r.ActualizaEstadoModuloRadicacio("DA", 444, 1, 321))
            .ReturnsAsync(new AppResponses<bool>
            {
                success = false,
                message = "actualiza estado failed",
                errors = [new AppError { Field = "estado", Message = "actualiza estado failed", Type = "Repository" }],
                data = false
            });

        var request = BuildRequest();
        request.RE_flujo_trabajo = new FlujoTrabajoRadicacionDto { id_tipo_flujo_workflow = 99 };

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(request, 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("actualiza estado failed", result.message);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoFaltaClaimWorkflowParaRegistrarTarea_RetornaErrorControlado()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow(),
                    Relaciones = []
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(18) });
        fx.SolicitaDatosActividadInicioFlujoRepository.Setup(r => r.SolicitaDatosActividadInicioFlujoAsync(99, "WF"))
            .ReturnsAsync(new AppResponses<SolicitaDatosActividadInicioFlujo> { success = true, data = BuildActividadInicioFlujo(), errors = [] });
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>
            {
                success = true,
                message = "OK",
                errors = [],
                data = new SolicitaExistenciaRadicadoRutaWorkflowDto { IdTareaWorkflow = 0 }
            });
        var workflowClaimReads = 0;
        fx.CurrentUserService.Setup(s => s.GetClaimValue("defaulaliaswf"))
            .Returns(() =>
            {
                workflowClaimReads++;
                return workflowClaimReads < 5 ? "WF" : string.Empty;
            });

        var request = BuildRequest();
        request.RE_flujo_trabajo = new FlujoTrabajoRadicacionDto { id_tipo_flujo_workflow = 99 };

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(request, 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("Claim defaulaliaswf requerido para registrar tarea workflow", result.message);
        fx.RegistroTareaWorkflowRepository.Verify(r => r.RegistrarTareaWorkflowAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<IReadOnlyCollection<RelacionCamposRutaWorklflowDto>>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoYaExisteTareaWorkflow_NoRegistraNuevaTarea()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow(),
                    Relaciones = []
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(18) });
        fx.SolicitaDatosActividadInicioFlujoRepository.Setup(r => r.SolicitaDatosActividadInicioFlujoAsync(99, "WF"))
            .ReturnsAsync(new AppResponses<SolicitaDatosActividadInicioFlujo> { success = true, data = BuildActividadInicioFlujo(), errors = [] });
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>
            {
                success = true,
                message = "OK",
                errors = [],
                data = new SolicitaExistenciaRadicadoRutaWorkflowDto { IdTareaWorkflow = 222 }
            });

        var request = BuildRequest();
        request.RE_flujo_trabajo = new FlujoTrabajoRadicacionDto { id_tipo_flujo_workflow = 99 };

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(request, 10, "DA", "127.0.0.1", 1);

        Assert.True(result.success);
        fx.RegistroTareaWorkflowRepository.Verify(r => r.RegistrarTareaWorkflowAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<IReadOnlyCollection<RelacionCamposRutaWorklflowDto>>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoRegistroTareaWorkflowFalla_RetornaErrorControlado()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow(),
                    Relaciones = []
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(18) });
        fx.SolicitaDatosActividadInicioFlujoRepository.Setup(r => r.SolicitaDatosActividadInicioFlujoAsync(99, "WF"))
            .ReturnsAsync(new AppResponses<SolicitaDatosActividadInicioFlujo> { success = true, data = BuildActividadInicioFlujo(), errors = [] });
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("RAD-TEST-1", "RUTA-1", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>
            {
                success = true,
                message = "OK",
                errors = [],
                data = new SolicitaExistenciaRadicadoRutaWorkflowDto { IdTareaWorkflow = 0 }
            });
        fx.RegistroTareaWorkflowRepository.Setup(r => r.RegistrarTareaWorkflowAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<RelacionCamposRutaWorklflowDto>>(), It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync(new AppResponses<RegistroTareaWorkflowResultDto>
            {
                success = false,
                message = "registro tarea workflow failed",
                errors = [new AppError { Field = "workflow", Message = "registro tarea workflow failed", Type = "Workflow" }],
                data = null
            });

        var request = BuildRequest();
        request.RE_flujo_trabajo = new FlujoTrabajoRadicacionDto { id_tipo_flujo_workflow = 99 };

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(request, 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("registro tarea workflow failed", result.message);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoPreRegistroWorkflowNoRetornaRuta_RetornaValidacionYNoRegistra()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = null
                },
                errors = []
            });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 2);

        Assert.False(result.success);
        Assert.Equal("No existe estructura workflow valida en la prevalidacion para continuar el registro", result.message);
        fx.RegistrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(), It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoTipoEnvioWorkflowRequiereActividadInicialYNoExiste_RetornaValidacion()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 3) });
        fx.SolicitaDatosActividadInicioFlujoRepository.Setup(r => r.SolicitaDatosActividadInicioFlujoAsync(99, "WF"))
            .ReturnsAsync(new AppResponses<SolicitaDatosActividadInicioFlujo> { success = true, data = null, errors = [] });

        var request = BuildRequest();
        request.RE_flujo_trabajo = new FlujoTrabajoRadicacionDto { id_tipo_flujo_workflow = 99 };

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(request, 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("No existe actividad inicial configurada para el flujo workflow 99", result.message);
        fx.RegistrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(), It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoFaltaClaimWorkflowParaRutaInterna_RetornaValidacion()
    {
        var fx = BuildFixture();
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.CurrentUserService.Setup(s => s.GetClaimValue("defaulaliaswf")).Returns(string.Empty);

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("Claim defaulaliaswf requerido para consultar ruta workflow", result.message);
        fx.UsuarioWorkflowRepository.Verify(r => r.SolicitaEstructuraIdUsuarioWorkflowId(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        fx.RegistrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(), It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoRutaWorkflowInternaNoExiste_RetornaValidacion()
    {
        var fx = BuildFixture();
        fx.SolicitaEstructuraRutaWorkflowService.Setup(r => r.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(new AppResponses<List<SolicitaEstructuraRutaWorkflowDto>>
            {
                success = true,
                message = "OK",
                errors = [],
                data = []
            });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("No existe estructura workflow activa valida", result.message);
        fx.UsuarioWorkflowRepository.Verify(r => r.SolicitaEstructuraIdUsuarioWorkflowId(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoNoAplicaWorkflowPorTipoEnvio_RelacionWorkflowInvalidaNoBloqueaRegistro()
    {
        var fx = BuildFixture();
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 0) });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.True(result.success);
        fx.UsuarioWorkflowRepository.Verify(r => r.SolicitaEstructuraIdUsuarioWorkflowId(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoConsultaUsuarioWorkflowFallaYTipoEnvioLoRequiere_RetornaErrorControlado()
    {
        var fx = BuildFixture();
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow()
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = false, Message = "workflow lookup failed", ErrorMessage = "workflow lookup failed" });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("workflow lookup failed", result.message);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoTipoModuloEnvioRequiereActividadYGrupoNoTieneActividad_RetornaValidacion()
    {
        var fx = BuildFixture();
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow()
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901, 7) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 3) });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(7, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(0) });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("El usuario workflow no tiene relacionada una actividad workflow", result.message);
        fx.RegistrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(), It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoConsultaUsuarioWorkflowLanzaExcepcionYTipoEnvioLoRequiere_RetornaErrorControlado()
    {
        var fx = BuildFixture();
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                data = new ValidaPreRegistroWorkflowResultDto
                {
                    NombreRuta = "RUTA-1",
                    RutaWorkflow = BuildRutaWorkflow()
                },
                errors = []
            });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(utilTipoModuloEnvio: 2) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ThrowsAsync(new InvalidOperationException("workflow exploded"));

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("Error al validar usuario workflow interno", result.message);
        Assert.Contains(result.errors.Cast<AppError>(), error => error.Field == "ValidarUsuarioWorkflowInternoAsync" && error.Message.Contains("workflow exploded"));
        fx.RegistrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(), It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_NormalizaReturnRegistraRadicacionLegacy()
    {
        var fx = BuildFixture(new RegistrarRadicacionEntranteResponseDto
        {
            ConsecutivoRadicado = "RAD-LEGACY-90",
            MetadataOperativa = new Dictionary<string, object?> { ["idRadicado"] = 9001, ["idEstadoRadicado"] = 321 }
        });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901) });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.True(result.success);
        Assert.NotNull(result.data?.ReturnRegistraRadicacion);
        Assert.Equal(9001, result.data!.ReturnRegistraRadicacion.IdRadicado);
        Assert.Equal(321, result.data.ReturnRegistraRadicacion.IdEstadoRadicado);
    }

    private static Fixture BuildFixture(RegistrarRadicacionEntranteResponseDto? response = null, string moduloRegistro = "RADICACION")
    {
        var fx = new Fixture();
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55) });
        fx.PlantillaRepo.Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado> { success = true, data = BuildPlantilla(), errors = [] });
        fx.DetalleRepo.Setup(r => r.SolicitaCamposDnamicos(100, "DA"))
            .ReturnsAsync(new AppResponse<DetallePlantillaRadicado[]> { Success = true, Data = [] });
        fx.ParametrosService.Setup(s => s.SolicitaParametrosRadicados(33, 302, 55, "DA"))
            .ReturnsAsync(new AppResponses<ParametrosRadicadosDto> { success = true, data = BuildParametros() });
        fx.TipoDocEntranteRepo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante() });
        fx.ValidaCamposRadicacionService.Setup(s => s.ValidaCamposRadicacionAsync("DA", It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<List<ValidationError>?> { success = true, data = null });
        fx.ConfiguracionPlantillaService.Setup(s => s.SolicitaConfiguracionPlantillaAsync(100, 1, "DA"))
            .ReturnsAsync(new AppResponses<RaRadConfigPlantillaRadicacionDto?> { success = true, data = new RaRadConfigPlantillaRadicacionDto { Descripcion_tipo_radicacion = moduloRegistro }, errors = [] });
        fx.CurrentUserService.Setup(s => s.GetClaimValue("defaulaliaswf")).Returns("WF");
        fx.CurrentUserService.Setup(s => s.GetClaimValue("defaulalias")).Returns("DA");
        fx.SolicitaEstructuraRutaWorkflowService.Setup(r => r.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(new AppResponses<List<SolicitaEstructuraRutaWorkflowDto>>
            {
                success = true,
                message = "OK",
                errors = [],
                data = [BuildRutaWorkflow()]
            });
        fx.SolicitaExistenciaRadicadoRutaWorkflowService.Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync(It.IsAny<string>(), It.IsAny<string>(), "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>
            {
                success = true,
                message = "OK",
                errors = [],
                data = new SolicitaExistenciaRadicadoRutaWorkflowDto()
            });
        fx.RegistroTareaWorkflowRepository.Setup(r => r.RegistrarTareaWorkflowAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<RelacionCamposRutaWorklflowDto>>(), It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync(new AppResponses<RegistroTareaWorkflowResultDto>
            {
                success = true,
                message = "OK",
                errors = [],
                data = new RegistroTareaWorkflowResultDto { idTareaWorkflow = 321 }
            });
        fx.RaRadEstadosModuloRadicacionRepository.Setup(r => r.ActualizaEstadoModuloRadicacio("DA", 444, 1, 321))
            .ReturnsAsync(new AppResponses<bool>
            {
                success = true,
                message = "YES",
                errors = [],
                data = true
            });
        fx.GruposWorkflowRepository.Setup(r => r.SolicitaIdActividadRelacionadaGrupo(It.IsAny<int>(), "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow> { Success = true, Message = "YES", Data = BuildGrupoWorkflow(11) });
        fx.RegistrarRepo.Setup(r => r.RegistrarRadicacionEntranteAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 55, 10, "DA", It.IsAny<string>(), It.IsAny<int>(), moduloRegistro,
                It.IsAny<SystemPlantillaRadicado>(), It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
                It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto>
            {
                success = true,
                data = response ?? new RegistrarRadicacionEntranteResponseDto
                {
                    ConsecutivoRadicado = "RAD-TEST-1",
                    ReturnRegistraRadicacion = new ReturnRegistraRadicacionDto
                    {
                        ConsecutivoRadicado = "RAD-TEST-1",
                        IdEstadoRadicado = 444
                    }
                }
            });
        fx.Service = new RegistrarRadicacionEntranteService(
            fx.DetalleRepo.Object, fx.RemitRepo.Object, fx.PlantillaRepo.Object, fx.RegistrarRepo.Object,
            fx.ConfiguracionPlantillaService.Object, fx.ParametrosService.Object, fx.TipoDocEntranteRepo.Object,
            fx.ValidaCamposRadicacionService.Object, fx.ValidaPreRegistroWorkflowService.Object,
            fx.SolicitaDatosActividadInicioFlujoRepository.Object, fx.RaRadEstadosModuloRadicacionRepository.Object, fx.CurrentUserService.Object,
            fx.SolicitaEstructuraRutaWorkflowService.Object, fx.SolicitaExistenciaRadicadoRutaWorkflowService.Object,
            fx.RegistroTareaWorkflowRepository.Object,
            fx.UsuarioWorkflowRepository.Object, fx.GruposWorkflowRepository.Object);
        return fx;
    }

    private static RegistrarRadicacionEntranteRequestDto BuildRequest() => new()
    {
        IdPlantilla = 100,
        TipoRadicado = new TipoRadicadoEntradaDto { IdTipoRadicado = 1, TipoRadicacion = "ENTRANTE" },
        TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1 },
        Tipo_tramite = new TipoTramiteRadicacionDto { tipo_doc_entrante = 302, Descripcion = "TRAMITE" },
        Destinatario = new DestinatarioRadicacionDto { id_Remit_Dest_Int = 33 },
        Remitente = new RemitenteRadicacionDto { Nombre = "R" },
        ASUNTO = "A"
    };

    private static ParametrosRadicadosDto BuildParametros() => new()
    {
        NombreAreaRemitdest = new MiApp.DTOs.DTOs.GestorDocumental.usuario.NombreAreaRemitdestDto(),
        TipoDocEntrante = new TipoDocEntranteParametroDto { IdTipoDocEntrante = 302, DescripcionDoc = "TRAMITE", SystemPlantillaRadicadoIdPlantilla = 100, EstadoTipoDocumento = 1, FlowTipo = 1, RequiereRespuesta = 1, CodigoGabineteWorkflow = 1, TipoTramite = 1, UtilRadicacionSimple = 1 },
        IdSedeNombre = new MiApp.DTOs.DTOs.GestorDocumental.Sede.IdSedeNombreDto()
    };

    private static SystemPlantillaRadicado BuildPlantilla() => new()
    {
        id_Plantilla = 100,
        Nombre_Plantilla_Radicado = "ra_plantilla_100",
        Tipo_Plantilla = "RAD",
        Fecha_Creacion = DateTime.UtcNow,
        Estado_Plantilla = 1,
        Consecutivo_Rad = 10,
        Consecutivo_CodBarra = 10,
        util_activo_plantilla_flujo = 1,
        util_default_radicado = 1
    };

    private static TipoDocEntrante BuildTipoDocEntrante(int utilTipoModuloEnvio = 1) => new()
    {
        id_Tipo_Doc_Entrante = 302,
        Descripcion_Doc = "TRAMITE",
        system_plantilla_radicado_id_plantilla = 100,
        estado_tipo_documento = 1,
        flow_tipo = 1,
        requiere_respuesta = 1,
        codigo_gabinete_workflow = 1,
        resp_correo_fisico_electronico = 1,
        id_ruta = 1,
        tipo_tramite = 1,
        estado_ruta_open_close = 1,
        activo_modulo_respuesta = 1,
        util_tipo_modulo_envio = utilTipoModuloEnvio,
        tipo_tramite_entrante_saliente = 1,
        id_gabinete = 1,
        util_radicacion_simple = 1
    };

    private static RemitDestInterno BuildRemitDestInterno(int idUsuarioRadicacion, int relacionWorkflow = 0) => new()
    {
        Id_Remit_Dest_Int = 1,
        Areas_Dep_Radicacion_Id_Areas_Dep = 1,
        Relacion_Id_Usuario_Radicacion = idUsuarioRadicacion,
        Relacion_Workflow = relacionWorkflow,
        Nombre_Remitente = string.Empty,
        Cargo_Remite = string.Empty,
        Login_Usuario = string.Empty,
        Pasw_Usuario = string.Empty,
        Estado_Usuario = 1,
        Correo_Electronico = string.Empty,
        Telefono_Usuario = string.Empty,
        Firma_Usuario = [],
        Cambio_Clave = 0,
        Pasw_Encript = string.Empty,
        Empresa_Gestion_Documental_Id_Empresa = 1,
        Id_Sedes_Empresa = 1,
        Relacion_Da = 1,
        Relacion_Workflow_Login = string.Empty,
        Relacion_Da_Login = string.Empty,
        Relacion_Login_Radicacion = string.Empty,
        Estado_Usuario_Para_Gestion_Respuesta = 1,
        Estado_Usuario_Para_Gestion_Pqr = 1,
        Identificacion = string.Empty,
        Direccion = string.Empty,
        Relacion_Workflow_Extend = 0,
        Relacion_Workflow_Login_Extend = string.Empty,
        estado_radicacion_correspondencia = 1,
        estado_reasignacion_correspondencia = 1
    };

    private static SolicitaEstructuraRutaWorkflowDto BuildRutaWorkflow() => new()
    {
        id_Ruta = 1,
        Nombre_Ruta = "RUTA-1"
    };

    private static UsuarioWorkflow BuildUsuarioWorkflow(int idUsuarioWorkflow, int idGrupoWorkflow = 1) => new()
    {
        idU_suario = idUsuarioWorkflow,
        login_Usuario = $"wf{idUsuarioWorkflow}",
        Pasword_Usuario = string.Empty,
        Nombre_Usuario = "WF",
        Cargo_Usuario = "ROL",
        Fecha_Creacion = DateTime.UtcNow,
        Cambio_Clave = 0,
        Intervalo_Usuario = 1,
        pasw_encript = string.Empty,
        numerodias_pasw = 1,
        Grupos_Workflow_Rutas_Workflow_id_Ruta = 1,
        Grupos_Workflow_Id_Grupo = idGrupoWorkflow,
        Relacion_Gestion = 1,
        Relacion_Gestion_Login = string.Empty,
        ESTADO_USUARIO = 1,
        Permiso_precausar = 0,
        Permiso_registrar_pago = 0,
        estado_envio_correo = 0,
        estado_balanceo_grupo = 0,
        UTIL_ASIGNA_TAREA = 0,
        Correo_Usuario = string.Empty,
        fecha_limite_acceso = DateTime.UtcNow.AddDays(1)
      };

    private static GruposWorkflow BuildGrupoWorkflow(int idActividad) => new()
    {
        Id_Grupo = 7,
        Rutas_Workflow_id_Ruta = 1,
        Nombre_Grupo = "GRUPO-1",
        Fecha_Creacion = DateTime.UtcNow,
        Estado_Grupo = 1,
        id_Actividad = idActividad
    };

    private static SolicitaDatosActividadInicioFlujo BuildActividadInicioFlujo() => new()
    {
        IdRegistroActividadFlujoTrabajo = 66,
        IdActividadFlujoTrabajo = 77,
        IdUsuarioWorkflowFlujoTrabajo = 88
    };

    private sealed class Fixture
    {
        public Mock<IDetallePlantillaRadicadoR> DetalleRepo { get; } = new();
        public Mock<IRemitDestInternoR> RemitRepo { get; } = new();
        public Mock<ISystemPlantillaRadicadoR> PlantillaRepo { get; } = new();
        public Mock<IRegistrarRadicacionEntranteRepository> RegistrarRepo { get; } = new();
        public Mock<ISolicitaParametrosRadicadosService> ParametrosService { get; } = new();
        public Mock<ITipoDocEntranteR> TipoDocEntranteRepo { get; } = new();
        public Mock<IValidaCamposRadicacionService> ValidaCamposRadicacionService { get; } = new();
        public Mock<IConfiguracionPlantillaService> ConfiguracionPlantillaService { get; } = new();
        public Mock<IValidaPreRegistroWorkflowService> ValidaPreRegistroWorkflowService { get; } = new();
        public Mock<ISolicitaDatosActividadInicioFlujoRepository> SolicitaDatosActividadInicioFlujoRepository { get; } = new();
        public Mock<IRaRadEstadosModuloRadicacionR> RaRadEstadosModuloRadicacionRepository { get; } = new();
        public Mock<ICurrentUserService> CurrentUserService { get; } = new();
        public Mock<ISolicitaEstructuraRutaWorkflowService> SolicitaEstructuraRutaWorkflowService { get; } = new();
        public Mock<ISolicitaExistenciaRadicadoRutaWorkflowService> SolicitaExistenciaRadicadoRutaWorkflowService { get; } = new();
        public Mock<IRegistroRadicadoTareaWorkflowRepository> RegistroTareaWorkflowRepository { get; } = new();
        public Mock<IUsuarioWorkflowR> UsuarioWorkflowRepository { get; } = new();
        public Mock<IGruposWorkflowR> GruposWorkflowRepository { get; } = new();
        public RegistrarRadicacionEntranteService Service { get; set; } = null!;
    }
}
