using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Models.Models.GestorDocumental.Common.Audit;
using MiApp.Repository.Repositorio.GestorDocumental.Common.Audit;
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
                new Mock<ILogDocuarchiRepository>(MockBehavior.Strict).Object,
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
                new Mock<ILogDocuarchiRepository>(MockBehavior.Strict).Object,
                Mock.Of<ILogger<WorkflowStorageLogRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var invalid = BuildModel();
            invalid = new WorkflowStorageLogModel
            {
                IdTran = 0,
                DescOp = invalid.DescOp,
                UserOper = invalid.UserOper,
                DateTrans = invalid.DateTrans,
                RutDocu = invalid.RutDocu,
                ModuloRegistro = invalid.ModuloRegistro,
                Gabinete = invalid.Gabinete,
                Campos = invalid.Campos,
                IpTrans = invalid.IpTrans,
                HoraRegistro = invalid.HoraRegistro,
                Radicado = invalid.Radicado,
                IdTareaWorkflow = invalid.IdTareaWorkflow,
                IdRutaWorkflow = invalid.IdRutaWorkflow,
                UserPropietario = invalid.UserPropietario,
                TipologiaDocumental = invalid.TipologiaDocumental
            };

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                repo.InsertAsync(invalid, connection.Object, Mock.Of<IDbTransaction>()));
        }

        [Fact]
        public async Task InsertAsync_ShouldDelegateToGenericLogRepository()
        {
            LogDocuarchiEntryModel? captured = null;
            var logRepo = new Mock<ILogDocuarchiRepository>();
            logRepo.Setup(x => x.InsertBeginTransAsync(
                    It.IsAny<LogDocuarchiEntryModel>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<string>()))
                .Callback<LogDocuarchiEntryModel, IDbConnection, IDbTransaction, string>((m, _, _, _) => captured = m)
                .ReturnsAsync(1);

            var repo = new WorkflowStorageLogRepository(logRepo.Object, Mock.Of<ILogger<WorkflowStorageLogRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var rows = await repo.InsertAsync(BuildModel(), connection.Object, Mock.Of<IDbTransaction>());

            Assert.Equal(1, rows);
            Assert.NotNull(captured);
            Assert.Equal(11L, captured!.IdTran);
            Assert.Equal(77L, captured.IdTareaWorkflow);
            Assert.Equal("gab", captured.Gabinete);
        }

        [Fact]
        public async Task InsertAsync_ShouldPropagateError_WhenGenericLogFails()
        {
            var logRepo = new Mock<ILogDocuarchiRepository>();
            logRepo.Setup(x => x.InsertBeginTransAsync(
                    It.IsAny<LogDocuarchiEntryModel>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new StorageTransactionException("No se insertó registro en logdocuarchi"));

            var repo = new WorkflowStorageLogRepository(logRepo.Object, Mock.Of<ILogger<WorkflowStorageLogRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                repo.InsertAsync(BuildModel(), connection.Object, Mock.Of<IDbTransaction>()));
        }

        private static WorkflowStorageLogModel BuildModel()
        {
            return new WorkflowStorageLogModel
            {
                IdTran = 11,
                DescOp = "Registra",
                UserOper = "user",
                DateTrans = DateTime.UtcNow.Date,
                RutDocu = "tmp/doc-1.pdf",
                ModuloRegistro = "WORKFLOW",
                Gabinete = "gab",
                Campos = "campoA:valorA",
                IpTrans = string.Empty,
                HoraRegistro = "10:20:30",
                Radicado = "RAD-1",
                IdTareaWorkflow = 77,
                IdRutaWorkflow = 9,
                UserPropietario = "user",
                TipologiaDocumental = "10"
            };
        }
    }
}
