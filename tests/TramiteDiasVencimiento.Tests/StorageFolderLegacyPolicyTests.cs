using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Physical;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageFolderLegacyPolicyTests
    {
        [Fact]
        public void ResolveFolder_ShouldBuildLegacyRoute_WhenInputIsValid()
        {
            var policy = new StorageFolderLegacyPolicy();

            var result = policy.ResolveFolder(@"D:\DATA\", "GAB_A", 2, 7);

            Assert.Equal("00007", result.CarpetaLegacy);
            Assert.Contains(@"GAB_A2", result.RutaGabineteDisco, StringComparison.OrdinalIgnoreCase);
            Assert.EndsWith(@"GAB_A2\00007", result.RutaFinal, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ResolveFolder_ShouldThrow_WhenDiscoIsInvalid()
        {
            var policy = new StorageFolderLegacyPolicy();
            Assert.Throws<InvalidOperationException>(() => policy.ResolveFolder(@"D:\DATA\", "GAB_A", 0, 7));
        }

        [Fact]
        public void ResolveFolder_ShouldThrow_WhenCarpetaIsInvalid()
        {
            var policy = new StorageFolderLegacyPolicy();
            Assert.Throws<InvalidOperationException>(() => policy.ResolveFolder(@"D:\DATA\", "GAB_A", 2, 0));
        }
    }
}
