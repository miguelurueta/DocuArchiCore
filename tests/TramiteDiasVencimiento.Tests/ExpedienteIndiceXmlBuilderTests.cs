using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Expediente;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.ExpedienteXml;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Metadata;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.ExpedienteXml;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public sealed class ExpedienteIndiceXmlBuilderTests
    {
        private readonly ExpedienteIndiceXmlBuilder _builder = new();

        [Fact]
        public void Build_ShouldCreateDocumentModel_WhenInputIsValid()
        {
            var context = BuildContext();
            var tx = BuildTxResult();
            var naming = new StorageNamingResult
            {
                NombreArchivoPrincipal = "DIG00000001.pdf",
                NombreXml = "FXL00000001.xml",
                SegundoNombre = "SEGUNDO.pdf"
            };
            var path = new StoragePhysicalPathModel
            {
                StorageRoot = @"C:\storage",
                NombreGabinete = "gab",
                Disco = 1,
                Carpeta = 1,
                RutaGabineteDisco = @"C:\storage\000000001",
                CarpetaLegacy = "00001",
                RutaFinal = @"C:\storage\000000001\00001"
            };

            var model = _builder.Build(context, tx, naming, path);

            Assert.Equal(500, model.IdRegistroProduccionDocumental);
            Assert.Equal(100, model.IdAlmacen);
            Assert.Equal(77, model.IdExpediente);
            Assert.Equal("DIG00000001.PDF", model.NombreDocumento);
            Assert.Equal("SEGUNDO.PDF", model.SegundoNombreDocumento);
            Assert.Equal("Contrato", model.TipologiaDocumental);
            Assert.Equal("SHA256", model.FuncionResumen);
            Assert.Equal(4, model.NumeroFolios);
            Assert.EndsWith("DIG00000001.pdf", model.RutaDocumento);
            Assert.Equal(64, model.ValorHuella.Length);
            Assert.Equal(".PDF", model.Formato);
            Assert.Equal("1 Kb", model.Tamano);
        }

        [Fact]
        public void Build_ShouldThrow_WhenPhysicalMetadataMissing()
        {
            var context = BuildContext();
            context.PhysicalMetadata = null;

            Assert.Throws<InvalidOperationException>(() =>
                _builder.Build(
                    context,
                    BuildTxResult(),
                    new StorageNamingResult
                    {
                        NombreArchivoPrincipal = "DIG00000001.pdf",
                        NombreXml = "FXL00000001.xml",
                        SegundoNombre = "SEGUNDO.pdf"
                    },
                    new StoragePhysicalPathModel { RutaFinal = @"C:\tmp" }));
        }

        [Fact]
        public void Build_ShouldFallbackToIdTipoDocumento_WhenTipologiaTextIsMissing()
        {
            var context = BuildContext(idTipoDocumento: 43, nombreTipoDocumento: null);

            var model = _builder.Build(
                context,
                BuildTxResult(),
                new StorageNamingResult
                {
                    NombreArchivoPrincipal = "DIG00000043.pdf",
                    NombreXml = "FXL00000043.xml",
                    SegundoNombre = "SEGUNDO.pdf"
                },
                new StoragePhysicalPathModel
                {
                    RutaFinal = @"C:\tmp"
                });

            Assert.Equal("43", model.TipologiaDocumental);
        }

        private static StorageContext BuildContext(
            int idTipoDocumento = 12,
            string? nombreTipoDocumento = "Contrato")
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
                PhysicalMetadata = new StorageDocumentPhysicalMetadata
                {
                    TotalBytes = 100,
                    TamanoLegacy = "1 Kb",
                    Formato = ".PDF",
                    NumeroPaginas = 4,
                    PaginasCalculadasDesdeArchivo = true
                },
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
                    Trd = new TrdStorageDto
                    {
                        IdTipoDocumento = idTipoDocumento,
                        NombreTipoDocumento = nombreTipoDocumento
                    },
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
    }
}
