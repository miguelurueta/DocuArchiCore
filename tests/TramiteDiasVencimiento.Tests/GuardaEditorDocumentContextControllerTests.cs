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

public sealed class GuardaEditorDocumentContextControllerTests
{
    [Fact]
    public async Task GuardaContext_CuandoClaimInvalido_RetornaBadRequest()
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

        var service = new Mock<IServiceGuardaEditorDocumentContext>();
        var controller = new GuardaEditorDocumentContextController(claimValidation.Object, service.Object);

        var action = await controller.GuardaContext(new GuardaEditorDocumentContextDto
        {
            DocumentId = 1,
            ContextCode = "RAD_GESTION_RESPUESTA",
            EntityId = 911
        });

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GuardaContext_CuandoRequestInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceGuardaEditorDocumentContext>();
        var controller = new GuardaEditorDocumentContextController(claimValidation.Object, service.Object);

        var action = await controller.GuardaContext(new GuardaEditorDocumentContextDto
        {
            DocumentId = 0,
            ContextCode = "RAD_GESTION_RESPUESTA",
            EntityId = 911
        });

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GuardaContext_CuandoServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceGuardaEditorDocumentContext>();
        service.Setup(s => s.GuardaEditorDocumentContextAsync(It.IsAny<GuardaEditorDocumentContextDto>(), "db1"))
            .ReturnsAsync(new AppResponses<RaEditorDocumentContext?>
            {
                success = true,
                message = "YES",
                data = new RaEditorDocumentContext { ContextId = 10, DocumentId = 1, ContextDefinitionId = 1, EntityId = 911, IsActive = true },
                errors = []
            });

        var controller = new GuardaEditorDocumentContextController(claimValidation.Object, service.Object);

        var action = await controller.GuardaContext(new GuardaEditorDocumentContextDto
        {
            DocumentId = 1,
            ContextCode = "RAD_GESTION_RESPUESTA",
            EntityId = 911,
            CreatedBy = "user1"
        });

        Assert.IsType<OkObjectResult>(action.Result);
    }
}

