using DocuArchi.Api.Controllers.GestorDocumental.ConfiguracionUpload;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.ConfiguracionUpload;
using MiApp.Services.Service.GestorDocumental.ConfiguracionUpload;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaEstructuraConfiguracionUploadControllerTests
{
    [Fact]
    public async Task Get_CuandoClaimEsInvalido_RetornaBadRequest()
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

        var service = new Mock<IServiceSolicitaEstructuraConfiguracionUpload>();
        var controller = new SolicitaEstructuraConfiguracionUploadController(claimValidation.Object, service.Object);

        var action = await controller.Get("WF");

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.Verify(s => s.SolicitaEstructuraConfiguracionUploadNameProcesoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Get_CuandoNameProcesoInvalido_RetornaBadRequest(string? nameProceso)
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "db1",
                Response = null
            });

        var service = new Mock<IServiceSolicitaEstructuraConfiguracionUpload>();
        var controller = new SolicitaEstructuraConfiguracionUploadController(claimValidation.Object, service.Object);

        var action = await controller.Get(nameProceso!);

        Assert.IsType<BadRequestObjectResult>(action.Result);
        service.Verify(s => s.SolicitaEstructuraConfiguracionUploadNameProcesoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Get_CuandoServiceError_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "db1",
                Response = null
            });

        var service = new Mock<IServiceSolicitaEstructuraConfiguracionUpload>();
        service.Setup(s => s.SolicitaEstructuraConfiguracionUploadNameProcesoAsync("WF", "db1"))
            .ReturnsAsync(new AppResponses<List<RaConfiguracionUploadModel>>
            {
                success = false,
                message = "fail",
                data = [],
                errors = []
            });

        var controller = new SolicitaEstructuraConfiguracionUploadController(claimValidation.Object, service.Object);

        var action = await controller.Get("WF");

        Assert.IsType<BadRequestObjectResult>(action.Result);
    }

    [Fact]
    public async Task Get_CuandoServiceOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = true,
                ClaimValue = "db1",
                Response = null
            });

        var service = new Mock<IServiceSolicitaEstructuraConfiguracionUpload>();
        service.Setup(s => s.SolicitaEstructuraConfiguracionUploadNameProcesoAsync("WF", "db1"))
            .ReturnsAsync(new AppResponses<List<RaConfiguracionUploadModel>>
            {
                success = true,
                message = "YES",
                data = [],
                errors = []
            });

        var controller = new SolicitaEstructuraConfiguracionUploadController(claimValidation.Object, service.Object);

        var action = await controller.Get("WF");

        Assert.IsType<OkObjectResult>(action.Result);
    }
}

