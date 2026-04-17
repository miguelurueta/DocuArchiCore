using AutoMapper;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSincronizaEditorDocumentImagesTests
{
    private static IMapper BuildMapper()
    {
        var mock = new Mock<IMapper>();
        // No configuramos mapeos específicos si no se requieren para los tests actuales
        return mock.Object;
    }

    [Fact]
    public async Task SincronizaAsync_CuandoDocumentIdInvalido_RetornaError()
    {
        var repo = new Mock<ISincronizaEditorDocumentImagesRepository>();
        var svc = new ServiceSincronizaEditorDocumentImages(repo.Object, BuildMapper());

        var result = await svc.SincronizaAsync(new SincronizaEditorDocumentImagesRequestDto
        {
            DocumentId = 0,
            ImageUids = new List<string> { "a" }
        }, "db1");

        Assert.False(result.success);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SincronizaAsync_CuandoRepoFalla_PropagaError()
    {
        var repo = new Mock<ISincronizaEditorDocumentImagesRepository>();
        repo.Setup(r => r.SincronizaAsync(1, It.IsAny<IReadOnlyCollection<string>>(), "db1", null, null))
            .ReturnsAsync(new AppResponses<bool>
            {
                success = false,
                message = "Fallo repo",
                data = false,
                errors = []
            });

        var svc = new ServiceSincronizaEditorDocumentImages(repo.Object, BuildMapper());

        var result = await svc.SincronizaAsync(new SincronizaEditorDocumentImagesRequestDto
        {
            DocumentId = 1,
            ImageUids = new List<string> { "a" }
        }, "db1");

        Assert.False(result.success);
    }

    [Fact]
    public async Task SincronizaAsync_CuandoRepoOk_RetornaOk()
    {
        var repo = new Mock<ISincronizaEditorDocumentImagesRepository>();
        repo.Setup(r => r.SincronizaAsync(1, It.IsAny<IReadOnlyCollection<string>>(), "db1", null, null))
            .ReturnsAsync(new AppResponses<bool>
            {
                success = true,
                message = "OK",
                data = true,
                errors = []
            });

        var svc = new ServiceSincronizaEditorDocumentImages(repo.Object, BuildMapper());

        var result = await svc.SincronizaAsync(new SincronizaEditorDocumentImagesRequestDto
        {
            DocumentId = 1,
            ImageUids = new List<string> { "a", "b" }
        }, "db1");

        Assert.True(result.success);
        Assert.True(result.data);
    }
}
