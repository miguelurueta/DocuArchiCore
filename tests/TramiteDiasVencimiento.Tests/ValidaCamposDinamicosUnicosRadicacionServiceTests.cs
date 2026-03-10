using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ValidaCamposDinamicosUnicosRadicacionServiceTests
{
    [Fact]
    public async Task ValidaCamposDinamicosUnicosRadicacionAsync_CuandoDatosValidos_RetornaSuccess()
    {
        var repo = new Mock<IValidaCamposDinamicosUnicosRadicacionRepository>();
        repo.Setup(r => r.SolicitaCoincidenciasCamposUnicosAsync(100, "DA", It.IsAny<IReadOnlyDictionary<string, string>>()))
            .ReturnsAsync(new AppResponses<Dictionary<string, int>?>
            {
                success = true,
                message = "Sin resultados",
                data = null
            });

        var service = new ValidaCamposDinamicosUnicosRadicacionService(repo.Object);
        var result = await service.ValidaCamposDinamicosUnicosRadicacionAsync(
            BuildRequest("NIT-123"),
            "DA",
            BuildDetalleUnico());

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task ValidaCamposDinamicosUnicosRadicacionAsync_CuandoSinRegistrosCoincidentes_RetornaSinResultados()
    {
        var repo = new Mock<IValidaCamposDinamicosUnicosRadicacionRepository>();
        repo.Setup(r => r.SolicitaCoincidenciasCamposUnicosAsync(100, "DA", It.IsAny<IReadOnlyDictionary<string, string>>()))
            .ReturnsAsync(new AppResponses<Dictionary<string, int>?>
            {
                success = true,
                message = "Sin resultados",
                data = null
            });

        var service = new ValidaCamposDinamicosUnicosRadicacionService(repo.Object);
        var result = await service.ValidaCamposDinamicosUnicosRadicacionAsync(
            BuildRequest(""),
            "DA",
            BuildDetalleUnico());

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task ValidaCamposDinamicosUnicosRadicacionAsync_CuandoExcepcion_RetornaErrorControlado()
    {
        var repo = new Mock<IValidaCamposDinamicosUnicosRadicacionRepository>();
        repo.Setup(r => r.SolicitaCoincidenciasCamposUnicosAsync(100, "DA", It.IsAny<IReadOnlyDictionary<string, string>>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var service = new ValidaCamposDinamicosUnicosRadicacionService(repo.Object);
        var result = await service.ValidaCamposDinamicosUnicosRadicacionAsync(
            BuildRequest("NIT-123"),
            "DA",
            BuildDetalleUnico());

        Assert.False(result.success);
        Assert.Equal("Error validando campos dinamicos unicos", result.message);
        Assert.NotNull(result.errors);
    }

    [Fact]
    public async Task ValidaCamposDinamicosUnicosRadicacionAsync_CuandoDuplicado_UsaAliasEnMensaje()
    {
        var repo = new Mock<IValidaCamposDinamicosUnicosRadicacionRepository>();
        repo.Setup(r => r.SolicitaCoincidenciasCamposUnicosAsync(100, "DA", It.IsAny<IReadOnlyDictionary<string, string>>()))
            .ReturnsAsync(new AppResponses<Dictionary<string, int>?>
            {
                success = true,
                message = "OK",
                data = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    ["CampoIdentificador"] = 1
                }
            });

        var service = new ValidaCamposDinamicosUnicosRadicacionService(repo.Object);
        var result = await service.ValidaCamposDinamicosUnicosRadicacionAsync(
            BuildRequest("NIT-123"),
            "DA",
            BuildDetalleUnico("NIT del Solicitante"));

        Assert.False(result.success);
        Assert.NotNull(result.data);
        Assert.Contains(result.data!, e =>
            e.Field == "CampoIdentificador"
            && e.Message == "Campo NIT del Solicitante: valor existente.");
    }

    private static RegistrarRadicacionEntranteRequestDto BuildRequest(string valorUnico)
    {
        var dto = new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = "Asunto",
            Remitente = new RemitenteRadicacionDto { Nombre = "Remitente", id_Dest_Ext = 1 },
            Destinatario = new DestinatarioRadicacionDto { Destinatario = "Dest", id_Remit_Dest_Int = 2 },
            TipoRadicado = new TipoRadicadoEntradaDto { IdTipoRadicado = 1, TipoRadicacion = "ENTRANTE" },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1, TipoPlantillaRadicado = "RAD" },
            Campos = []
        };

        if (!string.IsNullOrWhiteSpace(valorUnico))
        {
            dto.Campos.Add(new CampoRadicacionDto
            {
                NombreCampo = "CampoIdentificador",
                Valor = valorUnico
            });
        }

        return dto;
    }

    private static IReadOnlyCollection<DetallePlantillaRadicado> BuildDetalleUnico(string aliasCampo = "CampoIdentificador")
    {
        return
        [
            new DetallePlantillaRadicado
            {
                System_Plantilla_Radicado_id_Plantilla = 100,
                Campo_Plantilla = "CampoIdentificador",
                Tipo_Campo = "VARCHAR",
                Comportamiento_Campo = "UNICO",
                Alias_Campo = aliasCampo,
                Orden_Campo = 1,
                Estado_Campo = 1,
                Descripcion_Campo = "Campo unico",
                Campo_Obligatorio = 1,
                Campo_rad_interno = 1,
                Campo_rad_externo = 1,
                Campo_rad_simple = 1,
                tam_campo = 50,
                id_detalle_plantilla_radicado = 1,
                TagSesion = "UNICO"
            }
        ];
    }
}
