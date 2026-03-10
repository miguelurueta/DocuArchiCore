using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Repository.ErrorController;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ValidaCamposRadicacionServiceTests
{
    [Fact]
    public async Task ValidaCamposRadicacionAsync_CuandoDatosValidos_RetornaSuccess()
    {
        var service = BuildService(
            Success("OK", []),
            Success("OK", []),
            Success("Sin resultados", null));

        var result = await service.ValidaCamposRadicacionAsync("DA", BuildRequest(), BuildDetalle());

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!);
    }

    [Fact]
    public async Task ValidaCamposRadicacionAsync_CuandoSinResultados_RetornaSinResultados()
    {
        var service = BuildService(
            Success("Sin resultados", null),
            Success("Sin resultados", null),
            Success("Sin resultados", null));

        var result = await service.ValidaCamposRadicacionAsync("DA", BuildRequest(), BuildDetalle());

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task ValidaCamposRadicacionAsync_CuandoExcepcion_RetornaErrorControlado()
    {
        var obligatorios = new Mock<IValidaCamposObligatoriosService>();
        var dimension = new Mock<IValidaDimensionCamposService>();
        var unicos = new Mock<IValidaCamposDinamicosUnicosRadicacionService>();

        obligatorios.Setup(s => s.ValidaCamposObligatoriosAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                "DA",
                It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var service = new ValidaCamposRadicacionService(obligatorios.Object, dimension.Object, unicos.Object);
        var result = await service.ValidaCamposRadicacionAsync("DA", BuildRequest(), BuildDetalle());

        Assert.False(result.success);
        Assert.Equal("Error validando campos de radicacion", result.message);
        Assert.NotNull(result.errors);
    }

    private static ValidaCamposRadicacionService BuildService(
        AppResponses<List<ValidationError>?> obligatoriosResult,
        AppResponses<List<ValidationError>?> dimensionResult,
        AppResponses<List<ValidationError>?> unicosResult)
    {
        var obligatorios = new Mock<IValidaCamposObligatoriosService>();
        var dimension = new Mock<IValidaDimensionCamposService>();
        var unicos = new Mock<IValidaCamposDinamicosUnicosRadicacionService>();

        obligatorios.Setup(s => s.ValidaCamposObligatoriosAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                "DA",
                It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(obligatoriosResult);

        dimension.Setup(s => s.ValidaDimensionCamposAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                "DA",
                It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(dimensionResult);

        unicos.Setup(s => s.ValidaCamposDinamicosUnicosRadicacionAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                "DA",
                It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(unicosResult);

        return new ValidaCamposRadicacionService(obligatorios.Object, dimension.Object, unicos.Object);
    }

    private static AppResponses<List<ValidationError>?> Success(string message, List<ValidationError>? data)
    {
        return new AppResponses<List<ValidationError>?>
        {
            success = true,
            message = message,
            errors = [],
            data = data
        };
    }

    private static RegistrarRadicacionEntranteRequestDto BuildRequest()
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = "Asunto",
            ANEXOS_COR = "Anexos",
            FECHALIMITERESPUESTA = "2026-12-31",
            numeroFolios = 1,
            Remitente = new RemitenteRadicacionDto { Nombre = "Remitente", id_Dest_Ext = 1 },
            Destinatario = new DestinatarioRadicacionDto { Destinatario = "Destinatario", id_Remit_Dest_Int = 2 },
            TipoRadicado = new TipoRadicadoEntradaDto { IdTipoRadicado = 1, TipoRadicacion = "ENTRANTE" },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1, TipoPlantillaRadicado = "RAD" },
            Campos =
            [
                new CampoRadicacionDto { NombreCampo = "Descripcion_Documento", Valor = "Descripcion valida" }
            ]
        };
    }

    private static IReadOnlyCollection<DetallePlantillaRadicado> BuildDetalle()
    {
        return
        [
            new DetallePlantillaRadicado
            {
                System_Plantilla_Radicado_id_Plantilla = 100,
                Campo_Plantilla = "CampoDinamico",
                Tipo_Campo = "VARCHAR",
                Comportamiento_Campo = "DIGITACION",
                Alias_Campo = "CampoDinamico",
                Orden_Campo = 1,
                Estado_Campo = 1,
                Descripcion_Campo = "Campo dinamico",
                Campo_Obligatorio = 0,
                Campo_rad_interno = 1,
                Campo_rad_externo = 1,
                Campo_rad_simple = 1,
                tam_campo = 50,
                id_detalle_plantilla_radicado = 1,
                TagSesion = "TEST"
            }
        ];
    }
}
