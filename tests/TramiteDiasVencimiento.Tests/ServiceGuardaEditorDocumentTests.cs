using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.Editor;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceGuardaEditorDocumentTests
{
    [Fact]
    public async Task CuandoAliasInvalido_RetornaError()
    {
        var repo = new Mock<IGuardaEditorDocumentRepository>(MockBehavior.Strict);
        var service = new ServiceGuardaEditorDocument(repo.Object);

        var res = await service.GuardaEditorDocumentAsync(new GuardaEditorDocumentRequestDto { DocumentHtml = "<p>x</p>" }, "   ");

        Assert.False(res.success);
        Assert.Null(res.data);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CuandoHtmlInvalido_RetornaError()
    {
        var repo = new Mock<IGuardaEditorDocumentRepository>(MockBehavior.Strict);
        var service = new ServiceGuardaEditorDocument(repo.Object);

        var res = await service.GuardaEditorDocumentAsync(new GuardaEditorDocumentRequestDto { DocumentHtml = "   " }, "db1");

        Assert.False(res.success);
        Assert.Null(res.data);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CuandoRepoOk_RetornaOk()
    {
        var repo = new Mock<IGuardaEditorDocumentRepository>();
        repo.Setup(r => r.GuardaEditorDocumentAsync(It.IsAny<GuardaEditorDocumentRequestDto>(), "db1"))
            .ReturnsAsync(new AppResponses<RaEditorDocument?>
            {
                success = true,
                message = "YES",
                data = new RaEditorDocument { DocumentId = 1, FormatCode = "html", DocumentHtml = "<p>x</p>", StatusCode = "saved" },
                errors = []
            });

        var service = new ServiceGuardaEditorDocument(repo.Object);

        var res = await service.GuardaEditorDocumentAsync(new GuardaEditorDocumentRequestDto { DocumentHtml = "<p>x</p>" }, "db1");

        Assert.True(res.success);
        Assert.NotNull(res.data);
    }
}

