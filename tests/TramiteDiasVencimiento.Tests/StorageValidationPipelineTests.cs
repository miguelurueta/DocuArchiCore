using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Metadata;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Options;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Physical;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Preindex;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Transaction;
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
            reader.Verify(x => x.ReadAsync(It.IsAny<StoragePreindexFile>()), Times.Never);
            integrator.Verify(x => x.Integrate(It.IsAny<StorageContext>(), It.IsAny<StoragePreindexResult>()), Times.Never);
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
            reader.Verify(x => x.ReadAsync(It.IsAny<StoragePreindexFile>()), Times.Never);
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
            var context = BuildContext(TipoAlmacenamientoEnum.BatchPreindex);
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = context.Command!.NombreGabinete,
                RutaTemporalId = context.Command.RutaTemporalId,
                NombreDocumento = context.Command.NombreDocumento,
                RequestId = context.Command.RequestId,
                TipoAlmacenamiento = context.Command.TipoAlmacenamiento,
                Documentos = context.Command.Documentos,
                CamposIndexacion = new List<CampoIndexacionDto>
                {
                    new() { NombreCampo = "campo1", Valor = "" },
                    new() { NombreCampo = "campo2", Valor = "" }
                }
            };
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
        public async Task PreindexValidator_ShouldReturnPathInvalid_WhenPathIsInvalid()
        {
            var resolver = new Mock<IStoragePreindexResolver>();
            resolver.Setup(x => x.Resolve(It.IsAny<StorageContext>()))
                .Throws(new InvalidOperationException("PREINDEX_PATH_INVALID"));

            var reader = new Mock<IStoragePreindexReader>();
            var integrator = new Mock<IStoragePreindexIntegrator>(MockBehavior.Strict);

            var validator = new PreindexValidator(
                resolver.Object,
                reader.Object,
                integrator.Object,
                new Mock<ILogger<PreindexValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(BuildContext(TipoAlmacenamientoEnum.BatchPreindex), errors);

            Assert.Contains(errors, e => e.Code == "PREINDEX_PATH_INVALID");
        }

        [Fact]
        public async Task PreindexValidator_ShouldReturnMismatch_WhenValuesCountDoesNotMatchCampos()
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
                    Values = new[] { "uno", "dos" }
                });

            var context = BuildContext(TipoAlmacenamientoEnum.BatchPreindex);
            var validator = new PreindexValidator(
                resolver.Object,
                reader.Object,
                new StoragePreindexIntegrator(),
                new Mock<ILogger<PreindexValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(context, errors);

            Assert.Contains(errors, e => e.Code == "PREINDEX_FIELDS_MISMATCH");
        }

        [Fact]
        public async Task PreindexValidator_ShouldNotOverwriteManualValues_WhenPreindexHasData()
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
                    Values = new[] { "pre-1", "pre-2" }
                });

            var context = BuildContext(TipoAlmacenamientoEnum.BatchPreindex);
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = context.Command!.NombreGabinete,
                RutaTemporalId = context.Command.RutaTemporalId,
                NombreDocumento = context.Command.NombreDocumento,
                RequestId = context.Command.RequestId,
                TipoAlmacenamiento = context.Command.TipoAlmacenamiento,
                Documentos = context.Command.Documentos,
                CamposIndexacion = new List<CampoIndexacionDto>
                {
                    new() { NombreCampo = "campo1", Valor = "manual-1" },
                    new() { NombreCampo = "campo2", Valor = "" }
                }
            };

            var validator = new PreindexValidator(
                resolver.Object,
                reader.Object,
                new StoragePreindexIntegrator(),
                new Mock<ILogger<PreindexValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(context, errors);

            Assert.Empty(errors);
            Assert.Equal("manual-1", context.EffectiveCamposIndexacion[0].Valor);
            Assert.Equal("pre-2", context.EffectiveCamposIndexacion[1].Valor);
        }

        [Fact]
        public async Task GabineteRequiredFieldsValidator_ShouldReturnNotFound_WhenMetadataEmpty()
        {
            var provider = new Mock<IStorageGabineteMetadataProvider>();
            provider.Setup(x => x.GetMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("No existe metadata para gabinete"));

            var requiredFieldsValidator = new Mock<IStorageRequiredFieldsValidator>(MockBehavior.Strict);

            var validator = new GabineteRequiredFieldsValidator(
                provider.Object,
                requiredFieldsValidator.Object,
                new Mock<ILogger<GabineteRequiredFieldsValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(BuildContext(), errors);

            Assert.Contains(errors, e => e.Code == "GAB_FIELDS_NOT_FOUND");
        }

        [Fact]
        public async Task GabineteRequiredFieldsValidator_ShouldReturnMismatch_WhenCountDoesNotMatch()
        {
            var provider = new Mock<IStorageGabineteMetadataProvider>();
            provider.Setup(x => x.GetMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new GabineteMetadataResult
                {
                    Campos = new List<GabineteFieldMetadata>
                    {
                        new() { FieldName = "campo1", IsRequired = true, Orden = 1 },
                        new() { FieldName = "campo2", IsRequired = false, Orden = 2 }
                    }
                });

            var context = BuildContext();
            context.Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = "gabinete",
                RutaTemporalId = "tmp1",
                NombreDocumento = "doc.pdf",
                RequestId = "req-1",
                Documentos = context.Command!.Documentos,
                CamposIndexacion = new List<CampoIndexacionDto>
                {
                    new() { NombreCampo = "campo1", Valor = "" }
                },
                EvaluarCamposObligatorios = true
            };

            var requiredFieldsValidator = new Mock<IStorageRequiredFieldsValidator>();
            requiredFieldsValidator
                .Setup(v => v.Validate(
                    It.IsAny<List<GabineteFieldMetadata>>(),
                    It.IsAny<List<CampoIndexacionDto>>(),
                    It.IsAny<bool>()))
                .Throws(new InvalidOperationException("Cantidad de campos no coincide con metadata"));

            var validator = new GabineteRequiredFieldsValidator(
                provider.Object,
                requiredFieldsValidator.Object,
                new Mock<ILogger<GabineteRequiredFieldsValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(context, errors);

            Assert.Contains(errors, e => e.Code == "GAB_FIELDS_MISMATCH");
        }

        [Fact]
        public async Task GabineteRequiredFieldsValidator_ShouldReturnRequiredEmpty_WhenRequiredIsBlank()
        {
            var provider = new Mock<IStorageGabineteMetadataProvider>();
            provider.Setup(x => x.GetMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new GabineteMetadataResult
                {
                    Campos = new List<GabineteFieldMetadata>
                    {
                        new() { FieldName = "campo1", IsRequired = true, Orden = 1 }
                    }
                });

            var requiredFieldsValidator = new Mock<IStorageRequiredFieldsValidator>();
            requiredFieldsValidator
                .Setup(v => v.Validate(
                    It.IsAny<List<GabineteFieldMetadata>>(),
                    It.IsAny<List<CampoIndexacionDto>>(),
                    It.IsAny<bool>()))
                .Throws(new InvalidOperationException("Campo obligatorio vacío: campo1"));

            var validator = new GabineteRequiredFieldsValidator(
                provider.Object,
                requiredFieldsValidator.Object,
                new Mock<ILogger<GabineteRequiredFieldsValidator>>().Object);

            var errors = new List<StorageError>();
            await validator.ValidateAsync(BuildContext(), errors);

            Assert.Contains(errors, e => e.Code == "GAB_REQUIRED_EMPTY");
        }

        [Fact]
        public async Task StorageOptionsValidator_ShouldRequireInventario_WhenOptionApplies()
        {
            var resolver = new Mock<IStorageOptionsResolver>();
            resolver.Setup(x => x.ResolveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new StorageOptionsModel { AplicaInventarioDocumental = true });

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
                .ReturnsAsync(new StorageOptionsModel { AplicaTrd = true });

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
                .ReturnsAsync(new StorageOptionsModel { AplicaUnidadConservacion = true });

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
        }

        [Fact]
        public async Task Orchestrator_ShouldThrowStorageValidationException_WhenPipelineInvalid()
        {
            var pipeline = new Mock<IStorageValidationPipeline>();
            pipeline.Setup(x => x.ValidateAsync(It.IsAny<StorageContext>()))
                .ReturnsAsync(new StorageValidationResult
                {
                    IsValid = false,
                    Errors = new List<StorageError> { new() { Code = "VAL001", Message = "invalid" } }
                });

            var orchestrator = new DocumentStorageOrchestrator(
                pipeline.Object,
                Mock.Of<IStorageTransactionCoordinator>(),
                Mock.Of<IStoragePhysicalPhaseExecutor>(),
                new Mock<ILogger<DocumentStorageOrchestrator>>().Object);

            await Assert.ThrowsAsync<StorageValidationException>(() => orchestrator.ExecuteAsync(BuildContext()));
        }

        private static StorageContext BuildContext(TipoAlmacenamientoEnum tipo = TipoAlmacenamientoEnum.Manual)
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
                    Documentos = new List<DocumentoEntradaDto>
                    {
                        new DocumentoEntradaDto
                        {
                            IdDocumento = "1",
                            ArchivoTemporalId = "file1",
                            NumeroPaginas = 1
                        }
                    },
                    CamposIndexacion = new List<CampoIndexacionDto>
                    {
                        new CampoIndexacionDto { NombreCampo = "campo1", Valor = "valor1" }
                    }
                }
            };
        }
    }
}
