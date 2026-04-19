using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.Editor;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceFullSaveEditorDocumentTests
{
    [Fact]
    public async Task CuandoAliasInvalido_NoAbreConexion()
    {
        var guarda = new Mock<IGuardaEditorDocumentRepository>(MockBehavior.Strict);
        var catalog = new Mock<ISolicitaEditorContextDefinitionRepository>(MockBehavior.Strict);
        var context = new Mock<IGuardaEditorDocumentContextRepository>(MockBehavior.Strict);
        var sync = new Mock<ISincronizaEditorDocumentImagesRepository>(MockBehavior.Strict);
        var factory = new Mock<IDbConnectionFactory>(MockBehavior.Strict);

        var service = new ServiceFullSaveEditorDocument(guarda.Object, catalog.Object, context.Object, sync.Object, factory.Object);

        var res = await service.FullSaveAsync(new FullSaveEditorDocumentRequestDto
        {
            DocumentHtml = "<p>x</p>",
            ContextCode = "RAD",
            EntityId = 1,
            ImageUids = []
        }, "   ");

        Assert.False(res.success);
        factory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CuandoCatalogoInvalido_RollbackYNoContinua()
    {
        var guarda = new Mock<IGuardaEditorDocumentRepository>();
        guarda.Setup(r => r.GuardaEditorDocumentAsync(It.IsAny<GuardaEditorDocumentRequestDto>(), "db1", It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(new AppResponses<RaEditorDocument?> { success = true, message = "YES", data = new RaEditorDocument { DocumentId = 10 }, errors = [] });

        var catalog = new Mock<ISolicitaEditorContextDefinitionRepository>();
        catalog.Setup(r => r.SolicitaPorContextCodeAsync("RAD", "db1", It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(new AppResponses<RaEditorContextDefinition?> { success = false, message = "ContextCode no existe", data = null, errors = [] });

        var context = new Mock<IGuardaEditorDocumentContextRepository>(MockBehavior.Strict);
        var sync = new Mock<ISincronizaEditorDocumentImagesRepository>(MockBehavior.Strict);

        var fakeConn = new FakeDbConnection();
        var factory = new Mock<IDbConnectionFactory>();
        factory.Setup(f => f.GetOpenConnectionAsync("db1")).ReturnsAsync(fakeConn);

        var service = new ServiceFullSaveEditorDocument(guarda.Object, catalog.Object, context.Object, sync.Object, factory.Object);

        var res = await service.FullSaveAsync(new FullSaveEditorDocumentRequestDto
        {
            DocumentHtml = "<p>x</p>",
            ContextCode = "rad",
            EntityId = 911,
            ImageUids = []
        }, "db1");

        Assert.False(res.success);
        Assert.True(fakeConn.LastTransaction?.RolledBack);
        sync.VerifyNoOtherCalls();
        context.VerifyNoOtherCalls();
    }

    private sealed class FakeDbConnection : IDbConnection
    {
        public FakeDbTransaction? LastTransaction { get; private set; }

        public string ConnectionString { get; set; } = string.Empty;
        public int ConnectionTimeout => 0;
        public string Database => "db";
        public ConnectionState State => ConnectionState.Open;
        public IDbTransaction BeginTransaction() => LastTransaction = new FakeDbTransaction(this);
        public IDbTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();
        public void ChangeDatabase(string databaseName) { }
        public void Close() { }
        public IDbCommand CreateCommand() => throw new NotSupportedException();
        public void Open() { }
        public void Dispose() { }
    }

    private sealed class FakeDbTransaction : IDbTransaction
    {
        private readonly IDbConnection _conn;
        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public FakeDbTransaction(IDbConnection conn) => _conn = conn;

        public IDbConnection Connection => _conn;
        public IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
        public void Commit() => Committed = true;
        public void Rollback() => RolledBack = true;
        public void Dispose() { }
    }
}

