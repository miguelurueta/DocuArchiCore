using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Expediente;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.Expediente;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Expediente;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public sealed class ExpedienteUnidadLegacyServiceTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldSkip_WhenPlanHasNoExecution()
        {
            var builder = new Mock<IExpedienteUnidadLegacyBuilder>();
            builder.Setup(x => x.Build(It.IsAny<StorageContext>())).Returns(new ExpedienteUnidadLegacyPlan { NumeroFolios = 0 });

            var service = BuildService(builder.Object);

            var result = await service.ExecuteAsync(BuildContext(), Mock.Of<IDbConnection>(), Mock.Of<IDbTransaction>());

            Assert.False(result.Ejecutado);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUpdateExpedienteElectronico_WhenPlanHasExpediente()
        {
            var builder = new Mock<IExpedienteUnidadLegacyBuilder>();
            builder.Setup(x => x.Build(It.IsAny<StorageContext>())).Returns(new ExpedienteUnidadLegacyPlan
            {
                TieneExpediente = true,
                IdExpediente = 10,
                IdClaseDocumento = 50,
                NumeroFolios = 3
            });

            var expedienteRepo = new Mock<IExpedienteLegacyRepository>();
            expedienteRepo.Setup(x => x.LockByIdAsync(10, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(new ExpedienteLegacyInfoModel
                {
                    IdExpediente = 10,
                    EstadoExpediente = 1,
                    EstadoExpedienteElectronico = 2,
                    NumeroElectronicoContenido = 7
                });
            expedienteRepo.Setup(x => x.UpdateFoliosElectronicosAsync(10, 7, 10, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(1);

            var claseRepo = new Mock<IClaseDocumentoLegacyRepository>();
            claseRepo.Setup(x => x.GetUnidadConservaTipoAsync(50, "db")).ReturnsAsync(UnidadConservaTipoEnum.Electronico);

            var service = BuildService(builder.Object, expedienteRepo: expedienteRepo.Object, claseRepo: claseRepo.Object);
            var context = BuildContext();

            var result = await service.ExecuteAsync(context, Mock.Of<IDbConnection>(), Mock.Of<IDbTransaction>());

            Assert.True(result.Ejecutado);
            Assert.True(result.TieneExpediente);
            Assert.Equal(2, result.IdTipoUnidadDocumental);
            expedienteRepo.VerifyAll();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUpdateUnidadDigitalizada_WhenPlanHasUnidad()
        {
            var builder = new Mock<IExpedienteUnidadLegacyBuilder>();
            builder.Setup(x => x.Build(It.IsAny<StorageContext>())).Returns(new ExpedienteUnidadLegacyPlan
            {
                TieneUnidadConservacion = true,
                IdUnidadConservacion = 20,
                IdClaseDocumento = 60,
                NumeroFolios = 4
            });

            var unidadRepo = new Mock<IUnidadConservacionLegacyRepository>();
            unidadRepo.Setup(x => x.LockByIdAsync(20, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(new UnidadConservacionLegacyInfoModel
                {
                    IdUnidadConservacion = 20,
                    NumeroDigitalizadoContenido = 6,
                    NumeroElectronicoContenido = 1
                });
            unidadRepo.Setup(x => x.UpdateFoliosDigitalizadosAsync(20, 6, 10, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .ReturnsAsync(1);

            var claseRepo = new Mock<IClaseDocumentoLegacyRepository>();
            claseRepo.Setup(x => x.GetUnidadConservaTipoAsync(60, "db")).ReturnsAsync(UnidadConservaTipoEnum.Digitalizado);

            var service = BuildService(builder.Object, unidadRepo: unidadRepo.Object, claseRepo: claseRepo.Object);
            var context = BuildContext();

            var result = await service.ExecuteAsync(context, Mock.Of<IDbConnection>(), Mock.Of<IDbTransaction>());

            Assert.True(result.Ejecutado);
            Assert.True(result.TieneUnidadConservacion);
            Assert.Equal(UnidadConservaTipoEnum.Digitalizado, result.UnidadConservaTipo);
            unidadRepo.VerifyAll();
        }

        private static ExpedienteUnidadLegacyService BuildService(
            IExpedienteUnidadLegacyBuilder builder,
            IExpedienteLegacyRepository? expedienteRepo = null,
            IUnidadConservacionLegacyRepository? unidadRepo = null,
            IClaseDocumentoLegacyRepository? claseRepo = null)
        {
            return new ExpedienteUnidadLegacyService(
                builder,
                expedienteRepo ?? Mock.Of<IExpedienteLegacyRepository>(),
                unidadRepo ?? Mock.Of<IUnidadConservacionLegacyRepository>(),
                claseRepo ?? Mock.Of<IClaseDocumentoLegacyRepository>(),
                Mock.Of<ILogger<ExpedienteUnidadLegacyService>>());
        }

        private static StorageContext BuildContext()
        {
            return new StorageContext
            {
                DefaultDbAlias = "db",
                Usuario = "u",
                UsuarioId = 1,
                RequestId = "req",
                NombreGabinete = "gab",
                RutaTemporalId = "tmp",
                NombreDocumento = "doc",
                ArchivosTemporales = new[] { "tmp-1" }
            };
        }
    }
}
