using System.Data;
using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.IndiceElectronico;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class IndiceElectronicoRepositoryTests
    {
        [Fact]
        public async Task InsertAsync_ShouldUseExpectedTableAndReturnId()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;

            dapper.Setup(x => x.InsertBeginTrandAsync(
                    It.IsAny<QueryOptions>(),
                    It.IsAny<object>(),
                    It.IsAny<string>(),
                    It.IsAny<IDbConnection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>()))
                .Callback<QueryOptions, object, string, IDbConnection, IDbTransaction, bool, string>((o, _, _, _, _, _, _) => captured = o)
                .ReturnsAsync(new QueryResult<int> { Success = true, Data = new[] { 77 } });

            var repo = new IndiceElectronicoRepository(dapper.Object, Mock.Of<ILogger<IndiceElectronicoRepository>>());
            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var id = await repo.InsertAsync(BuildModel(), connection.Object, Mock.Of<IDbTransaction>());

            Assert.Equal(77L, id);
            Assert.NotNull(captured);
            Assert.Equal("ra_cert_indice_expediente", captured!.TableName);
            Assert.Equal(17, captured.CampoParameterRegla.Count);
            Assert.Equal("SHA256", captured.ReglasValidacionCampo["Funcion_resumen"]);
        }

        [Fact]
        public async Task InsertAsync_ShouldThrow_WhenModelIsInvalid()
        {
            var repo = new IndiceElectronicoRepository(
                new Mock<IDapperCrudEngine>(MockBehavior.Strict).Object,
                Mock.Of<ILogger<IndiceElectronicoRepository>>());

            var connection = new Mock<IDbConnection>();
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);

            var model = BuildModel();
            model = new IndiceElectronicoInsertModel
            {
                IdRegistroProduccionDocumental = 0,
                IdExpediente = model.IdExpediente,
                NombreDocumento = model.NombreDocumento,
                TipologiaDocumental = model.TipologiaDocumental,
                FechaDeclaracionDocumento = model.FechaDeclaracionDocumento,
                FechaIncorporacionDocumento = model.FechaIncorporacionDocumento,
                ValorHuella = model.ValorHuella,
                FuncionResumen = model.FuncionResumen,
                OrdenDocumentoExpedicion = model.OrdenDocumentoExpedicion,
                PaginaInicial = model.PaginaInicial,
                PaginaFinal = model.PaginaFinal,
                Formato = model.Formato,
                DimensionKb = model.DimensionKb,
                Origen = model.Origen,
                RutaDocumento = model.RutaDocumento,
                NumeroFolios = model.NumeroFolios,
                SegundoNombre = model.SegundoNombre
            };

            await Assert.ThrowsAsync<StorageTransactionException>(() =>
                repo.InsertAsync(model, connection.Object, Mock.Of<IDbTransaction>()));
        }

        private static IndiceElectronicoInsertModel BuildModel()
        {
            return new IndiceElectronicoInsertModel
            {
                IdRegistroProduccionDocumental = 100,
                IdExpediente = 10,
                NombreDocumento = "doc.pdf",
                TipologiaDocumental = "77",
                FechaDeclaracionDocumento = DateTime.UtcNow.Date,
                FechaIncorporacionDocumento = DateTime.UtcNow.Date,
                ValorHuella = new string('A', 64),
                FuncionResumen = "SHA256",
                OrdenDocumentoExpedicion = 4,
                PaginaInicial = 11,
                PaginaFinal = 14,
                Formato = "pdf",
                DimensionKb = "120",
                Origen = "STORAGE_ENGINE",
                RutaDocumento = "tmp/tmp-1",
                NumeroFolios = 4,
                SegundoNombre = string.Empty
            };
        }
    }
}
