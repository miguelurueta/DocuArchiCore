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
        public void ResolveSafePath_ShouldReturnSafePath_WhenInputIsValid()
        {
            var resolver = new StoragePathResolver();

            var path = resolver.ResolveSafePath(@"C:\storage-root", @"gab1\00001");

            Assert.Contains("storage-root", path, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("gab1", path, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("00001", path, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ResolveSafePath_ShouldThrow_WhenTraversalEscapesRoot()
        {
            var resolver = new StoragePathResolver();

            Assert.Throws<StoragePhysicalException>(() =>
                resolver.ResolveSafePath(@"C:\storage-root", @"..\outside"));
        }
    }
}
