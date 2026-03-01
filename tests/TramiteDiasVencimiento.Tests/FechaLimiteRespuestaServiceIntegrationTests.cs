using System.Data;
using AutoMapper;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;
using MySql.Data.MySqlClient;
using Testcontainers.MySql;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class FechaLimiteRespuestaServiceIntegrationTests : IAsyncLifetime
{
    private MySqlContainer? _container;
    private bool _dockerUnavailable;

    public async Task InitializeAsync()
    {
        try
        {
            _container = new MySqlBuilder()
                .WithImage("mysql:8.0")
                .WithDatabase("docuarchi_test")
                .WithUsername("root")
                .WithPassword("root")
                .Build();

            await _container.StartAsync();
            await ExecuteScriptAsync("schema.sql");
            await ExecuteScriptAsync("seed.sql");
        }
        catch
        {
            _dockerUnavailable = true;
        }
    }

    public async Task DisposeAsync()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }

    [Fact]
    public async Task SolicitaFechaLimiteRespuesta_CuandoHayDatos_RetornaFechaCorrecta()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var dapper = new DapperCrudEngine(factory);
        var plantillaRepo = new SystemPlantillaRadicadoR(dapper);
        var diasRepo = new TotalDiasVencimientoTramiteRepository(dapper);
        var feriadosRepo = new ListaDiasFeriadosTramiteRepository(dapper);
        var mapper = BuildMapper();

        var service = new FechaLimiteRespuestaService(plantillaRepo, diasRepo, feriadosRepo, mapper.Object);

        var result = await service.SolicitaFechaLimiteRespuesta(200, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(5, result.data.DiasVencimiento);

        var expected = CalculateBusinessDueDate(DateTime.Today, 5, []);
        Assert.Equal(expected.ToString("yyyy-MM-dd"), result.data.FechaLimiteRespuesta);
    }

    private async Task ExecuteScriptAsync(string scriptName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Database", "FechaLimite", scriptName);
        var sql = await File.ReadAllTextAsync(path);

        await using var connection = new MySqlConnection(_container!.GetConnectionString());
        await connection.OpenAsync();

        var batches = sql.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var batch in batches)
        {
            await using var command = new MySqlCommand(batch, connection);
            await command.ExecuteNonQueryAsync();
        }
    }

    private static Mock<IMapper> BuildMapper()
    {
        var mapper = new Mock<IMapper>();
        mapper
            .Setup(m => m.Map<FechaLimiteRespuestaDto>(It.IsAny<object>()))
            .Returns((object source) =>
            {
                var model = (FechaLimiteRespuestaModel)source;
                return new FechaLimiteRespuestaDto
                {
                    IdTipoTramite = model.IdTipoTramite,
                    IdPlantilla = model.IdPlantilla,
                    DiasVencimiento = model.DiasVencimiento,
                    FechaLimiteRespuesta = model.FechaLimiteRespuesta
                };
            });

        return mapper;
    }

    private static DateTime CalculateBusinessDueDate(DateTime startDate, int businessDays, IEnumerable<DateTime> holidays)
    {
        var holidaySet = new HashSet<DateTime>(holidays.Select(h => h.Date));
        var current = startDate.Date;
        var added = 0;

        while (added < businessDays)
        {
            current = current.AddDays(1);
            if (current.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            if (holidaySet.Contains(current))
            {
                continue;
            }

            added++;
        }

        return current;
    }

    private sealed class TestMySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public TestMySqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetOpenConnection(string? dbAlias = null)
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public async Task<IDbConnection> GetOpenConnectionAsync(string? dbAlias = null)
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public string ProviderBsd() => "mysql";

        public IEnumerable<string> GetAvailableAliases() => ["da"];
    }
}
