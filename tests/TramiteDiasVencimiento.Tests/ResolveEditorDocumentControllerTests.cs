using DocuArchi.Api.Controllers.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Editor;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ResolveEditorDocumentControllerTests
{
    [Fact]
    public async Task Resolve_CuandoClaimInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "No claim", data = "", errors = [] }
            });

        var service = new Mock<IServiceResolveEditorDocument>();
        var controller = new ResolveEditorDocumentController(claimValidation.Object, service.Object);

        var action = await controller.Resolve("RAD", 1, 0, null, null, null);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Resolve_CuandoParametrosInvalidos_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceResolveEditorDocument>();
        var controller = new ResolveEditorDocumentController(claimValidation.Object, service.Object);

        var action = await controller.Resolve("   ", 0, 0, null, null, null);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Resolve_CuandoPreferInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceResolveEditorDocument>();
        var controller = new ResolveEditorDocumentController(claimValidation.Object, service.Object);

        var action = await controller.Resolve("RAD", 1, 0, null, null, "otro");

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Resolve_CuandoServiceConflict_RetornaConflict()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceResolveEditorDocument>();
        service.Setup(s => s.ResolveAsync(0, "RAD", 911, null, null, null, "db1"))
            .ReturnsAsync(new AppResponses<EditorResolveDocumentResponseDto?>
            {
                success = false,
                message = "Conflict",
                data = null,
                errors =
                [
                    new MiApp.DTOs.DTOs.Errors.AppError { Type = "Conflict", Field = "contextCode", Message = "Multiple docs" }
                ]
            });

        var controller = new ResolveEditorDocumentController(claimValidation.Object, service.Object);

        var action = await controller.Resolve("RAD", 911, 0, null, null, null);

        Assert.IsType<ConflictObjectResult>(action.Result);
    }

    [Fact]
    public async Task Resolve_CuandoServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceResolveEditorDocument>();
        service.Setup(s => s.ResolveAsync(0, "RAD", 911, null, null, null, "db1"))
            .ReturnsAsync(new AppResponses<EditorResolveDocumentResponseDto?>
            {
                success = true,
                message = "OK",
                data = null,
                errors = []
            });

        var controller = new ResolveEditorDocumentController(claimValidation.Object, service.Object);

        var action = await controller.Resolve("RAD", 911, 0, null, null, null);

        Assert.IsType<OkObjectResult>(action.Result);
    }
}

