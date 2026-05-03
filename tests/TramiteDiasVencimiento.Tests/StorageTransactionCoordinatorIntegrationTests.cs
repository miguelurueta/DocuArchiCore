using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Repository.DataAccess;
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

namespace TramiteDiasVencimiento.Tests;

public sealed class StorageTransactionCoordinatorIntegrationTests
{
    [Fact]
    public async Task ExecuteAsync_ConcurrentRequests_ShouldKeepTransactionBoundariesPerRequest()
    {
        var txFactory = new TrackingDbConnectionFactory();
        var identityAllocator = BuildIdentityAllocator();
        var diskRepo = BuildDiskRepo();
        var dapper = BuildDapperEngine();
        var expedienteRepo = new Mock<IExpedienteRepository>();
        var unidadRepo = new Mock<IUnidadConservacionRepository>();
        var indiceRepo = new Mock<IIndiceElectronicoRepository>();
        var indiceCalculator = new Mock<IIndiceElectronicoCalculator>();
        var indiceBuilder = new Mock<IIndiceElectronicoBuilder>();
        var workflowRepo = new Mock<IWorkflowStorageLogRepository>();
        var workflowBuilder = new Mock<IWorkflowStorageLogBuilder>();

        var gabineteRepo = new GabineteStorageRepository(dapper.Object, Mock.Of<ILogger<GabineteStorageRepository>>());
        var inventarioRepo = new InventarioDocumentalRepository(dapper.Object, Mock.Of<ILogger<InventarioDocumentalRepository>>());
        var coordinator = new StorageTransactionCoordinator(
            txFactory,
            identityAllocator.Object,
            gabineteRepo,
            inventarioRepo,
            expedienteRepo.Object,
            unidadRepo.Object,
            indiceRepo.Object,
            indiceCalculator.Object,
            indiceBuilder.Object,
            workflowRepo.Object,
            workflowBuilder.Object,
            diskRepo.Object,
            Mock.Of<ILogger<StorageTransactionCoordinator>>());

        var tasks = Enumerable.Range(1, 20).Select(i => coordinator.ExecuteAsync(BuildContext($"RAD-{i:D4}", $"req-{i:D4}")));
        var results = await Task.WhenAll(tasks);

        Assert.Equal(20, results.Length);
        Assert.All(results, r => Assert.True(r.Success));
        Assert.Equal(20, results.Select(r => r.IdRegistroProduccionDocumental).Distinct().Count());

        Assert.Equal(20, txFactory.Transactions.Count);
        Assert.All(txFactory.Transactions.Values, tx =>
        {
            Assert.True(tx.Committed);
            Assert.False(tx.RolledBack);
        });

        Assert.All(dapper.InvocationMap.Values, tables =>
        {
            Assert.Contains("gab", tables);
            Assert.Contains("registro_producion_documental", tables);
            Assert.Equal(2, tables.Count);
        });
    }

    [Fact]
    public async Task ExecuteAsync_ConcurrentMixedResult_ShouldRollbackOnlyFailingTransaction()
    {
        var txFactory = new TrackingDbConnectionFactory();
        var identityAllocator = BuildIdentityAllocator();
        var diskRepo = BuildDiskRepo();
        var dapper = BuildDapperEngine(radicadoToFail: "RAD-FAIL");
        var expedienteRepo = new Mock<IExpedienteRepository>();
        var unidadRepo = new Mock<IUnidadConservacionRepository>();
        var indiceRepo = new Mock<IIndiceElectronicoRepository>();
        var indiceCalculator = new Mock<IIndiceElectronicoCalculator>();
        var indiceBuilder = new Mock<IIndiceElectronicoBuilder>();
        var workflowRepo = new Mock<IWorkflowStorageLogRepository>();
        var workflowBuilder = new Mock<IWorkflowStorageLogBuilder>();

        var gabineteRepo = new GabineteStorageRepository(dapper.Object, Mock.Of<ILogger<GabineteStorageRepository>>());
        var inventarioRepo = new InventarioDocumentalRepository(dapper.Object, Mock.Of<ILogger<InventarioDocumentalRepository>>());
        var coordinator = new StorageTransactionCoordinator(
            txFactory,
            identityAllocator.Object,
            gabineteRepo,
            inventarioRepo,
            expedienteRepo.Object,
            unidadRepo.Object,
            indiceRepo.Object,
            indiceCalculator.Object,
            indiceBuilder.Object,
            workflowRepo.Object,
            workflowBuilder.Object,
            diskRepo.Object,
            Mock.Of<ILogger<StorageTransactionCoordinator>>());

        var successTask = coordinator.ExecuteAsync(BuildContext("RAD-OK", "req-ok"));
        var failTask = coordinator.ExecuteAsync(BuildContext("RAD-FAIL", "req-fail"));

        var successResult = await successTask;
        await Assert.ThrowsAsync<StorageTransactionException>(async () => await failTask);

        Assert.True(successResult.Success);
        Assert.NotNull(dapper.FailingTransactionKey);
        Assert.NotNull(dapper.SuccessTransactionKey);

        var failTx = txFactory.Transactions[dapper.FailingTransactionKey!.Value];
        var successTx = txFactory.Transactions[dapper.SuccessTransactionKey!.Value];

        Assert.True(successTx.Committed);
        Assert.False(successTx.RolledBack);
        Assert.False(failTx.Committed);
        Assert.True(failTx.RolledBack);
    }

