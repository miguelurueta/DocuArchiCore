using MiApp.Models.Models.GestorDocumental.Editor;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class GuardaEditorImageRepositoryTests
{
    [Fact]
    public async Task InsertOk_ThenQueryOk_RetornaImagen()
    {
        var engine = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
        engine.Setup(e => e.InsertAsync(It.IsAny<QueryOptions>(), It.IsAny<RaEditorDocumentImage>(), It.IsAny<string>(), true))
            .ReturnsAsync(new QueryResult<int>
            {
                Success = true,
                Message = "YES",
                Data = new[] { 5 }
            });
        engine.Setup(e => e.GetAllAsync<RaEditorDocumentImage>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RaEditorDocumentImage>
            {
                Success = true,
                Message = "YES",
                Data = new[]
                {
                    new RaEditorDocumentImage
                    {
                        ImageId = 5,
                        ImageUid = "uid",
                        FileName = "a.png",
                        ContentType = "image/png",
                        FileSize = 3,
                        StorageTypeCode = "db"
                    }
                }
            });

        var repo = new GuardaEditorImageRepository(engine.Object);

        var res = await repo.GuardaEditorImageAsync(new RaEditorDocumentImage
        {
            ImageUid = "uid",
            FileName = "a.png",
            ContentType = "image/png",
            FileSize = 3,
            StorageTypeCode = "db",
            ImageBytes = new byte[] { 1, 2, 3 }
        }, "db1");

        Assert.True(res.success);
        Assert.NotNull(res.data);
        Assert.Equal(5, res.data!.ImageId);
    }
}

