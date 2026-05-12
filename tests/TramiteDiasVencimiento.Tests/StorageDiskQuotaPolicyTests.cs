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
            Assert.Throws<StorageTransactionException>(() => _policy.ValidateDiskAvailable(null, 572523149));
        }

        [Fact]
        public void ValidateDiskAvailable_ShouldThrow_WhenNumeroImagenesIsNull()
        {
            var status = new DiskQuotaStatusModel
            {
                Disco = 7,
                NumeroImagenesIsNull = true
            };

            var ex = Assert.Throws<StorageTransactionException>(() => _policy.ValidateDiskAvailable(status, 572523149));
            Assert.Contains("estado null", ex.Message);
        }

        [Fact]
        public void ValidateDiskAvailable_ShouldThrow_WhenNumeroImagenesIsZero()
        {
            var status = new DiskQuotaStatusModel
            {
                Disco = 7,
                NumeroImagenes = 0
            };

            var ex = Assert.Throws<StorageTransactionException>(() => _policy.ValidateDiskAvailable(status, 572523149));
            Assert.Contains("no esta sincronizado para alamcenar", ex.Message);
        }

        [Fact]
        public void ValidateDiskAvailable_ShouldThrow_WhenLargeDiskExceedsThreshold()
        {
            var status = new DiskQuotaStatusModel
            {
                Disco = 7,
                NumeroImagenes = 80001
            };

            var ex = Assert.Throws<StorageTransactionException>(() => _policy.ValidateDiskAvailable(status, 572523150));
            Assert.Contains("Sobrepaso el limite de capacidad", ex.Message);
        }

        [Fact]
        public void ValidateDiskAvailable_ShouldThrow_WhenSmallDiskExceedsThreshold()
        {
            var status = new DiskQuotaStatusModel
            {
                Disco = 7,
                NumeroImagenes = 7501
            };

            var ex = Assert.Throws<StorageTransactionException>(() => _policy.ValidateDiskAvailable(status, 572523148));
            Assert.Contains("Sobrepaso el limite de capacidad", ex.Message);
        }

        [Fact]
        public void ValidateDiskAvailable_ShouldPass_WhenTamDiscEqualsBoundary()
        {
            var status = new DiskQuotaStatusModel
            {
                Disco = 7,
                NumeroImagenes = 90000
            };

            _policy.ValidateDiskAvailable(status, 572523149);
        }

        [Fact]
        public void ValidateDiskAvailable_ShouldThrow_WhenEstadoDiscoSL()
        {
            var status = new DiskQuotaStatusModel
            {
                Disco = 7,
                EstadoDisco = "SL",
                NumeroImagenes = 1
            };

            var ex = Assert.Throws<StorageTransactionException>(() => _policy.ValidateDiskAvailable(status, 572523149));
            Assert.Contains("Sobrepaso el limite de capacidad", ex.Message);
        }

        [Fact]
        public void ValidateDiskAvailable_ShouldPass_WhenWithinThreshold()
        {
            var status = new DiskQuotaStatusModel
            {
                Disco = 7,
                EstadoDisco = "OK",
                NumeroImagenes = 100
            };

            _policy.ValidateDiskAvailable(status, 572523150);
        }
    }
}
