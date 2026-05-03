using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Workflow;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class WorkflowStorageLogRepositoryTests
    {
        [Fact]
        public async Task InsertAsync_ShouldThrow_WhenConnectionIsClosed()
        {
            var repo = new WorkflowStorageLogRepository(
                new Mock<IDapperCrudEngine>(MockBehavior.Strict).Object,
                Mock.Of<ILogger<WorkflowStorageLogRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Closed);

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                repo.InsertAsync(BuildModel(), connection.Object, Mock.Of<IDbTransaction>()));
        }

        [Fact]
        public async Task InsertAsync_ShouldThrow_WhenModelIsInvalid()
        {
            var repo = new WorkflowStorageLogRepository(
                new Mock<IDapperCrudEngine>(MockBehavior.Strict).Object,
                Mock.Of<ILogger<WorkflowStorageLogRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var invalid = BuildModel();
            invalid = new WorkflowStorageLogModel
            {
                IdAlmacen = 0,
                UsuarioOperacion = invalid.UsuarioOperacion,
                FechaTransaccion = invalid.FechaTransaccion,
                RutaDocumento = invalid.RutaDocumento,
                ModuloRegistro = invalid.ModuloRegistro,
                NombreGabinete = invalid.NombreGabinete,
                Campos = invalid.Campos,
                IpTransaccion = invalid.IpTransaccion,
                HoraRegistro = invalid.HoraRegistro,
                Radicado = invalid.Radicado,
                IdTareaWorkflow = invalid.IdTareaWorkflow,
                IdRutaWorkflow = invalid.IdRutaWorkflow,
                UsuarioPropietario = invalid.UsuarioPropietario,
                TipologiaDocumental = invalid.TipologiaDocumental
            };

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                repo.InsertAsync(invalid, connection.Object, Mock.Of<IDbTransaction>()));
        }

        [Fact]
        public async Task InsertAsync_ShouldUseDapperCrudEngineWithLogdocuarchi()
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

            var repo = new WorkflowStorageLogRepository(dapper.Object, Mock.Of<ILogger<WorkflowStorageLogRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var rows = await repo.InsertAsync(BuildModel(), connection.Object, Mock.Of<IDbTransaction>());

            Assert.Equal(1, rows);
            Assert.NotNull(captured);
            Assert.Equal("logdocuarchi", captured!.TableName);
            Assert.Equal(15, captured.CampoParameterRegla.Count);
            Assert.Equal(11L, captured.ReglasValidacionCampo["id_tran"]);
            Assert.Equal(77L, captured.ReglasValidacionCampo["ID_TAREA_WF"]);
        }

        [Fact]
        public async Task InsertAsync_ShouldThrow_WhenRowsAreNotOne()
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
                .ReturnsAsync(new QueryResult<int> { Success = true, TotalRecords = 0, ErrorMessage = "YES" });

            var repo = new WorkflowStorageLogRepository(dapper.Object, Mock.Of<ILogger<WorkflowStorageLogRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                repo.InsertAsync(BuildModel(), connection.Object, Mock.Of<IDbTransaction>()));
        }

        private static WorkflowStorageLogModel BuildModel()
        {
            return new WorkflowStorageLogModel
            {
                IdAlmacen = 11,
                DescripcionOperacion = "Registra",
                UsuarioOperacion = "user",
                FechaTransaccion = DateTime.UtcNow.Date,
                RutaDocumento = "tmp/doc-1.pdf",
                ModuloRegistro = "WORKFLOW",
                NombreGabinete = "gab",
                Campos = "campoA:valorA",
                IpTransaccion = string.Empty,
                HoraRegistro = new TimeSpan(10, 20, 30),
                Radicado = "RAD-1",
                IdTareaWorkflow = 77,
                IdRutaWorkflow = 9,
                UsuarioPropietario = "user",
                TipologiaDocumental = "10"
            };
        }
    }
}
