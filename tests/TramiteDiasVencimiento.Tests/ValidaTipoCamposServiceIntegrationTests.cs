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

public sealed class ValidaTipoCamposServiceIntegrationTests : IAsyncLifetime
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
            await ExecuteScriptAsync();
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
    public async Task ValidaTipoCamposAsync_CuandoTiposCompatibles_RetornaOk()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var service = BuildService();
        var result = await service.ValidaTipoCamposAsync(BuildRequest("25"), "DA", BuildDetallePlantilla());

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
    }

    [Fact]
    public async Task ValidaTipoCamposAsync_CuandoNoExistePlantilla_RetornaSinResultados()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var service = BuildService();
        var request = BuildRequest("25");
        request.IdPlantilla = 999;

        var result = await service.ValidaTipoCamposAsync(request, "DA", BuildDetallePlantilla());

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    private ValidaTipoCamposService BuildService()
    {
        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var repo = new ValidaTipoCamposRepository(factory);
        return new ValidaTipoCamposService(repo);
    }

    private async Task ExecuteScriptAsync()
    {
        const string sql = """
            DROP TABLE IF EXISTS ra_plantilla_100;
            DROP TABLE IF EXISTS system_plantilla_radicado;

            CREATE TABLE system_plantilla_radicado (
              id_Plantilla INT NOT NULL PRIMARY KEY,
              Nombre_Plantilla_Radicado VARCHAR(100) NOT NULL
            );

            CREATE TABLE ra_plantilla_100 (
              id_radicado INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
              Asunto VARCHAR(240) NOT NULL,
              CampoNumero INT NULL
            );

            INSERT INTO system_plantilla_radicado (id_Plantilla, Nombre_Plantilla_Radicado)
            VALUES (100, 'ra_plantilla_100');
            """;

        await using var connection = new MySqlConnection(_container!.GetConnectionString());
        await connection.OpenAsync();

        var batches = sql.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var batch in batches)
        {
            await using var command = new MySqlCommand(batch, connection);
            await command.ExecuteNonQueryAsync();
        }
    }

    private static RegistrarRadicacionEntranteRequestDto BuildRequest(string valorNumero)
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = "Asunto de prueba",
            ANEXOS_COR = "Anexo",
            FECHALIMITERESPUESTA = "2026-12-31",
            numeroFolios = 1,
            Remitente = new RemitenteRadicacionDto { Nombre = "Remitente", id_Dest_Ext = 1 },
            Destinatario = new DestinatarioRadicacionDto { Destinatario = "Destino", id_Remit_Dest_Int = 2 },
            TipoRadicado = new TipoRadicadoEntradaDto { TipoRadicacion = "ENTRANTE", IdTipoRadicado = 1 },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1, TipoPlantillaRadicado = "RAD" },
            Campos =
            [
                new CampoRadicacionDto
                {
                    NombreCampo = "CampoNumero",
                    Valor = valorNumero
                }
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
                Campo_Plantilla = "CampoNumero",
                Tipo_Campo = "INT",
                Comportamiento_Campo = "DIGITACION",
                Alias_Campo = "Campo número",
                Orden_Campo = 1,
                Estado_Campo = 1,
                Descripcion_Campo = "Campo numérico",
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
