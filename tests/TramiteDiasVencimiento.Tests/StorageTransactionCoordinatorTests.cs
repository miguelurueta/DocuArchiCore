using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Disk;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Expediente;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Gabinete;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.IndiceElectronico;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Inventario;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.UnidadConservacion;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Workflow;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Expediente;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Identity;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Transaction;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Workflow;
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
            var gabineteRepo = new Mock<IGabineteStorageRepository>();
            var inventarioRepo = new Mock<IInventarioDocumentalRepository>();
            var expedienteRepo = new Mock<IExpedienteRepository>();
            var unidadRepo = new Mock<IUnidadConservacionRepository>();
            var indiceRepo = new Mock<IIndiceElectronicoRepository>();
            var indiceCalculator = new Mock<IIndiceElectronicoCalculator>();
            var indiceBuilder = new Mock<IIndiceElectronicoBuilder>();
            var workflowRepo = new Mock<IWorkflowStorageLogRepository>();
            var workflowBuilder = new Mock<IWorkflowStorageLogBuilder>();
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
            gabineteRepo.Setup(x => x.InsertAsync(It.IsAny<GabineteInsertModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);
            inventarioRepo.Setup(x => x.InsertAsync(It.IsAny<InventarioInsertModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(200L);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object)).ReturnsAsync(status);
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(
                    It.IsAny<DiskQuotaUpdateModel>(),
                    connection.Object,
                    transaction.Object))
                .ReturnsAsync(1);

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                gabineteRepo.Object,
                inventarioRepo.Object,
                expedienteRepo.Object,
                unidadRepo.Object,
                indiceRepo.Object,
                indiceCalculator.Object,
                indiceBuilder.Object,
                workflowRepo.Object,
                workflowBuilder.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>());

            var result = await coordinator.ExecuteAsync(context);

            Assert.True(result.Success);
            Assert.True(result.DiskUsageUpdated);
            Assert.False(result.WorkflowLogInserted);
            Assert.Equal(StorageDocumentState.Reserved, result.Estado);
            Assert.Equal(11, result.IdentityReservation.Identity.IdAlmacen);
            Assert.Equal(200L, result.IdRegistroProduccionDocumental);

            identityAllocator.Verify(x => x.ReserveAsync(context, connection.Object, transaction.Object), Times.Once);
            gabineteRepo.Verify(x => x.InsertAsync(It.IsAny<GabineteInsertModel>(), connection.Object, transaction.Object), Times.Once);
            inventarioRepo.Verify(x => x.InsertAsync(It.IsAny<InventarioInsertModel>(), connection.Object, transaction.Object), Times.Once);
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
            var gabineteRepo = new Mock<IGabineteStorageRepository>();
            var inventarioRepo = new Mock<IInventarioDocumentalRepository>();
            var expedienteRepo = new Mock<IExpedienteRepository>();
            var unidadRepo = new Mock<IUnidadConservacionRepository>();
            var indiceRepo = new Mock<IIndiceElectronicoRepository>();
            var indiceCalculator = new Mock<IIndiceElectronicoCalculator>();
            var indiceBuilder = new Mock<IIndiceElectronicoBuilder>();
            var workflowRepo = new Mock<IWorkflowStorageLogRepository>();
            var workflowBuilder = new Mock<IWorkflowStorageLogBuilder>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 2, workflowTaskId: null);
            var reservation = BuildReservation();

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            gabineteRepo.Setup(x => x.InsertAsync(It.IsAny<GabineteInsertModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);
            inventarioRepo.Setup(x => x.InsertAsync(It.IsAny<InventarioInsertModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(200L);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", EstadoDisco = "OK", NumeroImagenes = 3, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ThrowsAsync(new StorageTransactionException("disk update failed"));

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                gabineteRepo.Object,
                inventarioRepo.Object,
                expedienteRepo.Object,
                unidadRepo.Object,
                indiceRepo.Object,
                indiceCalculator.Object,
                indiceBuilder.Object,
                workflowRepo.Object,
                workflowBuilder.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>());

            await Assert.ThrowsAsync<StorageTransactionException>(() => coordinator.ExecuteAsync(context));

            transaction.Verify(x => x.Rollback(), Times.Once);
            transaction.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldInvokeWorkflowLog_WhenTaskIdIsPositive()
        {
            var dbFactory = new Mock<IDbConnectionFactory>();
            var identityAllocator = new Mock<IStorageIdentityAllocator>();
            var gabineteRepo = new Mock<IGabineteStorageRepository>();
            var inventarioRepo = new Mock<IInventarioDocumentalRepository>();
            var expedienteRepo = new Mock<IExpedienteRepository>();
            var unidadRepo = new Mock<IUnidadConservacionRepository>();
            var indiceRepo = new Mock<IIndiceElectronicoRepository>();
            var indiceCalculator = new Mock<IIndiceElectronicoCalculator>();
            var indiceBuilder = new Mock<IIndiceElectronicoBuilder>();
            var workflowBuilder = new Mock<IWorkflowStorageLogBuilder>();
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
            gabineteRepo.Setup(x => x.InsertAsync(It.IsAny<GabineteInsertModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);
            inventarioRepo.Setup(x => x.InsertAsync(It.IsAny<InventarioInsertModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(200L);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", EstadoDisco = "OK", NumeroImagenes = 8, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);
            var workflowModel = new WorkflowStorageLogModel
            {
                IdAlmacen = 11,
                UsuarioOperacion = "u",
                FechaTransaccion = DateTime.UtcNow.Date,
                RutaDocumento = "tmp/f1",
                NombreGabinete = "gab",
                IdTareaWorkflow = 77,
                IdRutaWorkflow = 9,
                UsuarioPropietario = "u"
            };
            workflowBuilder.Setup(x => x.Build(context, It.IsAny<StorageTransactionResult>()))
                .Returns(workflowModel);
            workflowRepo.Setup(x => x.InsertAsync(workflowModel, connection.Object, transaction.Object))
                .ReturnsAsync(1);

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                gabineteRepo.Object,
                inventarioRepo.Object,
                expedienteRepo.Object,
                unidadRepo.Object,
                indiceRepo.Object,
                indiceCalculator.Object,
                indiceBuilder.Object,
                workflowRepo.Object,
                workflowBuilder.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>());

            var result = await coordinator.ExecuteAsync(context);

            Assert.True(result.WorkflowLogInserted);
            workflowRepo.Verify(x => x.InsertAsync(workflowModel, connection.Object, transaction.Object), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldExecuteExpedienteIndicePhase_WhenExpedienteIsProvided()
        {
            var dbFactory = new Mock<IDbConnectionFactory>();
            var identityAllocator = new Mock<IStorageIdentityAllocator>();
            var gabineteRepo = new Mock<IGabineteStorageRepository>();
            var inventarioRepo = new Mock<IInventarioDocumentalRepository>();
            var expedienteRepo = new Mock<IExpedienteRepository>();
            var unidadRepo = new Mock<IUnidadConservacionRepository>();
            var indiceRepo = new Mock<IIndiceElectronicoRepository>();
            var indiceCalculator = new Mock<IIndiceElectronicoCalculator>();
            var indiceBuilder = new Mock<IIndiceElectronicoBuilder>();
            var workflowRepo = new Mock<IWorkflowStorageLogRepository>();
            var workflowBuilder = new Mock<IWorkflowStorageLogBuilder>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 2, workflowTaskId: null, idExpediente: 99, idUnidadConservacion: 77);
            var reservation = BuildReservation();
            var expediente = new ExpedienteInfoModel
            {
                IdExpediente = 99,
                IdUnidadConservacion = 77,
                EstadoExpediente = 1,
                EstadoExpedienteElectronico = 1,
                NumeroFoliosContenidos = 10,
                OrdenIndice = 2,
                UltimaPaginaIndice = 8,
                CodigoUnico = "EXP-99"
            };
            var unidad = new UnidadConservacionInfoModel
            {
                IdUnidadConservacion = 77,
                CodigoUnico = "UC-77",
                EstadoUnidadConservacion = 1,
                NumeroFolioUnidadConservacion = 10
            };
            var calc = new IndiceElectronicoCalculationResult
            {
                NuevoOrden = 3,
                PaginaInicial = 9,
                PaginaFinal = 10,
                NumeroFolios = 2
            };
            var insertModel = new IndiceElectronicoInsertModel
            {
                IdRegistroProduccionDocumental = 200,
                IdExpediente = 99,
                NombreDocumento = "doc",
                TipologiaDocumental = "10",
                FechaDeclaracionDocumento = DateTime.UtcNow,
                FechaIncorporacionDocumento = DateTime.UtcNow,
                ValorHuella = new string('A', 64),
                OrdenDocumentoExpedicion = 3,
                PaginaInicial = 9,
                PaginaFinal = 10,
                RutaDocumento = "tmp/f1",
                NumeroFolios = 2
            };

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            gabineteRepo.Setup(x => x.InsertAsync(It.IsAny<GabineteInsertModel>(), connection.Object, transaction.Object)).ReturnsAsync(1);
            inventarioRepo.Setup(x => x.InsertAsync(It.IsAny<InventarioInsertModel>(), connection.Object, transaction.Object)).ReturnsAsync(200L);
            expedienteRepo.Setup(x => x.LockExpedienteAsync(99, connection.Object, transaction.Object)).ReturnsAsync(expediente);
            unidadRepo.Setup(x => x.LockAsync(77, connection.Object, transaction.Object)).ReturnsAsync(unidad);
            indiceCalculator.Setup(x => x.Calculate(expediente, 2)).Returns(calc);
            expedienteRepo.Setup(x => x.UpdateIndiceAsync(expediente, calc, connection.Object, transaction.Object)).ReturnsAsync(1);
            unidadRepo.Setup(x => x.UpdateFoliosAsync(unidad, calc, connection.Object, transaction.Object)).ReturnsAsync(1);
            indiceBuilder.Setup(x => x.Build(context, 200L, expediente, calc)).Returns(insertModel);
            indiceRepo.Setup(x => x.InsertAsync(insertModel, connection.Object, transaction.Object)).ReturnsAsync(900L);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", EstadoDisco = "OK", NumeroImagenes = 8, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                gabineteRepo.Object,
                inventarioRepo.Object,
                expedienteRepo.Object,
                unidadRepo.Object,
                indiceRepo.Object,
                indiceCalculator.Object,
                indiceBuilder.Object,
                workflowRepo.Object,
                workflowBuilder.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>());

            var result = await coordinator.ExecuteAsync(context);

            Assert.True(result.Success);
            expedienteRepo.Verify(x => x.LockExpedienteAsync(99, connection.Object, transaction.Object), Times.Once);
            unidadRepo.Verify(x => x.LockAsync(77, connection.Object, transaction.Object), Times.Once);
            indiceRepo.Verify(x => x.InsertAsync(insertModel, connection.Object, transaction.Object), Times.Once);
            transaction.Verify(x => x.Commit(), Times.Once);
        }

        private static StorageContext BuildContext(int numeroPaginasDeclaradas, long? workflowTaskId, int? idExpediente = null, int? idUnidadConservacion = null)
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
                    Expediente = (idExpediente.HasValue || idUnidadConservacion.HasValue)
                        ? new ExpedienteStorageDto
                        {
                            IdExpediente = idExpediente,
                            IdUnidadConservacion = idUnidadConservacion,
                            IdClaseDocumento = 1
                        }
                        : null,
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
