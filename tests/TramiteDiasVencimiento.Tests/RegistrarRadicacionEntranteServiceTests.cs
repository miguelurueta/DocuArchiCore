using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Radicacion.Configuracion;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.usuario;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Models.Models.Radicacion.TipoTramite;
using MiApp.Models.Models.Workflow.Usuario;
using MiApp.Repository.ErrorController;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Repository.Repositorio.Workflow.Flujo;
using MiApp.Repository.Repositorio.Workflow.usuario;
using MiApp.Services.Service.Radicacion.Configuracion;
using MiApp.Services.Service.Radicacion.PlantillaRadicado;
using MiApp.Services.Service.Radicacion.RelacionCamposRutaWorklflow;
using MiApp.Services.Service.Radicacion.Tramite;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.Usuario;
using Moq;
using System.Linq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RegistrarRadicacionEntranteServiceTests
{
    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoRequestValidoNoWorkflow_ConsultaUsuarioWorkflowYRegistra()
    {
        var fx = BuildFixture();
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.CurrentUserService.Setup(s => s.GetClaimValue("defaulaliaswf")).Returns("WF");
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901) });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.True(result.success);
        fx.UsuarioWorkflowRepository.Verify(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"), Times.Once);
        fx.RegistrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 55, 10, "DA", It.IsAny<string>(), 1, "RADICACION",
            It.IsAny<SystemPlantillaRadicado>(), It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
            It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoTipoModuloEsWorkflow_NoConsultaUsuarioWorkflowInterno()
    {
        var fx = BuildFixture(moduloRegistro: "CORRESPONDENCIA");
        fx.ValidaPreRegistroWorkflowService.Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto> { success = true, data = new ValidaPreRegistroWorkflowResultDto(), errors = [] });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 2);

        Assert.True(result.success);
        fx.UsuarioWorkflowRepository.Verify(r => r.SolicitaEstructuraIdUsuarioWorkflowId(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoFaltaClaimWorkflow_RetornaValidacion()
    {
        var fx = BuildFixture();
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.CurrentUserService.Setup(s => s.GetClaimValue("defaulaliaswf")).Returns(string.Empty);

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("Claim defaulaliaswf requerido para consultar usuario workflow", result.message);
        fx.RegistrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(), It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoRelacionWorkflowEsInvalida_RetornaValidacion()
    {
        var fx = BuildFixture();
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 0) });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("Destinatario sin Relacion_Workflow valida", result.message);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoConsultaUsuarioWorkflowFalla_RetornaErrorControlado()
    {
        var fx = BuildFixture();
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.CurrentUserService.Setup(s => s.GetClaimValue("defaulaliaswf")).Returns("WF");
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = false, Message = "workflow lookup failed", ErrorMessage = "workflow lookup failed" });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("workflow lookup failed", result.message);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoConsultaUsuarioWorkflowLanzaExcepcion_RetornaErrorControlado()
    {
        var fx = BuildFixture();
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.CurrentUserService.Setup(s => s.GetClaimValue("defaulaliaswf")).Returns("WF");
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
            MetadataOperativa = new Dictionary<string, object?> { ["idRadicado"] = 9001 }
        });
        fx.RemitRepo.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(33, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55, 901) });
        fx.CurrentUserService.Setup(s => s.GetClaimValue("defaulaliaswf")).Returns("WF");
        fx.UsuarioWorkflowRepository.Setup(r => r.SolicitaEstructuraIdUsuarioWorkflowId(901, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow> { Success = true, Message = "YES", Data = BuildUsuarioWorkflow(901) });

        var result = await fx.Service.RegistrarRadicacionEntranteAsync(BuildRequest(), 10, "DA", "127.0.0.1", 1);

        Assert.True(result.success);
        Assert.NotNull(result.data?.ReturnRegistraRadicacion);
        Assert.Equal(9001, result.data!.ReturnRegistraRadicacion.IdRadicado);
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
        fx.RegistrarRepo.Setup(r => r.RegistrarRadicacionEntranteAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 55, 10, "DA", It.IsAny<string>(), It.IsAny<int>(), moduloRegistro,
                It.IsAny<SystemPlantillaRadicado>(), It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
                It.IsAny<ParametrosRadicadosDto>(), It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto> { success = true, data = response ?? new RegistrarRadicacionEntranteResponseDto { ConsecutivoRadicado = "RAD-TEST-1" } });
        fx.Service = new RegistrarRadicacionEntranteService(
            fx.DetalleRepo.Object, fx.RemitRepo.Object, fx.PlantillaRepo.Object, fx.RegistrarRepo.Object,
            fx.ConfiguracionPlantillaService.Object, fx.ParametrosService.Object, fx.TipoDocEntranteRepo.Object,
            fx.ValidaCamposRadicacionService.Object, fx.ValidaPreRegistroWorkflowService.Object,
            fx.SolicitaDatosActividadInicioFlujoRepository.Object, fx.CurrentUserService.Object, fx.UsuarioWorkflowRepository.Object);
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

    private static TipoDocEntrante BuildTipoDocEntrante() => new()
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
        util_tipo_modulo_envio = 1,
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

    private static UsuarioWorkflow BuildUsuarioWorkflow(int idUsuarioWorkflow) => new()
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
        Grupos_Workflow_Id_Grupo = 1,
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
        public Mock<ICurrentUserService> CurrentUserService { get; } = new();
        public Mock<IUsuarioWorkflowR> UsuarioWorkflowRepository { get; } = new();
        public RegistrarRadicacionEntranteService Service { get; set; } = null!;
    }
}
