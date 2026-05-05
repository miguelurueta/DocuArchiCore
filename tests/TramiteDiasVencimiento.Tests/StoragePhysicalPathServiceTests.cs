using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.StorageRoute;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Physical;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StoragePhysicalPathServiceTests
    {
        [Fact]
        public async Task ResolveAsync_ShouldThrow_WhenRouteIsNull()
        {
            var routeRepo = new Mock<IStorageRouteRepository>();
            routeRepo.Setup(r => r.GetRouteAsync("gab", "da")).ReturnsAsync((StorageRouteModel?)null);

            var service = BuildService(routeRepo.Object, new StorageFolderLegacyPolicy());

            await Assert.ThrowsAsync<StoragePhysicalException>(() =>
                service.ResolveAsync(BuildContext("gab"), BuildIdentity()));
        }

        [Fact]
        public async Task ResolveAsync_ShouldThrow_WhenRouteIsOutsideRoot()
        {
            var routeRepo = new Mock<IStorageRouteRepository>();
            routeRepo.Setup(r => r.GetRouteAsync("gab", "da"))
                .ReturnsAsync(new StorageRouteModel { NombreGabinete = "gab", RutaAlmacenamiento = @"D:\root" });

            var folderPolicy = new Mock<IStorageFolderLegacyPolicy>();
            folderPolicy.Setup(p => p.ResolveFolder(@"D:\root", "gab", 2, 15))
                .Returns(new StorageFolderResult
                {
                    CarpetaLegacy = "00015",
                    RutaGabineteDisco = @"D:\other\gab2",
                    RutaFinal = @"D:\other\gab2\00015"
                });

            var service = BuildService(routeRepo.Object, folderPolicy.Object);

            await Assert.ThrowsAsync<StoragePhysicalException>(() =>
                service.ResolveAsync(BuildContext("gab"), BuildIdentity()));
        }

        [Fact]
        public async Task ResolveAsync_ShouldReturnPhysicalPath_WhenInputIsValid()
        {
            var routeRepo = new Mock<IStorageRouteRepository>();
            routeRepo.Setup(r => r.GetRouteAsync("gab", "da"))
                .ReturnsAsync(new StorageRouteModel { NombreGabinete = "gab", RutaAlmacenamiento = @"D:\storage-root" });

            var service = BuildService(routeRepo.Object, new StorageFolderLegacyPolicy());

            var result = await service.ResolveAsync(BuildContext("gab"), BuildIdentity());

            Assert.Equal("gab", result.NombreGabinete);
            Assert.Equal(2, result.Disco);
            Assert.Equal(15, result.Carpeta);
            Assert.Equal("00015", result.CarpetaLegacy);
            Assert.Contains(@"gab2", result.RutaGabineteDisco, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(@"gab2\00015", result.RutaFinal, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(@"storage-root", result.StorageRoot, StringComparison.OrdinalIgnoreCase);
        }

        private static StoragePhysicalPathService BuildService(
            IStorageRouteRepository routeRepository,
            IStorageFolderLegacyPolicy folderPolicy)
        {
            return new StoragePhysicalPathService(
                routeRepository,
                folderPolicy,
                new StoragePathResolver(),
                Mock.Of<ILogger<StoragePhysicalPathService>>());
        }

        private static StorageIdentityModel BuildIdentity()
        {
            return new StorageIdentityModel
            {
                IdAlmacen = 100,
                Disco = 2,
                Carpeta = 15,
                NumeroPaginasCarpeta = 10
            };
        }

        private static StorageContext BuildContext(string gabinete)
        {
            return new StorageContext
            {
                DefaultDbAlias = "da",
                Usuario = "user",
                UsuarioId = 1,
                RequestId = "req-1",
                NombreGabinete = gabinete,
                RutaTemporalId = "tmp-1",
                NombreDocumento = "doc.pdf",
                ArchivosTemporales = new[] { "file-1" },
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = gabinete,
                    RutaTemporalId = "tmp-1",
                    NombreDocumento = "doc.pdf",
                    RequestId = "req-1",
                    Documentos = new List<DocumentoEntradaDto>
                    {
                        new DocumentoEntradaDto
                        {
                            IdDocumento = "d1",
                            ArchivoTemporalId = "file-1",
                            NumeroPaginas = 1
                        }
                    }
                }
            };
        }
    }
}
