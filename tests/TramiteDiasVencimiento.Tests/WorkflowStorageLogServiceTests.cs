using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Workflow;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Workflow;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public sealed class WorkflowStorageLogServiceTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldSkipInsert_WhenBuilderReturnsNoWorkflow()
        {
            var builder = new Mock<IWorkflowStorageLogBuilder>();
            var repository = new Mock<IWorkflowStorageLogRepository>();
            var service = new WorkflowStorageLogService(
                builder.Object,
                repository.Object,
                Mock.Of<ILogger<WorkflowStorageLogService>>());

            builder.Setup(x => x.Build(
                    It.IsAny<StorageContext>(),
                    It.IsAny<StorageIdentityModel>(),
                    It.IsAny<StorageNamingResult>(),
                    It.IsAny<StoragePhysicalPathModel>()))
                .Returns(new WorkflowStorageLogBuildResult
                {
                    ShouldInsert = false,
                    Estado = "NO_WORKFLOW"
                });

            await service.ExecuteAsync(
                new StorageContext
                {
                    DefaultDbAlias = "db",
                    Usuario = "user",
                    UsuarioId = 1,
                    RequestId = "req",
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp",
                    NombreDocumento = "doc.pdf",
                    ArchivosTemporales = new[] { "doc.pdf" },
                    Command = new AlmacenarDocumentoCommand
                    {
                        NombreGabinete = "gab",
                        RutaTemporalId = "tmp",
                        NombreDocumento = "doc.pdf",
                        RequestId = "req",
                        Documentos = new[] { new MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental.DocumentoEntradaDto { IdDocumento = "1", ArchivoTemporalId = "doc.pdf" } }
                    }
                },
                new StorageIdentityModel { IdAlmacen = 1, Disco = 1, Carpeta = 1, NumeroPaginasCarpeta = 1 },
                new StorageNamingResult { NombreArchivoPrincipal = "DIG00000001.pdf", NombreXml = "FXL00000001.xml", SegundoNombre = "DIG00000001.pdf" },
                new StoragePhysicalPathModel { RutaFinal = @"C:\tmp" },
                Mock.Of<IDbConnection>(),
                Mock.Of<IDbTransaction>());

            repository.Verify(x => x.InsertAsync(It.IsAny<WorkflowStorageLogModel>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldInsert_WhenBuilderReturnsReady()
        {
            var builder = new Mock<IWorkflowStorageLogBuilder>();
            var repository = new Mock<IWorkflowStorageLogRepository>();
            var service = new WorkflowStorageLogService(
                builder.Object,
                repository.Object,
                Mock.Of<ILogger<WorkflowStorageLogService>>());

            var model = new WorkflowStorageLogModel
            {
                IdTran = 10,
                DescOp = "Registra",
                UserOper = "user",
                DateTrans = DateTime.UtcNow.Date,
                RutDocu = @"C:\tmp\DIG00000010.pdf",
                ModuloRegistro = "WORKFLOW",
                Gabinete = "gab",
                Campos = "|a|b",
                IpTrans = "10.0.0.1",
                HoraRegistro = "10:00:00",
                IdTareaWorkflow = 7,
                IdRutaWorkflow = 2,
                UserPropietario = "user"
            };

            builder.Setup(x => x.Build(
                    It.IsAny<StorageContext>(),
                    It.IsAny<StorageIdentityModel>(),
                    It.IsAny<StorageNamingResult>(),
                    It.IsAny<StoragePhysicalPathModel>()))
                .Returns(new WorkflowStorageLogBuildResult
                {
                    ShouldInsert = true,
                    Estado = "READY",
                    Model = model
                });

            repository.Setup(x => x.InsertAsync(model, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(1);

            await service.ExecuteAsync(
                new StorageContext
                {
                    DefaultDbAlias = "db",
                    Usuario = "user",
                    UsuarioId = 1,
                    RequestId = "req",
                    NombreGabinete = "gab",
                    RutaTemporalId = "tmp",
                    NombreDocumento = "doc.pdf",
                    ArchivosTemporales = new[] { "doc.pdf" },
                    Command = new AlmacenarDocumentoCommand
                    {
                        NombreGabinete = "gab",
                        RutaTemporalId = "tmp",
                        NombreDocumento = "doc.pdf",
                        RequestId = "req",
                        Documentos = new[] { new MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental.DocumentoEntradaDto { IdDocumento = "1", ArchivoTemporalId = "doc.pdf" } }
                    }
                },
                new StorageIdentityModel { IdAlmacen = 10, Disco = 1, Carpeta = 1, NumeroPaginasCarpeta = 1 },
                new StorageNamingResult { NombreArchivoPrincipal = "DIG00000010.pdf", NombreXml = "FXL00000010.xml", SegundoNombre = "DIG00000010.pdf" },
                new StoragePhysicalPathModel { RutaFinal = @"C:\tmp" },
                Mock.Of<IDbConnection>(),
                Mock.Of<IDbTransaction>());

            repository.Verify(x => x.InsertAsync(model, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()), Times.Once);
        }
    }
}
