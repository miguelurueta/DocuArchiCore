using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ValidaTipoCamposServiceTests
{
    [Fact]
    public async Task ValidaTipoCamposAsync_CuandoDatosValidos_RetornaSuccess()
    {
        var repo = new Mock<IValidaTipoCamposRepository>();
        repo.Setup(r => r.SolicitaTiposCamposAsync(100, "DA", It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<Dictionary<string, string>?>
            {
                success = true,
                message = "OK",
                data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["CampoNumero"] = "int",
                    ["CampoFecha"] = "date",
                    ["Asunto"] = "varchar"
                }
            });

        var service = new ValidaTipoCamposService(repo.Object);
        var result = await service.ValidaTipoCamposAsync(BuildRequest("12", "2026-03-10"), "DA", BuildDetalle());

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!);
    }

    [Fact]
    public async Task ValidaTipoCamposAsync_CuandoSinResultados_RetornaSinResultados()
    {
        var repo = new Mock<IValidaTipoCamposRepository>();
        repo.Setup(r => r.SolicitaTiposCamposAsync(100, "DA", It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<Dictionary<string, string>?>
            {
                success = true,
                message = "Sin resultados",
                data = null
            });

        var service = new ValidaTipoCamposService(repo.Object);
        var result = await service.ValidaTipoCamposAsync(BuildRequest("12", "2026-03-10"), "DA", BuildDetalle());

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task ValidaTipoCamposAsync_CuandoTipoInvalido_RetornaErrorConAlias()
    {
        var repo = new Mock<IValidaTipoCamposRepository>();
        repo.Setup(r => r.SolicitaTiposCamposAsync(100, "DA", It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<Dictionary<string, string>?>
            {
                success = true,
                message = "OK",
                data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["CampoNumero"] = "int"
                }
            });

        var service = new ValidaTipoCamposService(repo.Object);
        var result = await service.ValidaTipoCamposAsync(BuildRequest("abc", "2026-03-10"), "DA", BuildDetalle());

        Assert.False(result.success);
        Assert.Equal("Validacion fallida", result.message);
        Assert.NotNull(result.data);
        Assert.Contains(result.data!, e =>
            e.Field == "CampoNumero" &&
            e.Message == "Campo Número de Oficio: formato no compatible.");
    }

    [Fact]
    public async Task ValidaTipoCamposAsync_CuandoExcepcion_RetornaErrorControlado()
    {
        var repo = new Mock<IValidaTipoCamposRepository>();
        repo.Setup(r => r.SolicitaTiposCamposAsync(100, "DA", It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var service = new ValidaTipoCamposService(repo.Object);
        var result = await service.ValidaTipoCamposAsync(BuildRequest("12", "2026-03-10"), "DA", BuildDetalle());

        Assert.False(result.success);
        Assert.Equal("Error validando tipo de campos", result.message);
        Assert.NotNull(result.errors);
    }

    private static RegistrarRadicacionEntranteRequestDto BuildRequest(string campoNumero, string campoFecha)
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = "Asunto",
            ANEXOS_COR = "Anexo",
            FECHALIMITERESPUESTA = "2026-12-31",
            numeroFolios = 1,
            Remitente = new RemitenteRadicacionDto { Nombre = "Remitente", id_Dest_Ext = 1 },
            Destinatario = new DestinatarioRadicacionDto { Destinatario = "Dest", id_Remit_Dest_Int = 2 },
            TipoRadicado = new TipoRadicadoEntradaDto { TipoRadicacion = "ENTRANTE", IdTipoRadicado = 1 },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1, TipoPlantillaRadicado = "RAD" },
            Campos =
            [
                new CampoRadicacionDto { NombreCampo = "CampoNumero", Valor = campoNumero },
                new CampoRadicacionDto { NombreCampo = "CampoFecha", Valor = campoFecha }
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
                Campo_Plantilla = "CampoNumero",
                Tipo_Campo = "INT",
                Comportamiento_Campo = "DIGITACION",
                Alias_Campo = "Número de Oficio",
                Orden_Campo = 1,
                Estado_Campo = 1,
                Descripcion_Campo = "Campo numero",
                Campo_Obligatorio = 0,
                Campo_rad_interno = 1,
                Campo_rad_externo = 1,
                Campo_rad_simple = 1,
                tam_campo = 10,
                id_detalle_plantilla_radicado = 1,
                TagSesion = "TEST"
            },
            new DetallePlantillaRadicado
            {
                System_Plantilla_Radicado_id_Plantilla = 100,
                Campo_Plantilla = "CampoFecha",
                Tipo_Campo = "DATE",
                Comportamiento_Campo = "DIGITACION",
                Alias_Campo = "Fecha de Documento",
                Orden_Campo = 2,
                Estado_Campo = 1,
                Descripcion_Campo = "Campo fecha",
                Campo_Obligatorio = 0,
                Campo_rad_interno = 1,
                Campo_rad_externo = 1,
                Campo_rad_simple = 1,
                tam_campo = 10,
                id_detalle_plantilla_radicado = 2,
                TagSesion = "TEST"
            }
        ];
    }
}
