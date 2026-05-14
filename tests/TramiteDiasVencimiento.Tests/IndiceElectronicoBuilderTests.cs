using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Expediente;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class IndiceElectronicoBuilderTests
    {
        [Fact]
        public void Build_ShouldCreateDeterministicHashForSameInput()
        {
            var builder = new IndiceElectronicoBuilder();
            var context = BuildContext();
            var expediente = BuildExpediente();
            var calculation = new IndiceElectronicoCalculationResult
            {
                NuevoOrden = 4,
                PaginaInicial = 11,
                PaginaFinal = 14,
                NumeroFolios = 4
            };
            var naming = BuildNaming();
            var physicalPath = BuildPhysicalPath();

            var first = builder.Build(context, 100, expediente, calculation, naming, physicalPath);
            var second = builder.Build(context, 100, expediente, calculation, naming, physicalPath);

            Assert.Equal("SHA256", first.FuncionResumen);
            Assert.Equal(first.ValorHuella, second.ValorHuella);
            Assert.NotEqual(string.Empty, first.ValorHuella);
            Assert.Equal(64, first.ValorHuella.Length);
            Assert.Equal("DIG00000100.pdf", first.NombreDocumento);
            Assert.Equal("TIPO-TRD", first.TipologiaDocumental);
            Assert.Equal("D:/imagenes/discos/CONTABIL7/00093/DIG00000100.pdf", first.RutaDocumento);
        }

        [Fact]
        public void Build_ShouldThrow_WhenNombreDocumentoIsMissingInNaming()
        {
            var builder = new IndiceElectronicoBuilder();
            var context = BuildContext();
            var naming = new StorageNamingResult
            {
                NombreArchivoPrincipal = string.Empty,
                NombreXml = "FXL00000100.xml",
                SegundoNombre = "SEGUNDO.pdf"
            };

            Assert.Throws<StorageTransactionException>(() => builder.Build(
                context,
                100,
                BuildExpediente(),
                new IndiceElectronicoCalculationResult
                {
                    NuevoOrden = 4,
                    PaginaInicial = 11,
                    PaginaFinal = 14,
                    NumeroFolios = 4
                },
                naming,
                BuildPhysicalPath()));
        }

        [Fact]
        public void Build_ShouldFallbackToIdTipoDocumento_WhenTipologiaTextIsMissing()
        {
            var builder = new IndiceElectronicoBuilder();
            var context = BuildContext(
                resolvedTipoDocumento: null,
                idTipoDocumento: 43,
                nombreTipoDocumento: null);

            var model = builder.Build(
                context,
                100,
                BuildExpediente(),
                new IndiceElectronicoCalculationResult
                {
                    NuevoOrden = 4,
                    PaginaInicial = 11,
                    PaginaFinal = 14,
                    NumeroFolios = 4
                },
                BuildNaming(),
                BuildPhysicalPath());

            Assert.Equal("43", model.TipologiaDocumental);
        }

        private static StorageContext BuildContext(
            string? resolvedTipoDocumento = "TIPO-TRD",
            int idTipoDocumento = 77,
            string? nombreTipoDocumento = null)
        {
            return new StorageContext
            {
                DefaultDbAlias = "da",
                Usuario = "user",
                UsuarioId = 1,
                RequestId = "req-1",
                NombreGabinete = "gab",
                RutaTemporalId = "tmp",
                NombreDocumento = "documento.pdf",
                ArchivosTemporales = new[] { "tmp-1" },
                ResolvedDescriptors = resolvedTipoDocumento == null
                    ? null
                    : new StorageResolvedDescriptorsModel
                    {
                        TipoDocumento = resolvedTipoDocumento
                    },
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp",
                    NombreDocumento = "documento.pdf",
                    RequestId = "req-1",
                    NumeroPaginasDeclaradas = 4,
                    Trd = new TrdStorageDto
                    {
                        IdTipoDocumento = idTipoDocumento,
                        NombreTipoDocumento = nombreTipoDocumento
                    },
                    Inventario = new InventarioDocumentalDto { IdEmpresa = 1, IdUsuarioGestion = 1, Radicado = "RAD-1" },
                    Documentos = new[]
                    {
                        new DocumentoEntradaDto
                        {
                            IdDocumento = "1",
                            ArchivoTemporalId = "tmp-1",
                            Extension = "pdf",
                            NumeroPaginas = 4
                        }
                    }
                }
            };
        }

        private static StorageNamingResult BuildNaming()
        {
            return new StorageNamingResult
            {
                NombreArchivoPrincipal = "DIG00000100.pdf",
                NombreXml = "FXL00000100.xml",
                SegundoNombre = "SEGUNDO.pdf"
            };
        }

        private static StoragePhysicalPathModel BuildPhysicalPath()
        {
            return new StoragePhysicalPathModel
            {
                StorageRoot = "D:/imagenes/discos",
                NombreGabinete = "CONTABIL",
                Disco = 7,
                Carpeta = 93,
                RutaGabineteDisco = "D:/imagenes/discos/CONTABIL7",
                CarpetaLegacy = "00093",
                RutaFinal = @"D:\imagenes\discos\CONTABIL7\00093"
            };
        }

        private static ExpedienteInfoModel BuildExpediente()
        {
            return new ExpedienteInfoModel
            {
                IdExpediente = 10,
                IdUnidadConservacion = 20,
                EstadoExpediente = 1,
                EstadoExpedienteElectronico = 1,
                NumeroFoliosContenidos = 50,
                OrdenIndice = 3,
                UltimaPaginaIndice = 10,
                CodigoUnico = "EXP-1"
            };
        }
    }
}
