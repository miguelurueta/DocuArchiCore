using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.UnidadConservacion;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class UnidadConservacionRepositoryTests
    {
        [Fact]
        public async Task LockAsync_ShouldUseForUpdateQueryOptions()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;

            dapper.Setup(x => x.GetAllBeginTransAsync<UnidadConservacionInfoModel>(
                    It.IsAny<QueryOptions>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<bool>()))
                .Callback<QueryOptions, IDbConnection, IDbTransaction, bool>((o, _, _, _) => captured = o)
                .ReturnsAsync(new QueryResult<UnidadConservacionInfoModel>
                {
                    Success = true,
                    Data =
                    [
                        new UnidadConservacionInfoModel
                        {
                            IdUnidadConservacion = 11,
                            CodigoUnico = "UC-11",
                            EstadoUnidadConservacion = 1,
                            NumeroFolioUnidadConservacion = 30
                        }
                    ]
                });

            var repo = new UnidadConservacionRepository(dapper.Object, Mock.Of<ILogger<UnidadConservacionRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var model = await repo.LockAsync(11, connection.Object, Mock.Of<IDbTransaction>());

            Assert.NotNull(model);
            Assert.NotNull(captured);
            Assert.Equal("unidad_conservacion", captured!.TableName);
            Assert.Equal(QueryLockMode.ForUpdate, captured.LockMode);
            Assert.Equal(11, captured.Filters["ID_UNIDAD_CONSERVACION"]);
        }

        [Fact]
        public async Task UpdateFoliosAsync_ShouldUseOptimisticFilters()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;

            dapper.Setup(x => x.UpdateBeginTransAsync(
                    It.IsAny<QueryOptions>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<string>()))
                .Callback<QueryOptions, IDbConnection, IDbTransaction, string>((o, _, _, _) => captured = o)
                .ReturnsAsync(new QueryResult<bool> { Success = true, TotalRecords = 1 });

            var repo = new UnidadConservacionRepository(dapper.Object, Mock.Of<ILogger<UnidadConservacionRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var rows = await repo.UpdateFoliosAsync(
                new UnidadConservacionInfoModel
                {
                    IdUnidadConservacion = 11,
                    CodigoUnico = "UC-11",
                    EstadoUnidadConservacion = 1,
                    NumeroFolioUnidadConservacion = 30
                },
                new IndiceElectronicoCalculationResult
                {
                    NuevoOrden = 4,
                    PaginaInicial = 31,
                    PaginaFinal = 35,
                    NumeroFolios = 5
                },
                connection.Object,
                Mock.Of<IDbTransaction>());

            Assert.Equal(1, rows);
            Assert.NotNull(captured);
            Assert.Equal(11, captured!.Filters["ID_UNIDAD_CONSERVACION"]);
            Assert.Equal(30, captured.Filters["NUMERO_FOLIO_UNIDAD_CONSERVACION"]);
            Assert.Equal(35, captured.UpdateValues["NUMERO_FOLIO_UNIDAD_CONSERVACION"]);
        }
    }
}
