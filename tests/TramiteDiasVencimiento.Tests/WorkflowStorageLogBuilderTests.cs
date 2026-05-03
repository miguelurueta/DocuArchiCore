using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Workflow;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class WorkflowStorageLogBuilderTests
    {
        [Fact]
        public void Build_ShouldMapExpectedFields_WhenWorkflowTaskIsValid()
        {
            var builder = new WorkflowStorageLogBuilder();
            var context = BuildContext(idTareaWorkflow: 77);
            var result = BuildResult(idAlmacen: 101);

            var model = builder.Build(context, result);

            Assert.Equal(101, model.IdAlmacen);
            Assert.Equal("Registra", model.DescripcionOperacion);
            Assert.Equal("user-1", model.UsuarioOperacion);
            Assert.Equal("gab", model.NombreGabinete);
            Assert.Equal("tmp/doc-1.pdf", model.RutaDocumento);
            Assert.Equal("RAD-1", model.Radicado);
            Assert.Equal(77, model.IdTareaWorkflow);
            Assert.Equal(9, model.IdRutaWorkflow);
            Assert.Equal("user-1", model.UsuarioPropietario);
            Assert.Equal("10", model.TipologiaDocumental);
            Assert.Equal("campoA:valorA|campoB:valorB", model.Campos);
        }

        [Fact]
        public void Build_ShouldThrow_WhenWorkflowTaskIsInvalid()
        {
            var builder = new WorkflowStorageLogBuilder();
            var context = BuildContext(idTareaWorkflow: 0);
            var result = BuildResult(idAlmacen: 101);

            Assert.Throws<StorageTransactionException>(() => builder.Build(context, result));
        }

        [Fact]
        public void Build_ShouldThrow_WhenIdAlmacenIsInvalid()
        {
            var builder = new WorkflowStorageLogBuilder();
            var context = BuildContext(idTareaWorkflow: 99);
            var result = BuildResult(idAlmacen: 0);

            Assert.Throws<StorageTransactionException>(() => builder.Build(context, result));
        }

        private static StorageContext BuildContext(long idTareaWorkflow)
        {
            return new StorageContext
            {
                DefaultDbAlias = "da",
                Usuario = "user-1",
                UsuarioId = 1,
                RequestId = "req-1",
                NombreGabinete = "gab",
                RutaTemporalId = "tmp",
                NombreDocumento = "doc-1.pdf",
                ArchivosTemporales = new[] { "doc-1.pdf" },
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp",
                    NombreDocumento = "doc-1.pdf",
                    RequestId = "req-1",
                    NumeroPaginasDeclaradas = 2,
                    Trd = new TrdStorageDto { IdTipoDocumento = 10 },
                    Inventario = new InventarioDocumentalDto
                    {
                        IdUsuarioGestion = 1,
                        IdEmpresa = 1,
                        Radicado = "RAD-1"
                    },
                    Workflow = new WorkflowStorageDto
                    {
                        IdTareaWorkflow = idTareaWorkflow,
                        IdRutaWorkflow = 9
                    },
                    CamposIndexacion =
                    {
                        new CampoIndexacionDto { NombreCampo = "campoA", Valor = "valorA" },
                        new CampoIndexacionDto { NombreCampo = "campoB", Valor = "valorB" }
                    },
                    Documentos = new[]
                    {
                        new DocumentoEntradaDto
                        {
                            IdDocumento = "1",
                            ArchivoTemporalId = "doc-1.pdf",
                            NumeroPaginas = 2
                        }
                    }
                }
            };
        }

        private static StorageTransactionResult BuildResult(long idAlmacen)
        {
            return new StorageTransactionResult
            {
                IdentityReservation = new StorageIdentityReservationResult
                {
                    Identity = new StorageIdentityModel
                    {
                        IdAlmacen = idAlmacen,
                        Disco = 1,
                        Carpeta = 1,
                        NumeroPaginasCarpeta = 2
                    }
                }
            };
        }
    }
}