    private static Mock<IStorageIdentityAllocator> BuildIdentityAllocator()
    {
        var idCounter = 1000;
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
                    TamDisc = 1000
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

    private static DapperHarness BuildDapperEngine(string? radicadoToFail = null)
    {
        var harness = new DapperHarness();
        var idCounter = 4000;

        harness.Mock.Setup(x => x.InsertBeginTrandAsync(
                It.IsAny<QueryOptions>(),
                It.IsAny<object>(),
                It.IsAny<string>(),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<bool>(),
                It.IsAny<string>()))
            .ReturnsAsync((QueryOptions options, object _, string _, IDbConnection _, IDbTransaction tx, bool _, string _) =>
            {
                var txKey = tx.GetHashCode();
                harness.InvocationMap.AddOrUpdate(
                    txKey,
                    _ => new HashSet<string>(StringComparer.OrdinalIgnoreCase) { options.TableName },
                    (_, set) =>
                    {
                        lock (set)
                        {
                            set.Add(options.TableName);
                            return set;
                        }
                    });

                if (string.Equals(options.TableName, "registro_producion_documental", StringComparison.OrdinalIgnoreCase))
                {
                    var radicado = options.ReglasValidacionCampo.TryGetValue("COD_RADICADO", out var value)
                        ? value?.ToString()
                        : null;

                    if (!string.IsNullOrWhiteSpace(radicadoToFail)
                        && string.Equals(radicadoToFail, radicado, StringComparison.OrdinalIgnoreCase))
                    {
                        harness.FailingTransactionKey = txKey;
                        return new QueryResult<int> { Success = false, ErrorMessage = "NO" };
                    }

                    harness.SuccessTransactionKey ??= txKey;
                    return new QueryResult<int>
                    {
                        Success = true,
                        Data = [Interlocked.Increment(ref idCounter)],
                        ErrorMessage = "YES"
                    };
                }

                return new QueryResult<int> { Success = true, TotalRecords = 1, ErrorMessage = "YES" };
            });

        return harness;
    }

    private static StorageContext BuildContext(string radicado, string requestId)
    {
        return new StorageContext
        {
            DefaultDbAlias = "da",
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
                Workflow = null,
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

    private sealed class DapperHarness
    {
        public Mock<IDapperCrudEngine> Mock { get; } = new();
        public ConcurrentDictionary<int, HashSet<string>> InvocationMap { get; } = new();
        public int? FailingTransactionKey { get; set; }
        public int? SuccessTransactionKey { get; set; }
    }

    private sealed class TrackingDbConnectionFactory : IDbConnectionFactory
    {
        public ConcurrentDictionary<int, TrackingDbTransaction> Transactions { get; } = new();

        public IDbConnection GetOpenConnection(string? dbAlias = null)
        {
            var connection = new TrackingDbConnection(Transactions);
            connection.Open();
            return connection;
        }

        public Task<IDbConnection> GetOpenConnectionAsync(string? dbAlias = null)
            => Task.FromResult(GetOpenConnection(dbAlias));

        public string ProviderBsd() => "fake";

        public IEnumerable<string> GetAvailableAliases() => ["da"];
    }

    private sealed class TrackingDbConnection : DbConnection
    {
        private readonly ConcurrentDictionary<int, TrackingDbTransaction> _transactions;
        private ConnectionState _state = ConnectionState.Closed;

        public TrackingDbConnection(ConcurrentDictionary<int, TrackingDbTransaction> transactions)
        {
            _transactions = transactions;
        }

        public override string ConnectionString { get; set; } = "fake";
        public override string Database => "fake";
        public override string DataSource => "fake";
        public override string ServerVersion => "1.0";
        public override ConnectionState State => _state;

        public override void Open() => _state = ConnectionState.Open;
        public override void Close() => _state = ConnectionState.Closed;
        public override void ChangeDatabase(string databaseName) { }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            var tx = new TrackingDbTransaction(this);
            _transactions[tx.GetHashCode()] = tx;
            return tx;
        }

        protected override DbCommand CreateDbCommand()
            => throw new NotSupportedException("No command execution expected in coordinator integration tests.");
    }

    private sealed class TrackingDbTransaction : DbTransaction
    {
        private readonly DbConnection _connection;

        public TrackingDbTransaction(DbConnection connection)
        {
            _connection = connection;
        }

        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public override IsolationLevel IsolationLevel => IsolationLevel.Serializable;

        protected override DbConnection DbConnection => _connection;

        public override void Commit() => Committed = true;

        public override void Rollback() => RolledBack = true;
    }
}
