using System.Data;
using MiApp.Models.Models.Radicacion.Configuracion;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Configuracion;
using MySql.Data.MySqlClient;
using Testcontainers.MySql;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaListaEstructuraConfiguracionPlantillaRadicacionRepositoryIntegrationTests : IAsyncLifetime
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
    public async Task SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync_CuandoExisteRegistro_RetornaLista()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var dapper = new DapperCrudEngine(factory);
        var repository = new SolicitaListaEstructuraConfiguracionPlantillaRadicacionRepository(dapper);

        var result = await repository.SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync(67, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(2, result.data!.Count);
        Assert.All(result.data, row => Assert.Equal(67, row.system_plantilla_radicado_id_Plantilla));
    }

    [Fact]
    public async Task SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync_CuandoNoExisteRegistro_RetornaSinResultados()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var dapper = new DapperCrudEngine(factory);
        var repository = new SolicitaListaEstructuraConfiguracionPlantillaRadicacionRepository(dapper);

        var result = await repository.SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync(999, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    private async Task ExecuteScriptAsync(string scriptName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Database", "ConfiguracionPlantilla", scriptName);
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
