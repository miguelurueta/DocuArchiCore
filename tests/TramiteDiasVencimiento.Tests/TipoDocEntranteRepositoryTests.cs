using MiApp.Models.Models.Radicacion.TipoTramite;
using MiApp.DTOs.DTOs.Errors;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class TipoDocEntranteRepositoryTests
{
    [Fact]
    public async Task SolicitaEstructuraTipoDoEntrante_CuandoHayDatos_RetornaYes()
    {
        var dapper = new Mock<IDapperCrudEngine>();

        dapper.Setup(d => d.GetAllAsync<TipoDocEntrante>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<TipoDocEntrante>
            {
                Success = true,
                Message = "OK",
                Data =
                [
                    new TipoDocEntrante
                    {
                        id_Tipo_Doc_Entrante = 302,
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
                    }
                ]
            });

        var repository = new TipoDocEntranteR(dapper.Object);

        var result = await repository.SolicitaEstructuraTipoDoEntrante(302, "DA");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(302, result.data!.id_Tipo_Doc_Entrante);
    }

    [Fact]
    public async Task SolicitaEstructuraTipoDoEntrante_CuandoNoHayDatos_RetornaError()
    {
        var dapper = new Mock<IDapperCrudEngine>();

        dapper.Setup(d => d.GetAllAsync<TipoDocEntrante>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<TipoDocEntrante>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        var repository = new TipoDocEntranteR(dapper.Object);

        var result = await repository.SolicitaEstructuraTipoDoEntrante(999, "DA");

        Assert.False(result.success);
        Assert.Contains("Imposible encontrar estructura tipo documento entrante", result.message);
        Assert.Null(result.data);
        Assert.NotNull(result.errors);
        Assert.Contains(result.errors!, e => e is AppError err && err.Field == "idTipoDocEntrante");
    }

    [Fact]
    public async Task SolicitaEstructuraTipoDoEntrante_ConstruyeQueryEsperada()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        QueryOptions? captured = null;

        dapper.Setup(d => d.GetAllAsync<TipoDocEntrante>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(q => captured = q)
            .ReturnsAsync(new QueryResult<TipoDocEntrante>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        var repository = new TipoDocEntranteR(dapper.Object);

        await repository.SolicitaEstructuraTipoDoEntrante(302, "DA");

        Assert.NotNull(captured);
        Assert.Equal("tipo_doc_entrante", captured!.TableName);
        Assert.Equal("DA", captured.DefaultAlias);
        Assert.NotNull(captured.Filters);
        Assert.Equal(302, captured.Filters!["id_Tipo_Doc_Entrante"]);
        Assert.Equal(1, captured.Limit);
    }
}
