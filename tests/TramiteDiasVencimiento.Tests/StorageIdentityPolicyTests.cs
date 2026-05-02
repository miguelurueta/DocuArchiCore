using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Identity;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageIdentityPolicyTests
    {
        private readonly StorageIdentityPolicy _policy = new();

        [Fact]
        public void Calculate_ShouldThrow_WhenTamDiscInvalid()
        {
            var row = new SystemStorageRow { Disco = 1, ProxId = 10, TamDisc = 123, NumCarp = 2, NumPagCarp = 10 };

            Assert.Throws<StorageTransactionException>(() => _policy.Calculate(row, 1));
        }

        [Fact]
        public void Calculate_ShouldUseLegacyProxIdPlusOne()
        {
            var row = new SystemStorageRow { Disco = 1, ProxId = 100, TamDisc = 572523149, NumCarp = 2, NumPagCarp = 20 };

            var result = _policy.Calculate(row, 5);

            Assert.Equal(101, result.Identity.IdAlmacen);
            Assert.Equal(101, result.NewProxId);
            Assert.Equal(100, result.PreviousProxId);
        }

        [Fact]
        public void Calculate_ShouldKeepFolder_WhenPageLimitNotExceeded()
        {
            var row = new SystemStorageRow { Disco = 1, ProxId = 10, TamDisc = 4310948432, NumCarp = 3, NumPagCarp = 200 };

            var result = _policy.Calculate(row, 20);

            Assert.Equal(3, result.NewFolder);
            Assert.Equal(220, result.NewFolderPages);
        }

        [Fact]
        public void Calculate_ShouldRotateFolder_WhenPageLimitExceeded()
        {
            var row = new SystemStorageRow { Disco = 1, ProxId = 10, TamDisc = 4310948432, NumCarp = 3, NumPagCarp = 220 };

            var result = _policy.Calculate(row, 20);

            Assert.Equal(4, result.NewFolder);
            Assert.Equal(20, result.NewFolderPages);
        }
    }
}
