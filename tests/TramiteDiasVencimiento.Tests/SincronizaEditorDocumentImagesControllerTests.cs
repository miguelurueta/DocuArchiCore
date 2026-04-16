using DocuArchi.Api.Controllers.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Editor;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SincronizaEditorDocumentImagesControllerTests
{
    [Fact]
    public async Task Sync_CuandoClaimInvalido_RetornaBadRequest()
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

        var service = new Mock<IServiceSincronizaEditorDocumentImages>();
        var controller = new SincronizaEditorDocumentImagesController(claimValidation.Object, service.Object);

        var action = await controller.Sync(new SincronizaEditorDocumentImagesRequestDto
        {
            DocumentId = 1,
            ImageUids = ["a"]
        });

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Sync_CuandoDocumentIdInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceSincronizaEditorDocumentImages>();
        var controller = new SincronizaEditorDocumentImagesController(claimValidation.Object, service.Object);

        var action = await controller.Sync(new SincronizaEditorDocumentImagesRequestDto
        {
            DocumentId = 0,
            ImageUids = ["a"]
        });

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Sync_CuandoServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceSincronizaEditorDocumentImages>();
        service.Setup(s => s.SincronizaAsync(It.IsAny<SincronizaEditorDocumentImagesRequestDto>(), "db1"))
            .ReturnsAsync(new AppResponses<bool> { success = true, message = "OK", data = true, errors = [] });

        var controller = new SincronizaEditorDocumentImagesController(claimValidation.Object, service.Object);

        var action = await controller.Sync(new SincronizaEditorDocumentImagesRequestDto
        {
            DocumentId = 12,
            ImageUids = ["img-1", "img-2"]
        });

        Assert.IsType<OkObjectResult>(action.Result);
    }
}

