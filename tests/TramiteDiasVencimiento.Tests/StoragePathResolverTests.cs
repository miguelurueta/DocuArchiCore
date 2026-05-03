using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Physical;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StoragePathResolverTests
    {
        [Fact]
        public void GetTemporaryFilePath_ShouldThrow_WhenPathTraversalIsUsed()
        {
            var resolver = new StoragePathResolver();

            Assert.Throws<StoragePhysicalException>(() =>
                resolver.GetTemporaryFilePath("tmp-1", "../evil.pdf"));
        }

        [Fact]
        public void GetFinalFolder_ShouldReturnSafePath_WhenInputIsValid()
        {
            var resolver = new StoragePathResolver();

            var path = resolver.GetFinalFolder("gabinete", 2, 5);

            Assert.Contains("docuarchi", path, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("storage-engine", path, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("gabinete", path, StringComparison.OrdinalIgnoreCase);
        }
    }
}
