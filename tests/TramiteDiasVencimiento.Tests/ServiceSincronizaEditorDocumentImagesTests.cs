using AutoMapper;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSincronizaEditorDocumentImagesTests
{
    private static IMapper BuildMapper()
    {
        var cfg = new MapperConfiguration(c =>
        {
            c.CreateMap<SincronizaEditorDocumentImagesRequestDto, SincronizaEditorDocumentImagesCommand>();
        });
        return cfg.CreateMapper();
    }

    [Fact]
    public async Task SincronizaAsync_CuandoDocumentIdInvalido_RetornaError()
    {
        var repo = new Mock<ISincronizaEditorDocumentImagesRepository>();
        var svc = new ServiceSincronizaEditorDocumentImages(repo.Object, BuildMapper());

        var result = await svc.SincronizaAsync(new SincronizaEditorDocumentImagesRequestDto
        {
            DocumentId = 0,
            ImageUids = ["a"]
        }, "db1");

        Assert.False(result.success);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SincronizaAsync_CuandoRepoFalla_PropagaError()
    {
        var repo = new Mock<ISincronizaEditorDocumentImagesRepository>();
        repo.Setup(r => r.SincronizaAsync(1, It.IsAny<IReadOnlyCollection<string>>(), "db1"))
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
            ImageUids = ["a"]
        }, "db1");

        Assert.False(result.success);
    }

    [Fact]
    public async Task SincronizaAsync_CuandoRepoOk_RetornaOk()
    {
        var repo = new Mock<ISincronizaEditorDocumentImagesRepository>();
        repo.Setup(r => r.SincronizaAsync(1, It.IsAny<IReadOnlyCollection<string>>(), "db1"))
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
            ImageUids = ["a", "b"]
        }, "db1");

        Assert.True(result.success);
        Assert.True(result.data);
    }
}

