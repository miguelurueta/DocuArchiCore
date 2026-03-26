using System.Data;
using System.Data.Common;
using MiApp.DTOs.DTOs.GestorDocumental.Sede;
using MiApp.DTOs.DTOs.GestorDocumental.usuario;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Models.Models.Radicacion.TipoTramite;
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
            10,
            "DA",
            "127.0.0.1",
            1,
            "RADICACION",
            BuildPlantilla(100),
            [],
            BuildBackendParams(),
            BuildTipoDocEntrante(302, 100));

        Assert.True(result.success);
        Assert.NotNull(factory.LastConnection);
        Assert.NotNull(factory.LastConnection!.LastTransaction);
        Assert.True(factory.LastConnection.LastTransaction!.Committed);
        Assert.False(factory.LastConnection.LastTransaction.RolledBack);
        Assert.Equal(["Q01", "Q02", "Q03", "Q04", "Q05", "Q06", "Q07"], factory.LastConnection.ExecutedSteps);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoFallaPaso_RealizaRollbackTotal()
    {
        var factory = new FakeDbConnectionFactory { FailOnStep = "Q04" };
        var repository = new RegistrarRadicacionEntranteRepository(factory);

        var result = await repository.RegistrarRadicacionEntranteAsync(
            BuildRequest("ENTRANTE", esRelacionado: true),
            55,
            10,
            "DA",
            "127.0.0.1",
            1,
            "RADICACION",
            BuildPlantilla(100),
            [],
            BuildBackendParams(),
            BuildTipoDocEntrante(302, 100));

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
            10,
            "DA",
            "127.0.0.1",
            1,
            "RADICACION",
            BuildPlantilla(100),
            [],
            BuildBackendParams(),
            BuildTipoDocEntrante(302, 100));

        Assert.True(result.success);
        Assert.NotNull(factory.LastConnection);
        Assert.Equal(
            ["Q01", "Q02", "Q03", "Q05", "Q06", "Q07", "Q09"],
            factory.LastConnection!.ExecutedSteps);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoTipoDocNoRequiereRespuesta_NoEjecutaQ06NiQ07()
    {
        var factory = new FakeDbConnectionFactory();
        var repository = new RegistrarRadicacionEntranteRepository(factory);
        var tipoDoc = BuildTipoDocEntrante(302, 100);
        tipoDoc.requiere_respuesta = 0;

        var result = await repository.RegistrarRadicacionEntranteAsync(
            BuildRequest("ENTRANTE"),
            55,
            10,
            "DA",
            "127.0.0.1",
            1,
            "RADICACION",
            BuildPlantilla(100),
            [],
            BuildBackendParams(),
            tipoDoc);

        Assert.True(result.success);
        Assert.NotNull(factory.LastConnection);
        Assert.Equal(
            ["Q01", "Q02", "Q03", "Q05"],
            factory.LastConnection!.ExecutedSteps);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoTipoModuloRadicacionEsWorkflow_EjecutaQ08()
    {
        var factory = new FakeDbConnectionFactory();
        var repository = new RegistrarRadicacionEntranteRepository(factory);
        var tipoDoc = BuildTipoDocEntrante(302, 100);
        tipoDoc.util_tipo_modulo_envio = 1;

        var result = await repository.RegistrarRadicacionEntranteAsync(
            BuildRequest("ENTRANTE"),
            55,
            10,
            "DA",
            "127.0.0.1",
            2,
            "RADICACION",
            BuildPlantilla(100),
            [],
            BuildBackendParams(),
            tipoDoc);

        Assert.True(result.success);
        Assert.Contains("Q08", factory.LastConnection!.ExecutedSteps);
        Assert.Equal(1, result.data!.ReturnRegistraRadicacion.IdEstadoRadicado);
        Assert.Equal(1, Assert.IsType<int>(result.data.MetadataOperativa["idEstadoRadicado"]!));
    }

    private static RegistrarRadicacionEntranteRequestDto BuildRequest(string tipoRadicacion, bool esRelacionado = false)
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            TipoRadicado = new TipoRadicadoEntradaDto
            {
                TipoRadicacion = tipoRadicacion,
                IdTipoRadicado = 1
            },
            ASUNTO = "Asunto",
            Remitente = new RemitenteRadicacionDto { Nombre = "Remitente", id_Dest_Ext = 123 },
            radicadoRelacionados = esRelacionado
                ? [new RadicadoRelacionadoDto { consecutivoRelacionadohijo = "777", idregistroradicadohijo = 0, idplantillahijo = 0 }]
                : []
        };
    }

    private static SystemPlantillaRadicado BuildPlantilla(int idPlantilla)
    {
        return new SystemPlantillaRadicado
        {
            id_Plantilla = idPlantilla,
            Nombre_Plantilla_Radicado = "ra_plantilla_100",
            Tipo_Plantilla = "RAD",
            Fecha_Creacion = DateTime.UtcNow,
            Estado_Plantilla = 1,
            Consecutivo_Rad = 10,
            Consecutivo_CodBarra = 10,
            util_activo_plantilla_flujo = 1,
            Util_activo_plantilla_default_rad_interno = 0,
            util_evalua_periodo_general = 0,
            utill_evalua_periodo_pqr = 0,
            util_evalua_periodo_interno = 0,
            util_evalua_festivo = 0,
            util_tipo_modulo_envio = 0,
            util_consecutivo_general = 0,
            util_default_simple = 0,
            util_estado_pendiente_rad = 0,
            utilIncuyeConsecutivoArea = 0,
            utilConseTipoRad = 0,
            util_default_radicado = 1
        };
    }

    private static ParametrosRadicadosDto BuildBackendParams()
    {
        return new ParametrosRadicadosDto
        {
            NombreAreaRemitdest = new NombreAreaRemitdestDto
            {
                IdArea = 7,
                NombreArea = "AREA TEST"
            },
            TipoDocEntrante = new TipoDocEntranteParametroDto
            {
                IdTipoDocEntrante = 302,
                DescripcionDoc = "TRAMITE TEST"
            },
            IdSedeNombre = new IdSedeNombreDto
            {
                IdSede = 4,
                NombreSede = "SEDE TEST"
            }
        };
    }

    private static TipoDocEntrante BuildTipoDocEntrante(int idTipoDocEntrante, int idPlantilla)
    {
        return new TipoDocEntrante
        {
            id_Tipo_Doc_Entrante = idTipoDocEntrante,
            Descripcion_Doc = "TRAMITE TEST",
            system_plantilla_radicado_id_plantilla = idPlantilla,
            estado_tipo_documento = 1,
            flow_tipo = 1,
            requiere_respuesta = 1,
            codigo_gabinete_workflow = 1,
            resp_correo_fisico_electronico = 1,
            id_ruta = 1,
            tipo_tramite = 1,
            estado_ruta_open_close = 1,
            obliga_exp_radicado = 0,
            activo_modulo_respuesta = 1,
            util_tipo_modulo_envio = 1,
            util_producion_documental = 0,
            tipo_tramite_entrante_saliente = 1,
            wf_copia_doc_expediente_actualiza_exped_gabinete = 0,
            wf_auto_vincula_doc_expediente_actualiza_exped_gabinete = 0,
            wf_copia_doc_expediente_produc_actualiza_exped_gabinete = 0,
            util_auto_vincula_migracion = 0,
            id_gabinete = 1,
            util_radicacion_simple = 1,
            util_nivel_padre_auto_vincula = 0,
            util_opcion_auto_vincula = 0,
            util_Estado_Crea_ExpedienteSII = 0,
            util_Estado_Multiple_expedienteSII = 0
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

            if (CommandText.Contains("SELECT 1", StringComparison.OrdinalIgnoreCase)
                && CommandText.Contains("FROM tipo_doc_entrante", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            if (CommandText.Contains("SELECT Descripcion_Doc", StringComparison.OrdinalIgnoreCase)
                && CommandText.Contains("FROM tipo_doc_entrante", StringComparison.OrdinalIgnoreCase))
            {
                return "TRAMITE";
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
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
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

            if (CommandText.Contains("information_schema.columns", StringComparison.OrdinalIgnoreCase))
            {
                var schemaTable = new DataTable();
                schemaTable.Columns.Add("COLUMN_NAME", typeof(string));

                string[] columns =
                [
                    "system_plantilla_radicado_id_plantilla",
                    "id_radicado_plantilla",
                    "nombre_plantilla",
                    "consecutivo",
                    "codbarra",
                    "flag_flow",
                    "id_usuario_responsable",
                    "asunto",
                    "tipo_radicacion",
                    "fecha_registro_utc",
                    "id_respuesta",
                    "id_usuario",
                    "desc_operacion",
                    "fecha_operacion_utc",
                    "id_radicado",
                    "consecutivo_radicado",
                    "estado",
                    "remitente",
                    "id_usuario_radicado",
                    "id_tarea_workflow",
                    "tipo_doc_entrante_id_Tipo_Doc_Entrante",
                    "tipo_plantilla_radicado",
                    "log_error_wf_asing"
                ];

                foreach (var column in columns)
                {
                    schemaTable.Rows.Add(column);
                }

                return schemaTable.CreateDataReader();
            }

            var table = new DataTable();
            table.Columns.Add("ConsecutivoRad", typeof(int));
            table.Columns.Add("ConsecutivoCodBarra", typeof(int));
            table.Rows.Add(10, 10);
            return table.CreateDataReader();
        }
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

