using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Extension;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Naming;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageNamingServiceTests
    {
        [Theory]
        [InlineData(1, ".pdf", "DIG00000001.pdf", "FXL00000001.xml")]
        [InlineData(10, ".tif", "DIG00000010.tif", "FXL00000010.xml")]
        [InlineData(100, "JPG", "DIG00000100.jpg", "FXL00000100.xml")]
        [InlineData(1000, ".BMP", "DIG00001000.bmp", "FXL00001000.xml")]
        public void Generate_ShouldBuildLegacyNames(long idAlmacen, string extension, string expectedMain, string expectedXml)
        {
            var sut = new StorageNamingService();

            var result = sut.Generate(idAlmacen, extension);

            Assert.Equal(expectedMain, result.NombreArchivoPrincipal);
            Assert.Equal(expectedXml, result.NombreXml);
            Assert.Equal(expectedMain, result.SegundoNombre);
        }

        [Fact]
        public async Task ResolveAsync_ShouldNormalizeExtension_WhenRepositoryReturnsWithoutDot()
        {
            var repository = new Mock<IStorageExtensionRepository>();
            repository.Setup(x => x.GetExtensionAsync(3, "da")).ReturnsAsync("PDF");
            var sut = new StorageExtensionResolver(repository.Object);

            var extension = await sut.ResolveAsync(3, "da");

            Assert.Equal(".pdf", extension);
        }

        [Fact]
        public async Task ResolveAsync_ShouldThrow_WhenRepositoryReturnsNull()
        {
            var repository = new Mock<IStorageExtensionRepository>();
            repository.Setup(x => x.GetExtensionAsync(3, "da")).ReturnsAsync((string?)null);
            var sut = new StorageExtensionResolver(repository.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ResolveAsync(3, "da"));
        }
    }
}
