using System.Data;
using System.Data.Common;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RegistrarRadicacionEntranteRepositoryTransactionTests
{
    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoTodoOk_ConfirmaCommit()
    {
        var factory = new FakeDbConnectionFactory();
        var repository = new RegistrarRadicacionEntranteRepository(factory);

        var result = await repository.RegistrarRadicacionEntranteAsync(
            BuildRequest("ENTRANTE", esRelacionado: true),
            55,
            "DA");

        Assert.True(result.success);
        Assert.NotNull(factory.LastConnection);
        Assert.NotNull(factory.LastConnection!.LastTransaction);
        Assert.True(factory.LastConnection.LastTransaction!.Committed);
        Assert.False(factory.LastConnection.LastTransaction.RolledBack);
        Assert.Equal(["Q01", "Q02", "Q03", "Q05", "Q06", "Q07", "Q08"], factory.LastConnection.ExecutedSteps);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoFallaPaso_RealizaRollbackTotal()
    {
        var factory = new FakeDbConnectionFactory { FailOnStep = "Q04" };
        var repository = new RegistrarRadicacionEntranteRepository(factory);

        var result = await repository.RegistrarRadicacionEntranteAsync(
            BuildRequest("ENTRANTE"),
            55,
            "DA");

        Assert.False(result.success);
        Assert.NotNull(factory.LastConnection);
        Assert.NotNull(factory.LastConnection!.LastTransaction);
        Assert.False(factory.LastConnection.LastTransaction!.Committed);
        Assert.True(factory.LastConnection.LastTransaction.RolledBack);
        Assert.Contains("Q04", result.errors!.OfType<MiApp.DTOs.DTOs.Errors.AppError>().First().Message);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoTipoPqr_EjecutaQ09()
    {
        var factory = new FakeDbConnectionFactory();
        var repository = new RegistrarRadicacionEntranteRepository(factory);

        var result = await repository.RegistrarRadicacionEntranteAsync(
            BuildRequest("PQR"),
            55,
            "DA");

        Assert.True(result.success);
        Assert.NotNull(factory.LastConnection);
        Assert.Equal(
            ["Q01", "Q02", "Q03", "Q05", "Q06", "Q07", "Q08", "Q09"],
            factory.LastConnection!.ExecutedSteps);
    }

    private static RegistrarRadicacionEntranteRequestDto BuildRequest(string tipoRadicacion, bool esRelacionado = false)
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            TipoRadicacion = tipoRadicacion,
            Asunto = "Asunto",
            Remitente = "Remitente",
            EsRelacionado = esRelacionado,
            RadicadosRelacionados = esRelacionado ? [777] : []
        };
    }

    private sealed class FakeDbConnectionFactory : IDbConnectionFactory
    {
        public string? FailOnStep { get; set; }
        public FakeDbConnection? LastConnection { get; private set; }

        public IDbConnection GetOpenConnection(string? dbAlias = null)
        {
            LastConnection = new FakeDbConnection(FailOnStep);
            LastConnection.Open();
            return LastConnection;
        }

        public Task<IDbConnection> GetOpenConnectionAsync(string? dbAlias = null)
        {
            LastConnection = new FakeDbConnection(FailOnStep);
            LastConnection.Open();
            return Task.FromResult<IDbConnection>(LastConnection);
        }

        public string ProviderBsd() => "fake";

        public IEnumerable<string> GetAvailableAliases() => ["da"];
    }

    private sealed class FakeDbConnection : DbConnection
    {
        private readonly string? _failOnStep;
        private ConnectionState _state = ConnectionState.Closed;

        public FakeDbConnection(string? failOnStep)
        {
            _failOnStep = failOnStep;
        }

        public List<string> ExecutedSteps { get; } = [];
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
            return new FakeDbCommand(this, _failOnStep);
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
        private readonly FakeDbParameterCollection _parameters = new();

        public FakeDbCommand(FakeDbConnection connection, string? failOnStep)
        {
            _connection = connection;
            _failOnStep = failOnStep;
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
            var stepName = ExtractStepName(CommandText);
            if (string.Equals(stepName, _failOnStep, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Fallo simulado en paso {stepName}");
            }

            if (!string.IsNullOrWhiteSpace(stepName))
            {
                _connection.ExecutedSteps.Add(stepName);
            }

            return stepName == "Q01" ? 10 : 1;
        }

        public override int ExecuteNonQuery()
        {
            var stepName = ExtractStepName(CommandText);
            if (string.Equals(stepName, _failOnStep, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Fallo simulado en paso {stepName}");
            }

            if (!string.IsNullOrWhiteSpace(stepName))
            {
                _connection.ExecutedSteps.Add(stepName);
            }

            return 1;
        }

        protected override DbParameter CreateDbParameter() => new FakeDbParameter();
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotSupportedException();
        public override void Prepare() { }

        private static string ExtractStepName(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                return string.Empty;
            }

            var markers = new[] { "Q01", "Q02", "Q03", "Q04", "Q05", "Q06", "Q07", "Q08", "Q09" };
            foreach (var marker in markers)
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
            foreach (var value in values)
            {
                Add(value!);
            }
        }

        public override void Clear() => _items.Clear();
        public override bool Contains(object value) => _items.Contains((DbParameter)value);
        public override bool Contains(string value) => _items.Any(p => MatchesName(p, value));
        public override void CopyTo(Array array, int index) => ((System.Collections.ICollection)_items).CopyTo(array, index);
        public override System.Collections.IEnumerator GetEnumerator() => _items.GetEnumerator();

        public override int IndexOf(object value) => _items.IndexOf((DbParameter)value);
        public override int IndexOf(string parameterName) => _items.FindIndex(p => MatchesName(p, parameterName));

        public override void Insert(int index, object value) => _items.Insert(index, (DbParameter)value);
        public override void Remove(object value) => _items.Remove((DbParameter)value);
        public override void RemoveAt(int index) => _items.RemoveAt(index);

        public override void RemoveAt(string parameterName)
        {
            var index = IndexOf(parameterName);
            if (index >= 0)
            {
                _items.RemoveAt(index);
            }
        }

        protected override DbParameter GetParameter(int index) => _items[index];
        protected override DbParameter GetParameter(string parameterName) => _items[IndexOf(parameterName)];

        protected override void SetParameter(int index, DbParameter value) => _items[index] = value;

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index >= 0)
            {
                _items[index] = value;
                return;
            }

            _items.Add(value);
        }

        public DbParameter? GetByAnyName(string parameterName)
        {
            return _items.FirstOrDefault(p => MatchesName(p, parameterName));
        }

        private static bool MatchesName(DbParameter parameter, string expected)
        {
            var name = parameter.ParameterName?.TrimStart('@', ':', '?') ?? string.Empty;
            var target = expected.TrimStart('@', ':', '?');
            return string.Equals(name, target, StringComparison.OrdinalIgnoreCase);
        }
    }
}
