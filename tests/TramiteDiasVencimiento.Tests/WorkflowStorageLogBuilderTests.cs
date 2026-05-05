using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical;
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
            var identity = BuildIdentity(idAlmacen: 101);
            var naming = BuildNaming();
            var path = BuildPath();

            var result = builder.Build(context, identity, naming, path);
            var model = result.Model!;

            Assert.True(result.ShouldInsert);
            Assert.Equal("READY", result.Estado);
            Assert.Equal(101, model.IdTran);
            Assert.Equal("Registra", model.DescOp);
            Assert.Equal("user-1", model.UserOper);
            Assert.Equal("gab", model.Gabinete);
            Assert.Equal(@"C:\storage\gab2\00001\DIG00000101.pdf", model.RutDocu);
            Assert.Equal("RAD-1", model.Radicado);
            Assert.Equal(77, model.IdTareaWorkflow);
            Assert.Equal(9, model.IdRutaWorkflow);
            Assert.Equal("user-1", model.UserPropietario);
            Assert.Equal("TIPO-10", model.TipologiaDocumental);
            Assert.Equal("|valorA|valorB", model.Campos);
            Assert.Equal("10.1.1.20", model.IpTrans);
        }

        [Fact]
        public void Build_ShouldReturnNoWorkflow_WhenWorkflowTaskIsInvalid()
        {
            var builder = new WorkflowStorageLogBuilder();
            var context = BuildContext(idTareaWorkflow: 0);
            var identity = BuildIdentity(idAlmacen: 101);

            var result = builder.Build(context, identity, BuildNaming(), BuildPath());
            Assert.False(result.ShouldInsert);
            Assert.Equal("NO_WORKFLOW", result.Estado);
            Assert.Null(result.Model);
        }

        [Fact]
        public void Build_ShouldThrow_WhenIdentityIsInvalid()
        {
            var builder = new WorkflowStorageLogBuilder();
            var context = BuildContext(idTareaWorkflow: 99);
            var identity = BuildIdentity(idAlmacen: 0);

            Assert.Throws<InvalidOperationException>(() => builder.Build(context, identity, BuildNaming(), BuildPath()));
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
                    Trd = new TrdStorageDto { IdTipoDocumento = 10, NombreTipoDocumento = "TIPO-10" },
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
                    CamposIndexacion = new[]
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
                ,
                IpTrans = "10.1.1.20"
            };
        }

        private static StorageIdentityModel BuildIdentity(long idAlmacen)
        {
            return new StorageIdentityModel
            {
                IdAlmacen = idAlmacen,
                Disco = 2,
                Carpeta = 1,
                NumeroPaginasCarpeta = 2
            };
        }

        private static StorageNamingResult BuildNaming() =>
            new()
            {
                NombreArchivoPrincipal = "DIG00000101.pdf",
                NombreXml = "FXL00000101.xml",
                SegundoNombre = "SEGUNDO.pdf"
            };

        private static StoragePhysicalPathModel BuildPath() =>
            new()
            {
                StorageRoot = @"C:\storage",
                NombreGabinete = "gab",
                Disco = 2,
                Carpeta = 1,
                RutaGabineteDisco = @"C:\storage\gab2",
                CarpetaLegacy = "00001",
                RutaFinal = @"C:\storage\gab2\00001"
            };
    }
}
