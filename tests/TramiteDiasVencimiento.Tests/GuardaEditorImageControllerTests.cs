using DocuArchi.Api.Controllers.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Editor;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class GuardaEditorImageControllerTests
{
    [Fact]
    public async Task GuardarImagen_CuandoClaimInvalido_RetornaBadRequest()
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

        var service = new Mock<IServiceGuardaEditorImage>();
        var controller = new GuardaEditorImageController(claimValidation.Object, service.Object);

        var file = new Mock<IFormFile>().Object;
        var action = await controller.GuardarImagen(file);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GuardarImagen_CuandoFileVacio_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "db1",
                Response = null
            });

        var service = new Mock<IServiceGuardaEditorImage>();
        var controller = new GuardaEditorImageController(claimValidation.Object, service.Object);

        var file = new Mock<IFormFile>();
        file.SetupGet(f => f.Length).Returns(0);

        var action = await controller.GuardarImagen(file.Object);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GuardarImagen_CuandoServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "db1",
                Response = null
            });

        var service = new Mock<IServiceGuardaEditorImage>();
        service.Setup(s => s.GuardaEditorImageAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), "db1", null))
            .ReturnsAsync(new AppResponses<GuardaEditorImageResponseDto?>
            {
                success = true,
                message = "YES",
                data = new GuardaEditorImageResponseDto { ImageId = 1, ImageUid = "x" },
                errors = []
            });

        var controller = new GuardaEditorImageController(claimValidation.Object, service.Object);

        var file = new FormFile(new MemoryStream(new byte[] { 1, 2, 3 }), 0, 3, "file", "a.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var action = await controller.GuardarImagen(file);

        Assert.IsType<OkObjectResult>(action.Result);
    }
}

