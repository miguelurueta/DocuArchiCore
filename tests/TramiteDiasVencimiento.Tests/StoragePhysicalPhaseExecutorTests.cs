using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Builders;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Physical;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StoragePhysicalPhaseExecutorTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldReturnCompleted_WhenPhysicalFlowSucceeds()
        {
            var planBuilder = new Mock<IStoragePlanBuilder>();
            var xmlBuilder = new Mock<IStorageXmlBuilder>();
            var fileWriter = new Mock<IStorageFileWriter>();
            var xmlWriter = new Mock<IStorageXmlWriter>();
            var compensation = new Mock<IStorageCompensationManager>();

            var plan = new StorageFilePlanModel
            {
                StorageRoot = @"C:\tmp",
                RutaFinal = @"C:\tmp\final",
                NombreArchivoPrincipal = "DIG00000010.pdf",
                NombreXml = "FXL00000010.xml",
                SegundoNombreDocumental = "DIG00000010.pdf",
                ArchivosOrigen = new List<string> { @"C:\tmp\source.pdf" }
            };

            planBuilder.Setup(x => x.BuildFilePlanAsync(It.IsAny<StorageContext>(), It.IsAny<StorageTransactionResult>()))
                .ReturnsAsync(plan);
            fileWriter.Setup(x => x.CopyAsync(plan, It.IsAny<StorageCompensationPlan>(), "req-1"))
                .ReturnsAsync(@"C:\tmp\final\DIG00000010.pdf");
            xmlBuilder.Setup(x => x.BuildXmlModel(It.IsAny<StorageContext>(), It.IsAny<StorageTransactionResult>()))
                .Returns(new StorageXmlModel
                {
                    IdAlmacen = 10,
                    Disco = 1,
                    Carpeta = 1,
                    Paginas = 2,
                    Usuario = "u",
                    Metadata = new Dictionary<string, string?>()
                });
            xmlWriter.Setup(x => x.WriteAsync(
                    plan,
                    It.IsAny<StorageXmlModel>(),
                    It.IsAny<StorageCompensationPlan>(),
                    "req-1"))
                .ReturnsAsync(@"C:\tmp\final\FXL00000010.xml");

            var executor = new StoragePhysicalPhaseExecutor(
                planBuilder.Object,
                xmlBuilder.Object,
                fileWriter.Object,
                xmlWriter.Object,
                compensation.Object,
                Mock.Of<ILogger<StoragePhysicalPhaseExecutor>>());

            var result = await executor.ExecuteAsync(BuildContext(), BuildTxResult());

            Assert.Equal(StorageDocumentState.Completed, result.Estado);
            Assert.Equal("DIG00000010.pdf", result.NombreArchivoFinal);
            compensation.Verify(x => x.ExecuteAsync(It.IsAny<StorageCompensationPlan>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCompensate_WhenWriterFails()
        {
            var planBuilder = new Mock<IStoragePlanBuilder>();
            var xmlBuilder = new Mock<IStorageXmlBuilder>();
            var fileWriter = new Mock<IStorageFileWriter>();
            var xmlWriter = new Mock<IStorageXmlWriter>();
            var compensation = new Mock<IStorageCompensationManager>();

            var plan = new StorageFilePlanModel
            {
                StorageRoot = @"C:\tmp",
                RutaFinal = @"C:\tmp\final",
                NombreArchivoPrincipal = "DIG00000010.pdf",
                NombreXml = "FXL00000010.xml",
                SegundoNombreDocumental = "DIG00000010.pdf",
                ArchivosOrigen = new List<string> { @"C:\tmp\source.pdf" }
            };

            planBuilder.Setup(x => x.BuildFilePlanAsync(It.IsAny<StorageContext>(), It.IsAny<StorageTransactionResult>()))
                .ReturnsAsync(plan);
            fileWriter.Setup(x => x.CopyAsync(plan, It.IsAny<StorageCompensationPlan>(), "req-1"))
                .ThrowsAsync(new StoragePhysicalException("copy failed"));

            var executor = new StoragePhysicalPhaseExecutor(
                planBuilder.Object,
                xmlBuilder.Object,
                fileWriter.Object,
                xmlWriter.Object,
                compensation.Object,
                Mock.Of<ILogger<StoragePhysicalPhaseExecutor>>());

            await Assert.ThrowsAsync<StoragePhysicalException>(() => executor.ExecuteAsync(BuildContext(), BuildTxResult()));
            compensation.Verify(x => x.ExecuteAsync(It.IsAny<StorageCompensationPlan>(), "req-1"), Times.Once);
        }

        private static StorageContext BuildContext()
        {
            return new StorageContext
            {
                DefaultDbAlias = "db",
                Usuario = "u",
                UsuarioId = 1,
                RequestId = "req-1",
                NombreGabinete = "gab",
                RutaTemporalId = "tmp-1",
                NombreDocumento = "doc.pdf",
                ArchivosTemporales = new[] { "doc.pdf" },
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp-1",
                    NombreDocumento = "doc.pdf",
                    RequestId = "req-1",
                    Documentos = new[]
                    {
                        new MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental.DocumentoEntradaDto
                        {
                            IdDocumento = "1",
                            ArchivoTemporalId = "doc.pdf",
                            NumeroPaginas = 2
                        }
                    }
                }
            };
        }

        private static StorageTransactionResult BuildTxResult()
        {
            return new StorageTransactionResult
            {
                IdentityReservation = new StorageIdentityReservationResult
                {
                    Identity = new StorageIdentityModel
                    {
                        IdAlmacen = 10,
                        Disco = 1,
                        Carpeta = 1,
                        NumeroPaginasCarpeta = 2
                    }
                },
                Success = true,
                Estado = StorageDocumentState.Reserved,
                RequestId = "req-1",
                FechaEjecucion = DateTime.UtcNow,
                DuracionMs = 1
            };
        }
    }
}
