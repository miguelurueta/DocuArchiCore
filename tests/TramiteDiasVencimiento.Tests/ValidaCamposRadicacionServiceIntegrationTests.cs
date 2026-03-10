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

public sealed class ValidaCamposRadicacionServiceIntegrationTests : IAsyncLifetime
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
    public async Task ValidaCamposRadicacionAsync_CuandoDatosValidos_RetornaOk()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var service = new ValidaCamposRadicacionService(
            new ValidaCamposObligatoriosService(),
            new ValidaDimensionCamposService(new ValidaDimensionCamposRepository(factory)),
            new ValidaCamposDinamicosUnicosRadicacionService(new ValidaCamposDinamicosUnicosRadicacionRepository(factory)),
            new ValidaTipoCamposService(new ValidaTipoCamposRepository(factory)));

        var result = await service.ValidaCamposRadicacionAsync("DA", BuildRequest(), BuildDetallePlantilla());

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
    }

    private async Task ExecuteScriptAsync()
    {
        const string sql = """
            DROP TABLE IF EXISTS detalle_plantilla_radicado;
            DROP TABLE IF EXISTS ra_plantilla_100;
            DROP TABLE IF EXISTS system_plantilla_radicado;

            CREATE TABLE system_plantilla_radicado (
              id_Plantilla INT NOT NULL PRIMARY KEY,
              Nombre_Plantilla_Radicado VARCHAR(100) NOT NULL
            );

            CREATE TABLE detalle_plantilla_radicado (
              System_Plantilla_Radicado_id_Plantilla INT NOT NULL,
              Campo_Plantilla VARCHAR(45) NOT NULL,
              Tipo_Campo VARCHAR(100) NOT NULL,
              Comportamiento_Campo VARCHAR(45) NOT NULL,
              Alias_Campo VARCHAR(45) NOT NULL,
              Orden_Campo INT NOT NULL,
              Estado_Campo INT NULL,
              Descripcion_Campo VARCHAR(100) NOT NULL,
              Campo_Obligatorio INT NOT NULL,
              Campo_rad_interno INT NOT NULL,
              Campo_rad_externo INT NOT NULL,
              Campo_rad_simple INT NOT NULL,
              tam_campo INT NOT NULL,
              id_detalle_plantilla_radicado INT NOT NULL PRIMARY KEY,
              TagSesion VARCHAR(30) NOT NULL
            );

            CREATE TABLE ra_plantilla_100 (
              id_radicado INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
              Asunto VARCHAR(240) NOT NULL,
              Descripcion_Documento VARCHAR(80) NULL,
              Destinatario_Cor VARCHAR(100) NULL,
              Remitente_Cor VARCHAR(100) NULL,
              CampoIdentificador VARCHAR(50) NOT NULL,
              CampoDinamico VARCHAR(10) NULL,
              CampoDinamicoObligatorio VARCHAR(100) NULL
            );

            INSERT INTO system_plantilla_radicado (id_Plantilla, Nombre_Plantilla_Radicado)
            VALUES (100, 'ra_plantilla_100');

            INSERT INTO ra_plantilla_100 (Asunto, Descripcion_Documento, Destinatario_Cor, Remitente_Cor, CampoIdentificador, CampoDinamico, CampoDinamicoObligatorio)
            VALUES ('Registro previo', 'Desc', 'Dest', 'Remit', 'NIT-EXISTENTE', '123', 'SI');
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

    private static RegistrarRadicacionEntranteRequestDto BuildRequest()
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            ASUNTO = "Asunto valido",
            ANEXOS_COR = "Anexos",
            FECHALIMITERESPUESTA = "2026-12-31",
            numeroFolios = 2,
            Remitente = new RemitenteRadicacionDto { Nombre = "Remitente", id_Dest_Ext = 123 },
            Destinatario = new DestinatarioRadicacionDto { Destinatario = "Destinatario", id_Remit_Dest_Int = 456 },
            TipoRadicado = new TipoRadicadoEntradaDto { TipoRadicacion = "ENTRANTE", IdTipoRadicado = 2 },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1, TipoPlantillaRadicado = "RAD" },
            Campos =
            [
                new CampoRadicacionDto { NombreCampo = "Usuario_Radicador_id_usuario", Valor = "10" },
                new CampoRadicacionDto { NombreCampo = "Consecutivo_Rad", Valor = "RAD-1" },
                new CampoRadicacionDto { NombreCampo = "Consecutivo_CodBarra", Valor = "CB-1" },
                new CampoRadicacionDto { NombreCampo = "Fecha_Radicado", Valor = "2026-03-10" },
                new CampoRadicacionDto { NombreCampo = "Descripcion_Documento", Valor = "Descripcion" },
                new CampoRadicacionDto { NombreCampo = "Codigo_Sede", Valor = "1" },
                new CampoRadicacionDto { NombreCampo = "Id_area_remit_dest_interno", Valor = "20" },
                new CampoRadicacionDto { NombreCampo = "Area_remit_dest_interno", Valor = "AREA" },
                new CampoRadicacionDto { NombreCampo = "CARGO_DESTINATARIO", Valor = "ANALISTA" },
                new CampoRadicacionDto { NombreCampo = "CampoDinamicoObligatorio", Valor = "SI" },
                new CampoRadicacionDto { NombreCampo = "CampoDinamico", Valor = "1234567890" },
                new CampoRadicacionDto { NombreCampo = "CampoIdentificador", Valor = "NIT-NUEVO" }
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
                Campo_Plantilla = "CampoDinamicoObligatorio",
                Tipo_Campo = "VARCHAR",
                Comportamiento_Campo = "DIGITACION",
                Alias_Campo = "CampoDinamicoObligatorio",
                Orden_Campo = 1,
                Estado_Campo = 1,
                Descripcion_Campo = "Campo obligatorio",
                Campo_Obligatorio = 1,
                Campo_rad_interno = 1,
                Campo_rad_externo = 1,
                Campo_rad_simple = 1,
                tam_campo = 100,
                id_detalle_plantilla_radicado = 1,
                TagSesion = "TEST"
            },
            new DetallePlantillaRadicado
            {
                System_Plantilla_Radicado_id_Plantilla = 100,
                Campo_Plantilla = "CampoDinamico",
                Tipo_Campo = "VARCHAR",
                Comportamiento_Campo = "DIGITACION",
                Alias_Campo = "CampoDinamico",
                Orden_Campo = 2,
                Estado_Campo = 1,
                Descripcion_Campo = "Campo dimension",
                Campo_Obligatorio = 0,
                Campo_rad_interno = 1,
                Campo_rad_externo = 1,
                Campo_rad_simple = 1,
                tam_campo = 10,
                id_detalle_plantilla_radicado = 2,
                TagSesion = "TEST"
            },
            new DetallePlantillaRadicado
            {
                System_Plantilla_Radicado_id_Plantilla = 100,
                Campo_Plantilla = "CampoIdentificador",
                Tipo_Campo = "VARCHAR",
                Comportamiento_Campo = "UNICO",
                Alias_Campo = "CampoIdentificador",
                Orden_Campo = 3,
                Estado_Campo = 1,
                Descripcion_Campo = "Campo unico",
                Campo_Obligatorio = 1,
                Campo_rad_interno = 1,
                Campo_rad_externo = 1,
                Campo_rad_simple = 1,
                tam_campo = 50,
                id_detalle_plantilla_radicado = 3,
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
