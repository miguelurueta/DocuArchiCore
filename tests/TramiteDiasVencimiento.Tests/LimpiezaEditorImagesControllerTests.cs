using DocuArchi.Api.Controllers.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Editor;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class LimpiezaEditorImagesControllerTests
{
    [Fact]
    public async Task Cleanup_CuandoClaimInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string>
                {
                    success = false,
                    message = "No claim",
                    data = string.Empty,
                    errors = []
                }
            });

        var service = new Mock<IServiceLimpiezaEditorImages>();
        var controller = new LimpiezaEditorImagesController(claimValidation.Object, service.Object);

        var action = await controller.Cleanup(new LimpiezaEditorImagesRequestDto());

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Cleanup_CuandoServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceLimpiezaEditorImages>();
        service.Setup(s => s.LimpiaImagenesHuerfanasAsync(It.IsAny<LimpiezaEditorImagesRequestDto>(), "db1"))
            .ReturnsAsync(new AppResponses<LimpiezaEditorImagesResponseDto?>
            {
                success = true,
                message = "OK",
                data = new LimpiezaEditorImagesResponseDto { TotalOrphansDetected = 0, TotalMarkedAsDeleted = 0, DryRun = true },
                errors = []
            });

        var controller = new LimpiezaEditorImagesController(claimValidation.Object, service.Object);

        var action = await controller.Cleanup(new LimpiezaEditorImagesRequestDto { DryRun = true, OlderThanMinutes = 60, Limit = 10 });

        Assert.IsType<OkObjectResult>(action.Result);
    }

    [Fact]
    public async Task DryRun_FuerzaDryRun_True()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceLimpiezaEditorImages>();
        service.Setup(s => s.LimpiaImagenesHuerfanasAsync(It.Is<LimpiezaEditorImagesRequestDto>(r => r.DryRun), "db1"))
            .ReturnsAsync(new AppResponses<LimpiezaEditorImagesResponseDto?>
            {
                success = true,
                message = "OK",
                data = new LimpiezaEditorImagesResponseDto { TotalOrphansDetected = 1, TotalMarkedAsDeleted = 0, DryRun = true, ImageIds = [1] },
                errors = []
            });

        var controller = new LimpiezaEditorImagesController(claimValidation.Object, service.Object);

        var action = await controller.DryRun(new LimpiezaEditorImagesRequestDto { DryRun = false, OlderThanMinutes = 0, Limit = 1 });

        Assert.IsType<OkObjectResult>(action.Result);
    }
}
