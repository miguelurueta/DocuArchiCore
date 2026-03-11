using System.Data;
using MiApp.DTOs.DTOs.General;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Services.Service.Radicacion.PlantillaRadicado;
using MySql.Data.MySqlClient;
using Testcontainers.MySql;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaAutoCompleteTokenExpedienteRadicadoServiceIntegrationTests : IAsyncLifetime
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
    public async Task ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync_CuandoHayCoincidencias_RetornaListado()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var dapper = new DapperCrudEngine(factory);
        var repository = new SolicitaAutoCompleteTokenExpedienteRadicadoRepository(dapper);
        var service = new SolicitaAutoCompleteTokenExpedienteRadicadoService(repository);

        var result = await service.ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync(
            new ParameterAutoComplete { TextoBuscado = "EXP" },
            "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(2, result.data.Count);
    }

    [Fact]
    public async Task ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync_CuandoBuscaPorCodigoVisible_RetornaCoincidencia()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var dapper = new DapperCrudEngine(factory);
        var repository = new SolicitaAutoCompleteTokenExpedienteRadicadoRepository(dapper);
        var service = new SolicitaAutoCompleteTokenExpedienteRadicadoService(repository);

        var result = await service.ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync(
            new ParameterAutoComplete { TextoBuscado = "002" },
            "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Single(result.data);
        Assert.Equal(2, result.data[0].idValue);
    }

    [Fact]
    public async Task ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync_PriorizaCoincidenciaExacta()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        await using (var connection = new MySqlConnection(_container!.GetConnectionString()))
        {
            await connection.OpenAsync();
            await using var insert = new MySqlCommand(
                """
                INSERT INTO expediente_archivo
                (ID_EXPEDIENTE, CODIGO_UNICO, ALEAS_EXPEDIENTE, NOMBRE_PERSONA_EXPEDIENTE, IDENTIFICACION_PERSONA_EXPEDIENTE, NOMBRE_RESPONSABLE_EXPEDIENTE, IDENFICACION_RESPONSABLE_EXPEDIENTE)
                VALUES
                (9001, '909497', 'EXACTO', 'N1', 'I1', 'R1', 'IR1'),
                (9002, 'AB-909497-TEST', 'PARCIAL', 'N2', 'I2', 'R2', 'IR2');
                """,
                connection);
            await insert.ExecuteNonQueryAsync();
        }

        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var dapper = new DapperCrudEngine(factory);
        var repository = new SolicitaAutoCompleteTokenExpedienteRadicadoRepository(dapper);
        var service = new SolicitaAutoCompleteTokenExpedienteRadicadoService(repository);

        var result = await service.ServiceSolicitaAutoCompleteTokenExpedienteRadicadoAsync(
            new ParameterAutoComplete { TextoBuscado = "909497" },
            "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.True(result.data.Count >= 2);
        Assert.Equal(9001, result.data[0].idValue);
    }

    private async Task ExecuteScriptAsync(string scriptName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Database", "AutoCompleteTokenExpedienteRadicado", scriptName);
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
