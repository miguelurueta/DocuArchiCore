using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Expediente;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.ExpedienteXml;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.ExpedienteXml;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.ExpedienteXml;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public sealed class ExpedienteIndiceXmlServiceTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldReturnNoExpediente_WhenExpedienteResultMissing()
        {
            var service = BuildService();
            var tx = BuildTxResult();
            tx = new StorageTransactionResult
            {
                IdentityReservation = tx.IdentityReservation,
                IdRegistroProduccionDocumental = tx.IdRegistroProduccionDocumental,
                Success = tx.Success,
                Estado = tx.Estado,
                RequestId = tx.RequestId,
                FechaEjecucion = tx.FechaEjecucion,
                DuracionMs = tx.DuracionMs,
                ExpedienteUnidadResult = null
            };

            var result = await service.ExecuteAsync(BuildContext(), tx, BuildNaming(), BuildPath());

            Assert.False(result.Updated);
            Assert.Equal("NO_EXPEDIENTE", result.Estado);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallWriter_WhenLegacyConditionsAreMet()
        {
            var repository = new Mock<IExpedienteIndiceXmlRepository>();
            repository.Setup(x => x.GetXmlRouteAsync(77, "db"))
                .ReturnsAsync(new ExpedienteIndiceXmlRouteModel { IdExpediente = 77, RutaArchivoXml = @"C:\tmp\idx.xml" });

            var builder = new Mock<IExpedienteIndiceXmlBuilder>();
            builder.Setup(x => x.Build(It.IsAny<StorageContext>(), It.IsAny<StorageTransactionResult>(), It.IsAny<StorageNamingResult>(), It.IsAny<StoragePhysicalPathModel>()))
                .Returns(new ExpedienteIndiceXmlDocumentModel { IdExpediente = 77, IdRegistroProduccionDocumental = 500 });

            var writer = new Mock<IExpedienteIndiceXmlWriter>();
            writer.Setup(x => x.UpdateAsync(@"C:\tmp\idx.xml", It.IsAny<ExpedienteIndiceXmlDocumentModel>()))
                .ReturnsAsync(new ExpedienteIndiceXmlUpdateResult { Updated = true, Estado = "UPDATED", RutaArchivoXml = @"C:\tmp\idx.xml" });

            var service = new ExpedienteIndiceXmlService(repository.Object, builder.Object, writer.Object, Mock.Of<ILogger<ExpedienteIndiceXmlService>>());
            var result = await service.ExecuteAsync(BuildContext(), BuildTxResult(), BuildNaming(), BuildPath());

            Assert.True(result.Updated);
            Assert.Equal("UPDATED", result.Estado);
            writer.Verify(x => x.UpdateAsync(@"C:\tmp\idx.xml", It.IsAny<ExpedienteIndiceXmlDocumentModel>()), Times.Once);
        }

        private static ExpedienteIndiceXmlService BuildService()
        {
            return new ExpedienteIndiceXmlService(
                Mock.Of<IExpedienteIndiceXmlRepository>(),
                Mock.Of<IExpedienteIndiceXmlBuilder>(),
                Mock.Of<IExpedienteIndiceXmlWriter>(),
                Mock.Of<ILogger<ExpedienteIndiceXmlService>>());
        }

        private static StorageContext BuildContext()
        {
            return new StorageContext
            {
                DefaultDbAlias = "db",
                Usuario = "user",
                UsuarioId = 1,
                RequestId = "req-1",
                NombreGabinete = "gab",
                RutaTemporalId = "tmp-1",
                NombreDocumento = "doc.pdf",
                ArchivosTemporales = new[] { "tmp-doc" },
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp-1",
                    NombreDocumento = "doc.pdf",
                    RequestId = "req-1",
                    Documentos =
                    [
                        new DocumentoEntradaDto
                        {
                            IdDocumento = "1",
                            ArchivoTemporalId = "tmp-doc",
                            NumeroPaginas = 4,
                            Extension = "pdf"
                        }
                    ],
                    Expediente = new ExpedienteStorageDto
                    {
                        IdExpediente = 77,
                        IdClaseDocumento = 9
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
                        IdAlmacen = 100,
                        Disco = 1,
                        Carpeta = 1,
                        NumeroPaginasCarpeta = 4
                    }
                },
                IdRegistroProduccionDocumental = 500,
                Success = true,
                Estado = StorageDocumentState.Reserved,
                RequestId = "req-1",
                FechaEjecucion = DateTime.UtcNow,
                DuracionMs = 1,
                ExpedienteUnidadResult = new ExpedienteUnidadLegacyResult
                {
                    Ejecutado = true,
                    TieneExpediente = true,
                    EstadoExpedienteElectronico = 2,
                    NumeroFolios = 4,
                    UnidadConservaTipo = UnidadConservaTipoEnum.Electronico
                }
            };
        }

        private static StorageNamingResult BuildNaming()
        {
            return new StorageNamingResult
            {
                NombreArchivoPrincipal = "DIG00000001.pdf",
                NombreXml = "FXL00000001.xml",
                SegundoNombre = "SEGUNDO.pdf"
            };
        }

        private static StoragePhysicalPathModel BuildPath()
        {
            return new StoragePhysicalPathModel
            {
                StorageRoot = @"C:\storage",
                NombreGabinete = "gab",
                Disco = 1,
                Carpeta = 1,
                RutaGabineteDisco = @"C:\storage\000000001",
                CarpetaLegacy = "00001",
                RutaFinal = @"C:\storage\000000001\00001"
            };
        }
    }
}

