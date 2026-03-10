using MiApp.DTOs.DTOs.GestorDocumental.Sede;
using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.GestorDocumental.usuario;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.Radicacion.TipoTramite;
using MiApp.Repository.Repositorio.GestorDocumental.Sede;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.PlantillaRadicado;
using MiApp.Services.Service.Usuario;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class SolicitaParametrosRadicadosServiceTests
{
    [Fact]
    public async Task SolicitaParametrosRadicados_CuandoTodoOk_RetornaYes()
    {
        var remitDestInternoRepository = new Mock<IRemitDestInternoR>();
        var tipoDocEntranteRepository = new Mock<ITipoDocEntranteR>();
        var sedeEmpresaRepository = new Mock<ISedeEmpresaR>();

        remitDestInternoRepository
            .Setup(r => r.SolicitaIdAreaNombreAreaDestinatario(11, "DA"))
            .ReturnsAsync(new AppResponses<NombreAreaRemitdestDto>
            {
                success = true,
                message = "YES",
                data = new NombreAreaRemitdestDto { IdArea = 5, NombreArea = "AREA TEST" },
                errors = []
            });

        tipoDocEntranteRepository
            .Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante>
            {
                success = true,
                message = "YES",
                data = BuildTipoDocEntrante(302),
                errors = []
            });

        sedeEmpresaRepository
            .Setup(r => r.RetornaIdNombreSedeEmpresa(17, "DA"))
            .ReturnsAsync(new AppResponses<IdSedeNombreDto>
            {
                success = true,
                message = "YES",
                data = new IdSedeNombreDto { IdSede = 4, NombreSede = "SEDE PRINCIPAL" },
                errors = []
            });
        var service = new SolicitaParametrosRadicadosService(
            remitDestInternoRepository.Object,
            tipoDocEntranteRepository.Object,
            sedeEmpresaRepository.Object);

        var result = await service.SolicitaParametrosRadicados(11, 302, 17, "DA");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(5, result.data!.NombreAreaRemitdest.IdArea);
        Assert.Equal(302, result.data.TipoDocEntrante.IdTipoDocEntrante);
        Assert.Equal(4, result.data.IdSedeNombre.IdSede);
    }

    [Fact]
    public async Task SolicitaParametrosRadicados_CuandoAreaFalla_RetornaError()
    {
        var remitDestInternoRepository = new Mock<IRemitDestInternoR>();
        var tipoDocEntranteRepository = new Mock<ITipoDocEntranteR>();
        var sedeEmpresaRepository = new Mock<ISedeEmpresaR>();

        remitDestInternoRepository
            .Setup(r => r.SolicitaIdAreaNombreAreaDestinatario(11, "DA"))
            .ReturnsAsync(new AppResponses<NombreAreaRemitdestDto>
            {
                success = false,
                message = "No se encontró área asociada al destinatario interno.",
                data = null!,
                errors = []
            });
        tipoDocEntranteRepository
            .Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante>
            {
                success = true,
                message = "YES",
                data = BuildTipoDocEntrante(302),
                errors = []
            });
        sedeEmpresaRepository
            .Setup(r => r.RetornaIdNombreSedeEmpresa(17, "DA"))
            .ReturnsAsync(new AppResponses<IdSedeNombreDto>
            {
                success = true,
                message = "YES",
                data = new IdSedeNombreDto { IdSede = 4, NombreSede = "SEDE PRINCIPAL" },
                errors = []
            });

        var service = new SolicitaParametrosRadicadosService(
            remitDestInternoRepository.Object,
            tipoDocEntranteRepository.Object,
            sedeEmpresaRepository.Object);

        var result = await service.SolicitaParametrosRadicados(11, 302, 17, "DA");

        Assert.False(result.success);
        Assert.Equal("No se encontró área asociada al destinatario interno.", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task SolicitaParametrosRadicados_CuandoIdTipoDocEntranteInvalido_RetornaValidacion()
    {
        var remitDestInternoRepository = new Mock<IRemitDestInternoR>();
        var tipoDocEntranteRepository = new Mock<ITipoDocEntranteR>();
        var sedeEmpresaRepository = new Mock<ISedeEmpresaR>();

        var service = new SolicitaParametrosRadicadosService(
            remitDestInternoRepository.Object,
            tipoDocEntranteRepository.Object,
            sedeEmpresaRepository.Object);

        var result = await service.SolicitaParametrosRadicados(11, 0, 17, "DA");

        Assert.False(result.success);
        Assert.Equal("Id de tipo documento entrante requerido", result.message);
        Assert.NotNull(result.errors);
        Assert.Contains(result.errors!, e => e is AppError err && err.Field == "idTipoDocEntrante");
    }

    private static TipoDocEntrante BuildTipoDocEntrante(int idTipoDocEntrante)
    {
        return new TipoDocEntrante
        {
            id_Tipo_Doc_Entrante = idTipoDocEntrante,
            Descripcion_Doc = "SOLICITUD DE AFILIACION",
            system_plantilla_radicado_id_plantilla = 67,
            estado_tipo_documento = 1,
            flow_tipo = 1,
            requiere_respuesta = 1,
            codigo_gabinete_workflow = 1,
            resp_correo_fisico_electronico = 1,
            id_ruta = 1,
            tipo_tramite = 1,
            estado_ruta_open_close = 0,
            obliga_exp_radicado = 0,
            activo_modulo_respuesta = 1,
            util_tipo_modulo_envio = 0,
            util_producion_documental = 0,
            tipo_tramite_entrante_saliente = 1,
            wf_copia_doc_expediente_actualiza_exped_gabinete = 0,
            wf_auto_vincula_doc_expediente_actualiza_exped_gabinete = 0,
            wf_copia_doc_expediente_produc_actualiza_exped_gabinete = 0,
            util_auto_vincula_migracion = 0,
            id_gabinete = 1,
            util_radicacion_simple = 1,
            util_nivel_padre_auto_vincula = 0,
            util_opcion_auto_vincula = 0,
            util_Estado_Crea_ExpedienteSII = 0,
            util_Estado_Multiple_expedienteSII = 0
        };
    }

}
