using DocuArchi.Api.Controllers.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class GuardaEditorDocumentControllerTests
{
    [Fact]
    public async Task GuardarDocumento_CuandoClaimEsInvalido_RetornaBadRequest()
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

        var service = new Mock<IServiceGuardaEditorDocument>();
        var controller = new GuardaEditorDocumentController(claimValidation.Object, service.Object);

        var action = await controller.GuardarDocumento(new GuardaEditorDocumentRequestDto { DocumentHtml = "<p>x</p>" });

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.Verify(s => s.GuardaEditorDocumentAsync(It.IsAny<GuardaEditorDocumentRequestDto>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GuardarDocumento_CuandoHtmlVacio_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "db1",
                Response = null
            });

        var service = new Mock<IServiceGuardaEditorDocument>();
        var controller = new GuardaEditorDocumentController(claimValidation.Object, service.Object);

        var action = await controller.GuardarDocumento(new GuardaEditorDocumentRequestDto { DocumentHtml = "   " });

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.Verify(s => s.GuardaEditorDocumentAsync(It.IsAny<GuardaEditorDocumentRequestDto>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GuardarDocumento_CuandoServiceError_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "db1",
                Response = null
            });

        var service = new Mock<IServiceGuardaEditorDocument>();
        service.Setup(s => s.GuardaEditorDocumentAsync(It.IsAny<GuardaEditorDocumentRequestDto>(), "db1"))
            .ReturnsAsync(new AppResponses<RaEditorDocument?>
            {
                success = false,
                message = "fail",
                data = null,
                errors = []
            });

        var controller = new GuardaEditorDocumentController(claimValidation.Object, service.Object);

        var action = await controller.GuardarDocumento(new GuardaEditorDocumentRequestDto { DocumentHtml = "<p>x</p>" });

        Assert.IsType<BadRequestObjectResult>(action.Result);
    }

    [Fact]
    public async Task GuardarDocumento_CuandoServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "db1",
                Response = null
            });

        var service = new Mock<IServiceGuardaEditorDocument>();
        service.Setup(s => s.GuardaEditorDocumentAsync(It.IsAny<GuardaEditorDocumentRequestDto>(), "db1"))
            .ReturnsAsync(new AppResponses<RaEditorDocument?>
            {
                success = true,
                message = "YES",
                data = new RaEditorDocument { DocumentId = 1, FormatCode = "html", DocumentHtml = "<p>x</p>", StatusCode = "saved" },
                errors = []
            });

        var controller = new GuardaEditorDocumentController(claimValidation.Object, service.Object);

        var action = await controller.GuardarDocumento(new GuardaEditorDocumentRequestDto { DocumentHtml = "<p>x</p>" });

        Assert.IsType<OkObjectResult>(action.Result);
    }
}

