using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.Documentos.ReemplazoPdf;
using MiApp.Repository.Repositorio.GestorDocumental.Common.Audit;
using MiApp.Repository.Repositorio.GestorDocumental.Documentos.ReemplazoPdf;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.StorageRoute;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Physical;
using MiApp.Services.Service.GestorDocumental.Documentos.ReemplazoPdf;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.TemporaryUpload;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class StorageDocumentReplacementServiceTests
{
    [Fact]
    public async Task ExecuteAsync_CuandoTemporalNoEsPdf_RetornaValidation()
    {
        var largeUpload = new Mock<IStorageLargeUploadService>();
        largeUpload.Setup(x => x.EnsureCompletedAsync("rt-1", It.IsAny<IReadOnlyList<string>>(), 99))
            .Returns(Task.CompletedTask);

        var uploadResolver = new Mock<IStorageUploadPathResolver>();
        uploadResolver.Setup(x => x.GetFinalFilePath("rt-1", "af-1.txt")).Returns("C:\\tmp\\af-1.txt");

        var service = BuildService(
            largeUpload: largeUpload.Object,
            uploadResolver: uploadResolver.Object);

        var result = await service.ExecuteAsync(
            new ReemplazarDocumentoPdfRequest
            {
                NombreGabinete = "contabil",
                IdDocumento = 10,
                RutaTemporalId = "rt-1",
                ArchivoTemporalId = "af-1.txt"
            },
            "DA",
            "qa.user",
            99,
            "10.0.0.10");

        Assert.False(result.success);
        Assert.Equal("validation", result.meta?.Status);
    }

    [Fact]
    public async Task ExecuteAsync_CuandoFlujoOk_ReemplazaYRegistraLog()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "replace-test", Guid.NewGuid().ToString("N"));
        var storageRoot = Path.Combine(tempRoot, "store");
        Directory.CreateDirectory(storageRoot);

        try
        {
            var tempFile = Path.Combine(tempRoot, "af-1.pdf");
            await File.WriteAllTextAsync(tempFile, "nuevo-pdf-contenido");

            var gabineteFolder = Path.Combine(storageRoot, "contabil7", "00093");
            Directory.CreateDirectory(gabineteFolder);
            var targetFile = Path.Combine(gabineteFolder, "DIG00000010.PDF");
            await File.WriteAllTextAsync(targetFile, "contenido-anterior");

            var largeUpload = new Mock<IStorageLargeUploadService>();
            largeUpload.Setup(x => x.EnsureCompletedAsync("rt-1", It.IsAny<IReadOnlyList<string>>(), 99))
                .Returns(Task.CompletedTask);

            var uploadResolver = new Mock<IStorageUploadPathResolver>();
            uploadResolver.Setup(x => x.GetFinalFilePath("rt-1", "af-1.pdf")).Returns(tempFile);
            uploadResolver.Setup(x => x.GetTempRoot()).Returns(tempRoot);

            var storagePathResolver = new Mock<IStoragePathResolver>();
            storagePathResolver.Setup(x => x.ResolveSafePath(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((root, rel) => Path.GetFullPath(Path.Combine(root, rel)));

            var routeRepo = new Mock<IStorageRouteRepository>();
            routeRepo.Setup(x => x.GetRouteAsync("contabil", "DA"))
                .ReturnsAsync(new MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical.StorageRouteModel
                {
                    NombreGabinete = "contabil",
                    RutaAlmacenamiento = storageRoot
                });

            var folderPolicy = new StorageFolderLegacyPolicy();

            var locationRepo = new Mock<IReemplazoPdfDocumentLocationRepository>();
            locationRepo.Setup(x => x.GetLocationByIdAsync("contabil", 10, "DA"))
                .ReturnsAsync(new MiApp.Models.Models.GestorDocumental.Documentos.ReemplazoPdf.StorageDocumentLocationModel
                {
                    IdAlmacen = 10,
                    Disco = 7,
                    Carpeta = 93,
                    Paginas = 1,
                    TipoDocumentoEstado = -2,
                    TipoDocumento = "FACTURA"
                });

            var logRepo = new Mock<ILogDocuarchiRepository>();
            logRepo.Setup(x => x.InsertAsync(It.IsAny<MiApp.Models.Models.GestorDocumental.Common.Audit.LogDocuarchiEntryModel>(), "DA"))
                .ReturnsAsync(1);

            var service = BuildService(
                largeUpload.Object,
                uploadResolver.Object,
                storagePathResolver.Object,
                routeRepo.Object,
                folderPolicy,
                locationRepo.Object,
                logRepo.Object);

            var result = await service.ExecuteAsync(
                new ReemplazarDocumentoPdfRequest
                {
                    NombreGabinete = "contabil",
                    IdDocumento = 10,
                    RutaTemporalId = "rt-1",
                    ArchivoTemporalId = "af-1.pdf",
                    Motivo = "ajuste"
                },
                "DA",
                "qa.user",
                99,
                "10.0.0.10");

            Assert.True(result.success);
            Assert.NotNull(result.data);
            Assert.Contains("DIG00000010.PDF", result.data!.RutaArchivoFinal);
            var contenidoFinal = await File.ReadAllTextAsync(targetFile);
            Assert.Equal("nuevo-pdf-contenido", contenidoFinal);
            logRepo.Verify(x => x.InsertAsync(It.IsAny<MiApp.Models.Models.GestorDocumental.Common.Audit.LogDocuarchiEntryModel>(), "DA"), Times.Once);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, true);
            }
        }
    }

    private static ReemplazoPdfService BuildService(
        IStorageLargeUploadService? largeUpload = null,
        IStorageUploadPathResolver? uploadResolver = null,
        IStoragePathResolver? storagePathResolver = null,
        IStorageRouteRepository? routeRepo = null,
        IStorageFolderLegacyPolicy? folderPolicy = null,
        IReemplazoPdfDocumentLocationRepository? locationRepository = null,
        ILogDocuarchiRepository? logRepository = null)
    {
        return new ReemplazoPdfService(
            largeUpload ?? Mock.Of<IStorageLargeUploadService>(),
            uploadResolver ?? Mock.Of<IStorageUploadPathResolver>(),
            storagePathResolver ?? Mock.Of<IStoragePathResolver>(),
            routeRepo ?? Mock.Of<IStorageRouteRepository>(),
            folderPolicy ?? Mock.Of<IStorageFolderLegacyPolicy>(),
            locationRepository ?? Mock.Of<IReemplazoPdfDocumentLocationRepository>(),
            logRepository ?? Mock.Of<ILogDocuarchiRepository>(),
            Mock.Of<ILogger<ReemplazoPdfService>>());
    }
}
