using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Metadata;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Physical;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Transaction;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Validation;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class AlmacenarDocumentoUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldReturnValidation_WhenRequestIsNull()
        {
            var orchestrator = new Mock<IDocumentStorageOrchestrator>(MockBehavior.Strict);
            var logger = new Mock<ILogger<AlmacenarDocumentoUseCase>>();
            var useCase = new AlmacenarDocumentoUseCase(orchestrator.Object, logger.Object);

            var result = await useCase.ExecuteAsync(null!, "db", "usr", 1);

            Assert.False(result.success);
            Assert.Equal("Request requerido", result.message);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldMapSuccessResponse_FromOrchestrator()
        {
            var orchestrator = new Mock<IDocumentStorageOrchestrator>();
            orchestrator
                .Setup(x => x.ExecuteAsync(It.IsAny<StorageContext>()))
                .ReturnsAsync((StorageContext ctx) => new AlmacenarDocumentoResult
                {
                    IdAlmacen = 99,
                    IdRegistroProduccionDocumental = 77,
                    NombreArchivoFinal = "final.pdf",
                    RequestId = ctx.RequestId,
                    Estado = StorageDocumentState.Completed
                });

            var logger = new Mock<ILogger<AlmacenarDocumentoUseCase>>();
            var useCase = new AlmacenarDocumentoUseCase(orchestrator.Object, logger.Object);

            var request = new AlmacenarDocumentoRequest
            {
                RequestId = "req-164",
                NombreGabinete = "g1",
                RutaTemporalId = "tmp-1",
                NombreDocumento = "doc.pdf",
                Documentos = new List<DocumentoEntradaDto>
                {
                    new DocumentoEntradaDto { IdDocumento = "1", ArchivoTemporalId = "f-1", NumeroPaginas = 1 }
                }
            };

            var result = await useCase.ExecuteAsync(request, "dbAlias", "usuario", 10);

            Assert.True(result.success);
            Assert.NotNull(result.data);
            Assert.Equal(99, result.data!.IdAlmacen);
            Assert.Equal("final.pdf", result.data.NombreArchivoFinal);
            Assert.Equal("req-164", result.data.RequestId);

            orchestrator.Verify(x => x.ExecuteAsync(It.Is<StorageContext>(ctx =>
                ctx.RequestId == "req-164"
                && ctx.DefaultDbAlias == "dbAlias"
                && ctx.Usuario == "usuario"
                && ctx.UsuarioId == 10
                && ctx.NombreGabinete == "g1"
                && ctx.RutaTemporalId == "tmp-1"
                && ctx.NombreDocumento == "doc.pdf"
                && ctx.ArchivosTemporales.Count == 1
                && ctx.ArchivosTemporales[0] == "f-1")), Times.Once);
        }

        [Fact]
        public async Task Orchestrator_ShouldThrowValidationException_WhenPipelineIsInvalid()
        {
            var logger = new Mock<ILogger<DocumentStorageOrchestrator>>();
            var pipeline = new Mock<IStorageValidationPipeline>();
            var metadata = new Mock<IStorageDocumentMetadataAnalyzer>();
            var pathResolver = new Mock<IStoragePathResolver>();
            var txCoordinator = new Mock<IStorageTransactionCoordinator>();
            var physicalExecutor = new Mock<IStoragePhysicalPhaseExecutor>();

            pipeline
                .Setup(x => x.ValidateAsync(It.IsAny<StorageContext>()))
                .ReturnsAsync(new StorageValidationResult
                {
                    IsValid = false,
                    Errors = new List<StorageError>
                    {
                        new StorageError { Code = "TEST", Message = "validation failed" }
                    }
                });

            var orchestrator = new DocumentStorageOrchestrator(
                logger.Object,
                pipeline.Object,
                metadata.Object,
                pathResolver.Object,
                txCoordinator.Object,
                physicalExecutor.Object);

            await Assert.ThrowsAsync<StorageValidationException>(() => orchestrator.ExecuteAsync(new StorageContext
            {
                DefaultDbAlias = "db",
                Usuario = "usr",
                UsuarioId = 1,
                RequestId = "req-abc",
                NombreGabinete = "g1",
                RutaTemporalId = "tmp-1",
                NombreDocumento = "doc.pdf",
                ArchivosTemporales = new List<string> { "f-1" }
            }));

            pipeline.Verify(x => x.ValidateAsync(It.IsAny<StorageContext>()), Times.Once);
            txCoordinator.Verify(x => x.ExecuteAsync(It.IsAny<StorageContext>()), Times.Never);
            physicalExecutor.Verify(x => x.ExecuteAsync(It.IsAny<StorageContext>(), It.IsAny<StorageTransactionResult>()), Times.Never);
        }
    }
}
