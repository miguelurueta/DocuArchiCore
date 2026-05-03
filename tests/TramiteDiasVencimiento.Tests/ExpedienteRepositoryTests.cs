using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Expediente;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class ExpedienteRepositoryTests
    {
        [Fact]
        public async Task LockExpedienteAsync_ShouldUseForUpdateQueryOptions()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;

            dapper.Setup(x => x.GetAllBeginTransAsync<ExpedienteInfoModel>(
                    It.IsAny<QueryOptions>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<bool>()))
                .Callback<QueryOptions, IDbConnection, IDbTransaction, bool>((o, _, _, _) => captured = o)
                .ReturnsAsync(new QueryResult<ExpedienteInfoModel>
                {
                    Success = true,
                    Data =
                    [
                        new ExpedienteInfoModel
                        {
                            IdExpediente = 5,
                            IdUnidadConservacion = 7,
                            EstadoExpediente = 1,
                            EstadoExpedienteElectronico = 1,
                            NumeroFoliosContenidos = 20,
                            OrdenIndice = 2,
                            UltimaPaginaIndice = 9,
                            CodigoUnico = "EXP-5"
                        }
                    ]
                });

            var repo = new ExpedienteRepository(dapper.Object, Mock.Of<ILogger<ExpedienteRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var model = await repo.LockExpedienteAsync(5, connection.Object, Mock.Of<IDbTransaction>());

            Assert.NotNull(model);
            Assert.NotNull(captured);
            Assert.Equal("expediente_archivo", captured!.TableName);
            Assert.Equal(QueryLockMode.ForUpdate, captured.LockMode);
            Assert.Equal(5, captured.Filters["ID_EXPEDIENTE"]);
        }

        [Fact]
        public async Task UpdateIndiceAsync_ShouldUseOptimisticFilters()
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

            var repo = new ExpedienteRepository(dapper.Object, Mock.Of<ILogger<ExpedienteRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var rows = await repo.UpdateIndiceAsync(
                new ExpedienteInfoModel
                {
                    IdExpediente = 5,
                    IdUnidadConservacion = 7,
                    EstadoExpediente = 1,
                    EstadoExpedienteElectronico = 1,
                    NumeroFoliosContenidos = 20,
                    OrdenIndice = 2,
                    UltimaPaginaIndice = 9,
                    CodigoUnico = "EXP-5"
                },
                new IndiceElectronicoCalculationResult
                {
                    NuevoOrden = 3,
                    PaginaInicial = 10,
                    PaginaFinal = 12,
                    NumeroFolios = 3
                },
                connection.Object,
                Mock.Of<IDbTransaction>());

            Assert.Equal(1, rows);
            Assert.NotNull(captured);
            Assert.Equal(2, captured!.Filters["ORDEN_INDICE"]);
            Assert.Equal(9, captured.Filters["ULTIMA_PAGINA_INDICE"]);
            Assert.Equal(3, captured.UpdateValues["ORDEN_INDICE"]);
            Assert.Equal(12, captured.UpdateValues["ULTIMA_PAGINA_INDICE"]);
            Assert.Equal(23, captured.UpdateValues["NUMERO_FOLIOS_CONTENIDOS"]);
        }
    }
}
