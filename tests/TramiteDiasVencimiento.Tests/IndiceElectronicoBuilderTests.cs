using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
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

            var first = builder.Build(context, 100, expediente, calculation);
            var second = builder.Build(context, 100, expediente, calculation);

            Assert.Equal("SHA256", first.FuncionResumen);
            Assert.Equal(first.ValorHuella, second.ValorHuella);
            Assert.NotEqual(string.Empty, first.ValorHuella);
            Assert.Equal(64, first.ValorHuella.Length);
        }

        [Fact]
        public void Build_ShouldThrow_WhenNombreDocumentoIsMissing()
        {
            var builder = new IndiceElectronicoBuilder();
            var context = BuildContext();
            context = new StorageContext
            {
                DefaultDbAlias = context.DefaultDbAlias,
                Usuario = context.Usuario,
                UsuarioId = context.UsuarioId,
                RequestId = context.RequestId,
                NombreGabinete = context.NombreGabinete,
                RutaTemporalId = context.RutaTemporalId,
                NombreDocumento = string.Empty,
                ArchivosTemporales = context.ArchivosTemporales,
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = context.Command!.NombreGabinete,
                    RutaTemporalId = context.Command.RutaTemporalId,
                    NombreDocumento = string.Empty,
                    RequestId = context.Command.RequestId,
                    NumeroPaginasDeclaradas = context.Command.NumeroPaginasDeclaradas,
                    Trd = context.Command.Trd,
                    Inventario = context.Command.Inventario,
                    Documentos = context.Command.Documentos
                }
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
                }));
        }

        private static StorageContext BuildContext()
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
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp",
                    NombreDocumento = "documento.pdf",
                    RequestId = "req-1",
                    NumeroPaginasDeclaradas = 4,
                    Trd = new TrdStorageDto { IdTipoDocumento = 77 },
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
