using System.Data;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using MySql.Data.MySqlClient;
using Testcontainers.MySql;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ValidaCamposDinamicosUnicosRadicacionServiceIntegrationTests : IAsyncLifetime
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
    public async Task ValidaCamposDinamicosUnicosRadicacionAsync_CuandoNoExisteDuplicado_RetornaSinResultados()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var service = BuildService();
        var result = await service.ValidaCamposDinamicosUnicosRadicacionAsync(
            BuildRequest("NIT-NUEVO"),
            "DA",
            BuildDetalleUnico());

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task ValidaCamposDinamicosUnicosRadicacionAsync_CuandoExisteDuplicado_RetornaValidationFail()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var service = BuildService();
        var result = await service.ValidaCamposDinamicosUnicosRadicacionAsync(
            BuildRequest("NIT-EXISTENTE"),
            "DA",
            BuildDetalleUnico());

        Assert.False(result.success);
        Assert.Equal("Validacion fallida", result.message);
        Assert.NotNull(result.data);
        Assert.NotEmpty(result.data!);
    }

    private ValidaCamposDinamicosUnicosRadicacionService BuildService()
    {
        var repo = new ValidaCamposDinamicosUnicosRadicacionRepository(
            new TestMySqlConnectionFactory(_container!.GetConnectionString()));
        return new ValidaCamposDinamicosUnicosRadicacionService(repo);
    }

    private async Task ExecuteScriptAsync(string scriptName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Database", "ValidaCamposDinamicosUnicos", scriptName);
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

    private static RegistrarRadicacionEntranteRequestDto BuildRequest(string valorUnico)
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = "Asunto",
            Remitente = new RemitenteRadicacionDto { Nombre = "Rem", id_Dest_Ext = 1 },
            Destinatario = new DestinatarioRadicacionDto { Destinatario = "Dest", id_Remit_Dest_Int = 2 },
            TipoRadicado = new TipoRadicadoEntradaDto { TipoRadicacion = "ENTRANTE", IdTipoRadicado = 1 },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1, TipoPlantillaRadicado = "RAD" },
            Campos =
            [
                new CampoRadicacionDto { NombreCampo = "CampoIdentificador", Valor = valorUnico }
            ]
        };
    }

    private static IReadOnlyCollection<DetallePlantillaRadicado> BuildDetalleUnico()
    {
        return
        [
            new DetallePlantillaRadicado
            {
                System_Plantilla_Radicado_id_Plantilla = 100,
                Campo_Plantilla = "CampoIdentificador",
                Tipo_Campo = "VARCHAR",
                Comportamiento_Campo = "UNICO",
                Alias_Campo = "CampoIdentificador",
                Orden_Campo = 1,
                Estado_Campo = 1,
                Descripcion_Campo = "Campo unico",
                Campo_Obligatorio = 1,
                Campo_rad_interno = 1,
                Campo_rad_externo = 1,
                Campo_rad_simple = 1,
                tam_campo = 50,
                id_detalle_plantilla_radicado = 1,
                TagSesion = "UNICO"
            }
        ];
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
