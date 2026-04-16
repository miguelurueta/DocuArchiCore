using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceGuardaEditorImageTests
{
    [Fact]
    public async Task CuandoContentTypeInvalido_RetornaError()
    {
        var repo = new Mock<IGuardaEditorImageRepository>(MockBehavior.Strict);
        var service = new ServiceGuardaEditorImage(repo.Object);

        var res = await service.GuardaEditorImageAsync(new byte[] { 1 }, "a.bin", "application/octet-stream", "db1");

        Assert.False(res.success);
        Assert.Null(res.data);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CuandoBytesVacios_RetornaError()
    {
        var repo = new Mock<IGuardaEditorImageRepository>(MockBehavior.Strict);
        var service = new ServiceGuardaEditorImage(repo.Object);

        var res = await service.GuardaEditorImageAsync(Array.Empty<byte>(), "a.png", "image/png", "db1");

        Assert.False(res.success);
        Assert.Null(res.data);
        repo.VerifyNoOtherCalls();
    }
}

