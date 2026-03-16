using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.Models.Models.Radicacion.RelacionCamposRutaWorklflow;
using MiApp.Services.Service.Radicacion.RelacionCamposRutaWorklflow;
using MiApp.Services.Service.Radicacion.Tramite;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class AsingacionValoresDatosRadicadoRutaWorklflowTests
{
    [Fact]
    public async Task AsignaDatosRadicacionAsync_CuandoHayCoincidencias_RetornaDatosAsignados()
    {
        var service = new AsingacionValoresDatosRadicadoRutaWorklflow();
        var relaciones = new List<RelacionCamposRutaWorklflow>
        {
            new() { NombreCampoPlantilla = "Asunto", NombreCampoRuta = "WF_ASUNTO" },
            new() { NombreCampoPlantilla = "id_tipo_flujo_workflow", NombreCampoRuta = "WF_ID_FLUJO" },
            new() { NombreCampoPlantilla = "CampoDinamico", NombreCampoRuta = "WF_CAMPO_DINAMICO" }
        };

        var result = await service.AsignaDatosRadicacionAsync(relaciones, BuildRequest());

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Collection(result.data!,
            item => Assert.Equal("Prueba workflow", item.DatoCampoPlantilla),
            item => Assert.Equal("77", item.DatoCampoPlantilla),
            item => Assert.Equal("valor dinamico", item.DatoCampoPlantilla));
    }

    [Fact]
    public async Task AsignaDatosRadicacionAsync_CuandoNoExisteCampo_DejaDatoVacio()
    {
        var service = new AsingacionValoresDatosRadicadoRutaWorklflow();
        var relaciones = new List<RelacionCamposRutaWorklflow>
        {
            new RelacionCamposRutaWorklflow
            {
                NombreCampoPlantilla = "CampoInexistente",
                NombreCampoRuta = "WF_CAMPO"
            }
        };

        var result = await service.AsignaDatosRadicacionAsync(relaciones, BuildRequest());

        Assert.True(result.success);
        Assert.Single(result.data!);
        Assert.Equal(string.Empty, result.data![0].DatoCampoPlantilla);
    }

    [Fact]
    public async Task AsignaDatosRadicacionAsync_CuandoListaVacia_RetornaSinResultados()
    {
        var service = new AsingacionValoresDatosRadicadoRutaWorklflow();

        var result = await service.AsignaDatosRadicacionAsync(
            new List<RelacionCamposRutaWorklflow>(),
            BuildRequest());

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!);
    }

    [Fact]
    public async Task AsignaDatosRadicacionAsync_CuandoRequestEsNulo_RetornaValidationError()
    {
        var service = new AsingacionValoresDatosRadicadoRutaWorklflow();

        var result = await service.AsignaDatosRadicacionAsync(
            new List<RelacionCamposRutaWorklflow>
            {
                new() { NombreCampoPlantilla = "Asunto" }
            },
            null);

        Assert.False(result.success);
        Assert.Equal("Request requerido", result.message);
        Assert.NotNull(result.errors);
    }

    private static RegistrarRadicacionEntranteRequestDto BuildRequest()
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = "Prueba workflow",
            ANEXOS_COR = "Adjunto",
            FECHALIMITERESPUESTA = "2026-03-15",
            numeroFolios = 3,
            Remitente = new RemitenteRadicacionDto
            {
                Nombre = "Empresa remitente",
                id_Dest_Ext = 10
            },
            Destinatario = new DestinatarioRadicacionDto
            {
                Destinatario = "Analista workflow",
                id_Remit_Dest_Int = 22
            },
            Tipo_tramite = new TipoTramiteRadicacionDto
            {
                Descripcion = "TRAMITE DE PRUEBA",
                tipo_doc_entrante = 4
            },
            RE_flujo_trabajo = new FlujoTrabajoRadicacionDto
            {
                NombreFlujo = "Flujo principal",
                id_tipo_flujo_workflow = 77
            },
            TipoRadicado = new TipoRadicadoEntradaDto
            {
                TipoRadicacion = "ENTRANTE",
                IdTipoRadicado = 2
            },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto
            {
                TipoPlantillaRadicado = "Workflow",
                IdTipoPlantillaRdicado = 5
            },
            expedienteRelacionado = new ExpedienteRelacionadoDto
            {
                Expediente = "EXP-001",
                idExpediente = 90
            },
            Campos =
            [
                new CampoRadicacionDto
                {
                    NombreCampo = "CampoDinamico",
                    Valor = "valor dinamico"
                },
                new CampoRadicacionDto
                {
                    NombreCampo = "CITARADICADO",
                    Valor = "RAD-001"
                }
            ]
        };
    }
}
