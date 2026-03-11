using System.Data;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using MySql.Data.MySqlClient;
using Testcontainers.MySql;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ValidaDimensionCamposServiceIntegrationTests : IAsyncLifetime
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
    public async Task ValidaDimensionCamposAsync_CuandoDatosValidos_RetornaOk()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var service = BuildService();
        var request = BuildRequest("Asunto corto", "1234567890");

        var result = await service.ValidaDimensionCamposAsync(request, "DA", BuildDetallePlantilla());

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
    }

    [Fact]
    public async Task ValidaDimensionCamposAsync_CuandoNoExistePlantilla_RetornaSinResultados()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var service = BuildService();
        var request = BuildRequest("Asunto", "123");
        request.IdPlantilla = 999;

        var result = await service.ValidaDimensionCamposAsync(request, "DA", BuildDetallePlantilla());

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    private ValidaDimensionCamposService BuildService()
    {
        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var repo = new ValidaDimensionCamposRepository(factory);
        return new ValidaDimensionCamposService(repo);
    }

    private async Task ExecuteScriptAsync(string scriptName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Database", "ValidaDimensionCampos", scriptName);
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

    private static RegistrarRadicacionEntranteRequestDto BuildRequest(string asunto, string campoDinamico)
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = asunto,
            ANEXOS_COR = "Anexo",
            FECHALIMITERESPUESTA = "2026-12-31",
            numeroFolios = 1,
            Remitente = new RemitenteRadicacionDto { Nombre = "Remitente", id_Dest_Ext = 1 },
            Destinatario = new DestinatarioRadicacionDto { Destinatario = "Destino", id_Remit_Dest_Int = 2 },
            TipoRadicado = new TipoRadicadoEntradaDto { TipoRadicacion = "ENTRANTE", IdTipoRadicado = 1 },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1, TipoPlantillaRadicado = "RAD" },
            Campos =
            [
                new CampoRadicacionDto { NombreCampo = "CampoDinamico", Valor = campoDinamico },
                new CampoRadicacionDto { NombreCampo = "Descripcion_Documento", Valor = "Descripcion valida" }
            ]
        };
    }

    private static IReadOnlyCollection<DetallePlantillaRadicado> BuildDetallePlantilla()
    {
        return
        [
            new DetallePlantillaRadicado
            {
                System_Plantilla_Radicado_id_Plantilla = 100,
                Campo_Plantilla = "CampoDinamico",
                Tipo_Campo = "VARCHAR",
                Comportamiento_Campo = "DIGITACION",
                Alias_Campo = "CampoDinamico",
                Orden_Campo = 1,
                Estado_Campo = 1,
                Descripcion_Campo = "Campo dinamico",
                Campo_Obligatorio = 0,
                Campo_rad_interno = 1,
                Campo_rad_externo = 1,
                Campo_rad_simple = 1,
                tam_campo = 10,
                id_detalle_plantilla_radicado = 1,
                TagSesion = "TEST"
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
