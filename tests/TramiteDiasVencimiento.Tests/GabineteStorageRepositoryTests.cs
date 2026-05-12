using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Gabinete;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class GabineteStorageRepositoryTests
    {
        [Fact]
        public async Task InsertAsync_ShouldThrow_WhenNombreGabineteIsInvalid()
        {
            var repo = new GabineteStorageRepository(
                new Mock<IDapperCrudEngine>(MockBehavior.Strict).Object,
                Mock.Of<ILogger<GabineteStorageRepository>>());

            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var model = BuildModel();
            model = new GabineteInsertModel
            {
                NombreGabinete = "gabinete;drop",
                IdAlmacen = model.IdAlmacen,
                Disco = model.Disco,
                Paginas = model.Paginas,
                TipoDocumento = model.TipoDocumento,
                Carpeta = model.Carpeta,
                Usuario = model.Usuario,
                Fecha = model.Fecha,
                Hora = model.Hora,
                CamposDinamicos = model.CamposDinamicos
            };

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                repo.InsertAsync(model, connection.Object, Mock.Of<IDbTransaction>()));
        }

        [Fact]
        public async Task InsertAsync_ShouldUseDapperEngine_WhenModelIsValid()
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
                .ReturnsAsync(new QueryResult<int> { Success = true, TotalRecords = 1, ErrorMessage = "YES" });

            var repo = new GabineteStorageRepository(dapper.Object, Mock.Of<ILogger<GabineteStorageRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var rows = await repo.InsertAsync(BuildModel(), connection.Object, Mock.Of<IDbTransaction>());

            Assert.Equal(1, rows);
            Assert.NotNull(captured);
            Assert.Equal("gabinete", captured!.TableName);
            Assert.Contains(captured.CampoParameterRegla, c => c.Nombre == "ID");
            Assert.Contains(captured.CampoParameterRegla, c => c.Nombre == "DISC");
            Assert.Contains(captured.CampoParameterRegla, c => c.Nombre == "TIME1");
            Assert.Contains(captured.CampoParameterRegla, c => c.Nombre == "CAMPO_A");
            Assert.Equal("valor", captured.ReglasValidacionCampo["CAMPO_A"]);
        }

        [Fact]
        public async Task InsertAsync_ShouldThrow_WhenDynamicColumnDuplicatesBaseColumn()
        {
            var dapper = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
            var repo = new GabineteStorageRepository(dapper.Object, Mock.Of<ILogger<GabineteStorageRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var model = BuildModel();
            model.CamposDinamicos["ID"] = "duplicado";

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                repo.InsertAsync(model, connection.Object, Mock.Of<IDbTransaction>()));
        }

        private static GabineteInsertModel BuildModel()
        {
            return new GabineteInsertModel
            {
                NombreGabinete = "gabinete",
                IdAlmacen = 100,
                Disco = 2,
                Paginas = 10,
                TipoDocumento = 5,
                Carpeta = 3,
                Usuario = "user1",
                Fecha = DateTime.UtcNow,
                Hora = "10:20:30",
                CamposDinamicos = new Dictionary<string, object?>
                {
                    ["CAMPO_A"] = "valor"
                }
            };
        }
    }
}
