using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Disk;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.SystemStorage;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Identity;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageIdentityAllocatorTests
    {
        [Fact]
        public async Task ReserveAsync_ShouldThrow_WhenConnectionClosed()
        {
            var allocator = new StorageIdentityAllocator(
                Mock.Of<ISystemStorageRepository>(),
                Mock.Of<IStorageIdentityPolicy>(),
                Mock.Of<IStorageDiskQuotaRepository>(),
                Mock.Of<IStorageDiskQuotaPolicy>(),
                Mock.Of<ILogger<StorageIdentityAllocator>>());
            var conn = new Mock<IDbConnection>();
            conn.SetupGet(x => x.State).Returns(ConnectionState.Closed);

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                allocator.ReserveAsync(BuildContext(), conn.Object, Mock.Of<IDbTransaction>()));
        }

        [Fact]
        public async Task ReserveAsync_ShouldReserveAndUpdateSystem1_WhenValid()
        {
            var systemRepo = new Mock<ISystemStorageRepository>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var identityPolicy = new Mock<IStorageIdentityPolicy>();
            var diskPolicy = new Mock<IStorageDiskQuotaPolicy>();
            var logger = new Mock<ILogger<StorageIdentityAllocator>>();

            var systemRow = new SystemStorageRow { Disco = 2, ProxId = 10, TamDisc = 572523149, NumCarp = 1, NumPagCarp = 10 };
            var reservation = new StorageIdentityReservationResult
            {
                Identity = new StorageIdentityModel { IdAlmacen = 11, Disco = 2, Carpeta = 1, NumeroPaginasCarpeta = 12 },
                PreviousProxId = 10,
                NewProxId = 11,
                PreviousFolder = 1,
                NewFolder = 1,
                PreviousFolderPages = 10,
                NewFolderPages = 12,
                TamDisc = 572523149
            };

            systemRepo
                .Setup(x => x.LockByGabineteAsync(It.IsAny<string>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(systemRow);
            identityPolicy
                .Setup(x => x.Calculate(systemRow, It.IsAny<int>()))
                .Returns(reservation);
            diskRepo
                .Setup(x => x.LockDiskStatusAsync(It.IsAny<string>(), 2, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", EstadoDisco = "OK" });
            systemRepo
                .Setup(x => x.UpdateReservationAsync(It.IsAny<string>(), reservation, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(1);

            var allocator = new StorageIdentityAllocator(
                systemRepo.Object,
                identityPolicy.Object,
                diskRepo.Object,
                diskPolicy.Object,
                logger.Object);

            var conn = new Mock<IDbConnection>();
            conn.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var result = await allocator.ReserveAsync(BuildContext(), conn.Object, Mock.Of<IDbTransaction>());

            Assert.Equal(11, result.Identity.IdAlmacen);
            systemRepo.Verify(x => x.UpdateReservationAsync("gab", reservation, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()), Times.Once);
            diskPolicy.Verify(x => x.ValidateDiskAvailable(It.IsAny<DiskQuotaStatusModel?>()), Times.Once);
        }

        [Fact]
        public async Task ReserveAsync_ShouldUsePhysicalMetadataPages_WhenPresent()
        {
            var systemRepo = new Mock<ISystemStorageRepository>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var identityPolicy = new Mock<IStorageIdentityPolicy>();
            var diskPolicy = new Mock<IStorageDiskQuotaPolicy>();

            var systemRow = new SystemStorageRow { Disco = 2, ProxId = 10, TamDisc = 572523149, NumCarp = 1, NumPagCarp = 10 };
            var reservation = new StorageIdentityReservationResult
            {
                Identity = new StorageIdentityModel { IdAlmacen = 11, Disco = 2, Carpeta = 1, NumeroPaginasCarpeta = 19 },
                PreviousProxId = 10,
                NewProxId = 11,
                PreviousFolder = 1,
                NewFolder = 1,
                PreviousFolderPages = 10,
                NewFolderPages = 19,
                TamDisc = 572523149
            };

            systemRepo
                .Setup(x => x.LockByGabineteAsync(It.IsAny<string>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(systemRow);
            identityPolicy
                .Setup(x => x.Calculate(systemRow, 9))
                .Returns(reservation);
            diskRepo
                .Setup(x => x.LockDiskStatusAsync(It.IsAny<string>(), 2, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", EstadoDisco = "OK" });
            systemRepo
                .Setup(x => x.UpdateReservationAsync(It.IsAny<string>(), reservation, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(1);

            var allocator = new StorageIdentityAllocator(
                systemRepo.Object,
                identityPolicy.Object,
                diskRepo.Object,
                diskPolicy.Object,
                Mock.Of<ILogger<StorageIdentityAllocator>>());

            var context = BuildContext();
            context.PhysicalMetadata = new MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Metadata.StorageDocumentPhysicalMetadata
            {
                NumeroPaginas = 9
            };

            var conn = new Mock<IDbConnection>();
            conn.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var result = await allocator.ReserveAsync(context, conn.Object, Mock.Of<IDbTransaction>());

            Assert.Equal(11, result.Identity.IdAlmacen);
            identityPolicy.Verify(x => x.Calculate(systemRow, 9), Times.Once);
        }

        private static StorageContext BuildContext()
        {
            return new StorageContext
            {
                DefaultDbAlias = "db",
                Usuario = "u",
                UsuarioId = 1,
                RequestId = "req",
                NombreGabinete = "gab",
                RutaTemporalId = "tmp",
                NombreDocumento = "doc",
                ArchivosTemporales = new[] { "f1" },
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp",
                    NombreDocumento = "doc",
                    RequestId = "req",
                    Documentos = new[]
                    {
                        new DocumentoEntradaDto { IdDocumento = "1", ArchivoTemporalId = "f1", NumeroPaginas = 2 }
                    }
                }
            };
        }
    }
}
