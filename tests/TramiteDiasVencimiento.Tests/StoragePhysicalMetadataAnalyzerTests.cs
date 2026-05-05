using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Metadata;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public sealed class StoragePhysicalMetadataAnalyzerTests
    {
        [Fact]
        public void StorageSizeFormatter_ShouldFormatLegacyValues()
        {
            var sut = new StorageSizeFormatter();

            Assert.Equal("1 Kb", sut.FormatLegacy(1024));
            Assert.Equal("1024 Kb", sut.FormatLegacy(1048576));
            Assert.Equal("2 Mb", sut.FormatLegacy(2 * 1024 * 1024));
        }

        [Fact]
        public void StorageSizeFormatter_ShouldThrow_WhenBytesNegative()
        {
            var sut = new StorageSizeFormatter();
            Assert.Throws<InvalidOperationException>(() => sut.FormatLegacy(-1));
        }

        [Fact]
        public async Task StoragePageCountReader_ShouldHandleBasicFormats()
        {
            var sut = new StoragePageCountReader(new Mock<ILogger<StoragePageCountReader>>().Object);

            var tempDir = Path.Combine(Path.GetTempPath(), "storage-pagecount-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                var png = Path.Combine(tempDir, "img.png");
                await File.WriteAllBytesAsync(png, [1, 2, 3]);
                var unsupported = Path.Combine(tempDir, "data.bin");
                await File.WriteAllBytesAsync(unsupported, [1, 2]);
                var pdf = Path.Combine(tempDir, "sample.pdf");
                await File.WriteAllTextAsync(pdf, "%PDF-1.4\n/Type /Page\n/Type /Pages\n/Type /Page\n");

                Assert.Equal(1, await sut.TryReadPageCountAsync(png));
                Assert.Null(await sut.TryReadPageCountAsync(unsupported));
                Assert.Equal(2, await sut.TryReadPageCountAsync(pdf));
                await Assert.ThrowsAsync<FileNotFoundException>(() => sut.TryReadPageCountAsync(Path.Combine(tempDir, "missing.pdf")));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task StorageDocumentMetadataAnalyzer_ShouldUsePagesFromFile_WhenAvailable()
        {
            var pageReader = new Mock<IStoragePageCountReader>();
            pageReader.Setup(x => x.TryReadPageCountAsync(It.IsAny<string>())).ReturnsAsync(7);

            var sizeFormatter = new Mock<IStorageSizeFormatter>();
            sizeFormatter.Setup(x => x.FormatLegacy(It.IsAny<long>())).Returns<long>(b => $"{b} bytes");

            var sut = new StorageDocumentMetadataAnalyzer(
                pageReader.Object,
                sizeFormatter.Object,
                new Mock<ILogger<StorageDocumentMetadataAnalyzer>>().Object);

            var tempDir = Path.Combine(Path.GetTempPath(), "storage-meta-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                var fileA = Path.Combine(tempDir, "a.pdf");
                var fileB = Path.Combine(tempDir, "b.pdf");
                await File.WriteAllBytesAsync(fileA, new byte[100]);
                await File.WriteAllBytesAsync(fileB, new byte[50]);

                var context = BuildContext(numeroPaginasDeclaradas: 2);
                var result = await sut.AnalyzeAsync(context, [fileA, fileB]);

                Assert.Equal(1174, result.TotalBytes);
                Assert.Equal("1174 bytes", result.TamanoLegacy);
                Assert.Equal(".PDF", result.Formato);
                Assert.Equal(7, result.NumeroPaginas);
                Assert.True(result.PaginasCalculadasDesdeArchivo);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task StorageDocumentMetadataAnalyzer_ShouldFallbackToDeclaredPages_WhenFilePagesUnavailable()
        {
            var pageReader = new Mock<IStoragePageCountReader>();
            pageReader.Setup(x => x.TryReadPageCountAsync(It.IsAny<string>())).ReturnsAsync((int?)null);

            var sut = new StorageDocumentMetadataAnalyzer(
                pageReader.Object,
                new StorageSizeFormatter(),
                new Mock<ILogger<StorageDocumentMetadataAnalyzer>>().Object);

            var tempDir = Path.Combine(Path.GetTempPath(), "storage-meta-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                var fileA = Path.Combine(tempDir, "a.pdf");
                await File.WriteAllBytesAsync(fileA, new byte[10]);

                var context = BuildContext(numeroPaginasDeclaradas: 3);
                var result = await sut.AnalyzeAsync(context, [fileA]);

                Assert.Equal(3, result.NumeroPaginas);
                Assert.False(result.PaginasCalculadasDesdeArchivo);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task StorageDocumentMetadataAnalyzer_ShouldThrow_WhenPagesCannotBeResolved()
        {
            var pageReader = new Mock<IStoragePageCountReader>();
            pageReader.Setup(x => x.TryReadPageCountAsync(It.IsAny<string>())).ReturnsAsync((int?)null);

            var sut = new StorageDocumentMetadataAnalyzer(
                pageReader.Object,
                new StorageSizeFormatter(),
                new Mock<ILogger<StorageDocumentMetadataAnalyzer>>().Object);

            var tempDir = Path.Combine(Path.GetTempPath(), "storage-meta-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                var fileA = Path.Combine(tempDir, "a.pdf");
                await File.WriteAllBytesAsync(fileA, new byte[10]);

                var context = BuildContext(numeroPaginasDeclaradas: 0, numeroPaginasDocumento: 0);
                await Assert.ThrowsAsync<InvalidOperationException>(() => sut.AnalyzeAsync(context, [fileA]));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        private static StorageContext BuildContext(int numeroPaginasDeclaradas, int numeroPaginasDocumento = 1)
        {
            return new StorageContext
            {
                DefaultDbAlias = "da",
                Usuario = "user",
                UsuarioId = 1,
                RequestId = "req-1",
                NombreGabinete = "gab",
                RutaTemporalId = "tmp",
                NombreDocumento = "doc.pdf",
                ArchivosTemporales = ["tmp-1"],
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp",
                    NombreDocumento = "doc.pdf",
                    RequestId = "req-1",
                    NumeroPaginasDeclaradas = numeroPaginasDeclaradas,
                    Documentos =
                    [
                        new DocumentoEntradaDto
                        {
                            IdDocumento = "1",
                            ArchivoTemporalId = "tmp-1",
                            Extension = "pdf",
                            NumeroPaginas = numeroPaginasDocumento
                        }
                    ]
                }
            };
        }
    }
}
