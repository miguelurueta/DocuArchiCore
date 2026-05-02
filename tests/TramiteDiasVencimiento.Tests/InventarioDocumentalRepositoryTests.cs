using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Inventario;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class InventarioDocumentalRepositoryTests
    {
        [Fact]
        public async Task InsertAsync_ShouldThrow_WhenModelIsInvalid()
        {
            var repo = new InventarioDocumentalRepository(
                new Mock<IDapperCrudEngine>(MockBehavior.Strict).Object,
                Mock.Of<ILogger<InventarioDocumentalRepository>>());

            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var model = BuildModel();
            model = new InventarioInsertModel
            {
                IdUsuarioGestion = model.IdUsuarioGestion,
                IdEmpresa = 0,
                Radicado = model.Radicado,
                FullText = model.FullText,
                NumeroFolios = model.NumeroFolios,
                IdAlmacen = model.IdAlmacen,
                NombreGabinete = model.NombreGabinete,
                Formato = model.Formato,
                Tamano = model.Tamano
            };

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                repo.InsertAsync(model, connection.Object, Mock.Of<IDbTransaction>()));
        }

        [Fact]
        public async Task InsertAsync_ShouldTruncateFullTextAndReturnId()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;

            dapper.Setup(x => x.InsertBeginTrandAsync(
                    It.IsAny<QueryOptions>(),
                    It.IsAny<object>(),
                    It.IsAny<string>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>()))
                .Callback<QueryOptions, object, string, IDbConnection, IDbTransaction, bool, string>((o, _, _, _, _, _, _) => captured = o)
                .ReturnsAsync(new QueryResult<int> { Success = true, Data = new[] { 33 }, ErrorMessage = "YES" });

            var repo = new InventarioDocumentalRepository(dapper.Object, Mock.Of<ILogger<InventarioDocumentalRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var model = BuildModel();
            model = new InventarioInsertModel
            {
                IdUsuarioGestion = model.IdUsuarioGestion,
                IdEmpresa = model.IdEmpresa,
                Radicado = model.Radicado,
                FullText = new string('A', 1_000_100),
                NumeroFolios = model.NumeroFolios,
                IdAlmacen = model.IdAlmacen,
                NombreGabinete = model.NombreGabinete,
                Formato = model.Formato,
                Tamano = model.Tamano
            };

            var id = await repo.InsertAsync(model, connection.Object, Mock.Of<IDbTransaction>());

            Assert.Equal(33L, id);
            Assert.NotNull(captured);
            Assert.Equal("registro_producion_documental", captured!.TableName);
            Assert.Equal(1_000_000, ((string)captured.ReglasValidacionCampo["FULTEXT_DOCUMENTO"]).Length);
        }

        [Fact]
        public async Task InsertAsync_ShouldThrow_WhenGeneratedIdIsInvalid()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            dapper.Setup(x => x.InsertBeginTrandAsync(
                    It.IsAny<QueryOptions>(),
                    It.IsAny<object>(),
                    It.IsAny<string>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new QueryResult<int> { Success = true, Data = new[] { 0 }, ErrorMessage = "YES" });

            var repo = new InventarioDocumentalRepository(dapper.Object, Mock.Of<ILogger<InventarioDocumentalRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                repo.InsertAsync(BuildModel(), connection.Object, Mock.Of<IDbTransaction>()));
        }

        private static InventarioInsertModel BuildModel()
        {
            return new InventarioInsertModel
            {
                IdUsuarioGestion = 1,
                IdEmpresa = 10,
                Radicado = "RAD-1",
                FullText = "texto",
                NumeroFolios = 4,
                IdAlmacen = 100,
                NombreGabinete = "gabinete",
                Formato = "pdf",
                Tamano = "100kb"
            };
        }
    }
}
