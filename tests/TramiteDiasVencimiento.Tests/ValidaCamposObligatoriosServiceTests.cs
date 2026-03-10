using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Services.Service.Radicacion.Tramite;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ValidaCamposObligatoriosServiceTests
{
    [Fact]
    public async Task ValidaCamposObligatoriosAsync_CuandoDatosValidos_RetornaSuccess()
    {
        var service = new ValidaCamposObligatoriosService();
        var request = BuildValidRequest();
        var detalle = new List<DetallePlantillaRadicado>
        {
            BuildDetalle("CampoDinamicoObligatorio", 1, 1)
        };
        request.Campos.Add(new CampoRadicacionDto
        {
            NombreCampo = "CampoDinamicoObligatorio",
            Valor = "OK"
        });

        var result = await service.ValidaCamposObligatoriosAsync(request, "DA", detalle);

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!);
    }

    [Fact]
    public async Task ValidaCamposObligatoriosAsync_CuandoSinRegistrosCoincidentes_RetornaSinResultados()
    {
        var service = new ValidaCamposObligatoriosService();
        var request = BuildValidRequest();

        var result = await service.ValidaCamposObligatoriosAsync(request, "DA", []);

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task ValidaCamposObligatoriosAsync_CuandoFaltanCamposGeneradosBackend_NoFalla()
    {
        var service = new ValidaCamposObligatoriosService();
        var request = BuildValidRequest();
        request.Campos = request.Campos
            .Where(c => c.NombreCampo != "Usuario_Radicador_id_usuario"
                && c.NombreCampo != "Consecutivo_Rad"
                && c.NombreCampo != "Consecutivo_CodBarra"
                && c.NombreCampo != "Fecha_Radicado"
                && c.NombreCampo != "Codigo_Sede"
                && c.NombreCampo != "Id_area_remit_dest_interno"
                && c.NombreCampo != "Area_remit_dest_interno"
                && c.NombreCampo != "CARGO_DESTINATARIO")
            .ToList();

        var result = await service.ValidaCamposObligatoriosAsync(request, "DA", []);

        Assert.True(result.success);
    }

    [Fact]
    public async Task ValidaCamposObligatoriosAsync_CuandoDescripcionVieneEnTipoTramite_NoExigeDescripcionDocumentoEnCampos()
    {
        var service = new ValidaCamposObligatoriosService();
        var request = BuildValidRequest();
        request.Tipo_tramite = new TipoTramiteRadicacionDto
        {
            Descripcion = "DERECHOS DE PETECION",
            tipo_doc_entrante = 1
        };
        request.Campos = request.Campos
            .Where(c => c.NombreCampo != "Descripcion_Documento")
            .ToList();

        var result = await service.ValidaCamposObligatoriosAsync(request, "DA", []);

        Assert.True(result.success);
    }

    [Fact]
    public async Task ValidaCamposObligatoriosAsync_CuandoExcepcion_RetornaErrorControlado()
    {
        var service = new ValidaCamposObligatoriosService();
        var request = BuildValidRequest();
        request.Campos = null!;

        var result = await service.ValidaCamposObligatoriosAsync(request, "DA", []);

        Assert.False(result.success);
        Assert.Equal("Validacion fallida", result.message);
        Assert.NotNull(result.errors);
    }

    [Fact]
    public async Task ValidaCamposObligatoriosAsync_CuandoFaltaCampoFijo_UsaAliasEnMensaje()
    {
        var service = new ValidaCamposObligatoriosService();
        var request = BuildValidRequest();
        request.FECHALIMITERESPUESTA = null!;
        request.Campos = request.Campos
            .Where(c => c.NombreCampo != "FECHALIMITERESPUESTA")
            .ToList();

        var result = await service.ValidaCamposObligatoriosAsync(request, "DA", []);

        Assert.False(result.success);
        Assert.NotNull(result.data);
        Assert.Contains(result.data!, e =>
            e.Field == "FECHALIMITERESPUESTA"
            && e.Message == "Fecha Límite Respuesta: valor inválido.");
    }

    [Fact]
    public async Task ValidaCamposObligatoriosAsync_CuandoFaltaCampoDinamico_UsaAliasCampoPlantillaEnMensaje()
    {
        var service = new ValidaCamposObligatoriosService();
        var request = BuildValidRequest();
        var detalle = new List<DetallePlantillaRadicado>
        {
            BuildDetalle("CampoDinamicoObligatorio", 1, 1, "Nro Oficio")
        };

        var result = await service.ValidaCamposObligatoriosAsync(request, "DA", detalle);

        Assert.False(result.success);
        Assert.NotNull(result.data);
        Assert.Contains(result.data!, e =>
            e.Field == "CampoDinamicoObligatorio"
            && e.Message == "Nro Oficio: valor inválido.");
    }

    private static RegistrarRadicacionEntranteRequestDto BuildValidRequest()
    {
        var request = new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = "Asunto",
            ANEXOS_COR = "Anexos",
            FECHALIMITERESPUESTA = "2026-12-31",
            numeroFolios = 3,
            Remitente = new RemitenteRadicacionDto { Nombre = "Remitente", id_Dest_Ext = 123 },
            Destinatario = new DestinatarioRadicacionDto { Destinatario = "Dest", id_Remit_Dest_Int = 456 },
            TipoRadicado = new TipoRadicadoEntradaDto { TipoRadicacion = "ENTRANTE", IdTipoRadicado = 2 },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1, TipoPlantillaRadicado = "RAD" },
            Campos =
            [
                Campo("Usuario_Radicador_id_usuario", "10"),
                Campo("Consecutivo_Rad", "RAD-1"),
                Campo("Consecutivo_CodBarra", "CB-1"),
                Campo("Fecha_Radicado", "2026-03-09"),
                Campo("Descripcion_Documento", "Descripcion"),
                Campo("Codigo_Sede", "1"),
                Campo("Id_area_remit_dest_interno", "20"),
                Campo("Area_remit_dest_interno", "AREA"),
                Campo("CARGO_DESTINATARIO", "ANALISTA")
            ]
        };

        return request;
    }

    private static CampoRadicacionDto Campo(string nombre, string valor)
    {
        return new CampoRadicacionDto { NombreCampo = nombre, Valor = valor };
    }

    private static DetallePlantillaRadicado BuildDetalle(string campoPlantilla, int campoObligatorio, int estadoCampo, string? aliasCampo = null)
    {
        return new DetallePlantillaRadicado
        {
            System_Plantilla_Radicado_id_Plantilla = 100,
            Campo_Plantilla = campoPlantilla,
            Tipo_Campo = "VARCHAR",
            Comportamiento_Campo = "DIGITACION",
            Alias_Campo = aliasCampo ?? campoPlantilla,
            Orden_Campo = 1,
            Estado_Campo = estadoCampo,
            Descripcion_Campo = campoPlantilla,
            Campo_Obligatorio = campoObligatorio,
            Campo_rad_interno = 1,
            Campo_rad_externo = 1,
            Campo_rad_simple = 1,
            tam_campo = 100,
            id_detalle_plantilla_radicado = 1,
            TagSesion = "TEST"
        };
    }
}
