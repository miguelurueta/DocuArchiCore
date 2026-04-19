using DocuArchi.Api.Controllers.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Editor;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class TemplateDefinitionsControllerTests
{
    [Fact]
    public async Task CreateDefinition_CuandoClaimInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<string> { success = false, message = "No claim", data = "", errors = [] }
            });

        var service = new Mock<IServiceTemplateDefinitions>();
        var controller = new TemplateDefinitionsController(claimValidation.Object, service.Object);

        var action = await controller.CreateDefinition(new CreateTemplateDefinitionDto { TemplateCode = "X", TemplateName = "N" });

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateDefinition_CuandoRequestInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "db1", Response = null });

        var service = new Mock<IServiceTemplateDefinitions>();
        var controller = new TemplateDefinitionsController(claimValidation.Object, service.Object);

        var action = await controller.CreateDefinition(new CreateTemplateDefinitionDto { TemplateCode = "   ", TemplateName = "N" });

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.VerifyNoOtherCalls();
    }
}

