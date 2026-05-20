using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.PermisosVisorPdf;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Repository.Repositorio.GestorDocumental.PermisosVisorPdf;
using MiApp.Services.Service.GestorDocumental.PermisosVisorPdf;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class PermisosVisorPdfServiceTests
{
    [Fact]
    public async Task GetMyPermissions_UsuarioInvalido_RetornaValidation()
    {
        var service = new PermisosVisorPdfService(
            Mock.Of<IPermisosVisorPdfRepository>(),
            Mock.Of<ILogger<PermisosVisorPdfService>>());

        var result = await service.GetMyPermissionsAsync("workflow", 0, "DA");

        Assert.False(result.success);
        Assert.Equal("validation", result.meta.Status);
    }

    [Fact]
    public async Task GetMyPermissions_CuandoOk_DelegaRepositorio()
    {
        var repository = new Mock<IPermisosVisorPdfRepository>();
        repository.Setup(x => x.GetEffectivePermissionsAsync("workflow", 141, "DA"))
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

        var service = new PermisosVisorPdfService(repository.Object, Mock.Of<ILogger<PermisosVisorPdfService>>());
        var result = await service.GetMyPermissionsAsync("workflow", 141, "DA");

        Assert.True(result.success);
        Assert.True(result.data.Permissions["pdf.view"]);
        repository.Verify(x => x.GetEffectivePermissionsAsync("workflow", 141, "DA"), Times.Once);
    }

    [Fact]
    public async Task UpsertUserOverrides_RequestNull_RetornaValidation()
    {
        var service = new PermisosVisorPdfService(
            Mock.Of<IPermisosVisorPdfRepository>(),
            Mock.Of<ILogger<PermisosVisorPdfService>>());

        var result = await service.UpsertUserOverridesAsync("workflow", 141, null!, "DA");

        Assert.False(result.success);
        Assert.Equal("validation", result.meta.Status);
    }
}
