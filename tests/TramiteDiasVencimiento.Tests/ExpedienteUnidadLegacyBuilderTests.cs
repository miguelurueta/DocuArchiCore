using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Expediente;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Metadata;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Expediente;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public sealed class ExpedienteUnidadLegacyBuilderTests
    {
        private readonly ExpedienteUnidadLegacyBuilder _builder = new();

        [Fact]
        public void Build_ShouldReturnNoExecution_WhenOptionDisabled()
        {
            var context = BuildContext(false, new ExpedienteStorageDto { IdExpediente = 10, IdClaseDocumento = 20 });

            var plan = _builder.Build(context);

            Assert.False(plan.TieneExpediente);
            Assert.False(plan.TieneUnidadConservacion);
            Assert.Equal(0, plan.NumeroFolios);
        }

        [Fact]
        public void Build_ShouldThrow_WhenExpedienteAndUnidadProvided()
        {
            var context = BuildContext(true, new ExpedienteStorageDto { IdExpediente = 10, IdUnidadConservacion = 20, IdClaseDocumento = 30 });

            var ex = Assert.Throws<InvalidOperationException>(() => _builder.Build(context));

            Assert.Contains("Ambigüedad", ex.Message);
        }

        [Fact]
        public void Build_ShouldReturnExpedientePlan_WhenOnlyExpedienteProvided()
        {
            var context = BuildContext(true, new ExpedienteStorageDto { IdExpediente = 10, IdClaseDocumento = 30 });

            var plan = _builder.Build(context);

            Assert.True(plan.TieneExpediente);
            Assert.False(plan.TieneUnidadConservacion);
            Assert.Equal(2, plan.IdTipoUnidadDocumental);
            Assert.Equal(5, plan.NumeroFolios);
        }

        [Fact]
        public void Build_ShouldReturnUnidadPlan_WhenOnlyUnidadProvided()
        {
            var context = BuildContext(true, new ExpedienteStorageDto { IdUnidadConservacion = 20, IdClaseDocumento = 30 });

            var plan = _builder.Build(context);

            Assert.False(plan.TieneExpediente);
            Assert.True(plan.TieneUnidadConservacion);
            Assert.Equal(1, plan.IdTipoUnidadDocumental);
            Assert.Equal(5, plan.NumeroFolios);
        }

        private static StorageContext BuildContext(bool aplicaUnidad, ExpedienteStorageDto? expediente)
        {
            return new StorageContext
            {
                DefaultDbAlias = "db",
                Usuario = "u",
                UsuarioId = 1,
                RequestId = "req",
                NombreGabinete = "gab",
                RutaTemporalId = "tmp",
                NombreDocumento = "doc",
                ArchivosTemporales = new[] { "tmp-1" },
                ResolvedOptions = new StorageResolvedOptionsModel
                {
                    NombreGabinete = "gab",
                    AplicaUnidadConservacion = aplicaUnidad
                },
                PhysicalMetadata = new StorageDocumentPhysicalMetadata
                {
                    TotalBytes = 100,
                    TamanoLegacy = "0.1 Kb",
                    Formato = ".PDF",
                    NumeroPaginas = 5,
                    PaginasCalculadasDesdeArchivo = true
                },
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp",
                    NombreDocumento = "doc",
                    RequestId = "req",
                    Documentos = new[] { new DocumentoEntradaDto { IdDocumento = "1", ArchivoTemporalId = "tmp-1", NumeroPaginas = 5 } },
                    Expediente = expediente
                }
            };
        }
    }
}
