using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Disk;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Identity;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Transaction;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageTransactionCoordinatorTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldCommitAndUpdateDiskQuota_WhenFlowIsValid()
        {
            var dbFactory = new Mock<IDbConnectionFactory>();
            var identityAllocator = new Mock<IStorageIdentityAllocator>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 4, workflowTaskId: null);
            var reservation = BuildReservation();
            var status = new DiskQuotaStatusModel
            {
                Disco = 2,
                NombreGabinete = "gab",
                EstadoDisco = "OK",
                NumeroImagenes = 10,
                NumPagCarp = 5
            };

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object)).ReturnsAsync(status);
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(
                    It.IsAny<DiskQuotaUpdateModel>(),
                    connection.Object,
                    transaction.Object))
                .ReturnsAsync(1);

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>());

            var result = await coordinator.ExecuteAsync(context);

            Assert.True(result.Success);
            Assert.True(result.DiskUsageUpdated);
            Assert.False(result.WorkflowLogInserted);
            Assert.Equal(StorageDocumentState.Reserved, result.Estado);
            Assert.Equal(11, result.IdentityReservation.Identity.IdAlmacen);

            diskRepo.Verify(x => x.UpdateDiskUsageAsync(
                    It.Is<DiskQuotaUpdateModel>(m =>
                        m.Disco == 2 &&
                        m.NombreGabinete == "gab" &&
                        m.NuevoNumeroImagenes == 14 &&
                        m.NuevoNumPagCarp == reservation.NewFolderPages),
                    connection.Object,
                    transaction.Object),
                Times.Once);
            transaction.Verify(x => x.Commit(), Times.Once);
            transaction.Verify(x => x.Rollback(), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRollback_WhenDiskQuotaUpdateFails()
        {
            var dbFactory = new Mock<IDbConnectionFactory>();
            var identityAllocator = new Mock<IStorageIdentityAllocator>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 2, workflowTaskId: null);
            var reservation = BuildReservation();

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", EstadoDisco = "OK", NumeroImagenes = 3, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ThrowsAsync(new StorageTransactionException("disk update failed"));

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>());

            await Assert.ThrowsAsync<StorageTransactionException>(() => coordinator.ExecuteAsync(context));

            transaction.Verify(x => x.Rollback(), Times.Once);
            transaction.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldInsertWorkflowLog_WhenTaskIdIsPositive()
        {
            var dbFactory = new Mock<IDbConnectionFactory>();
            var identityAllocator = new Mock<IStorageIdentityAllocator>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var workflowRepo = new Mock<IWorkflowStorageLogRepository>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 1, workflowTaskId: 77);
            var reservation = BuildReservation();

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", EstadoDisco = "OK", NumeroImagenes = 8, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);
            workflowRepo.Setup(x => x.InsertAsync(context, reservation, connection.Object, transaction.Object))
                .Returns(Task.CompletedTask);

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>(),
                workflowRepo.Object);

            var result = await coordinator.ExecuteAsync(context);

            Assert.True(result.WorkflowLogInserted);
            workflowRepo.Verify(x => x.InsertAsync(context, reservation, connection.Object, transaction.Object), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUsePhysicalMetadataPages_WhenAvailable()
        {
            var dbFactory = new Mock<IDbConnectionFactory>();
            var identityAllocator = new Mock<IStorageIdentityAllocator>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 1, workflowTaskId: null);
            context.PhysicalMetadata = new MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Metadata.StorageDocumentPhysicalMetadata
            {
                NumeroPaginas = 9
            };
            var reservation = BuildReservation();
            var status = new DiskQuotaStatusModel
            {
                Disco = 2,
                NombreGabinete = "gab",
                EstadoDisco = "OK",
                NumeroImagenes = 10,
                NumPagCarp = 5
            };

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object)).ReturnsAsync(status);
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(
                    It.IsAny<DiskQuotaUpdateModel>(),
                    connection.Object,
                    transaction.Object))
                .ReturnsAsync(1);

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>());

            await coordinator.ExecuteAsync(context);

            diskRepo.Verify(x => x.UpdateDiskUsageAsync(
                    It.Is<DiskQuotaUpdateModel>(m =>
                        m.NuevoNumeroImagenes == 19),
                    connection.Object,
                    transaction.Object),
                Times.Once);
        }

        private static StorageContext BuildContext(int numeroPaginasDeclaradas, long? workflowTaskId)
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
                    NumeroPaginasDeclaradas = numeroPaginasDeclaradas,
                    Trd = new TrdStorageDto { IdTipoDocumento = 10 },
                    Inventario = new InventarioDocumentalDto { IdUsuarioGestion = 1, IdEmpresa = 20, Radicado = "RAD-1" },
                    Workflow = workflowTaskId.HasValue ? new WorkflowStorageDto { IdTareaWorkflow = workflowTaskId } : null,
                    Documentos = new[]
                    {
                        new DocumentoEntradaDto { IdDocumento = "1", ArchivoTemporalId = "f1", NumeroPaginas = 2 }
                    }
                }
            };
        }

        private static StorageIdentityReservationResult BuildReservation()
        {
            return new StorageIdentityReservationResult
            {
                Identity = new StorageIdentityModel
                {
                    IdAlmacen = 11,
                    Disco = 2,
                    Carpeta = 1,
                    NumeroPaginasCarpeta = 6
                },
                PreviousProxId = 10,
                NewProxId = 11,
                PreviousFolder = 1,
                NewFolder = 1,
                PreviousFolderPages = 4,
                NewFolderPages = 6,
                TamDisc = 572523149
            };
        }
    }
}
