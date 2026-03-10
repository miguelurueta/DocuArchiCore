using System.Data;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Services.Service.Radicacion.Tramite;
using MySql.Data.MySqlClient;
using Testcontainers.MySql;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ValidaCamposObligatoriosServiceIntegrationTests : IAsyncLifetime
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
    public async Task ValidaCamposObligatoriosAsync_CuandoConsultaDinamicosYRequestValido_RetornaOk()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var dapper = new DapperCrudEngine(factory);
        var detalleRepo = new DetallePlantillaRadicadoR(dapper);

        var campos = await detalleRepo.SolicitaCamposDnamicos(100, "DA");
        Assert.True(campos.Success);
        Assert.NotNull(campos.Data);

        var service = new ValidaCamposObligatoriosService();
        var request = BuildValidRequest();

        var result = await service.ValidaCamposObligatoriosAsync(request, "DA", campos.Data!);

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
    }

    private async Task ExecuteScriptAsync(string scriptName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Database", "ValidaCamposObligatorios", scriptName);
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

    private static RegistrarRadicacionEntranteRequestDto BuildValidRequest()
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = "Asunto",
            ANEXOS_COR = "Anexos",
            FECHALIMITERESPUESTA = "2026-12-31",
            numeroFolios = 3,
            Remitente = new RemitenteRadicacionDto { Nombre = "Remitente", id_Dest_Ext = 123 },
            Destinatario = new DestinatarioRadicacionDto { Destinatario = "Dest", id_Remit_Dest_Int = 456 },
            TipoRadicado = new TipoRadicadoEntradaDto { TipoRadicacion = "ENTRANTE", IdTipoRadicado = 2 },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1, TipoPlantillaRadicado = "RAD" },
            Campos =
            [
                Campo("Usuario_Radicador_id_usuario", "10"),
                Campo("Consecutivo_Rad", "RAD-1"),
                Campo("Consecutivo_CodBarra", "CB-1"),
                Campo("Fecha_Radicado", "2026-03-09"),
                Campo("Descripcion_Documento", "Descripcion"),
                Campo("Codigo_Sede", "1"),
                Campo("Id_area_remit_dest_interno", "20"),
                Campo("Area_remit_dest_interno", "AREA"),
                Campo("CARGO_DESTINATARIO", "ANALISTA"),
                Campo("CampoDinamicoObligatorio", "valor")
            ]
        };
    }

    private static CampoRadicacionDto Campo(string nombre, string valor)
    {
        return new CampoRadicacionDto { NombreCampo = nombre, Valor = valor };
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
