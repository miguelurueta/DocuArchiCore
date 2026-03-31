using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.BandejaCorrespondencia;
using MiApp.Models.Models.GestorDocumental.usuario;
using MiApp.Models.Models.Workflow.Grupo;
using MiApp.Models.Models.Workflow.RutaTrabajo;
using MiApp.Models.Models.Workflow.Usuario;
using MiApp.Repository.ErrorController;
using MiApp.Repository.Repositorio.Workflow.Grupo;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using MiApp.Repository.Repositorio.Workflow.usuario;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.Usuario;
using MiApp.Services.Service.Workflow.BandejaCorrespondencia;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class WorkflowInboxContextResolverServiceTests
{
    [Fact]
    public async Task ResolveAsync_CuandoIdUsuarioGestionEsInvalido_RetornaValidacion()
    {
        var service = CreateService();

        var result = await service.ResolveAsync(0);

        Assert.False(result.success);
        Assert.Equal("IdUsuarioGestion requerido", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Field == "idUsuarioGestion");
    }

    [Fact]
    public async Task ResolveAsync_CuandoAliasGestionNoExiste_RetornaValidacion()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(service => service.GetClaimValue("defaulalias")).Returns((string?)null);
        currentUser.Setup(service => service.GetClaimValue("defaulaliaswf")).Returns("WF");

        var service = CreateService(currentUserService: currentUser);

        var result = await service.ResolveAsync(10);

        Assert.False(result.success);
        Assert.Equal("Claim defaulalias requerido para consultar usuario de gestion", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Field == "defaulalias");
    }

    [Fact]
    public async Task ResolveAsync_CuandoAliasWorkflowNoExiste_RetornaValidacion()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(service => service.GetClaimValue("defaulalias")).Returns("DA");
        currentUser.Setup(service => service.GetClaimValue("defaulaliaswf")).Returns((string?)null);

        var service = CreateService(currentUserService: currentUser);

        var result = await service.ResolveAsync(10);

        Assert.False(result.success);
        Assert.Equal("Claim defaulaliaswf requerido para consultar contexto workflow", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Field == "defaulaliaswf");
    }

    [Fact]
    public async Task ResolveAsync_CuandoContextoEsValido_RetornaDtoResuelto()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var usuarioRepo = new Mock<IUsuarioWorkflowR>();
        var rutaRepo = new Mock<ISolicitaEstructuraRutaWorkflowRepository>();
        var grupoRepo = new Mock<IGruposWorkflowR>();

        remitRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "YES",
                data = new RemitDestInterno
                {
                    Id_Remit_Dest_Int = 10,
                    Areas_Dep_Radicacion_Id_Areas_Dep = 1,
                    Nombre_Remitente = "Gestion",
                    Cargo_Remite = "Analista",
                    Relacion_Workflow = 91,
                    Login_Usuario = "gestion",
                    Pasw_Usuario = "secret",
                    Estado_Usuario = 1,
                    Correo_Electronico = "gestion@contasoft.local",
                    Telefono_Usuario = "3000000000",
                    Firma_Usuario = [],
                    Cambio_Clave = 0,
                    Pasw_Encript = "enc",
                    Empresa_Gestion_Documental_Id_Empresa = 1,
                    Id_Sedes_Empresa = 1,
                    Relacion_Da = 0,
                    Relacion_Workflow_Login = "wf",
                    Relacion_Da_Login = "da",
                    Relacion_Id_Usuario_Radicacion = 0,
                    Relacion_Login_Radicacion = "rad",
                    Estado_Usuario_Para_Gestion_Respuesta = 1,
                    Estado_Usuario_Para_Gestion_Pqr = 1,
                    Identificacion = "123",
                    Direccion = "Dir",
                    Relacion_Workflow_Extend = 0,
                    Relacion_Workflow_Login_Extend = string.Empty,
                    estado_radicacion_correspondencia = 1,
                    estado_reasignacion_correspondencia = 1
                },
                errors = []
            });

        usuarioRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioWorkflowId(91, "WF"))
            .ReturnsAsync(BuildUsuarioWorkflowResponse(14, 7));

        rutaRepo
            .Setup(repo => repo.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(BuildRutaResponse(7, "RUTA_WORKFLOW_A"));

        grupoRepo
            .Setup(repo => repo.SolicitaEstructuraGrupoWorkflow(14, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow>
            {
                Success = true,
                Message = "YES",
                Data = new GruposWorkflow
                {
                    Id_Grupo = 14,
                    Rutas_Workflow_id_Ruta = 7,
                    Nombre_Grupo = "Grupo A",
                    Fecha_Creacion = new DateTime(2026, 3, 31),
                    Estado_Grupo = 1,
                    id_Actividad = 44
                }
            });

        var service = CreateService(remitRepo, usuarioRepo, rutaRepo, grupoRepo);

        var result = await service.ResolveAsync(10);

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(91, result.data.IdUsuarioWorkflow);
        Assert.Equal(14, result.data.IdGrupoWorkflow);
        Assert.Equal("RUTA_WORKFLOW_A", result.data.NombreRuta);
        Assert.Equal(44, result.data.IdActividad);
    }

    [Fact]
    public async Task ResolveAsync_CuandoFallaRepositorioUsuarioGestion_RetornaErrorControlado()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        remitRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = false,
                message = "No existe",
                data = null!,
                errors = [new AppError { Field = "id_Remit_Dest_Int", Message = "No existe", Type = "Field" }]
            });

        var service = CreateService(remitRepo: remitRepo);

        var result = await service.ResolveAsync(10);

        Assert.False(result.success);
        Assert.Equal("No fue posible resolver el usuario de gestion.", result.message);
    }

    [Fact]
    public async Task ResolveAsync_CuandoFallaRepositorioUsuarioWorkflow_RetornaErrorControlado()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var usuarioRepo = new Mock<IUsuarioWorkflowR>();

        remitRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(BuildUsuarioGestionResponse(91));

        usuarioRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioWorkflowId(91, "WF"))
            .ReturnsAsync(new AppResponse<UsuarioWorkflow>
            {
                Success = false,
                Message = "No existe",
                Errors = [new AppError { Field = "idU_suario", Message = "No existe", Type = "Field" }]
            });

        var service = CreateService(remitRepo, usuarioRepo: usuarioRepo);

        var result = await service.ResolveAsync(10);

        Assert.False(result.success);
        Assert.Equal("No fue posible resolver el usuario workflow.", result.message);
    }

    [Fact]
    public async Task ResolveAsync_CuandoFallaRepositorioRutas_RetornaErrorControlado()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var usuarioRepo = new Mock<IUsuarioWorkflowR>();
        var rutaRepo = new Mock<ISolicitaEstructuraRutaWorkflowRepository>();

        remitRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(BuildUsuarioGestionResponse(91));

        usuarioRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioWorkflowId(91, "WF"))
            .ReturnsAsync(BuildUsuarioWorkflowResponse(14, 7));

        rutaRepo
            .Setup(repo => repo.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(new AppResponses<List<RutasWorkflow>?>
            {
                success = false,
                message = "Sin respuesta",
                data = null,
                errors = [new AppError { Field = "defaultDbAlias", Message = "Sin respuesta", Type = "Field" }]
            });

        var service = CreateService(remitRepo, usuarioRepo, rutaRepo: rutaRepo);

        var result = await service.ResolveAsync(10);

        Assert.False(result.success);
        Assert.Equal("No fue posible resolver la ruta workflow.", result.message);
    }

    [Fact]
    public async Task ResolveAsync_CuandoFallaRepositorioGrupo_RetornaErrorControlado()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var usuarioRepo = new Mock<IUsuarioWorkflowR>();
        var rutaRepo = new Mock<ISolicitaEstructuraRutaWorkflowRepository>();
        var grupoRepo = new Mock<IGruposWorkflowR>();

        remitRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(BuildUsuarioGestionResponse(91));

        usuarioRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioWorkflowId(91, "WF"))
            .ReturnsAsync(BuildUsuarioWorkflowResponse(14, 7));

        rutaRepo
            .Setup(repo => repo.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(BuildRutaResponse(7, "RUTA_A"));

        grupoRepo
            .Setup(repo => repo.SolicitaEstructuraGrupoWorkflow(14, "WF"))
            .ReturnsAsync(new AppResponse<GruposWorkflow>
            {
                Success = false,
                Message = "No existe",
                Errors = [new AppError { Field = "Id_Grupo", Message = "No existe", Type = "Field" }]
            });

        var service = CreateService(remitRepo, usuarioRepo, rutaRepo, grupoRepo);

        var result = await service.ResolveAsync(10);

        Assert.False(result.success);
        Assert.Equal("No fue posible resolver el grupo workflow.", result.message);
    }

    [Fact]
    public async Task ResolveAsync_CuandoContextoEsIncompleto_RetornaErrorControlado()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var usuarioRepo = new Mock<IUsuarioWorkflowR>();
        var rutaRepo = new Mock<ISolicitaEstructuraRutaWorkflowRepository>();

        remitRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(BuildUsuarioGestionResponse(91));

        usuarioRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioWorkflowId(91, "WF"))
            .ReturnsAsync(BuildUsuarioWorkflowResponse(14, 99));

        rutaRepo
            .Setup(repo => repo.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(BuildRutaResponse(7, "RUTA_A"));

        var service = CreateService(remitRepo, usuarioRepo, rutaRepo);

        var result = await service.ResolveAsync(10);

        Assert.False(result.success);
        Assert.Equal("No fue posible completar el contexto workflow con una ruta valida.", result.message);
    }

    [Fact]
    public async Task ResolveAsync_CuandoRepositorioLanzaExcepcion_RetornaErrorControlado()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        remitRepo
            .Setup(repo => repo.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var service = CreateService(remitRepo: remitRepo);

        var result = await service.ResolveAsync(10);

        Assert.False(result.success);
        Assert.Equal("Error al resolver el contexto workflow de la bandeja.", result.message);
        Assert.Contains(result.errors!.OfType<AppError>(), error => error.Type == "Exception");
    }

    private static WorkflowInboxContextResolverService CreateService(
        Mock<IRemitDestInternoR>? remitRepo = null,
        Mock<IUsuarioWorkflowR>? usuarioRepo = null,
        Mock<ISolicitaEstructuraRutaWorkflowRepository>? rutaRepo = null,
        Mock<IGruposWorkflowR>? grupoRepo = null,
        Mock<ICurrentUserService>? currentUserService = null)
    {
        if (currentUserService == null)
        {
            currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(service => service.GetClaimValue("defaulalias")).Returns("DA");
            currentUserService.Setup(service => service.GetClaimValue("defaulaliaswf")).Returns("WF");
        }

        return new WorkflowInboxContextResolverService(
            (remitRepo ?? new Mock<IRemitDestInternoR>()).Object,
            (usuarioRepo ?? new Mock<IUsuarioWorkflowR>()).Object,
            (rutaRepo ?? new Mock<ISolicitaEstructuraRutaWorkflowRepository>()).Object,
            (grupoRepo ?? new Mock<IGruposWorkflowR>()).Object,
            currentUserService.Object);
    }

    private static AppResponses<RemitDestInterno> BuildUsuarioGestionResponse(int idUsuarioWorkflow)
    {
        return new AppResponses<RemitDestInterno>
        {
            success = true,
            message = "YES",
            data = new RemitDestInterno
            {
                Id_Remit_Dest_Int = 10,
                Areas_Dep_Radicacion_Id_Areas_Dep = 1,
                Nombre_Remitente = "Gestion",
                Cargo_Remite = "Analista",
                Relacion_Workflow = idUsuarioWorkflow,
                Login_Usuario = "gestion",
                Pasw_Usuario = "secret",
                Estado_Usuario = 1,
                Correo_Electronico = "gestion@contasoft.local",
                Telefono_Usuario = "3000000000",
                Firma_Usuario = [],
                Cambio_Clave = 0,
                Pasw_Encript = "enc",
                Empresa_Gestion_Documental_Id_Empresa = 1,
                Id_Sedes_Empresa = 1,
                Relacion_Da = 0,
                Relacion_Workflow_Login = "wf",
                Relacion_Da_Login = "da",
                Relacion_Id_Usuario_Radicacion = 0,
                Relacion_Login_Radicacion = "rad",
                Estado_Usuario_Para_Gestion_Respuesta = 1,
                Estado_Usuario_Para_Gestion_Pqr = 1,
                Identificacion = "123",
                Direccion = "Dir",
                Relacion_Workflow_Extend = 0,
                Relacion_Workflow_Login_Extend = string.Empty,
                estado_radicacion_correspondencia = 1,
                estado_reasignacion_correspondencia = 1
            },
            errors = []
        };
    }

    private static AppResponse<UsuarioWorkflow> BuildUsuarioWorkflowResponse(int idGrupoWorkflow, int idRutaWorkflow)
    {
        return new AppResponse<UsuarioWorkflow>
        {
            Success = true,
            Message = "YES",
            Data = new UsuarioWorkflow
            {
                idU_suario = 91,
                Grupos_Workflow_Id_Grupo = idGrupoWorkflow,
                Grupos_Workflow_Rutas_Workflow_id_Ruta = idRutaWorkflow,
                login_Usuario = "wf",
                Pasword_Usuario = "x",
                Nombre_Usuario = "Workflow",
                Cargo_Usuario = "Aux",
                Fecha_Creacion = new DateTime(2026, 3, 31),
                Cambio_Clave = 0,
                Intervalo_Usuario = 0,
                pasw_encript = "x",
                numerodias_pasw = 0,
                Relacion_Gestion = 10,
                Relacion_Gestion_Login = "gestion",
                ESTADO_USUARIO = 1,
                Permiso_precausar = 0,
                Permiso_registrar_pago = 0,
                estado_envio_correo = 0,
                estado_balanceo_grupo = 0,
                UTIL_ASIGNA_TAREA = 0
            }
        };
    }

    private static AppResponses<List<RutasWorkflow>?> BuildRutaResponse(int idRuta, string nombreRuta)
    {
        return new AppResponses<List<RutasWorkflow>?>
        {
            success = true,
            message = "YES",
            data =
            [
                new RutasWorkflow
                {
                    id_Ruta = idRuta,
                    Nombre_Ruta = nombreRuta,
                    Descripcion_Ruta = "Ruta",
                    Fecha_Creacion = new DateTime(2026, 3, 31),
                    Estado_Ruta = 1,
                    Archivo_Plantilla = [],
                    Ruta_Archivo_Server = "/tmp"
                }
            ],
            errors = []
        };
    }
}
