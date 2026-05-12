using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Physical;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageFileWriterTests
    {
        [Fact]
        public async Task CopyAsync_ShouldAcceptAbsoluteRutaFinal_WhenRutaIsUnderStorageRoot()
        {
            var root = Path.Combine(Path.GetTempPath(), "storage-writer-tests", Guid.NewGuid().ToString("N"));
            var sourceDir = Path.Combine(Path.GetTempPath(), "storage-writer-tests-src", Guid.NewGuid().ToString("N"));
            var sourceFile = Path.Combine(sourceDir, "source.pdf");

            Directory.CreateDirectory(sourceDir);
            await File.WriteAllTextAsync(sourceFile, "contenido");

            var finalFolder = Path.Combine(root, "CONTABIL2", "00001");
            var expectedDestination = Path.Combine(finalFolder, "DIG00000010.pdf");

            var writer = new StorageFileWriter(new StoragePathResolver(), Mock.Of<ILogger<StorageFileWriter>>());
            var plan = new StorageFilePlanModel
            {
                StorageRoot = root,
                RutaFinal = finalFolder,
                NombreArchivoPrincipal = "DIG00000010.pdf",
                NombreXml = "FXL00000010.xml",
                SegundoNombreDocumental = "DIG00000010.pdf",
                ArchivosOrigen = new List<string> { sourceFile }
            };
            var compensation = new StorageCompensationPlan();

            try
            {
                var result = await writer.CopyAsync(plan, compensation, "req-1");

                Assert.Equal(expectedDestination, result);
                Assert.True(File.Exists(expectedDestination));
            }
            finally
            {
                if (Directory.Exists(root))
                {
                    Directory.Delete(root, true);
                }

                if (Directory.Exists(sourceDir))
                {
                    Directory.Delete(sourceDir, true);
                }
            }
        }

        [Fact]
        public async Task CopyAsync_ShouldThrow_WhenAbsoluteRutaFinalEscapesStorageRoot()
        {
            var root = Path.Combine(Path.GetTempPath(), "storage-writer-tests", Guid.NewGuid().ToString("N"));
            var outsideRoot = Path.Combine(Path.GetTempPath(), "storage-writer-tests-outside", Guid.NewGuid().ToString("N"));
            var sourceDir = Path.Combine(Path.GetTempPath(), "storage-writer-tests-src", Guid.NewGuid().ToString("N"));
            var sourceFile = Path.Combine(sourceDir, "source.pdf");

            Directory.CreateDirectory(sourceDir);
            await File.WriteAllTextAsync(sourceFile, "contenido");

            var writer = new StorageFileWriter(new StoragePathResolver(), Mock.Of<ILogger<StorageFileWriter>>());
            var plan = new StorageFilePlanModel
            {
                StorageRoot = root,
                RutaFinal = outsideRoot,
                NombreArchivoPrincipal = "DIG00000010.pdf",
                NombreXml = "FXL00000010.xml",
                SegundoNombreDocumental = "DIG00000010.pdf",
                ArchivosOrigen = new List<string> { sourceFile }
            };
            var compensation = new StorageCompensationPlan();

            try
            {
                await Assert.ThrowsAsync<StoragePhysicalException>(() => writer.CopyAsync(plan, compensation, "req-1"));
            }
            finally
            {
                if (Directory.Exists(root))
                {
                    Directory.Delete(root, true);
                }

                if (Directory.Exists(outsideRoot))
                {
                    Directory.Delete(outsideRoot, true);
                }

                if (Directory.Exists(sourceDir))
                {
                    Directory.Delete(sourceDir, true);
                }
            }
        }
    }
}
