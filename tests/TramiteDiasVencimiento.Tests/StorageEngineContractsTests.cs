using System;
using System.Collections.Generic;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Builders;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageEngineContractsTests
    {
        [Fact]
        public void DtoAndCommandContracts_ShouldBeInstantiable()
        {
            var request = new AlmacenarDocumentoRequest
            {
                NombreGabinete = "gabinete",
                RutaTemporalId = "tmp-1",
                NombreDocumento = "doc.pdf",
                RequestId = "req-1",
                Documentos = new List<DocumentoEntradaDto>
                {
                    new DocumentoEntradaDto
                    {
                        IdDocumento = "d1",
                        ArchivoTemporalId = "file-1",
                        NumeroPaginas = 1
                    }
                }
            };

            var command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = request.NombreGabinete,
                RutaTemporalId = request.RutaTemporalId,
                NombreDocumento = request.NombreDocumento,
                RequestId = request.RequestId,
                Documentos = request.Documentos
            };

            Assert.Equal("file-1", request.Documentos[0].ArchivoTemporalId);
            Assert.Equal("req-1", command.RequestId);
        }

        [Fact]
        public void ResponseAndResult_ShouldIncludeRequestIdAndEstado()
        {
            var response = new AlmacenarDocumentoResponse
            {
                IdAlmacen = 10,
                RequestId = "req-2"
            };

            var result = new AlmacenarDocumentoResult
            {
                IdAlmacen = 10,
                RequestId = "req-2",
                Estado = StorageDocumentState.PhysicalFailed
            };

            Assert.Equal("req-2", response.RequestId);
            Assert.Equal(StorageDocumentState.PhysicalFailed, result.Estado);
        }

        [Fact]
        public void ExceptionsAndValidationContracts_ShouldPreserveErrors()
        {
            var errors = new List<StorageError>
            {
                new StorageError { Code = "VAL001", Message = "invalid", Phase = StoragePhase.Validation }
            };

            var ex = new StorageValidationException(errors);
            var physicalEx = new StoragePhysicalException("fs failed");
            var txEx = new StorageTransactionException("tx failed", new InvalidOperationException("inner"));

            Assert.Single(ex.Errors);
            Assert.IsType<StoragePhysicalException>(physicalEx);
            Assert.IsType<StorageTransactionException>(txEx);
        }

        [Fact]
        public void RequiredDomainContracts_ShouldCompileAndHoldShape()
        {
            var idem = new StorageIdempotencyModel
            {
                RequestId = "req-3",
                UsuarioId = 99,
                Estado = StorageDocumentState.Pending,
                FechaCreacion = DateTime.UtcNow
            };

            var physical = new StoragePhysicalStatusModel
            {
                RequestId = "req-3",
                IdAlmacen = 22,
                Estado = StorageDocumentState.PhysicalFailed,
                FechaActualizacion = DateTime.UtcNow
            };

            Assert.Equal(StorageDocumentState.Pending, idem.Estado);
            Assert.Equal(StorageDocumentState.PhysicalFailed, physical.Estado);
            Assert.Equal(StorageDocumentState.PhysicalFailed, StorageDocumentState.PhysicalFailed);
        }

        [Fact]
        public void BuilderContracts_ShouldCompile()
        {
            Assert.True(typeof(IStoragePlanBuilder).IsInterface);
            Assert.True(typeof(IStorageXmlBuilder).IsInterface);
        }
    }
}