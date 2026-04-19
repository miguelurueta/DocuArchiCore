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

public sealed class FullSaveEditorDocumentControllerTests
{
    [Fact]
    public async Task FullSave_CuandoClaimInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "No claim", data = "", errors = [] }
            });

        var service = new Mock<IServiceFullSaveEditorDocument>();
        var controller = new FullSaveEditorDocumentController(service.Object, claimValidation.Object);

        var action = await controller.FullSave(new FullSaveEditorDocumentRequestDto());

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FullSave_CuandoRequestInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceFullSaveEditorDocument>();
        var controller = new FullSaveEditorDocumentController(service.Object, claimValidation.Object);

        var action = await controller.FullSave(new FullSaveEditorDocumentRequestDto
        {
            DocumentHtml = " ",
            ContextCode = "RAD",
            EntityId = 1
        });

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FullSave_CuandoServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceFullSaveEditorDocument>();
        service.Setup(s => s.FullSaveAsync(It.IsAny<FullSaveEditorDocumentRequestDto>(), "db1"))
            .ReturnsAsync(new AppResponses<RaEditorDocument?> { success = true, message = "YES", data = new RaEditorDocument { DocumentId = 1 }, errors = [] });

        var controller = new FullSaveEditorDocumentController(service.Object, claimValidation.Object);

        var action = await controller.FullSave(new FullSaveEditorDocumentRequestDto
        {
            DocumentHtml = "<p>x</p>",
            ContextCode = "RAD",
            EntityId = 911,
            ImageUids = []
        });

        Assert.IsType<OkObjectResult>(action.Result);
    }
}

