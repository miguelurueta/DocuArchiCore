using System.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Inventario;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Metadata;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Expediente;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Disk;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Expediente;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Gabinete;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.IndiceElectronico;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Inventario;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Identity;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Inventario;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Naming;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Physical;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Expediente;
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
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var gabineteRepo = BuildGabineteRepository();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 4, workflowTaskId: null);
            var reservation = BuildReservation();
            var status = new DiskQuotaStatusModel
            {
                Disco = 2,
                NombreGabinete = "gab",
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
                Mock.Of<ILogger<StorageTransactionCoordinator>>(),
                gabineteStorageRepository: gabineteRepo.Object);

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
            var gabineteRepo = BuildGabineteRepository();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 2, workflowTaskId: null);
            var reservation = BuildReservation();

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", NumeroImagenes = 3, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ThrowsAsync(new StorageTransactionException("disk update failed"));

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>(),
                gabineteStorageRepository: gabineteRepo.Object);

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
            var gabineteRepo = BuildGabineteRepository();
            var workflowLogService = new Mock<IWorkflowStorageLogService>();
            var namingService = new Mock<IStorageNamingService>();
            var pathService = new Mock<IStoragePhysicalPathService>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 1, workflowTaskId: 77);
            var reservation = BuildReservation();

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", NumeroImagenes = 8, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);
            namingService.Setup(x => x.Generate(
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>()))
                .Returns(new StorageNamingResult
                {
                    NombreArchivoPrincipal = "DIG00000011.pdf",
                    NombreXml = "FXL00000011.xml",
                    SegundoNombre = "DIG00000011.pdf"
                });
            pathService.Setup(x => x.ResolveAsync(context, reservation.Identity))
                .ReturnsAsync(new MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical.StoragePhysicalPathModel
                {
                    StorageRoot = @"C:\storage",
                    NombreGabinete = "gab",
                    Disco = 2,
                    Carpeta = 1,
                    RutaGabineteDisco = @"C:\storage\gab2",
                    CarpetaLegacy = "00001",
                    RutaFinal = @"C:\storage\gab2\00001"
                });
            workflowLogService.Setup(x => x.ExecuteAsync(
                    context,
                    reservation.Identity,
                    It.IsAny<StorageNamingResult>(),
                    It.IsAny<MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical.StoragePhysicalPathModel>(),
                    connection.Object,
                    transaction.Object))
                .Returns(Task.CompletedTask);

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>(),
                gabineteStorageRepository: gabineteRepo.Object,
                workflowStorageLogService: workflowLogService.Object,
                namingService: namingService.Object,
                storagePhysicalPathService: pathService.Object);

            var result = await coordinator.ExecuteAsync(context);

            Assert.True(result.WorkflowLogInserted);
            workflowLogService.Verify(x => x.ExecuteAsync(
                context,
                reservation.Identity,
                It.IsAny<StorageNamingResult>(),
                It.IsAny<MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical.StoragePhysicalPathModel>(),
                connection.Object,
                transaction.Object), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldInsertInventario_WhenOptionApplies()
        {
            var dbFactory = new Mock<IDbConnectionFactory>();
            var identityAllocator = new Mock<IStorageIdentityAllocator>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var gabineteRepo = BuildGabineteRepository();
            var inventarioBuilder = new Mock<IInventarioDocumentalBuilder>();
            var inventarioRepo = new Mock<IInventarioDocumentalRepository>();
            var namingService = new Mock<IStorageNamingService>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 3, workflowTaskId: null);
            context.ResolvedOptions = new StorageResolvedOptionsModel
            {
                NombreGabinete = "gab",
                AplicaInventarioDocumental = true
            };
            context.PhysicalMetadata = new StorageDocumentPhysicalMetadata
            {
                TotalBytes = 1200,
                TamanoLegacy = "1.17 Kb",
                Formato = ".PDF",
                NumeroPaginas = 3,
                PaginasCalculadasDesdeArchivo = true
            };

            var reservation = BuildReservation();

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", NumeroImagenes = 3, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);
            namingService.Setup(x => x.Generate(11, ".PDF", It.IsAny<string?>()))
                .Returns(new StorageNamingResult { NombreArchivoPrincipal = "DIG00000011.pdf", NombreXml = "FXL00000011.xml", SegundoNombre = "DIG00000011.pdf" });
            inventarioBuilder.Setup(x => x.Build(context, reservation.Identity, It.IsAny<StorageNamingResult>()))
                .Returns(new InventarioDocumentalBuildResult
                {
                    ShouldInsert = true,
                    Model = new InventarioDocumentalInsertModel
                    {
                        IdUsuarioGestion = 1,
                        IdEmpresaDocumento = 10,
                        IdDocumentoDocuarchiAlmacen = 11,
                        NombreGabinete = "gab",
                        NumeroFolios = 3,
                        Tamano = "1.17 Kb",
                        Formato = ".PDF"
                    }
                });
            inventarioRepo.Setup(x => x.InsertAsync(It.IsAny<InventarioDocumentalInsertModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(321);

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>(),
                gabineteStorageRepository: gabineteRepo.Object,
                workflowStorageLogService: null,
                inventarioBuilder: inventarioBuilder.Object,
                inventarioRepository: inventarioRepo.Object,
                namingService: namingService.Object);

            var result = await coordinator.ExecuteAsync(context);

            Assert.Equal(321, result.IdRegistroProduccionDocumental);
            inventarioRepo.Verify(x => x.InsertAsync(It.IsAny<InventarioDocumentalInsertModel>(), connection.Object, transaction.Object), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSkipInventario_WhenOptionDisabled()
        {
            var dbFactory = new Mock<IDbConnectionFactory>();
            var identityAllocator = new Mock<IStorageIdentityAllocator>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var gabineteRepo = BuildGabineteRepository();
            var inventarioBuilder = new Mock<IInventarioDocumentalBuilder>();
            var inventarioRepo = new Mock<IInventarioDocumentalRepository>();
            var namingService = new Mock<IStorageNamingService>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 3, workflowTaskId: null);
            context.ResolvedOptions = new StorageResolvedOptionsModel
            {
                NombreGabinete = "gab",
                AplicaInventarioDocumental = false
            };

            var reservation = BuildReservation();

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", NumeroImagenes = 3, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>(),
                gabineteStorageRepository: gabineteRepo.Object,
                workflowStorageLogService: null,
                inventarioBuilder: inventarioBuilder.Object,
                inventarioRepository: inventarioRepo.Object,
                namingService: namingService.Object);

            var result = await coordinator.ExecuteAsync(context);

            Assert.Null(result.IdRegistroProduccionDocumental);
            inventarioRepo.Verify(x => x.InsertAsync(It.IsAny<InventarioDocumentalInsertModel>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()), Times.Never);
        }


        [Fact]
        public async Task ExecuteAsync_ShouldRunExpedienteUnidadService_WhenOptionApplies()
        {
            var dbFactory = new Mock<IDbConnectionFactory>();
            var identityAllocator = new Mock<IStorageIdentityAllocator>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var gabineteRepo = BuildGabineteRepository();
            var expedienteUnidadService = new Mock<IExpedienteUnidadLegacyService>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 2, workflowTaskId: null);
            context.ResolvedOptions = new StorageResolvedOptionsModel
            {
                NombreGabinete = "gab",
                AplicaUnidadConservacion = true
            };
            context.PhysicalMetadata = new StorageDocumentPhysicalMetadata
            {
                TotalBytes = 100,
                TamanoLegacy = "0.1 Kb",
                Formato = ".PDF",
                NumeroPaginas = 2,
                PaginasCalculadasDesdeArchivo = true
            };

            var reservation = BuildReservation();

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", NumeroImagenes = 3, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);
            expedienteUnidadService.Setup(x => x.ExecuteAsync(context, connection.Object, transaction.Object))
                .ReturnsAsync(new ExpedienteUnidadLegacyResult
                {
                    Ejecutado = true,
                    TieneExpediente = true,
                    IdTipoUnidadDocumental = 2,
                    NumeroFolios = 2,
                    UnidadConservaTipo = UnidadConservaTipoEnum.Electronico
                });

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>(),
                gabineteStorageRepository: gabineteRepo.Object,
                expedienteUnidadLegacyService: expedienteUnidadService.Object);

            var result = await coordinator.ExecuteAsync(context);

            Assert.NotNull(result.ExpedienteUnidadResult);
            Assert.True(result.ExpedienteUnidadResult!.Ejecutado);
            expedienteUnidadService.Verify(x => x.ExecuteAsync(context, connection.Object, transaction.Object), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldInsertLogicalIndex_WhenExpedienteAppliesAndInventarioExists()
        {
            var dbFactory = new Mock<IDbConnectionFactory>();
            var identityAllocator = new Mock<IStorageIdentityAllocator>();
            var diskRepo = new Mock<IStorageDiskQuotaRepository>();
            var gabineteRepo = BuildGabineteRepository();
            var inventarioBuilder = new Mock<IInventarioDocumentalBuilder>();
            var inventarioRepo = new Mock<IInventarioDocumentalRepository>();
            var namingService = new Mock<IStorageNamingService>();
            var pathService = new Mock<IStoragePhysicalPathService>();
            var expedienteUnidadService = new Mock<IExpedienteUnidadLegacyService>();
            var expedienteRepo = new Mock<IExpedienteRepository>();
            var indiceCalculator = new Mock<IIndiceElectronicoCalculator>();
            var indiceBuilder = new Mock<IIndiceElectronicoBuilder>();
            var indiceRepo = new Mock<IIndiceElectronicoRepository>();
            var connection = new Mock<IDbConnection>();
            var transaction = new Mock<IDbTransaction>();

            var context = BuildContext(numeroPaginasDeclaradas: 2, workflowTaskId: null);
            var baseCommand = context.Command!;
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = baseCommand.NombreGabinete,
                RutaTemporalId = baseCommand.RutaTemporalId,
                NombreDocumento = baseCommand.NombreDocumento,
                RequestId = baseCommand.RequestId,
                Documentos = baseCommand.Documentos,
                CamposIndexacion = baseCommand.CamposIndexacion,
                Inventario = baseCommand.Inventario,
                Trd = baseCommand.Trd,
                Workflow = baseCommand.Workflow,
                FullText = baseCommand.FullText,
                NumeroPaginasDeclaradas = baseCommand.NumeroPaginasDeclaradas,
                TipoAlmacenamiento = baseCommand.TipoAlmacenamiento,
                EvaluarCamposObligatorios = baseCommand.EvaluarCamposObligatorios,
                Expediente = new ExpedienteStorageDto
                {
                    IdExpediente = 77,
                    IdClaseDocumento = 9
                }
            };
            context.ResolvedOptions = new StorageResolvedOptionsModel
            {
                NombreGabinete = "gab",
                AplicaInventarioDocumental = true,
                AplicaUnidadConservacion = true
            };
            context.PhysicalMetadata = new StorageDocumentPhysicalMetadata
            {
                TotalBytes = 100,
                TamanoLegacy = "0.1 Kb",
                Formato = ".PDF",
                NumeroPaginas = 2,
                PaginasCalculadasDesdeArchivo = true
            };

            var reservation = BuildReservation();

            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.BeginTransaction(IsolationLevel.Serializable)).Returns(transaction.Object);
            dbFactory.Setup(x => x.GetOpenConnectionAsync("db")).ReturnsAsync(connection.Object);
            identityAllocator.Setup(x => x.ReserveAsync(context, connection.Object, transaction.Object)).ReturnsAsync(reservation);
            diskRepo.Setup(x => x.LockDiskStatusAsync("gab", 2, connection.Object, transaction.Object))
                .ReturnsAsync(new DiskQuotaStatusModel { Disco = 2, NombreGabinete = "gab", NumeroImagenes = 3, NumPagCarp = 2 });
            diskRepo.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(1);
            namingService.Setup(x => x.Generate(11, ".PDF", It.IsAny<string?>()))
                .Returns(new StorageNamingResult { NombreArchivoPrincipal = "DIG00000011.pdf", NombreXml = "FXL00000011.xml", SegundoNombre = "DIG00000011.pdf" });
            pathService.Setup(x => x.ResolveAsync(context, reservation.Identity))
                .ReturnsAsync(new MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical.StoragePhysicalPathModel
                {
                    StorageRoot = @"D:\imagenes\discos",
                    NombreGabinete = "gab",
                    Disco = 2,
                    Carpeta = 1,
                    RutaGabineteDisco = @"D:\imagenes\discos\gab2",
                    CarpetaLegacy = "00001",
                    RutaFinal = @"D:\imagenes\discos\gab2\00001"
                });
            inventarioBuilder.Setup(x => x.Build(context, reservation.Identity, It.IsAny<StorageNamingResult>()))
                .Returns(new InventarioDocumentalBuildResult
                {
                    ShouldInsert = true,
                    Model = new InventarioDocumentalInsertModel
                    {
                        IdUsuarioGestion = 1,
                        IdEmpresaDocumento = 10,
                        IdDocumentoDocuarchiAlmacen = 11,
                        NombreGabinete = "gab",
                        NumeroFolios = 2,
                        Tamano = "0.1 Kb",
                        Formato = ".PDF"
                    }
                });
            inventarioRepo.Setup(x => x.InsertAsync(It.IsAny<InventarioDocumentalInsertModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(321);
            expedienteUnidadService.Setup(x => x.ExecuteAsync(context, connection.Object, transaction.Object))
                .ReturnsAsync(new ExpedienteUnidadLegacyResult
                {
                    Ejecutado = true,
                    TieneExpediente = true,
                    NumeroFolios = 2,
                    IdTipoUnidadDocumental = 2,
                    UnidadConservaTipo = UnidadConservaTipoEnum.Electronico
                });
            expedienteRepo.Setup(x => x.LockExpedienteAsync(77, connection.Object, transaction.Object))
                .ReturnsAsync(new ExpedienteInfoModel
                {
                    IdExpediente = 77,
                    EstadoExpediente = 1,
                    EstadoExpedienteElectronico = 2,
                    NumeroFoliosContenidos = 10,
                    OrdenIndice = 3,
                    UltimaPaginaIndice = 25
                });
            expedienteRepo.Setup(x => x.UpdateIndiceAsync(
                    It.IsAny<ExpedienteInfoModel>(),
                    It.IsAny<IndiceElectronicoCalculationResult>(),
                    connection.Object,
                    transaction.Object))
                .ReturnsAsync(1);
            indiceCalculator.Setup(x => x.Calculate(It.IsAny<ExpedienteInfoModel>(), 2))
                .Returns(new IndiceElectronicoCalculationResult
                {
                    NuevoOrden = 4,
                    PaginaInicial = 26,
                    PaginaFinal = 27,
                    NumeroFolios = 2
                });
            indiceBuilder.Setup(x => x.Build(
                    context,
                    321,
                    It.IsAny<ExpedienteInfoModel>(),
                    It.IsAny<IndiceElectronicoCalculationResult>(),
                    It.IsAny<StorageNamingResult>(),
                    It.IsAny<MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical.StoragePhysicalPathModel>()))
                .Returns(new IndiceElectronicoInsertModel
                {
                    IdRegistroProduccionDocumental = 321,
                    IdExpediente = 77,
                    NombreDocumento = "doc",
                    TipologiaDocumental = "10",
                    FechaDeclaracionDocumento = DateTime.UtcNow,
                    FechaIncorporacionDocumento = DateTime.UtcNow,
                    ValorHuella = "ABC",
                    OrdenDocumentoExpedicion = 4,
                    PaginaInicial = 26,
                    PaginaFinal = 27,
                    RutaDocumento = "tmp/f1",
                    NumeroFolios = 2
                });
            indiceRepo.Setup(x => x.InsertAsync(It.IsAny<IndiceElectronicoInsertModel>(), connection.Object, transaction.Object))
                .ReturnsAsync(999);

            var coordinator = new StorageTransactionCoordinator(
                dbFactory.Object,
                identityAllocator.Object,
                diskRepo.Object,
                Mock.Of<ILogger<StorageTransactionCoordinator>>(),
                gabineteStorageRepository: gabineteRepo.Object,
                inventarioBuilder: inventarioBuilder.Object,
                inventarioRepository: inventarioRepo.Object,
                namingService: namingService.Object,
                storagePhysicalPathService: pathService.Object,
                expedienteUnidadLegacyService: expedienteUnidadService.Object,
                expedienteRepository: expedienteRepo.Object,
                indiceElectronicoCalculator: indiceCalculator.Object,
                indiceElectronicoBuilder: indiceBuilder.Object,
                indiceElectronicoRepository: indiceRepo.Object);

            var result = await coordinator.ExecuteAsync(context);

            Assert.True(result.Success);
            indiceRepo.Verify(x => x.InsertAsync(It.IsAny<IndiceElectronicoInsertModel>(), connection.Object, transaction.Object), Times.Once);
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

        private static Mock<IGabineteStorageRepository> BuildGabineteRepository()
        {
            var mock = new Mock<IGabineteStorageRepository>();
            mock.Setup(x => x.InsertAsync(
                    It.IsAny<GabineteInsertModel>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>()))
                .ReturnsAsync(1);
            mock.Setup(x => x.UpdateByIdAsync(
                    It.IsAny<string>(),
                    It.IsAny<long>(),
                    It.IsAny<IReadOnlyDictionary<string, object?>>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>()))
                .ReturnsAsync(1);
            return mock;
        }
    }
}


