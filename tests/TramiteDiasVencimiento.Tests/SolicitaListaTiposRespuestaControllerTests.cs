using DocuArchi.Api.Controllers.GestionCorrespondencia.TiposRespuesta;
using MiApp.DTOs.DTOs.Common;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestionCorrespondencia.TiposRespuesta;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaListaTiposRespuestaControllerTests
{
    [Fact]
    public async Task Get_CuandoClaimYServicioOk_RetornaOk()
    {
        var claimService = new Mock<IClaimValidationService>();
        claimService
            .Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "DA" });

        var service = new Mock<IServiceSolicitaListaTiposRespuesta>();
        service
            .Setup(s => s.SolicitaListaTiposRespuestaAsync("DA", It.IsAny<string>()))
            .ReturnsAsync(new AppResponses<List<ResponseDropdownDto>>
            {
                success = true,
                message = "YES",
                data =
                [
                    new ResponseDropdownDto { Id = 1, Descripcion = "Positiva" }
                ],
                errors = []
            });

        var controller = new SolicitaListaTiposRespuestaController(
            claimService.Object,
            service.Object,
            Mock.Of<ILogger<SolicitaListaTiposRespuestaController>>());

        var result = await controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<List<ResponseDropdownDto>>>(ok.Value);
        Assert.True(payload.success);
        Assert.Single(payload.data);
    }

    [Fact]
    public async Task Get_CuandoFaltaClaimAlias_RetornaBadRequest()
    {
        var claimService = new Mock<IClaimValidationService>();
        claimService
            .Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "sin alias", data = string.Empty }
            });

        var controller = new SolicitaListaTiposRespuestaController(
            claimService.Object,
            Mock.Of<IServiceSolicitaListaTiposRespuesta>(),
            Mock.Of<ILogger<SolicitaListaTiposRespuestaController>>());

        var result = await controller.Get();

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
