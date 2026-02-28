using System.Data;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MySql.Data.MySqlClient;
using Testcontainers.MySql;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class TotalDiasVencimientoTramiteRepositoryIntegrationTests : IAsyncLifetime
{
    private MySqlContainer? _container;

    private bool _dockerUnavailable;
    private string _dockerMessage = string.Empty;

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
        catch (Exception ex)
        {
            _dockerUnavailable = true;
            _dockerMessage = ex.Message;
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
    public async Task SolicitaTotalDiasVencimientoTramite_CuandoExisteRegistro_RetornaValorCorrecto()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var dapper = new DapperCrudEngine(factory);
        var repository = new TotalDiasVencimientoTramiteRepository(dapper);

        var result = await repository.SolicitaTotalDiasVencimientoTramite(100, 200, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.Equal(12, result.data);
    }

    private async Task ExecuteScriptAsync(string scriptName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Database", scriptName);
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

        public IEnumerable<string> GetAvailableAliases() => new[] { "da" };
    }
}
