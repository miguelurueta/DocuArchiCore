using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental.TemporaryUpload;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.TemporaryUpload;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.TemporaryUpload;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public sealed class StorageLargeUploadServiceTests
    {
        [Fact]
        public async Task InitAsync_ShouldThrow_WhenSizeExceedsLimit()
        {
            using var fixture = CreateFixture(maxBytes: 1024, chunkSize: 256, allowedExt: ".pdf");

            var request = new StorageUploadInitRequestDto
            {
                NombreOriginal = "demo.pdf",
                TamanoBytes = 2048,
                Extension = ".pdf",
                NumeroChunks = 8
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Service.InitAsync(request, 1));
            Assert.Equal("UPLOAD_FILE_TOO_LARGE", ex.Message);
        }

        [Fact]
        public async Task UploadChunkAsync_ShouldFail_WhenOwnershipDoesNotMatch()
        {
            using var fixture = CreateFixture(maxBytes: 1024 * 1024, chunkSize: 1024, allowedExt: ".pdf");

            var init = await fixture.Service.InitAsync(new StorageUploadInitRequestDto
            {
                NombreOriginal = "demo.pdf",
                TamanoBytes = 4,
                Extension = ".pdf",
                NumeroChunks = 1
            }, usuarioId: 10);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("data"));
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                fixture.Service.UploadChunkAsync(init.RutaTemporalId, init.ArchivoTemporalId, 0, 1, 4, stream, usuarioId: 11));

            Assert.Equal("UPLOAD_ACCESS_DENIED", ex.Message);
        }

        [Fact]
        public async Task CompleteAsync_ShouldFail_WhenChunksAreMissing()
        {
            using var fixture = CreateFixture(maxBytes: 1024 * 1024, chunkSize: 1024, allowedExt: ".pdf");

            var init = await fixture.Service.InitAsync(new StorageUploadInitRequestDto
            {
                NombreOriginal = "demo.pdf",
                TamanoBytes = 8,
                Extension = ".pdf",
                NumeroChunks = 2
            }, usuarioId: 20);

            var chunk0 = new MemoryStream(Encoding.UTF8.GetBytes("1234"));
            await fixture.Service.UploadChunkAsync(init.RutaTemporalId, init.ArchivoTemporalId, 0, 2, 4, chunk0, usuarioId: 20);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                fixture.Service.CompleteAsync(init.RutaTemporalId, init.ArchivoTemporalId, usuarioId: 20));

            Assert.Equal("UPLOAD_NOT_COMPLETED", ex.Message);
        }

        [Fact]
        public async Task UploadChunkAsync_ShouldFail_WhenChunkIndexIsOutOfRange()
        {
            using var fixture = CreateFixture(maxBytes: 1024 * 1024, chunkSize: 1024, allowedExt: ".pdf");

            var init = await fixture.Service.InitAsync(new StorageUploadInitRequestDto
            {
                NombreOriginal = "demo.pdf",
                TamanoBytes = 4,
                Extension = ".pdf",
                NumeroChunks = 1
            }, usuarioId: 21);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                fixture.Service.UploadChunkAsync(
                    init.RutaTemporalId,
                    init.ArchivoTemporalId,
                    chunkIndex: 1,
                    totalChunks: 1,
                    contentLength: 4,
                    content: new MemoryStream(Encoding.UTF8.GetBytes("ABCD")),
                    usuarioId: 21));

            Assert.Equal("UPLOAD_INVALID_CHUNK", ex.Message);
        }

        [Fact]
        public async Task CompleteAsync_ShouldFail_WhenHashDoesNotMatch()
        {
            using var fixture = CreateFixture(maxBytes: 1024 * 1024, chunkSize: 1024, allowedExt: ".pdf");

            var init = await fixture.Service.InitAsync(new StorageUploadInitRequestDto
            {
                NombreOriginal = "demo.pdf",
                TamanoBytes = 4,
                Extension = ".pdf",
                NumeroChunks = 1,
                HashSha256Esperado = "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"
            }, usuarioId: 22);

            await fixture.Service.UploadChunkAsync(
                init.RutaTemporalId,
                init.ArchivoTemporalId,
                chunkIndex: 0,
                totalChunks: 1,
                contentLength: 4,
                content: new MemoryStream(Encoding.UTF8.GetBytes("ABCD")),
                usuarioId: 22);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                fixture.Service.CompleteAsync(init.RutaTemporalId, init.ArchivoTemporalId, usuarioId: 22));

            Assert.Equal("UPLOAD_HASH_MISMATCH", ex.Message);
        }

        [Fact]
        public async Task Flow_ShouldComplete_AndEnsureCompleted()
        {
            using var fixture = CreateFixture(maxBytes: 1024 * 1024, chunkSize: 1024, allowedExt: ".pdf");

            var init = await fixture.Service.InitAsync(new StorageUploadInitRequestDto
            {
                NombreOriginal = "demo.pdf",
                TamanoBytes = 4,
                Extension = ".pdf",
                NumeroChunks = 1
            }, usuarioId: 30);

            var chunk = new MemoryStream(Encoding.UTF8.GetBytes("ABCD"));
            await fixture.Service.UploadChunkAsync(init.RutaTemporalId, init.ArchivoTemporalId, 0, 1, 4, chunk, usuarioId: 30);
            await fixture.Service.CompleteAsync(init.RutaTemporalId, init.ArchivoTemporalId, usuarioId: 30);

            var status = await fixture.Service.GetStatusAsync(init.RutaTemporalId, init.ArchivoTemporalId, usuarioId: 30);
            Assert.Equal(StorageTemporaryUploadState.Completed, status.Estado);
            Assert.Single(status.ChunksRecibidos);
            Assert.Empty(status.ChunksPendientes);

            await fixture.Service.EnsureCompletedAsync(init.RutaTemporalId, new[] { init.ArchivoTemporalId }, usuarioId: 30);
        }

        [Fact]
        public async Task UploadChunkAsync_ShouldBeIdempotent_ForSameChunkRetry()
        {
            using var fixture = CreateFixture(maxBytes: 1024 * 1024, chunkSize: 1024, allowedExt: ".pdf");

            var init = await fixture.Service.InitAsync(new StorageUploadInitRequestDto
            {
                NombreOriginal = "demo.pdf",
                TamanoBytes = 4,
                Extension = ".pdf",
                NumeroChunks = 1
            }, usuarioId: 40);

            await fixture.Service.UploadChunkAsync(init.RutaTemporalId, init.ArchivoTemporalId, 0, 1, 4, new MemoryStream(Encoding.UTF8.GetBytes("ABCD")), usuarioId: 40);
            await fixture.Service.UploadChunkAsync(init.RutaTemporalId, init.ArchivoTemporalId, 0, 1, 4, new MemoryStream(Encoding.UTF8.GetBytes("ABCD")), usuarioId: 40);

            var status = await fixture.Service.GetStatusAsync(init.RutaTemporalId, init.ArchivoTemporalId, usuarioId: 40);
            Assert.Single(status.ChunksRecibidos);
            Assert.Equal(4, status.TamanoRecibidoBytes);
        }

        private static TestFixture CreateFixture(long maxBytes, int chunkSize, string allowedExt)
        {
            var root = Path.Combine(Path.GetTempPath(), "docuarchi-tests", Guid.NewGuid().ToString("N"));
            var pathOptions = Options.Create(new StoragePathOptions { Temp = root });
            var uploadOptions = Options.Create(new StorageUploadOptions
            {
                MaxFileSizeBytes = maxBytes,
                ChunkSizeBytes = chunkSize,
                AllowedExtensions = new[] { allowedExt },
                TtlMinutes = 60
            });

            var pathResolver = new StorageUploadPathResolver(pathOptions);
            var store = new StorageUploadSessionStore(pathResolver);
            var service = new StorageLargeUploadService(
                store,
                pathResolver,
                new StorageUploadPolicy(),
                uploadOptions,
                NullLogger<StorageLargeUploadService>.Instance);

            return new TestFixture(root, service);
        }

        private sealed class TestFixture : IDisposable
        {
            public TestFixture(string root, IStorageLargeUploadService service)
            {
                Root = root;
                Service = service;
            }

            public string Root { get; }
            public IStorageLargeUploadService Service { get; }

            public void Dispose()
            {
                if (Directory.Exists(Root))
                {
                    Directory.Delete(Root, true);
                }
            }
        }
    }
}
