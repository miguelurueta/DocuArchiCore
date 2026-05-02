using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Identity;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageDiskQuotaPolicyTests
    {
        private readonly StorageDiskQuotaPolicy _policy = new();

        [Fact]
        public void ValidateDiskAvailable_ShouldThrow_WhenStatusNull()
        {
            Assert.Throws<StorageTransactionException>(() => _policy.ValidateDiskAvailable(null));
        }

        [Fact]
        public void ValidateDiskAvailable_ShouldThrow_WhenEstadoDiscoSL()
        {
            var status = new DiskQuotaStatusModel { Disco = 7, EstadoDisco = "SL" };

            Assert.Throws<StorageTransactionException>(() => _policy.ValidateDiskAvailable(status));
        }

        [Fact]
        public void ValidateDiskAvailable_ShouldPass_WhenEstadoDiscoNotSL()
        {
            var status = new DiskQuotaStatusModel { Disco = 7, EstadoDisco = "OK" };

            _policy.ValidateDiskAvailable(status);
        }
    }
}
