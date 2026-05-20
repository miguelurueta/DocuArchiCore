using System.Data;
using MiApp.DTOs.DTOs.GestorDocumental.PermisosVisorPdf;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.PermisosVisorPdf;
using MySql.Data.MySqlClient;
using Testcontainers.MySql;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class PermisosVisorPdfRepositoryIntegrationTests : IAsyncLifetime
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
            await ExecuteScriptAsync("schema.mysql");
            await ExecuteScriptAsync("seed.mysql");
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
    public async Task GetEffectivePermissionsAsync_SinPerfilNiOverride_UsaDefaultImplementacion()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var repository = CreateRepository();
        var result = await repository.GetEffectivePermissionsAsync("workflow", 142, "DA");

        Assert.True(result.success);
        Assert.True(result.data.Permissions["pdf.view"]);
        Assert.False(result.data.Permissions["pdf.zoom"]);
        Assert.Equal("default_implementacion", result.data.Sources["pdf.view"]);
        Assert.Equal("default_implementacion", result.data.Sources["pdf.zoom"]);
    }

    [Fact]
    public async Task GetEffectivePermissionsAsync_ConPerfilActivo_UsaPerfil()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        await ExecuteNonQueryAsync(
            """
            INSERT INTO ra_vis_per_usuario_perfil(id_usuario, id_impl, id_perfil, estado, fecha_inicio, fecha_fin)
            SELECT 141, i.id_impl, pf.id_perfil, 1, CURDATE(), NULL
            FROM ra_vis_per_implementacion i
            JOIN ra_vis_per_perfil pf ON pf.id_impl = i.id_impl
            WHERE i.codigo_impl = 'workflow' AND pf.codigo_perfil = 'LECTOR'
            ON DUPLICATE KEY UPDATE
              id_perfil = VALUES(id_perfil),
              estado = VALUES(estado),
              fecha_inicio = VALUES(fecha_inicio),
              fecha_fin = VALUES(fecha_fin)
            """);

        var repository = CreateRepository();
        var result = await repository.GetEffectivePermissionsAsync("workflow", 141, "DA");

        Assert.True(result.success);
        Assert.True(result.data.Permissions["pdf.zoom"]);
        Assert.Equal("perfil_activo", result.data.Sources["pdf.zoom"]);
        Assert.False(result.data.Permissions["pdf.download"]);
        Assert.Equal("perfil_activo", result.data.Sources["pdf.download"]);
    }

    [Fact]
    public async Task GetEffectivePermissionsAsync_ConOverride_UsaUsuarioOverride()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        await ExecuteNonQueryAsync(
            """
            INSERT INTO ra_vis_per_usuario_perfil(id_usuario, id_impl, id_perfil, estado, fecha_inicio, fecha_fin)
            SELECT 141, i.id_impl, pf.id_perfil, 1, CURDATE(), NULL
            FROM ra_vis_per_implementacion i
            JOIN ra_vis_per_perfil pf ON pf.id_impl = i.id_impl
            WHERE i.codigo_impl = 'workflow' AND pf.codigo_perfil = 'LECTOR'
            ON DUPLICATE KEY UPDATE
              id_perfil = VALUES(id_perfil),
              estado = VALUES(estado),
              fecha_inicio = VALUES(fecha_inicio),
              fecha_fin = VALUES(fecha_fin)
            """);

        var repository = CreateRepository();

        var upsert = await repository.UpsertUserOverridesAsync("workflow", 141,
        [
            new PermissionOverrideItemDto
            {
                CodigoPermiso = "pdf.zoom",
                Permitido = 0,
                Motivo = "Bloqueo temporal"
            }
        ], "DA");

        Assert.True(upsert.success);
        Assert.Equal(1, upsert.data.Procesados);

        var result = await repository.GetEffectivePermissionsAsync("workflow", 141, "DA");
        Assert.True(result.success);
        Assert.False(result.data.Permissions["pdf.zoom"]);
        Assert.Equal("usuario_override", result.data.Sources["pdf.zoom"]);
    }

    [Fact]
    public async Task UpsertDeleteOverrideAsync_PersisteYEliminaRegistro()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        var repository = CreateRepository();

        var upsert = await repository.UpsertUserOverridesAsync("workflow", 142,
        [
            new PermissionOverrideItemDto
            {
                CodigoPermiso = "pdf.print",
                Permitido = 1,
                Motivo = "Habilitado por prueba"
            }
        ], "DA");

        Assert.True(upsert.success);
        Assert.Equal(1, upsert.data.Procesados);

        var inserted = await ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*)
            FROM ra_vis_per_usuario_override uo
            JOIN ra_vis_per_implementacion i ON i.id_impl = uo.id_impl
            JOIN ra_vis_per_permiso p ON p.id_perm = uo.id_perm
            WHERE uo.id_usuario = 142
              AND i.codigo_impl = 'workflow'
              AND p.codigo_perm = 'pdf.print'
              AND uo.permitido = 1
              AND uo.estado = 1
            """);

        Assert.Equal(1, inserted);

        var delete = await repository.DeleteUserOverrideAsync("workflow", 142, "pdf.print", "DA");
        Assert.True(delete.success);
        Assert.Equal(1, delete.data.Procesados);

        var afterDelete = await ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*)
            FROM ra_vis_per_usuario_override uo
            JOIN ra_vis_per_implementacion i ON i.id_impl = uo.id_impl
            JOIN ra_vis_per_permiso p ON p.id_perm = uo.id_perm
            WHERE uo.id_usuario = 142
              AND i.codigo_impl = 'workflow'
              AND p.codigo_perm = 'pdf.print'
            """);

        Assert.Equal(0, afterDelete);
    }

    [Fact]
    public async Task GetEffectivePermissionsAsync_SinMatrizFallback_RetornaFallbackDeny()
    {
        if (_dockerUnavailable)
        {
            return;
        }

        await ExecuteNonQueryAsync(
            """
            INSERT INTO ra_vis_per_permiso (codigo_perm, recurso, accion, descripcion, estado)
            VALUES ('pdf.experimental', 'pdf', 'experimental', 'Permiso sin matriz', 1)
            ON DUPLICATE KEY UPDATE estado = VALUES(estado)
            """);

        var repository = CreateRepository();
        var result = await repository.GetEffectivePermissionsAsync("workflow", 142, "DA");

        Assert.True(result.success);
        Assert.False(result.data.Permissions["pdf.experimental"]);
        Assert.Equal("fallback_deny", result.data.Sources["pdf.experimental"]);
    }

    private PermisosVisorPdfRepository CreateRepository()
    {
        var factory = new TestMySqlConnectionFactory(_container!.GetConnectionString());
        var engine = new DapperCrudEngine(factory);
        return new PermisosVisorPdfRepository(engine, factory);
    }

    private async Task ExecuteScriptAsync(string scriptName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Database", "PermisosVisorPdf", scriptName);
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

    private async Task ExecuteNonQueryAsync(string sql)
    {
        await using var connection = new MySqlConnection(_container!.GetConnectionString());
        await connection.OpenAsync();
        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task<T> ExecuteScalarAsync<T>(string sql)
    {
        await using var connection = new MySqlConnection(_container!.GetConnectionString());
        await connection.OpenAsync();
        await using var command = new MySqlCommand(sql, connection);
        var result = await command.ExecuteScalarAsync();
        return (T)Convert.ChangeType(result!, typeof(T));
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
