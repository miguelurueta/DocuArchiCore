using DocuArchi.Api.Controllers.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Editor;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaEditorDocumentByContextControllerTests
{
    [Fact]
    public async Task GetByContext_CuandoClaimInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "No claim", data = "", errors = [] }
            });

        var service = new Mock<IServiceSolicitaEditorDocumentByContext>();
        var controller = new SolicitaEditorDocumentByContextController(claimValidation.Object, service.Object);

        var action = await controller.GetByContext("RAD", 1);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByContext_CuandoParametrosInvalidos_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceSolicitaEditorDocumentByContext>();
        var controller = new SolicitaEditorDocumentByContextController(claimValidation.Object, service.Object);

        var action = await controller.GetByContext("   ", 0);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByContext_CuandoServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceSolicitaEditorDocumentByContext>();
        service.Setup(s => s.SolicitaEditorDocumentByContextAsync("RAD", 911, "db1"))
            .ReturnsAsync(new AppResponses<EditorDocumentDetailByContextResponseDto?>
            {
                success = true,
                message = "YES",
                data = null,
                errors = []
            });

        var controller = new SolicitaEditorDocumentByContextController(claimValidation.Object, service.Object);

        var action = await controller.GetByContext("RAD", 911);

        Assert.IsType<OkObjectResult>(action.Result);
    }
}

