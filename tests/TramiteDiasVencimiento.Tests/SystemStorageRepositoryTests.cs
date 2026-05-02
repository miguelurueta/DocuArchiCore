using System.Data;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.SystemStorage;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class SystemStorageRepositoryTests
    {
        [Fact]
        public async Task LockByGabineteAsync_ShouldUseDapperCrudEngineWithForUpdate()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;

            dapper.Setup(x => x.GetAllBeginTransAsync<SystemStorageRow>(
                    It.IsAny<QueryOptions>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<bool>()))
                .Callback<QueryOptions, IDbConnection, IDbTransaction, bool>((o, _, _, _) => captured = o)
                .ReturnsAsync(new QueryResult<SystemStorageRow>
                {
                    Success = true,
                    Data = new[] { new SystemStorageRow { Disco = 1, ProxId = 2, TamDisc = 572523149, NumCarp = 1, NumPagCarp = 1 } },
                    ErrorMessage = "YES"
                });

            var repo = new SystemStorageRepository(dapper.Object);
            await repo.LockByGabineteAsync("gab", Mock.Of<IDbConnection>(), Mock.Of<IDbTransaction>());

            Assert.NotNull(captured);
            Assert.Equal("system1", captured!.TableName);
            Assert.Equal(QueryLockMode.ForUpdate, captured.LockMode);
            Assert.Equal("gab", captured.Filters["nombre"]);
        }

        [Fact]
        public async Task UpdateReservationAsync_ShouldUseUpdateBeginTransAsync()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;
            dapper.Setup(x => x.UpdateBeginTransAsync(It.IsAny<QueryOptions>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>(), It.IsAny<string>()))
                .Callback<QueryOptions, IDbConnection, IDbTransaction, string>((o, _, _, _) => captured = o)
                .ReturnsAsync(new QueryResult<bool> { Success = true, TotalRecords = 1, ErrorMessage = "YES" });

            var repo = new SystemStorageRepository(dapper.Object);
            var reservation = new StorageIdentityReservationResult
            {
                Identity = new StorageIdentityModel { IdAlmacen = 11, Disco = 1, Carpeta = 2, NumeroPaginasCarpeta = 3 },
                PreviousProxId = 10,
                NewProxId = 11,
                PreviousFolder = 1,
                NewFolder = 2,
                PreviousFolderPages = 1,
                NewFolderPages = 3,
                TamDisc = 572523149
            };

            var rows = await repo.UpdateReservationAsync("gab", reservation, Mock.Of<IDbConnection>(), Mock.Of<IDbTransaction>());

            Assert.Equal(1, rows);
            Assert.NotNull(captured);
            Assert.Equal(11L, captured!.UpdateValues["proxid"]);
            Assert.Equal(10L, captured.Filters["proxid"]);
        }
    }
}
