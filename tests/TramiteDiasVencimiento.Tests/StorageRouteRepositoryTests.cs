using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Physical;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.AlmacenamientoDocumental.StorageRoute;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class StorageRouteRepositoryTests
    {
        [Fact]
        public async Task GetRouteAsync_ShouldQuerySystem1Rut_WithLegacyFilters()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            QueryOptions? captured = null;

            dapper.Setup(x => x.GetAllAsync<StorageRouteModel>(It.IsAny<QueryOptions>()))
                .Callback<QueryOptions>(o => captured = o)
                .ReturnsAsync(new QueryResult<StorageRouteModel>
                {
                    Success = true,
                    Data = new[]
                    {
                        new StorageRouteModel
                        {
                            NombreGabinete = "gab",
                            RutaAlmacenamiento = @"D:\root"
                        }
                    },
                    ErrorMessage = "YES"
                });

            var repo = new StorageRouteRepository(dapper.Object);
            var route = await repo.GetRouteAsync("gab", "da");

            Assert.NotNull(route);
            Assert.NotNull(captured);
            Assert.Equal("system1rut", captured!.TableName);
            Assert.Equal("da", captured.DefaultAlias);
            Assert.Equal("gab", captured.Filters["gabinete"]);
            Assert.Equal(1, captured.Filters["tipo_rut"]);
            Assert.Equal(1, captured.Filters["est_rut"]);
            Assert.Equal(1, captured.Limit);
        }

        [Fact]
        public async Task GetRouteAsync_ShouldReturnNull_WhenNoRows()
        {
            var dapper = new Mock<IDapperCrudEngine>();
            dapper.Setup(x => x.GetAllAsync<StorageRouteModel>(It.IsAny<QueryOptions>()))
                .ReturnsAsync(new QueryResult<StorageRouteModel>
                {
                    Success = true,
                    Data = Array.Empty<StorageRouteModel>(),
                    ErrorMessage = "YES"
                });

            var repo = new StorageRouteRepository(dapper.Object);
            var route = await repo.GetRouteAsync("gab", "da");

            Assert.Null(route);
        }
    }
}
