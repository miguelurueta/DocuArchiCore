using Microsoft.Extensions.Caching.Memory;
using MiApp.Repository.ErrorController;
using MiApp.Repository.Repositorio.Workflow.usuario;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.TemporaryUpload;
using MiApp.Services.Service.Workflow.Usuario;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceFirmaTemporalUsuarioWorkflowTests
{
    [Fact]
    public async Task SolicitaFirmaTemporalAsync_CuandoAliasInvalido_RetornaValidation()
    {
        var repository = new Mock<IUsuarioWorkflowR>();
        var pathResolver = new Mock<IStorageUploadPathResolver>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = new ServiceFirmaTemporalUsuarioWorkflow(repository.Object, pathResolver.Object, memoryCache);

        var result = await service.SolicitaFirmaTemporalAsync(10, " ");

        Assert.False(result.success);
        Assert.Equal("validation", result.meta?.Status);
        repository.Verify(
            r => r.SolicitaFirmaUsuarioWorkflowAsync(It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task SolicitaFirmaTemporalAsync_CuandoFirmaPngValida_GeneraTemporalYTokenResoluble()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "docuarchi-test-signatures", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var firmaPng = Convert.FromBase64String(
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO5D2foAAAAASUVORK5CYII=");

            var repository = new Mock<IUsuarioWorkflowR>();
            repository.Setup(r => r.SolicitaFirmaUsuarioWorkflowAsync(22, "WF"))
                .ReturnsAsync(new AppResponse<byte[]>
                {
                    Success = true,
                    Message = "YES",
                    ErrorMessage = "YES",
                    Data = firmaPng
                });

            var pathResolver = new Mock<IStorageUploadPathResolver>();
            pathResolver.Setup(p => p.GetTempRoot()).Returns(tempRoot);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new ServiceFirmaTemporalUsuarioWorkflow(repository.Object, pathResolver.Object, memoryCache);

            var result = await service.SolicitaFirmaTemporalAsync(22, "WF");

            Assert.True(result.success);
            Assert.NotNull(result.data);
            Assert.Equal("image/png", result.data!.ContentType);
            Assert.Contains("/api/workflow/usuarios/firma-temporal/download/", result.data.UrlTemporal);

            var token = result.data.UrlTemporal.Split('/').Last();
            var found = service.TryResolveFirmaTemporal(token, 22, out var filePath, out var contentType, out var fileName);

            Assert.True(found);
            Assert.True(File.Exists(filePath));
            Assert.Equal("image/png", contentType);
            Assert.EndsWith(".png", fileName);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, true);
            }
        }
    }
}
