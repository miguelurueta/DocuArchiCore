using System.Data;
using System.Data.Common;
using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RegistroRadicadoTareaWorkflowRepositoryTests
{
    [Fact]
    public async Task RegistrarTareaWorkflowAsync_CuandoTodoOk_ConfirmaCommitYRetornaId()
    {
        var factory = new FakeDbConnectionFactory();
        var repository = new RegistroRadicadoTareaWorkflowRepository(factory);

        var result = await repository.RegistrarTareaWorkflowAsync(
            9,
            "RUTA_TEST",
            101,
            202,
            15,
            20,
            30,
            40,
            50,
            1,
            2,
            3,
            BuildRelaciones(),
            new DateTime(2026, 3, 17, 10, 0, 0, DateTimeKind.Utc),
            "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(77, result.data!.idTareaWorkflow);
        Assert.NotNull(factory.LastConnection);
        Assert.True(factory.LastConnection!.LastTransaction!.Committed);
        Assert.False(factory.LastConnection.LastTransaction.RolledBack);
        Assert.Equal(["Q01", "Q02", "Q03", "Q04"], factory.LastConnection.ExecutedSteps);
        Assert.Contains(factory.LastConnection.ExecutedCommands, sql => sql.Contains("DAT_ADIC_TARRUTA_TEST", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task RegistrarTareaWorkflowAsync_CuandoFallaSegundaTabla_RealizaRollback()
    {
        var factory = new FakeDbConnectionFactory { FailOnStep = "Q03" };
        var repository = new RegistroRadicadoTareaWorkflowRepository(factory);

        var result = await repository.RegistrarTareaWorkflowAsync(
            9,
            "RUTA_TEST",
            101,
            0,
            15,
            20,
            30,
            40,
            50,
            1,
            2,
            3,
            BuildRelaciones(),
            DateTime.UtcNow,
            "DA");

        Assert.False(result.success);
        Assert.NotNull(factory.LastConnection);
        Assert.False(factory.LastConnection!.LastTransaction!.Committed);
        Assert.True(factory.LastConnection.LastTransaction.RolledBack);
        Assert.Contains("Q03", result.errors!.OfType<AppError>().First().Field);
    }

    [Fact]
    public async Task RegistrarTareaWorkflowAsync_CuandoRutaNoExiste_RetornaErrorControlado()
    {
        var factory = new FakeDbConnectionFactory { RouteExists = false };
        var repository = new RegistroRadicadoTareaWorkflowRepository(factory);

        var result = await repository.RegistrarTareaWorkflowAsync(
            9,
            "RUTA_TEST",
            101,
            null,
            15,
            20,
            30,
            40,
            50,
            1,
            2,
            3,
            BuildRelaciones(),
            DateTime.UtcNow,
            "DA");

        Assert.False(result.success);
        Assert.NotNull(factory.LastConnection);
        Assert.True(factory.LastConnection!.LastTransaction!.RolledBack);
        Assert.Contains("Q01", result.errors!.OfType<AppError>().First().Field);
    }

    [Fact]
    public async Task RegistrarTareaWorkflowAsync_CuandoNombreRutaInvalido_NoAbreConexion()
    {
        var factory = new FakeDbConnectionFactory();
        var repository = new RegistroRadicadoTareaWorkflowRepository(factory);

        var result = await repository.RegistrarTareaWorkflowAsync(
            9,
            "RUTA TEST",
            101,
            null,
            15,
            20,
            30,
            40,
            50,
            1,
            2,
            3,
            BuildRelaciones(),
            DateTime.UtcNow,
            "DA");

        Assert.False(result.success);
        Assert.Null(factory.LastConnection);
        Assert.Equal("nombreRuta", result.errors!.OfType<AppError>().First().Field);
    }

    private static IReadOnlyCollection<RelacionCamposRutaWorklflowDto> BuildRelaciones()
    {
        return
        [
            new RelacionCamposRutaWorklflowDto
            {
                NombreCampoRuta = "CAMPO_RUTA_1",
                DatoCampoPlantilla = "VALOR_1"
            },
            new RelacionCamposRutaWorklflowDto
            {
                NombreCampoRuta = "CAMPO_RUTA_2",
                DatoCampoPlantilla = "VALOR_2"
            }
        ];
    }

    private sealed class FakeDbConnectionFactory : IDbConnectionFactory
    {
        public string? FailOnStep { get; set; }
        public bool RouteExists { get; set; } = true;
        public FakeDbConnection? LastConnection { get; private set; }

        public IDbConnection GetOpenConnection(string? dbAlias = null)
        {
            LastConnection = new FakeDbConnection(FailOnStep, RouteExists);
            LastConnection.Open();
            return LastConnection;
        }

        public Task<IDbConnection> GetOpenConnectionAsync(string? dbAlias = null)
        {
            LastConnection = new FakeDbConnection(FailOnStep, RouteExists);
            LastConnection.Open();
            return Task.FromResult<IDbConnection>(LastConnection);
        }

        public string ProviderBsd() => "fake";
        public IEnumerable<string> GetAvailableAliases() => ["da"];
    }

    private sealed class FakeDbConnection : DbConnection
    {
        private readonly string? _failOnStep;
        private readonly bool _routeExists;
        private ConnectionState _state = ConnectionState.Closed;

        public FakeDbConnection(string? failOnStep, bool routeExists)
        {
            _failOnStep = failOnStep;
            _routeExists = routeExists;
        }

        public List<string> ExecutedSteps { get; } = [];
        public List<string> ExecutedCommands { get; } = [];
        public FakeDbTransaction? LastTransaction { get; private set; }

        public override string ConnectionString { get; set; } = "fake";
        public override string Database => "fake";
        public override string DataSource => "fake";
        public override string ServerVersion => "1.0";
        public override ConnectionState State => _state;

        public override void Open() => _state = ConnectionState.Open;
        public override void Close() => _state = ConnectionState.Closed;
        public override void ChangeDatabase(string databaseName) { }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            LastTransaction = new FakeDbTransaction(this);
            return LastTransaction;
        }

        protected override DbCommand CreateDbCommand()
        {
            return new FakeDbCommand(this, _failOnStep, _routeExists);
        }
    }

    private sealed class FakeDbTransaction : DbTransaction
    {
        private readonly DbConnection _connection;

        public FakeDbTransaction(DbConnection connection)
        {
            _connection = connection;
        }

        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public override IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
        protected override DbConnection DbConnection => _connection;
        public override void Commit() => Committed = true;
        public override void Rollback() => RolledBack = true;
    }

    private sealed class FakeDbCommand : DbCommand
    {
        private readonly FakeDbConnection _connection;
        private readonly string? _failOnStep;
        private readonly bool _routeExists;
        private readonly FakeDbParameterCollection _parameters = new();

        public FakeDbCommand(FakeDbConnection connection, string? failOnStep, bool routeExists)
        {
            _connection = connection;
            _failOnStep = failOnStep;
            _routeExists = routeExists;
        }

        public override string CommandText { get; set; } = string.Empty;
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; } = CommandType.Text;
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection DbConnection
        {
            get => _connection;
            set { }
        }

        protected override DbParameterCollection DbParameterCollection => _parameters;
        protected override DbTransaction? DbTransaction { get; set; }

        public override void Cancel() { }

        public override object? ExecuteScalar()
        {
            Track();
            var step = ExtractStep(CommandText);
            if (CommandText.Contains("FROM rutas_workflow", StringComparison.OrdinalIgnoreCase))
            {
                return _routeExists ? 1 : 0;
            }

            return step == "Q02" ? 77L : 1;
        }

        public override int ExecuteNonQuery()
        {
            Track();
            return 1;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            Track();
            var table = new DataTable();
            table.Columns.Add("COLUMN_NAME", typeof(string));
            foreach (var column in new[]
            {
                "INICIO_TAREAS_WORKFLOW_ID_TARE",
                "ID_GABINETE",
                "ID_IMAGEN",
                "FLUJO_TRABAJO_WF",
                "estado_modulo_radicado",
                "CAMPO_RUTA_1",
                "CAMPO_RUTA_2"
            })
            {
                table.Rows.Add(column);
            }

            return table.CreateDataReader();
        }

        public override void Prepare() { }
        protected override DbParameter CreateDbParameter() => new FakeDbParameter();

        private void Track()
        {
            _connection.ExecutedCommands.Add(CommandText);
            var step = ExtractStep(CommandText);
            if (string.Equals(step, _failOnStep, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Fallo simulado en paso {step}");
            }

            if (!string.IsNullOrWhiteSpace(step))
            {
                _connection.ExecutedSteps.Add(step);
            }
        }

        private static string ExtractStep(string commandText)
        {
            foreach (var marker in new[] { "Q01", "Q02", "Q03", "Q04" })
            {
                if (commandText.Contains(marker, StringComparison.OrdinalIgnoreCase))
                {
                    return marker;
                }
            }

            return string.Empty;
        }
    }

    private sealed class FakeDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; } = string.Empty;
        public override string SourceColumn { get; set; } = string.Empty;
        public override object? Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }
        public override void ResetDbType() { }
    }

    private sealed class FakeDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _items = [];

        public override int Count => _items.Count;
        public override object SyncRoot => ((System.Collections.ICollection)_items).SyncRoot;
        public override int Add(object value)
        {
            _items.Add((DbParameter)value);
            return _items.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var value in values) Add(value!);
        }

        public override void Clear() => _items.Clear();
        public override bool Contains(object value) => _items.Contains((DbParameter)value);
        public override bool Contains(string value) => _items.Any(p => string.Equals(p.ParameterName, value, StringComparison.OrdinalIgnoreCase));
        public override void CopyTo(Array array, int index) => ((System.Collections.ICollection)_items).CopyTo(array, index);
        public override System.Collections.IEnumerator GetEnumerator() => _items.GetEnumerator();
        public override int IndexOf(object value) => _items.IndexOf((DbParameter)value);
        public override int IndexOf(string parameterName) => _items.FindIndex(p => string.Equals(p.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));
        public override void Insert(int index, object value) => _items.Insert(index, (DbParameter)value);
        public override void Remove(object value) => _items.Remove((DbParameter)value);
        public override void RemoveAt(int index) => _items.RemoveAt(index);
        public override void RemoveAt(string parameterName)
        {
            var index = IndexOf(parameterName);
            if (index >= 0) _items.RemoveAt(index);
        }

        protected override DbParameter GetParameter(int index) => _items[index];
        protected override DbParameter GetParameter(string parameterName) => _items[IndexOf(parameterName)];
        protected override void SetParameter(int index, DbParameter value) => _items[index] = value;
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index >= 0) _items[index] = value;
        }
    }
}
