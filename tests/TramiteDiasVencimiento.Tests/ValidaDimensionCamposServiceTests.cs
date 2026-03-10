using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ValidaDimensionCamposServiceTests
{
    [Fact]
    public async Task ValidaDimensionCamposAsync_CuandoDatosValidos_RetornaSuccess()
    {
        var repo = new Mock<IValidaDimensionCamposRepository>();
        repo.Setup(r => r.SolicitaLongitudesCamposAsync(100, "DA", It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<Dictionary<string, int>?>
            {
                success = true,
                message = "OK",
                data = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Asunto"] = 240,
                    ["CampoDinamico"] = 10
                }
            });

        var service = new ValidaDimensionCamposService(repo.Object);
        var request = BuildRequest();
        request.Campos.Add(new CampoRadicacionDto { NombreCampo = "CampoDinamico", Valor = "1234567890" });

        var result = await service.ValidaDimensionCamposAsync(request, "DA", []);

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!);
    }

    [Fact]
    public async Task ValidaDimensionCamposAsync_CuandoSinResultados_RetornaSinResultados()
    {
        var repo = new Mock<IValidaDimensionCamposRepository>();
        repo.Setup(r => r.SolicitaLongitudesCamposAsync(100, "DA", It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<Dictionary<string, int>?>
            {
                success = true,
                message = "Sin resultados",
                data = null
            });

        var service = new ValidaDimensionCamposService(repo.Object);
        var result = await service.ValidaDimensionCamposAsync(BuildRequest(), "DA", []);

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task ValidaDimensionCamposAsync_CuandoExcepcion_RetornaErrorControlado()
    {
        var repo = new Mock<IValidaDimensionCamposRepository>();
        repo.Setup(r => r.SolicitaLongitudesCamposAsync(100, "DA", It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var service = new ValidaDimensionCamposService(repo.Object);
        var result = await service.ValidaDimensionCamposAsync(BuildRequest(), "DA", []);

        Assert.False(result.success);
        Assert.Equal("Error validando dimension de campos", result.message);
        Assert.NotNull(result.errors);
    }

    [Fact]
    public async Task ValidaDimensionCamposAsync_CuandoDescripcionVieneEnTipoTramite_ValidaLongitudContraDescripcionDocumento()
    {
        var repo = new Mock<IValidaDimensionCamposRepository>();
        repo.Setup(r => r.SolicitaLongitudesCamposAsync(100, "DA", It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<Dictionary<string, int>?>
            {
                success = true,
                message = "OK",
                data = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Descripcion_Documento"] = 10
                }
            });

        var service = new ValidaDimensionCamposService(repo.Object);
        var request = BuildRequest();
        request.Campos = [];
        request.Tipo_tramite = new TipoTramiteRadicacionDto
        {
            Descripcion = "DERECHOS DE PETECION",
            tipo_doc_entrante = 1
        };

        var result = await service.ValidaDimensionCamposAsync(request, "DA", []);

        Assert.False(result.success);
        Assert.Equal("Validacion fallida", result.message);
    }

    [Fact]
    public async Task ValidaDimensionCamposAsync_CuandoSuperaLongitudCampoFijo_UsaAliasEnMensaje()
    {
        var repo = new Mock<IValidaDimensionCamposRepository>();
        repo.Setup(r => r.SolicitaLongitudesCamposAsync(100, "DA", It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<Dictionary<string, int>?>
            {
                success = true,
                message = "OK",
                data = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    ["FECHALIMITERESPUESTA"] = 5
                }
            });

        var service = new ValidaDimensionCamposService(repo.Object);
        var request = BuildRequest();
        request.FECHALIMITERESPUESTA = "2026-12-31";

        var result = await service.ValidaDimensionCamposAsync(request, "DA", []);

        Assert.False(result.success);
        Assert.NotNull(result.data);
        Assert.Contains(result.data!, e =>
            e.Field == "FECHALIMITERESPUESTA"
            && e.Message.Contains("Fecha Límite Respuesta"));
    }

    private static RegistrarRadicacionEntranteRequestDto BuildRequest()
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = "Asunto corto",
            ANEXOS_COR = "A",
            FECHALIMITERESPUESTA = "2026-12-31",
            numeroFolios = 1,
            Remitente = new RemitenteRadicacionDto { Nombre = "Remitente", id_Dest_Ext = 1 },
            Destinatario = new DestinatarioRadicacionDto { Destinatario = "Destino", id_Remit_Dest_Int = 2 },
            TipoRadicado = new TipoRadicadoEntradaDto { IdTipoRadicado = 3, TipoRadicacion = "ENTRANTE" },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1, TipoPlantillaRadicado = "RAD" },
            Campos =
            [
                new CampoRadicacionDto { NombreCampo = "Descripcion_Documento", Valor = "Desc" }
            ]
        };
    }
}
