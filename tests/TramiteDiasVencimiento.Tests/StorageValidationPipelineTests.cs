using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Metadata;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Options;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Preindex;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Validation;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageValidationPipelineTests
    {
        [Fact]
        public async Task PreindexValidator_ShouldSkip_WhenTipoIsNotBatchPreindex()
        {
            var resolver = new Mock<IStoragePreindexResolver>(MockBehavior.Strict);
            var reader = new Mock<IStoragePreindexReader>(MockBehavior.Strict);
            var integrator = new Mock<IStoragePreindexIntegrator>(MockBehavior.Strict);
            var logger = new Mock<ILogger<PreindexValidator>>();
            var validator = new PreindexValidator(resolver.Object, reader.Object, integrator.Object, logger.Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(BuildContext(TipoAlmacenamientoEnum.Manual), errors);

            Assert.Empty(errors);
            resolver.Verify(x => x.Resolve(It.IsAny<StorageContext>()), Times.Never);
        }

        [Fact]
        public async Task PreindexValidator_ShouldReturnNotFound_WhenBatchAndNoFile()
        {
            var resolver = new Mock<IStoragePreindexResolver>();
            resolver.Setup(x => x.Resolve(It.IsAny<StorageContext>()))
                .Returns(new StoragePreindexFile { Found = false });

            var reader = new Mock<IStoragePreindexReader>();
            var integrator = new Mock<IStoragePreindexIntegrator>(MockBehavior.Strict);
            var validator = new PreindexValidator(
                resolver.Object,
                reader.Object,
                integrator.Object,
                new Mock<ILogger<PreindexValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(BuildContext(TipoAlmacenamientoEnum.BatchPreindex), errors);

            Assert.Contains(errors, e => e.Code == "PREINDEX_NOT_FOUND");
        }

        [Fact]
        public async Task PreindexValidator_ShouldReadValues_WhenBatchAndTxtValid()
        {
            var resolver = new Mock<IStoragePreindexResolver>();
            resolver.Setup(x => x.Resolve(It.IsAny<StorageContext>()))
                .Returns(new StoragePreindexFile { Found = true, SourceFile = "a.txt" });

            var reader = new Mock<IStoragePreindexReader>();
            reader.Setup(x => x.ReadAsync(It.IsAny<StoragePreindexFile>()))
                .ReturnsAsync(new StoragePreindexResult
                {
                    Found = true,
                    SourceFile = "a.txt",
                    Values = new[] { "a", "b" }
                });

            var integrator = new StoragePreindexIntegrator();
            var context = BuildContext(TipoAlmacenamientoEnum.BatchPreindex, campos: new List<CampoIndexacionDto>
            {
                new() { NombreCampo = "campo1", Valor = "" },
                new() { NombreCampo = "campo2", Valor = "" }
            });
            var validator = new PreindexValidator(
                resolver.Object,
                reader.Object,
                integrator,
                new Mock<ILogger<PreindexValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(context, errors);

            Assert.Empty(errors);
            Assert.Equal(2, context.PreindexValues.Count);
            Assert.Equal("a", context.EffectiveCamposIndexacion[0].Valor);
            Assert.Equal("b", context.EffectiveCamposIndexacion[1].Valor);
        }

        [Fact]
        public async Task GabineteRequiredFieldsValidator_ShouldReturnNotFound_WhenMetadataQueryFails()
        {
            var provider = new Mock<IStorageGabineteMetadataProvider>();
            provider.Setup(x => x.GetFieldsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("No existe metadata para gabinete"));

            var validator = new GabineteRequiredFieldsValidator(
                provider.Object,
                new Mock<ILogger<GabineteRequiredFieldsValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(BuildContext(), errors);

            Assert.Contains(errors, e => e.Code == "GAB_FIELDS_NOT_FOUND");
        }

        [Fact]
        public async Task GabineteRequiredFieldsValidator_ShouldReturnMismatch_WhenCountDoesNotMatch()
        {
            var provider = new Mock<IStorageGabineteMetadataProvider>();
            provider.Setup(x => x.GetFieldsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<GabineteFieldMetadata>
                {
                    new() { FieldName = "campo1", IsRequired = true, Orden = 1 },
                    new() { FieldName = "campo2", IsRequired = false, Orden = 2 }
                });

            var validator = new GabineteRequiredFieldsValidator(
                provider.Object,
                new Mock<ILogger<GabineteRequiredFieldsValidator>>().Object);
            var errors = new List<StorageError>();
            var context = BuildContext(campos: new List<CampoIndexacionDto>
            {
                new() { NombreCampo = "campo1", Valor = "v1" }
            });

            await validator.ValidateAsync(context, errors);

            Assert.Contains(errors, e => e.Code == "GAB_FIELDS_MISMATCH");
        }

        [Fact]
        public async Task GabineteRequiredFieldsValidator_ShouldReturnRequiredEmpty_WhenRequiredIsBlank()
        {
            var provider = new Mock<IStorageGabineteMetadataProvider>();
            provider.Setup(x => x.GetFieldsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<GabineteFieldMetadata>
                {
                    new() { FieldName = "campo1", IsRequired = true, Orden = 1 }
                });

            var validator = new GabineteRequiredFieldsValidator(
                provider.Object,
                new Mock<ILogger<GabineteRequiredFieldsValidator>>().Object);

            var errors = new List<StorageError>();
            await validator.ValidateAsync(BuildContext(evaluaObligatorios: true, campos: new List<CampoIndexacionDto>
            {
                new() { NombreCampo = "campo1", Valor = "" }
            }), errors);

            Assert.Contains(errors, e => e.Code == "GAB_REQUIRED_EMPTY");
        }

        [Fact]
        public async Task StorageOptionsValidator_ShouldRequireInventario_WhenOptionApplies()
        {
            var resolver = new Mock<IStorageOptionsResolver>();
            resolver.Setup(x => x.ResolveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new StorageResolvedOptionsModel { AplicaInventarioDocumental = true });

            var context = BuildContext();
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = "gabinete",
                RutaTemporalId = "tmp1",
                NombreDocumento = "doc.pdf",
                RequestId = "req-1",
                Documentos = context.Command!.Documentos,
                Inventario = null
            };

            var validator = new StorageOptionsValidator(resolver.Object, new Mock<ILogger<StorageOptionsValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(context, errors);

            Assert.Contains(errors, e => e.Code == "INV_REQUIRED");
        }

        [Fact]
        public async Task TrdRulesValidator_ShouldValidateNegativeIds_WhenTrdApplies()
        {
            var resolver = new Mock<IStorageOptionsResolver>();
            resolver.Setup(x => x.ResolveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new StorageResolvedOptionsModel { AplicaTrd = true });

            var context = BuildContext();
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = "gabinete",
                RutaTemporalId = "tmp1",
                NombreDocumento = "doc.pdf",
                RequestId = "req-1",
                Documentos = context.Command!.Documentos,
                Trd = new TrdStorageDto { IdArea = -1, IdSerie = -1, IdSubSerie = -1, IdTipoDocumento = -1 }
            };

            var validator = new TrdRulesValidator(resolver.Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(context, errors);

            Assert.Contains(errors, e => e.Code == "TRD_INVALID_AREA");
            Assert.Contains(errors, e => e.Code == "TRD_INVALID_SERIE");
            Assert.Contains(errors, e => e.Code == "TRD_INVALID_SUBSERIE");
            Assert.Contains(errors, e => e.Code == "TRD_INVALID_TIPO_DOCUMENTO");
        }

        [Fact]
        public async Task ExpedienteUnidadRulesValidator_ShouldRequireClase_WhenOptionApplies()
        {
            var resolver = new Mock<IStorageOptionsResolver>();
            resolver.Setup(x => x.ResolveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new StorageResolvedOptionsModel { AplicaUnidadConservacion = true });

            var context = BuildContext();
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = "gabinete",
                RutaTemporalId = "tmp1",
                NombreDocumento = "doc.pdf",
                RequestId = "req-1",
                Documentos = context.Command!.Documentos,
                Expediente = new ExpedienteStorageDto { IdExpediente = 10, IdUnidadConservacion = 20, IdClaseDocumento = null }
            };

            var validator = new ExpedienteUnidadRulesValidator(resolver.Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(context, errors);

            Assert.Contains(errors, e => e.Code == "EXP_CLASE_REQUIRED");
            Assert.Contains(errors, e => e.Code == "UNI_CLASE_REQUIRED");
            Assert.Contains(errors, e => e.Code == "EXP_UNI_AMBIGUO");
        }

        private static StorageContext BuildContext(
            TipoAlmacenamientoEnum tipo = TipoAlmacenamientoEnum.Manual,
            bool evaluaObligatorios = false,
            IReadOnlyList<CampoIndexacionDto>? campos = null)
        {
            return new StorageContext
            {
                DefaultDbAlias = "db",
                Usuario = "usr",
                UsuarioId = 1,
                RequestId = "req-1",
                NombreGabinete = "gabinete",
                RutaTemporalId = "tmp1",
                NombreDocumento = "doc.pdf",
                ArchivosTemporales = new List<string> { "file1" },
                Command = new AlmacenarDocumentoCommand
                {
                    NombreGabinete = "gabinete",
                    RutaTemporalId = "tmp1",
                    NombreDocumento = "doc.pdf",
                    RequestId = "req-1",
                    TipoAlmacenamiento = tipo,
                    EvaluarCamposObligatorios = evaluaObligatorios,
                    Documentos = new List<DocumentoEntradaDto>
                    {
                        new DocumentoEntradaDto
                        {
                            IdDocumento = "1",
                            ArchivoTemporalId = "file1",
                            NumeroPaginas = 1
                        }
                    },
                    CamposIndexacion = campos ?? new List<CampoIndexacionDto>
                    {
                        new CampoIndexacionDto { NombreCampo = "campo1", Valor = "valor1" }
                    }
                }
            };
        }
    }
}

