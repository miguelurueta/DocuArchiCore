using System.Data;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Disk;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageDiskQuotaRepositoryTests
    {
        [Fact]
        public async Task LockDiskStatusAsync_ShouldUseForUpdateQuery()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;

            dapper.Setup(x => x.GetAllBeginTransAsync<DiskQuotaStatusModel>(
                    It.IsAny<QueryOptions>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<bool>()))
                .Callback<QueryOptions, IDbConnection, IDbTransaction, bool>((o, _, _, _) => captured = o)
                .ReturnsAsync(new QueryResult<DiskQuotaStatusModel>
                {
                    Success = true,
                    Data = new[] { new DiskQuotaStatusModel { Disco = 1, NombreGabinete = "gab" } },
                    ErrorMessage = "YES"
                });

            var repo = new StorageDiskQuotaRepository(dapper.Object);
            await repo.LockDiskStatusAsync("gab", 1, Mock.Of<IDbConnection>(), Mock.Of<IDbTransaction>());

            Assert.NotNull(captured);
            Assert.Equal("disco_detalle", captured!.TableName);
            Assert.Equal(QueryLockMode.ForUpdate, captured.LockMode);
            Assert.Equal(1, captured.Filters["disco"]);
            Assert.Equal("gab", captured.Filters["gabinete"]);
        }

        [Fact]
        public async Task LockDiskStatusAsync_ShouldQueryWithoutEstadoDiscoColumn()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;

            dapper.Setup(x => x.GetAllBeginTransAsync<DiskQuotaStatusModel>(
                    It.IsAny<QueryOptions>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<bool>()))
                .Callback<QueryOptions, IDbConnection, IDbTransaction, bool>((o, _, _, _) => captured = o)
                .ReturnsAsync(new QueryResult<DiskQuotaStatusModel>
                {
                    Success = true,
                    Data = new[]
                    {
                        new DiskQuotaStatusModel
                        {
                            Disco = 1,
                            NombreGabinete = "gab",
                            NumeroImagenes = 10,
                            NumeroImagenesIsNull = false
                        }
                    },
                    ErrorMessage = "YES"
                });

            var repo = new StorageDiskQuotaRepository(dapper.Object);
            var status = await repo.LockDiskStatusAsync("gab", 1, Mock.Of<IDbConnection>(), Mock.Of<IDbTransaction>());

            Assert.NotNull(status);
            Assert.NotNull(captured);
            Assert.DoesNotContain(captured!.Columns, c => c.Contains("ESTADO_DISCO"));
            Assert.DoesNotContain(captured.Columns, c => c.Contains("EstadoDisco"));
        }

        [Fact]
        public async Task UpdateDiskUsageAsync_ShouldUseUpdateBeginTransAsync()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;

            dapper.Setup(x => x.UpdateBeginTransAsync(It.IsAny<QueryOptions>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>(), It.IsAny<string>()))
                .Callback<QueryOptions, IDbConnection, IDbTransaction, string>((o, _, _, _) => captured = o)
                .ReturnsAsync(new QueryResult<bool> { Success = true, TotalRecords = 1, ErrorMessage = "YES" });

            var repo = new StorageDiskQuotaRepository(dapper.Object);
            var rows = await repo.UpdateDiskUsageAsync(new DiskQuotaUpdateModel
            {
                Disco = 1,
                NombreGabinete = "gab",
                NuevoNumeroImagenes = 20,
                NuevoNumPagCarp = 100
            }, Mock.Of<IDbConnection>(), Mock.Of<IDbTransaction>());

            Assert.Equal(1, rows);
            Assert.NotNull(captured);
            Assert.Equal(20, captured!.UpdateValues["NUMERO_IMAGENES"]);
            Assert.Equal(100, captured.UpdateValues["NUMPAG_CARP"]);
        }
    }
}
