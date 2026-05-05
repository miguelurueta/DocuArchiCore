using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.SystemOptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Options;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageOptionsResolverTests
    {
        [Fact]
        public async Task ResolveAsync_ShouldThrow_WhenNombreGabineteIsEmpty()
        {
            var sut = new StorageOptionsResolver(
                new Mock<IStorageSystemOptionsRepository>().Object,
                new Mock<ILogger<StorageOptionsResolver>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => sut.ResolveAsync("", "da"));
        }

        [Fact]
        public async Task ResolveAsync_ShouldThrow_WhenAliasIsEmpty()
        {
            var sut = new StorageOptionsResolver(
                new Mock<IStorageSystemOptionsRepository>().Object,
                new Mock<ILogger<StorageOptionsResolver>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => sut.ResolveAsync("gab", ""));
        }

        [Fact]
        public async Task ResolveAsync_ShouldThrow_WhenSystem1RowDoesNotExist()
        {
            var repo = new Mock<IStorageSystemOptionsRepository>();
            repo.Setup(r => r.GetOptionsAsync("gab", "da"))
                .ReturnsAsync((StorageSystemOptionsModel?)null);

            var sut = new StorageOptionsResolver(repo.Object, new Mock<ILogger<StorageOptionsResolver>>().Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ResolveAsync("gab", "da"));
        }

        [Theory]
        [InlineData(1, 0, 0, true, false, false)]
        [InlineData(0, 1, 0, false, true, false)]
        [InlineData(0, 0, 1, false, false, true)]
        public async Task ResolveAsync_ShouldMapNumericFlagsToBooleans(
            int inventario,
            int trd,
            int unidad,
            bool expectedInventario,
            bool expectedTrd,
            bool expectedUnidad)
        {
            var repo = new Mock<IStorageSystemOptionsRepository>();
            repo.Setup(r => r.GetOptionsAsync("gab", "da"))
                .ReturnsAsync(new StorageSystemOptionsModel
                {
                    NombreGabinete = "gab",
                    AplicaInventarioDocumental = inventario,
                    AplicaTablaRetencion = trd,
                    AplicaUnidadConservacion = unidad
                });

            var sut = new StorageOptionsResolver(repo.Object, new Mock<ILogger<StorageOptionsResolver>>().Object);

            var result = await sut.ResolveAsync("gab", "da");

            Assert.Equal(expectedInventario, result.AplicaInventarioDocumental);
            Assert.Equal(expectedTrd, result.AplicaTrd);
            Assert.Equal(expectedUnidad, result.AplicaUnidadConservacion);
        }
    }
}
