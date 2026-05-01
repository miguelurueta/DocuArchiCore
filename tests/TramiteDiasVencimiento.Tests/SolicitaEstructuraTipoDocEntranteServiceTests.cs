using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.Radicacion.TipoTramite;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class SolicitaEstructuraTipoDocEntranteServiceTests
{
    [Fact]
    public async Task SolicitaEstructuraTipoDocEntranteAsync_CuandoTodoOk_RetornaYesConFlags()
    {
        var repo = new Mock<ITipoDocEntranteR>();
        repo.Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante>
            {
                success = true,
                message = "YES",
                data = BuildTipoDocEntrante(),
                errors = []
            });

        var service = new SolicitaEstructuraTipoDocEntranteService(repo.Object);
        var result = await service.SolicitaEstructuraTipoDocEntranteAsync(302, "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(302, result.data!.id_Tipo_Doc_Entrante);
        Assert.Equal(1, result.data.util_envio_correo_certificado);
        Assert.Equal(0, result.data.util_firma_digital_protocolo_respuesta);
        Assert.Equal(1, result.data.util_agrega_digital_protocolo_respuesta);
    }

    [Fact]
    public async Task SolicitaEstructuraTipoDocEntranteAsync_CuandoIdInvalido_RetornaError()
    {
        var repo = new Mock<ITipoDocEntranteR>();
        var service = new SolicitaEstructuraTipoDocEntranteService(repo.Object);

        var result = await service.SolicitaEstructuraTipoDocEntranteAsync(0, "DA");

        Assert.False(result.success);
        Assert.Contains("requerido", result.message, StringComparison.OrdinalIgnoreCase);
    }

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
        obliga_exp_radicado = 0,
        activo_modulo_respuesta = 1,
        util_tipo_modulo_envio = 1,
        util_producion_documental = 0,
        util_envio_correo_certificado = 1,
        util_firma_digital_protocolo_respuesta = 0,
        util_agrega_digital_protocolo_respuesta = 1,
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
