using DocuArchi.Api.Controllers.Radicacion.Configuracion;
using MiApp.DTOs.DTOs.Radicacion.Configuracion;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.Radicacion.Configuracion;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ConfiguracionPlantillaControllerContractTests
{
    [Fact]
    public async Task SolicitaConfiguracionPlantilla_CuandoClaimYServicioOk_RetornaOk()
    {
        var claim = BuildClaimService("DA");
        var service = new Mock<IConfiguracionPlantillaService>();
        service
            .Setup(s => s.SolicitaConfiguracionPlantillaAsync(67, 1, "DA"))
            .ReturnsAsync(new AppResponses<RaRadConfigPlantillaRadicacionDto?>
            {
                success = true,
                message = "OK",
                data = new RaRadConfigPlantillaRadicacionDto
                {
                    id_rad_config_plantilla_radicacion = 1,
                    system_plantilla_radicado_id_Plantilla = 67,
                    Tipo_radicacion_plantilla = 1,
                    Descripcion_tipo_radicacion = "Externa",
                    util_notificacion_remitente = 1,
                    util_notificacion_destinatario = 0,
                    util_valida_restriccion_radicacion = 1
                },
                errors = []
            });

        var controller = new ConfiguracionPlantillaController(claim.Object, service.Object);
        var result = await controller.SolicitaConfiguracionPlantilla(67, 1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<RaRadConfigPlantillaRadicacionDto?>>(ok.Value);
        Assert.True(payload.success);
        Assert.Equal(67, payload.data!.system_plantilla_radicado_id_Plantilla);
    }

    [Fact]
    public async Task SolicitaConfiguracionPlantilla_CuandoFaltaAliasClaim_RetornaBadRequest()
    {
        var claim = new Mock<IClaimValidationService>();
        claim
            .Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "sin alias", data = string.Empty }
            });

        var controller = new ConfiguracionPlantillaController(claim.Object, Mock.Of<IConfiguracionPlantillaService>());
        var result = await controller.SolicitaConfiguracionPlantilla(67, 1);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    private static Mock<IClaimValidationService> BuildClaimService(string alias)
    {
        var claimService = new Mock<IClaimValidationService>();
        claimService
            .Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = alias });
        return claimService;
    }
}

