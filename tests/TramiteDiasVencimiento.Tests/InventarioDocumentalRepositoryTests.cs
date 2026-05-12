using System;
using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Inventario;
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
            model = new InventarioDocumentalInsertModel
            {
                IdUsuarioGestion = model.IdUsuarioGestion,
                IdEmpresaDocumento = 0,
                IdDocumentoDocuarchiAlmacen = model.IdDocumentoDocuarchiAlmacen,
                NombreGabinete = model.NombreGabinete,
                NumeroFolios = model.NumeroFolios,
                Tamano = model.Tamano,
                Formato = model.Formato
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
            model = new InventarioDocumentalInsertModel
            {
                IdUsuarioGestion = model.IdUsuarioGestion,
                IdEmpresaDocumento = model.IdEmpresaDocumento,
                RadicadoDocumento = model.RadicadoDocumento,
                FullTextDocumento = new string('A', 1_000_100),
                NumeroFolios = model.NumeroFolios,
                IdDocumentoDocuarchiAlmacen = model.IdDocumentoDocuarchiAlmacen,
                NombreGabinete = model.NombreGabinete,
                Formato = model.Formato,
                Tamano = model.Tamano,
                FechaDocumento = model.FechaDocumento
            };

            var id = await repo.InsertAsync(model, connection.Object, Mock.Of<IDbTransaction>());

            Assert.Equal(33L, id);
            Assert.NotNull(captured);
            Assert.Equal("registro_producion_documental", captured!.TableName);
            Assert.Equal(1_000_000, ((string)captured.ReglasValidacionCampo["FULTEXT_DOCUMENTO"]).Length);
            Assert.True(captured.ReglasValidacionCampo.ContainsKey("SEGUNDO_NOMBRE_DOCUMENTO"));
            Assert.True(captured.ReglasValidacionCampo.ContainsKey("ID_TIPO_UNIDAD_DOCUMENTAL"));
            Assert.Null(captured.ReglasValidacionCampo["SERIE_DOCUMENTO"]);
            Assert.Null(captured.ReglasValidacionCampo["SUBSERIE_DOCUMENTO"]);
            Assert.Null(captured.ReglasValidacionCampo["ID_SERIE_DOCUMENTO"]);
            Assert.Null(captured.ReglasValidacionCampo["ID_SUBSERIE_DOCUMENTO"]);
            Assert.DoesNotContain(captured.ReglasValidacionCampo, kvp => kvp.Value is DBNull);
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

        private static InventarioDocumentalInsertModel BuildModel()
        {
            return new InventarioDocumentalInsertModel
            {
                IdUsuarioGestion = 1,
                IdEmpresaDocumento = 10,
                RadicadoDocumento = "RAD-1",
                FullTextDocumento = "texto",
                NumeroFolios = 4,
                IdDocumentoDocuarchiAlmacen = 100,
                NombreGabinete = "gabinete",
                Formato = "pdf",
                Tamano = "100kb",
                FechaDocumento = DateTime.UtcNow
            };
        }
    }
}
