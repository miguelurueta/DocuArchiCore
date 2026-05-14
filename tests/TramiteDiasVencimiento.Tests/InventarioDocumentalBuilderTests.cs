using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Expediente;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Metadata;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Inventario;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public sealed class InventarioDocumentalBuilderTests
    {
        private readonly InventarioDocumentalBuilder _builder = new();

        [Fact]
        public void Build_ShouldSkip_WhenInventarioOptionDisabled()
        {
            var context = BuildContext(aplicaInventario: false);

            var result = _builder.Build(context, new StorageIdentityModel { IdAlmacen = 100 }, BuildNaming());

            Assert.False(result.ShouldInsert);
            Assert.Null(result.Model);
        }

        [Fact]
        public void Build_ShouldThrow_WhenInventarioIsRequiredAndMissing()
        {
            var context = BuildContext(aplicaInventario: true);
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = context.NombreGabinete,
                RutaTemporalId = "tmp",
                NombreDocumento = "doc.pdf",
                RequestId = context.RequestId,
                Documentos = new[] { new DocumentoEntradaDto { IdDocumento = "1", ArchivoTemporalId = "tmp-1", NumeroPaginas = 2 } },
                Inventario = null
            };

            Assert.Throws<InvalidOperationException>(() =>
                _builder.Build(context, new StorageIdentityModel { IdAlmacen = 101 }, BuildNaming()));
        }

        [Fact]
        public void Build_ShouldThrow_WhenPhysicalMetadataMissing()
        {
            var context = BuildContext(aplicaInventario: true);
            context.PhysicalMetadata = null;

            Assert.Throws<InvalidOperationException>(() =>
                _builder.Build(context, new StorageIdentityModel { IdAlmacen = 102 }, BuildNaming()));
        }

        [Fact]
        public void Build_ShouldMapLegacyFields_WhenOptionsApply()
        {
            var context = BuildContext(aplicaInventario: true);

            var result = _builder.Build(
                context,
                new StorageIdentityModel { IdAlmacen = 103 },
                BuildNaming());

            Assert.True(result.ShouldInsert);
            Assert.NotNull(result.Model);
            Assert.Equal(1, result.Model!.IdUsuarioGestion);
            Assert.Equal(10, result.Model.IdEmpresaDocumento);
            Assert.Equal(103, result.Model.IdDocumentoDocuarchiAlmacen);
            Assert.Equal("gab", result.Model.NombreGabinete);
            Assert.Equal(7, result.Model.NumeroFolios);
            Assert.Equal(".PDF", result.Model.Formato);
            Assert.Equal("3.52 Mb", result.Model.Tamano);
            Assert.Equal("SEGUNDO.pdf", result.Model.SegundoNombreDocumento);
            Assert.Equal(0, result.Model.EstadoDocumentoArchivo);
            Assert.Equal(1, result.Model.IdTipoUnidadDocumental);
            Assert.Equal("v1\r\nv2", result.Model.FullTextDocumento);
            Assert.Null(result.Model.UnidadConserva);
        }

        [Fact]
        public void Build_ShouldUseCommandFullText_WhenProvided()
        {
            var context = BuildContext(aplicaInventario: true);
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = "gab",
                RutaTemporalId = "tmp",
                NombreDocumento = "doc.pdf",
                RequestId = "req",
                FullText = "FULLTEXT-CMD",
                Documentos = new[]
                {
                    new DocumentoEntradaDto
                    {
                        IdDocumento = "1",
                        ArchivoTemporalId = "tmp-1",
                        NumeroPaginas = 2,
                        Extension = ".pdf"
                    }
                },
                Inventario = new InventarioDocumentalDto { IdUsuarioGestion = 1, IdEmpresa = 10 },
                Expediente = new ExpedienteStorageDto()
            };

            var result = _builder.Build(
                context,
                new StorageIdentityModel { IdAlmacen = 104 },
                BuildNaming());

            Assert.Equal("FULLTEXT-CMD", result.Model!.FullTextDocumento);
        }

        [Fact]
        public void Build_ShouldPrioritizeIdTipoUnidadDocumental_FromExpedienteUnidadResult()
        {
            var context = BuildContext(aplicaInventario: true);
            context.ExpedienteUnidadResult = new ExpedienteUnidadLegacyResult
            {
                Ejecutado = true,
                IdTipoUnidadDocumental = 2,
                TieneExpediente = true,
                NumeroFolios = 7,
                UnidadConservaTipo = UnidadConservaTipoEnum.Electronico
            };

            var result = _builder.Build(
                context,
                new StorageIdentityModel { IdAlmacen = 105 },
                BuildNaming());

            Assert.Equal(2, result.Model!.IdTipoUnidadDocumental);
        }

        [Fact]
        public void Build_ShouldNotFallbackToIdText_ForDescriptorFields()
        {
            var context = BuildContext(aplicaInventario: true);
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = "gab",
                RutaTemporalId = "tmp",
                NombreDocumento = "doc.pdf",
                RequestId = "req",
                Documentos =
                [
                    new DocumentoEntradaDto
                    {
                        IdDocumento = "1",
                        ArchivoTemporalId = "tmp-1",
                        NumeroPaginas = 2,
                        Extension = ".pdf"
                    }
                ],
                Inventario = new InventarioDocumentalDto
                {
                    IdUsuarioGestion = 1,
                    IdEmpresa = 10,
                    FechaElaboracion = "2025-12-24",
                    Radicado = "RAD-1"
                },
                Trd = new TrdStorageDto
                {
                    IdArea = 11,
                    IdSerie = 22,
                    IdSubSerie = 33,
                    IdTipoDocumento = 44
                },
                Expediente = new ExpedienteStorageDto
                {
                    IdExpediente = 55,
                    IdClaseDocumento = 99
                }
            };
            context.ResolvedDescriptors = null;

            var result = _builder.Build(
                context,
                new StorageIdentityModel { IdAlmacen = 106 },
                BuildNaming());

            Assert.Null(result.Model!.NombreAreaDepartamento);
            Assert.Null(result.Model.SerieDocumento);
            Assert.Null(result.Model.SubserieDocumento);
            Assert.Null(result.Model.DescripcionTipoDocumento);
        }

        [Fact]
        public void Build_ShouldHomologateDescripcionTipoDocumento_FromGabineteTipodocumentoCampo()
        {
            var context = BuildContext(aplicaInventario: true);
            context.ResolvedDescriptors = null;
            var original = context.Command!;
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = original.NombreGabinete,
                RutaTemporalId = original.RutaTemporalId,
                NombreDocumento = original.NombreDocumento,
                RequestId = original.RequestId,
                Documentos = original.Documentos,
                Inventario = original.Inventario,
                CamposIndexacion = original.CamposIndexacion,
                Workflow = original.Workflow,
                FullText = original.FullText,
                NumeroPaginasDeclaradas = original.NumeroPaginasDeclaradas,
                TipoAlmacenamiento = original.TipoAlmacenamiento,
                EvaluarCamposObligatorios = original.EvaluarCamposObligatorios,
                Expediente = original.Expediente,
                Trd = new TrdStorageDto
                {
                    IdArea = original.Trd!.IdArea,
                    IdSerie = original.Trd.IdSerie,
                    IdSubSerie = original.Trd.IdSubSerie,
                    IdTipoDocumento = original.Trd.IdTipoDocumento,
                    NombreArea = original.Trd.NombreArea,
                    NombreSerie = original.Trd.NombreSerie,
                    NombreSubSerie = original.Trd.NombreSubSerie,
                    NombreTipoDocumento = null
                }
            };
            context.EffectiveCamposIndexacion = new[]
            {
                new CampoIndexacionDto { NombreCampo = "TIPODOCUMENTO", Valor = "FACTURA ELECTRONICA" }
            };

            var result = _builder.Build(
                context,
                new StorageIdentityModel { IdAlmacen = 107 },
                BuildNaming());

            Assert.Equal("FACTURA ELECTRONICA", result.Model!.DescripcionTipoDocumento);
        }

        private static StorageContext BuildContext(bool aplicaInventario)
        {
            return new StorageContext
            {
                DefaultDbAlias = "db",
                Usuario = "u",
                UsuarioId = 1,
                RequestId = "req",
                NombreGabinete = "gab",
                RutaTemporalId = "tmp",
                NombreDocumento = "doc.pdf",
                ArchivosTemporales = new[] { "tmp-1" },
                ResolvedOptions = new StorageResolvedOptionsModel
                {
                    NombreGabinete = "gab",
                    AplicaInventarioDocumental = aplicaInventario
                },
                PhysicalMetadata = new StorageDocumentPhysicalMetadata
                {
                    TotalBytes = 3_690_000,
                    TamanoLegacy = "3.52 Mb",
                    Formato = ".PDF",
                    NumeroPaginas = 7,
                    PaginasCalculadasDesdeArchivo = true
                },
                EffectiveCamposIndexacion = new[]
                {
                    new CampoIndexacionDto { NombreCampo = "A", Valor = "v1" },
                    new CampoIndexacionDto { NombreCampo = "B", Valor = "v2" }
                },
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp",
                    NombreDocumento = "doc.pdf",
                    RequestId = "req",
                    Documentos = new[]
                    {
                        new DocumentoEntradaDto
                        {
                            IdDocumento = "1",
                            ArchivoTemporalId = "tmp-1",
                            NumeroPaginas = 2,
                            Extension = ".pdf"
                        }
                    },
                    Inventario = new InventarioDocumentalDto
                    {
                        IdUsuarioGestion = 1,
                        IdEmpresa = 10,
                        FechaElaboracion = "2025-12-24",
                        Radicado = "RAD-1"
                    },
                    Trd = new TrdStorageDto
                    {
                        IdArea = 11,
                        IdSerie = 22,
                        IdSubSerie = 33,
                        IdTipoDocumento = 44,
                        NombreArea = "Area",
                        NombreSerie = "Serie",
                        NombreSubSerie = "Subserie",
                        NombreTipoDocumento = "TipoDoc"
                    },
                    Expediente = new ExpedienteStorageDto
                    {
                        IdExpediente = 55,
                        IdTipoExpediente = 66,
                        IdUnidadConservacion = 77,
                        IdTipoUnidadConservacion = 88,
                        IdClaseDocumento = 99,
                        NombreExpediente = "EXP-55",
                        ClaseDocumento = "CLASE-A",
                        NombreUnidadConservacion = "UNIDAD-X"
                    }
                }
            };
        }

        private static StorageNamingResult BuildNaming()
        {
            return new StorageNamingResult
            {
                NombreArchivoPrincipal = "DIG00000103.pdf",
                NombreXml = "FXL00000103.xml",
                SegundoNombre = "SEGUNDO.pdf"
            };
        }
    }
}
