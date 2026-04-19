using DocuArchi.Api.Controllers.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Editor;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class InitialContentEditorControllerTests
{
    [Fact]
    public async Task GetInitialContent_CuandoClaimInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "No claim", data = "", errors = [] }
            });

        var service = new Mock<IServiceInitialContentEditor>();
        var controller = new InitialContentEditorController(claimValidation.Object, service.Object);

        var action = await controller.GetInitialContent(1, "RAD", 1);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetInitialContent_CuandoServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceInitialContentEditor>();
        service.Setup(s => s.GetInitialContentAsync(1, "RAD", 1, "db1"))
            .ReturnsAsync(new AppResponses<EditorInitialContentResponseDto?>
            {
                success = true,
                message = "OK",
                data = new EditorInitialContentResponseDto { IdTareaWf = 1, EntityId = 1, ContextCode = "RAD", TemplateCode = "RAD", TemplateVersion = 1, HtmlInicial = "<p>x</p>" },
                errors = []
            });

        var controller = new InitialContentEditorController(claimValidation.Object, service.Object);

        var action = await controller.GetInitialContent(1, "RAD", 1);

        Assert.IsType<OkObjectResult>(action.Result);
    }
}

