using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental;
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
        }

        [Fact]
        public async Task Orchestrator_ShouldReturnPendingResult()
        {
            var logger = new Mock<ILogger<DocumentStorageOrchestrator>>();
            var orchestrator = new DocumentStorageOrchestrator(logger.Object);

            var result = await orchestrator.ExecuteAsync(new StorageContext
            {
                DefaultDbAlias = "db",
                Usuario = "usr",
                UsuarioId = 1,
                RequestId = "req-abc",
                NombreGabinete = "g1",
                RutaTemporalId = "tmp-1",
                NombreDocumento = "doc.pdf",
                ArchivosTemporales = new List<string> { "f-1" }
            });

            Assert.Equal(StorageDocumentState.Pending, result.Estado);
            Assert.Equal("req-abc", result.RequestId);
        }
    }
}