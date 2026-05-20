using DocuArchi.Api.Controllers.GestorDocumental.PermisosVisorPdf;
using Microsoft.AspNetCore.Mvc;
using MiApp.DTOs.DTOs.GestorDocumental.PermisosVisorPdf;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.PermisosVisorPdf;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class PermisosVisorPdfControllerTests
{
    [Fact]
    public async Task GetMyPermissions_SinClaimAlias_RetornaBadRequest()
    {
        var claims = new Mock<IClaimValidationService>();
        claims.Setup(x => x.ValidateClaim<VisorPdfPermissionsResponseDto>("defaulalias"))
            .Returns(new ClaimValidationResult<VisorPdfPermissionsResponseDto>
            {
                Success = false,
                ClaimValue = null,
                Response = new AppResponses<VisorPdfPermissionsResponseDto>
                {
                    success = false,
                    message = "No claim",
                    data = new VisorPdfPermissionsResponseDto()
                }
            });

        var controller = new PermisosVisorPdfController(
            claims.Object,
            Mock.Of<ICurrentUserService>(),
            Mock.Of<IPermisosVisorPdfService>());

        var result = await controller.GetMyPermissions("workflow");

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetUserPermissions_NoAdmin_RetornaUnauthorized()
    {
        var claims = new Mock<IClaimValidationService>();
        claims.Setup(x => x.ValidateClaim<VisorPdfPermissionsResponseDto>("defaulalias"))
            .Returns(new ClaimValidationResult<VisorPdfPermissionsResponseDto>
            {
                Success = true,
                ClaimValue = "DA"
            });

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.HasPermission("pdf.permissions.admin")).Returns(false);
        currentUser.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(string.Empty);

        var controller = new PermisosVisorPdfController(
            claims.Object,
            currentUser.Object,
            Mock.Of<IPermisosVisorPdfService>());

        var result = await controller.GetUserPermissions("workflow", 10);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetMyPermissions_CuandoOk_RetornaOk()
    {
        var claims = new Mock<IClaimValidationService>();
        claims.Setup(x => x.ValidateClaim<VisorPdfPermissionsResponseDto>("defaulalias"))
            .Returns(new ClaimValidationResult<VisorPdfPermissionsResponseDto> { Success = true, ClaimValue = "DA" });
        claims.Setup(x => x.ValidateClaim<VisorPdfPermissionsResponseDto>("usuarioid"))
            .Returns(new ClaimValidationResult<VisorPdfPermissionsResponseDto> { Success = true, ClaimValue = "141" });

        var service = new Mock<IPermisosVisorPdfService>();
        service.Setup(x => x.GetMyPermissionsAsync("workflow", 141, "DA"))
            .ReturnsAsync(new AppResponses<VisorPdfPermissionsResponseDto>
            {
                success = true,
                message = "OK",
                data = new VisorPdfPermissionsResponseDto
                {
                    CodigoImplementacion = "workflow",
                    IdUsuario = 141,
                    Permissions = new Dictionary<string, bool> { ["pdf.view"] = true }
                }
            });

        var controller = new PermisosVisorPdfController(
            claims.Object,
            Mock.Of<ICurrentUserService>(),
            service.Object);

        var result = await controller.GetMyPermissions("workflow");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<VisorPdfPermissionsResponseDto>>(ok.Value);
        Assert.True(payload.success);
        Assert.True(payload.data.Permissions["pdf.view"]);
    }

    [Fact]
    public async Task UpsertOverrides_AdminOk_RetornaContratoOperacion()
    {
        var claims = new Mock<IClaimValidationService>();
        claims.Setup(x => x.ValidateClaim<SimpleOperationResultDto>("defaulalias"))
            .Returns(new ClaimValidationResult<SimpleOperationResultDto> { Success = true, ClaimValue = "DA" });

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.HasPermission("pdf.permissions.admin")).Returns(true);

        var service = new Mock<IPermisosVisorPdfService>();
        service.Setup(x => x.UpsertUserOverridesAsync("workflow", 141, It.IsAny<UpsertUserOverridesRequestDto>(), "DA"))
            .ReturnsAsync(new AppResponses<SimpleOperationResultDto>
            {
                success = true,
                message = "OK",
                data = new SimpleOperationResultDto
                {
                    CodigoImplementacion = "workflow",
                    IdUsuario = 141,
                    Procesados = 2
                }
            });

        var controller = new PermisosVisorPdfController(claims.Object, currentUser.Object, service.Object);
        var request = new UpsertUserOverridesRequestDto
        {
            Overrides =
            [
                new PermissionOverrideItemDto { CodigoPermiso = "pdf.print", Permitido = 1 },
                new PermissionOverrideItemDto { CodigoPermiso = "pdf.download", Permitido = 1 }
            ]
        };

        var result = await controller.UpsertOverrides("workflow", 141, request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<SimpleOperationResultDto>>(ok.Value);
        Assert.True(payload.success);
        Assert.Equal(2, payload.data.Procesados);
        Assert.Equal("workflow", payload.data.CodigoImplementacion);
    }

    [Fact]
    public async Task DeleteOverride_AdminOk_RetornaContratoOperacion()
    {
        var claims = new Mock<IClaimValidationService>();
        claims.Setup(x => x.ValidateClaim<SimpleOperationResultDto>("defaulalias"))
            .Returns(new ClaimValidationResult<SimpleOperationResultDto> { Success = true, ClaimValue = "DA" });

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.HasPermission("pdf.permissions.admin")).Returns(true);

        var service = new Mock<IPermisosVisorPdfService>();
        service.Setup(x => x.DeleteUserOverrideAsync("workflow", 141, "pdf.print", "DA"))
            .ReturnsAsync(new AppResponses<SimpleOperationResultDto>
            {
                success = true,
                message = "OK",
                data = new SimpleOperationResultDto
                {
                    CodigoImplementacion = "workflow",
                    IdUsuario = 141,
                    Procesados = 1
                }
            });

        var controller = new PermisosVisorPdfController(claims.Object, currentUser.Object, service.Object);
        var result = await controller.DeleteOverride("workflow", 141, "pdf.print");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<SimpleOperationResultDto>>(ok.Value);
        Assert.True(payload.success);
        Assert.Equal(1, payload.data.Procesados);
        Assert.Equal(141, payload.data.IdUsuario);
    }
}
