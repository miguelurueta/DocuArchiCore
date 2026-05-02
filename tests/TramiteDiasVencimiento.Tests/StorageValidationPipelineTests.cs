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
            var reader = new Mock<IStoragePreindexReader>(MockBehavior.Strict);
            var logger = new Mock<ILogger<PreindexValidator>>();
            var validator = new PreindexValidator(reader.Object, logger.Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(BuildContext(TipoAlmacenamientoEnum.Manual), errors);

            Assert.Empty(errors);
            reader.Verify(x => x.ReadAsync(It.IsAny<StorageContext>()), Times.Never);
        }

        [Fact]
        public async Task PreindexValidator_ShouldReturnNotFound_WhenBatchAndNoFile()
        {
            var reader = new Mock<IStoragePreindexReader>();
            reader.Setup(x => x.ReadAsync(It.IsAny<StorageContext>()))
                .ReturnsAsync(new StoragePreindexResult { Found = false });

            var validator = new PreindexValidator(reader.Object, new Mock<ILogger<PreindexValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(BuildContext(TipoAlmacenamientoEnum.BatchPreindex), errors);

            Assert.Contains(errors, e => e.Code == "PREINDEX_NOT_FOUND");
        }

        [Fact]
        public async Task PreindexValidator_ShouldReadValues_WhenBatchAndTxtValid()
        {
            var reader = new Mock<IStoragePreindexReader>();
            reader.Setup(x => x.ReadAsync(It.IsAny<StorageContext>()))
                .ReturnsAsync(new StoragePreindexResult
                {
                    Found = true,
                    SourceFile = "a.txt",
                    Values = new[] { "a", "b" }
                });

            var context = BuildContext(TipoAlmacenamientoEnum.BatchPreindex);
            var validator = new PreindexValidator(reader.Object, new Mock<ILogger<PreindexValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(context, errors);

            Assert.Empty(errors);
            Assert.Equal(2, context.PreindexValues.Count);
        }

        [Fact]
        public async Task PreindexValidator_ShouldReturnPathInvalid_WhenPathIsInvalid()
        {
            var reader = new Mock<IStoragePreindexReader>();
            reader.Setup(x => x.ReadAsync(It.IsAny<StorageContext>()))
                .ThrowsAsync(new InvalidOperationException("PREINDEX_PATH_INVALID"));

            var validator = new PreindexValidator(reader.Object, new Mock<ILogger<PreindexValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(BuildContext(TipoAlmacenamientoEnum.BatchPreindex), errors);

            Assert.Contains(errors, e => e.Code == "PREINDEX_PATH_INVALID");
        }

        [Fact]
        public async Task GabineteRequiredFieldsValidator_ShouldReturnNotFound_WhenMetadataEmpty()
        {
            var provider = new Mock<IStorageGabineteMetadataProvider>();
            provider.Setup(x => x.GetFieldsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Array.Empty<GabineteFieldMetadata>());

            var validator = new GabineteRequiredFieldsValidator(provider.Object, new Mock<ILogger<GabineteRequiredFieldsValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(BuildContext(), errors);

            Assert.Contains(errors, e => e.Code == "GAB_FIELDS_NOT_FOUND");
        }

        [Fact]
        public async Task GabineteRequiredFieldsValidator_ShouldReturnMismatchAndRequiredEmpty()
        {
            var provider = new Mock<IStorageGabineteMetadataProvider>();
            provider.Setup(x => x.GetFieldsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<GabineteFieldMetadata>
                {
                    new() { FieldName = "campo1", IsRequired = true, Orden = 1 },
                    new() { FieldName = "campo2", IsRequired = false, Orden = 2 }
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

            var validator = new GabineteRequiredFieldsValidator(provider.Object, new Mock<ILogger<GabineteRequiredFieldsValidator>>().Object);
            var errors = new List<StorageError>();

            await validator.ValidateAsync(context, errors);

            Assert.Contains(errors, e => e.Code == "GAB_FIELDS_MISMATCH");
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
