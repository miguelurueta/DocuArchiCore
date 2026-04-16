using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.ConfiguracionUpload;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.ConfiguracionUpload;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class SolicitaEstructuraConfiguracionUploadNameProcesoRepositoryTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task defaultDbAlias_invalido_retorna_error_y_no_invoca_engine(string? defaultDbAlias)
        {
            var engine = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
            var repo = new SolicitaEstructuraConfiguracionUploadNameProcesoRepository(engine.Object);

            var res = await repo.SolicitaEstructuraConfiguracionUploadNameProcesoAsync("PROC", defaultDbAlias!);

            Assert.False(res.success);
            Assert.NotNull(res.data);
            Assert.Empty(res.data);
            engine.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task nameProceso_invalido_retorna_error_y_no_invoca_engine(string? nameProceso)
        {
            var engine = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
            var repo = new SolicitaEstructuraConfiguracionUploadNameProcesoRepository(engine.Object);

            var res = await repo.SolicitaEstructuraConfiguracionUploadNameProcesoAsync(nameProceso!, "db1");

            Assert.False(res.success);
            Assert.NotNull(res.data);
            Assert.Empty(res.data);
            engine.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task sin_resultados_retorna_success_true_data_vacio()
        {
            var engine = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
            engine
                .Setup(e => e.GetAllAsync<RaConfiguracionUploadModel>(It.IsAny<QueryOptions>()))
                .ReturnsAsync(new QueryResult<RaConfiguracionUploadModel>
                {
                    Success = true,
                    Message = "YES",
                    Data = Array.Empty<RaConfiguracionUploadModel>()
                });

            var repo = new SolicitaEstructuraConfiguracionUploadNameProcesoRepository(engine.Object);

            var res = await repo.SolicitaEstructuraConfiguracionUploadNameProcesoAsync("PROC", "db1");

            Assert.True(res.success);
            Assert.NotNull(res.data);
            Assert.Empty(res.data);
            Assert.Equal("Sin resultados", res.message);
        }

        [Fact]
        public async Task con_resultados_retorna_success_true_y_lista()
        {
            var engine = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
            engine
                .Setup(e => e.GetAllAsync<RaConfiguracionUploadModel>(It.IsAny<QueryOptions>()))
                .ReturnsAsync(new QueryResult<RaConfiguracionUploadModel>
                {
                    Success = true,
                    Message = "YES",
                    Data =
                    [
                        new RaConfiguracionUploadModel
                        {
                            NameProceso = "PROC",
                            ExtensionUpload = "pdf"
                        }
                    ]
                });

            var repo = new SolicitaEstructuraConfiguracionUploadNameProcesoRepository(engine.Object);

            var res = await repo.SolicitaEstructuraConfiguracionUploadNameProcesoAsync("PROC", "db1");

            Assert.True(res.success);
            Assert.Single(res.data);
            Assert.Equal("YES", res.message);
        }

        [Fact]
        public async Task engine_exception_retorna_success_false()
        {
            var engine = new Mock<IDapperCrudEngine>(MockBehavior.Strict);
            engine
                .Setup(e => e.GetAllAsync<RaConfiguracionUploadModel>(It.IsAny<QueryOptions>()))
                .ThrowsAsync(new InvalidOperationException("boom"));

            var repo = new SolicitaEstructuraConfiguracionUploadNameProcesoRepository(engine.Object);

            var res = await repo.SolicitaEstructuraConfiguracionUploadNameProcesoAsync("PROC", "db1");

            Assert.False(res.success);
            Assert.NotNull(res.data);
            Assert.Empty(res.data);
        }
    }
}

