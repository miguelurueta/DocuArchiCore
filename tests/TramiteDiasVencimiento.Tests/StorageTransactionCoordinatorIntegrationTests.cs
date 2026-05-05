using System.Collections.Concurrent;
using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Disk;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Identity;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Transaction;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class StorageTransactionCoordinatorIntegrationTests
{
    [Fact]
    public async Task ExecuteAsync_ConcurrentRequests_ShouldCommitOneTransactionPerRequest()
    {
        var dbFactory = new Mock<IDbConnectionFactory>();
        var identityAllocator = BuildIdentityAllocator();
        var diskRepo = BuildDiskRepo();
        var txStates = new ConcurrentBag<TxState>();

        dbFactory.Setup(x => x.GetOpenConnectionAsync("db"))
            .ReturnsAsync(() =>
            {
                var txState = new TxState();
                var tx = new Mock<IDbTransaction>();
                tx.Setup(t => t.Commit()).Callback(() => txState.Committed = true);
                tx.Setup(t => t.Rollback()).Callback(() => txState.RolledBack = true);

                var conn = new Mock<IDbConnection>();
                conn.SetupGet(c => c.State).Returns(ConnectionState.Open);
                conn.Setup(c => c.BeginTransaction(IsolationLevel.Serializable)).Returns(tx.Object);
                txStates.Add(txState);
                return conn.Object;
            });

        var coordinator = new StorageTransactionCoordinator(
            dbFactory.Object,
            identityAllocator.Object,
            diskRepo.Object,
            Mock.Of<ILogger<StorageTransactionCoordinator>>());

        var tasks = Enumerable.Range(1, 10)
            .Select(i => coordinator.ExecuteAsync(BuildContext($"req-{i:D2}", $"rad-{i:D2}")));
        var results = await Task.WhenAll(tasks);

        Assert.Equal(10, results.Length);
        Assert.All(results, r => Assert.True(r.Success));
        Assert.Equal(10, txStates.Count);
        Assert.All(txStates, s =>
        {
            Assert.True(s.Committed);
            Assert.False(s.RolledBack);
        });
    }

    [Fact]
    public async Task ExecuteAsync_MixedRequests_ShouldRollbackOnlyFailingTransaction()
    {
        var dbFactory = new Mock<IDbConnectionFactory>();
        var diskRepo = BuildDiskRepo();
        var txStates = new ConcurrentBag<TxState>();

        dbFactory.Setup(x => x.GetOpenConnectionAsync("db"))
            .ReturnsAsync(() =>
            {
                var txState = new TxState();
                var tx = new Mock<IDbTransaction>();
                tx.Setup(t => t.Commit()).Callback(() => txState.Committed = true);
                tx.Setup(t => t.Rollback()).Callback(() => txState.RolledBack = true);

                var conn = new Mock<IDbConnection>();
                conn.SetupGet(c => c.State).Returns(ConnectionState.Open);
                conn.Setup(c => c.BeginTransaction(IsolationLevel.Serializable)).Returns(tx.Object);
                txStates.Add(txState);
                return conn.Object;
            });

        var idCounter = 1000;
        var identityAllocator = new Mock<IStorageIdentityAllocator>();
        identityAllocator
            .Setup(x => x.ReserveAsync(It.IsAny<StorageContext>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync((StorageContext context, IDbConnection _, IDbTransaction _) =>
            {
                if (context.RequestId == "req-fail")
                {
                    throw new StorageTransactionException("forced failure");
                }

                var next = Interlocked.Increment(ref idCounter);
                return new StorageIdentityReservationResult
                {
                    Identity = new StorageIdentityModel
                    {
                        IdAlmacen = next,
                        Disco = 1,
                        Carpeta = 2,
                        NumeroPaginasCarpeta = 5
                    },
                    PreviousProxId = next - 1,
                    NewProxId = next,
                    PreviousFolder = 2,
                    NewFolder = 2,
                    PreviousFolderPages = 3,
                    NewFolderPages = 5,
                    TamDisc = 572523149
                };
            });

        var coordinator = new StorageTransactionCoordinator(
            dbFactory.Object,
            identityAllocator.Object,
            diskRepo.Object,
            Mock.Of<ILogger<StorageTransactionCoordinator>>());

        var successTask = coordinator.ExecuteAsync(BuildContext("req-ok", "rad-ok"));
        var failTask = coordinator.ExecuteAsync(BuildContext("req-fail", "rad-fail"));

        var successResult = await successTask;
        await Assert.ThrowsAsync<StorageTransactionException>(() => failTask);

        Assert.True(successResult.Success);
        Assert.Equal(2, txStates.Count);
        Assert.Equal(1, txStates.Count(s => s.Committed));
        Assert.Equal(1, txStates.Count(s => s.RolledBack));
    }

    private static Mock<IStorageIdentityAllocator> BuildIdentityAllocator()
    {
        var idCounter = 2000;
        var mock = new Mock<IStorageIdentityAllocator>();
        mock.Setup(x => x.ReserveAsync(It.IsAny<StorageContext>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(() =>
            {
                var next = Interlocked.Increment(ref idCounter);
                return new StorageIdentityReservationResult
                {
                    Identity = new StorageIdentityModel
                    {
                        IdAlmacen = next,
                        Disco = 1,
                        Carpeta = 2,
                        NumeroPaginasCarpeta = 5
                    },
                    PreviousProxId = next - 1,
                    NewProxId = next,
                    PreviousFolder = 2,
                    NewFolder = 2,
                    PreviousFolderPages = 3,
                    NewFolderPages = 5,
                    TamDisc = 572523149
                };
            });
        return mock;
    }

    private static Mock<IStorageDiskQuotaRepository> BuildDiskRepo()
    {
        var mock = new Mock<IStorageDiskQuotaRepository>();
        mock.Setup(x => x.LockDiskStatusAsync("gab", 1, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(new DiskQuotaStatusModel
            {
                Disco = 1,
                NombreGabinete = "gab",
                EstadoDisco = "OK",
                NumeroImagenes = 10,
                NumPagCarp = 3
            });
        mock.Setup(x => x.UpdateDiskUsageAsync(It.IsAny<DiskQuotaUpdateModel>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(1);
        return mock;
    }

    private static StorageContext BuildContext(string requestId, string radicado)
    {
        return new StorageContext
        {
            DefaultDbAlias = "db",
            Usuario = "user",
            UsuarioId = 1,
            RequestId = requestId,
            NombreGabinete = "gab",
            RutaTemporalId = "tmp",
            NombreDocumento = "doc.pdf",
            ArchivosTemporales = ["tmp-1"],
            Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = "gab",
                RutaTemporalId = "tmp",
                NombreDocumento = "doc.pdf",
                RequestId = requestId,
                NumeroPaginasDeclaradas = 2,
                Trd = new TrdStorageDto { IdTipoDocumento = 5 },
                Inventario = new InventarioDocumentalDto
                {
                    IdUsuarioGestion = 1,
                    IdEmpresa = 10,
                    Radicado = radicado
                },
                Documentos =
                [
                    new DocumentoEntradaDto
                    {
                        IdDocumento = "1",
                        ArchivoTemporalId = "tmp-1",
                        NumeroPaginas = 2
                    }
                ]
            }
        };
    }

    private sealed class TxState
    {
        public bool Committed { get; set; }
        public bool RolledBack { get; set; }
    }
}
