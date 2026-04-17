using MiApp.Repository.Repositorio.DataAccess;
using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.Models.Models.GestorDocumental.Editor;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class GuardaEditorDocumentRepositoryTests
{
    [Fact]
    public async Task CuandoAliasInvalido_NoInvocaEngine()
    {
        var engine = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
        var repo = new GuardaEditorDocumentRepository(engine.Object, new Mock<IDbConnectionFactory>().Object);

        var res = await repo.GuardaEditorDocumentAsync(new GuardaEditorDocumentRequestDto { DocumentHtml = "<p>x</p>" }, "   ");

        Assert.False(res.success);
        engine.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CuandoHtmlInvalido_NoInvocaEngine()
    {
        var engine = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
        var repo = new GuardaEditorDocumentRepository(engine.Object, new Mock<IDbConnectionFactory>().Object);

        var res = await repo.GuardaEditorDocumentAsync(new GuardaEditorDocumentRequestDto { DocumentHtml = "   " }, "db1");

        Assert.False(res.success);
        engine.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_CuandoInsertOk_YQueryOk_RetornaDocumento()
    {
        var engine = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
        engine.Setup(e => e.InsertAsync(It.IsAny<QueryOptions>(), It.IsAny<RaEditorDocument>(), It.IsAny<string>(), true))
            .ReturnsAsync(new QueryResult<int>
            {
                Success = true,
                Message = "YES",
                Data = new[] { 10 }
            });
        engine.Setup(e => e.GetAllAsync<RaEditorDocument>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RaEditorDocument>
            {
                Success = true,
                Message = "YES",
                Data = new[]
                {
                    new RaEditorDocument { DocumentId = 10, FormatCode = "html", DocumentHtml = "<p>x</p>", StatusCode = "saved" }
                }
            });

        var repo = new GuardaEditorDocumentRepository(engine.Object, new Mock<IDbConnectionFactory>().Object);

        var res = await repo.GuardaEditorDocumentAsync(new GuardaEditorDocumentRequestDto { DocumentHtml = "<p>x</p>" }, "db1");

        Assert.True(res.success);
        Assert.NotNull(res.data);
        Assert.Equal(10, res.data!.DocumentId);
    }

    [Fact]
    public async Task Update_CuandoNoExiste_RetornaError()
    {
        var engine = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
        engine.Setup(e => e.GetAllAsync<RaEditorDocument>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RaEditorDocument>
            {
                Success = true,
                Message = "YES",
                Data = Array.Empty<RaEditorDocument>()
            });

        var repo = new GuardaEditorDocumentRepository(engine.Object, new Mock<IDbConnectionFactory>().Object);

        var res = await repo.GuardaEditorDocumentAsync(new GuardaEditorDocumentRequestDto { DocumentId = 99, DocumentHtml = "<p>x</p>" }, "db1");

        Assert.False(res.success);
        Assert.Null(res.data);
    }
}


