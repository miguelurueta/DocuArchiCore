## Context

- Jira issue key: SCRUM-168
- Jira summary: CREA-FUNCION-INSERTCION-GABINETE-ALMACENAMINETO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-168

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 7 — Inserción en Gabinete + Inventario Documental (ENTERPRISE CRÍTICO — COMPLETO, SEGURO, DOCUMENTADO Y CONSISTENTE) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Arquitecto Master Backend .NET especialista en: Clean Architecture DapperCrudEngine QueryOptions SQL dinámico seguro sistemas documentales ECM transacciones críticas validación de integridad protección contra SQL Injection repositorios enterprise inventario documental observabilidad pruebas unitarias, integración, concurrencia y QT documentación técnica enterprise ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar la persistencia principal del Storage Engine: inserción segura en tabla dinámica del gabinete inserción en registro_producion_documental integración dentro de la transacción existente del TransactionCoordinator construcción segura de SQL dinámico validación completa de modelos protección contra SQL Injection trazabilidad por RequestId documentación y pruebas completas Este bloque reemplaza la lógica legacy equivalente a: INSERT INTO {gabinete} INSERT INTO registro_producion_documental REFERENCIA DE TABLAS Ver: Docs/DataModel/StorageEngine-Tables.md Tablas usadas en este prompt: registro_producion_documental GABINETE DINAMICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO Y DEPENDENCIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Depende de: PROMPT 2 SCRUMCORE-163 — contratos, models, enums y exceptions PROMPT 5 SCRUMCORE-166 — IdentityAllocator PROMPT 6 SCRUMCORE-167 — StorageTransactionCoordinator Flujo esperado: StorageTransactionCoordinator → IdentityAllocator → GabineteStorageRepository → InventarioDocumentalRepository → Commit / Rollback Este prompt NO implementa todavía: expediente unidad de conservación índice electrónico filesystem XML compensación física ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLA GLOBAL OBLIGATORIA — ACCESO A DATOS CON DAPPERCRUDENGINE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Toda función de repositorio que realice consulta, inserción, actualización, eliminación, bloqueo lógico o lectura transaccional de datos DEBE usar obligatoriamente: DapperCrudEngine QueryOptions QueryFilter / filtros equivalentes del motor modelos tipados de entrada/salida contexto transaccional soportado por el DapperCrudEngine cuando aplique Ruta de referencia obligatoria: D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\DataAccess\DapperCrudEngine.cs Regla de arquitectura: Repository = DapperCrudEngine + QueryOptions PROHIBIDO en repositories: connection.ExecuteAsync(...) connection.QueryAsync(...) connection.ExecuteScalarAsync(...) SQL manual concatenado SQL interpolado con datos de usuario construir comandos SQL libres fuera del DapperCrudEngine abrir conexiones directamente abrir transacciones directamente hacer commit o rollback retornar AppResponses<T> resolver claims o lógica HTTP Permitido únicamente si el DapperCrudEngine ya lo soporta o se implementa primero como extensión centralizada: ejecución transaccional mediante DapperCrudEngine QueryOptions con TransactionContext QueryOptions con LockMode = ForUpdate QueryOptions con Insert/Update/Delete tipado QueryOptions con condición de concurrencia QueryOptions con retorno de identidad generada Si el DapperCrudEngine actual NO soporta alguna operación requerida por este prompt, se debe crear primero una extensión centralizada del DapperCrudEngine, documentada y probada, antes de implementar el repository. Queda prohibido saltarse el motor con Dapper directo. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLA TRANSACCIONAL CON DAPPERCRUDENGINE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Cuando una operación de repository deba participar en una transacción del Storage Engine: El TransactionCoordinator abre la conexión/transacción o solicita al DapperCrudEngine un contexto transaccional. El repository NO manipula IDbConnection/IDbTransaction directamente salvo que el DapperCrudEngine exponga un contexto transaccional tipado. El repository recibe un contexto de ejecución transaccional, por ejemplo: StorageDbExecutionContext DapperCrudTransactionContext QueryExecutionContext Usar el nombre real disponible en el proyecto. Toda consulta, insert, update, lock o condición de concurrencia debe ejecutarse a través de DapperCrudEngine. Si se requiere SELECT ... FOR UPDATE, debe modelarse mediante QueryOptions.LockMode = ForUpdate o una propiedad equivalente. Si se requiere UPDATE con condición de concurrencia, debe modelarse en QueryOptions con filtros WHERE adicionales. Si se requiere INSERT y retorno de identidad, debe usarse método centralizado del DapperCrudEngine. Si no existe, implementarlo en el motor. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Repository: MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/ Archivos: IGabineteStorageRepository.cs GabineteStorageRepository.cs IInventarioDocumentalRepository.cs InventarioDocumentalRepository.cs Models: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/ Archivos: GabineteInsertModel.cs InventarioInsertModel.cs Documentación: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: public sealed class GabineteInsertModel { public required string NombreGabinete { get; init; } public required long IdAlmacen { get; init; }
public required int Disco { get; init; }
public required int Paginas { get; init; }
public required int TipoDocumento { get; init; }
public required int Carpeta { get; init; }

public required string Usuario { get; init; }
public required DateTime Fecha { get; init; }

public required Dictionary<string, object?> CamposDinamicos { get; init; } } Crear: public sealed class InventarioInsertModel { public int IdUsuarioGestion { get; init; } public int IdEmpresa { get; init; } public string? Radicado { get; init; }
public string? FullText { get; init; }

public int NumeroFolios { get; init; }

public long IdAlmacen { get; init; }

public string NombreGabinete { get; init; } = string.Empty;

public string Formato { get; init; } = string.Empty;
public string Tamano { get; init; } = string.Empty; } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: public interface IGabineteStorageRepository { Task<int> InsertAsync( GabineteInsertModel model, IDbConnection connection, IDbTransaction transaction ); } Crear: public interface IInventarioDocumentalRepository { Task<long> InsertAsync( InventarioInsertModel model, IDbConnection connection, IDbTransaction transaction ); } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE SEGURIDAD — SQL DINÁMICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Reglas obligatorias: El nombre del gabinete debe validarse SIEMPRE en el repository. Las columnas dinámicas deben validarse SIEMPRE en el repository. No confiar únicamente en validaciones de capas anteriores. No aceptar espacios, puntos, guiones, comillas, backticks ni caracteres especiales en nombres de tabla/columna. Solo permitir patrón: ^[a-zA-Z0-9_]+$ Todos los valores deben ir parametrizados con DynamicParameters. Prohibido concatenar valores. El único SQL dinámico permitido es: nombre de tabla validado nombres de columna validados Prohibido: SELECT * SQL libre desde request columnas enviadas sin validar tabla enviada sin validar concatenar valores interpolar valores de usuario ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ AJUSTE ESPECÍFICO PROMPT 7 — GABINETE E INVENTARIO CON DAPPERCRUDENGINE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Este prompt reemplaza cualquier ejemplo anterior basado en connection.ExecuteAsync o ExecuteScalarAsync. GabineteStorageRepository: Debe validar NombreGabinete y CamposDinamicos. Debe construir InsertOptions/QueryOptions mediante DapperCrudEngine. El nombre de tabla dinámica solo se permite si fue validado contra regex y, preferiblemente, contra metadata de gabinete. Los valores deben ir en diccionario parametrizado dentro de QueryOptions. Ejemplo conceptual: var options = new QueryOptions { TableName = model.NombreGabinete, InsertValues = insertValues, ValidateIdentifier = true }; var rows = await _DaperCrudEngine.InsertAsync(options, executionContext); InventarioDocumentalRepository: var options = new QueryOptions { TableName = "registro_producion_documental", InsertValues = new Dictionary<string, object?> { ["ID_USUARIO_GESTION"] = model.IdUsuarioGestion, ["ID_EMPRESA_DOCUMENTO"] = model.IdEmpresa, ["RADICADO_DOCUMENTO"] = model.Radicado, ["FULTEXT_DOCUMENTO"] = model.FullText, ["NUMERO_FOLIOS"] = model.NumeroFolios, ["ID_DOCUMENTO_DOCUARCHI_ALMACEN"] = model.IdAlmacen, ["NOMBRE_GABINETE"] = model.NombreGabinete, ["FORMATO"] = model.Formato, ["TAMANO"] = model.Tamano }, ReturnGeneratedIdentity = true }; var id = await _DaperCrudEngine.InsertAndReturnIdAsync<long>(options, executionContext); Si InsertValues, ReturnGeneratedIdentity o InsertAndReturnIdAsync no existen, deben implementarse en el DapperCrudEngine antes de este prompt. No se permite usar ExecuteScalarAsync directo. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — GabineteStorageRepository ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Responsabilidad: validar modelo validar nombre de tabla dinámica validar columnas dinámicas construir INSERT seguro usar transacción recibida no abrir conexión no hacer commit no hacer rollback Implementación esperada: public sealed class GabineteStorageRepository : IGabineteStorageRepository { private static readonly Regex SqlIdentifierRegex = new("^[a-zA-Z0-9_]+$", RegexOptions.Compiled); private readonly ILogger<GabineteStorageRepository> _logger;

public GabineteStorageRepository(ILogger<GabineteStorageRepository> logger)
{
    _logger = logger;
}

public async Task<int> InsertAsync(
    GabineteInsertModel model,
    IDbConnection connection,
    IDbTransaction transaction)
{
    if (model == null)
        throw new StorageTransactionException("GabineteInsertModel requerido");

    if (connection == null)
        throw new StorageTransactionException("Connection requerida");

    if (transaction == null)
        throw new StorageTransactionException("Transaction requerida");

    if (connection.State != ConnectionState.Open)
        throw new StorageTransactionException("Connection no está abierta");

    if (string.IsNullOrWhiteSpace(model.NombreGabinete))
        throw new StorageTransactionException("NombreGabinete requerido");

    if (!SqlIdentifierRegex.IsMatch(model.NombreGabinete))
        throw new StorageTransactionException("NombreGabinete inválido");

    if (model.IdAlmacen <= 0)
        throw new StorageTransactionException("IdAlmacen inválido");

    if (model.Disco <= 0)
        throw new StorageTransactionException("Disco inválido");

    if (model.Paginas <= 0)
        throw new StorageTransactionException("Paginas inválidas");

    if (model.TipoDocumento <= 0)
        throw new StorageTransactionException("TipoDocumento inválido");

    if (model.Carpeta <= 0)
        throw new StorageTransactionException("Carpeta inválida");

    if (string.IsNullOrWhiteSpace(model.Usuario))
        throw new StorageTransactionException("Usuario requerido");

    if (model.Fecha == default)
        throw new StorageTransactionException("Fecha inválida");

    if (model.CamposDinamicos == null)
        throw new StorageTransactionException("CamposDinamicos requerido");

    var columnasUnicas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    var columnas = new List<string>
    {
        "ID",
        "DISC",
        "PAG",
        "DBT",
        "IDEX",
        "USER",
        "DATE1"
    };

    foreach (var baseColumn in columnas)
    {
        columnasUnicas.Add(baseColumn);
    }

    var parametros = new DynamicParameters();

    parametros.Add("ID", model.IdAlmacen);
    parametros.Add("DISC", model.Disco);
    parametros.Add("PAG", model.Paginas);
    parametros.Add("DBT", model.TipoDocumento);
    parametros.Add("IDEX", model.Carpeta);
    parametros.Add("USER", model.Usuario);
    parametros.Add("DATE1", model.Fecha);

    foreach (var campo in model.CamposDinamicos)
    {
        if (string.IsNullOrWhiteSpace(campo.Key))
            throw new StorageTransactionException("Columna dinámica vacía");

        if (!SqlIdentifierRegex.IsMatch(campo.Key))
            throw new StorageTransactionException($"Columna dinámica inválida: {campo.Key}");

        if (!columnasUnicas.Add(campo.Key))
            throw new StorageTransactionException($"Columna duplicada: {campo.Key}");

        columnas.Add(campo.Key);
        parametros.Add(campo.Key, campo.Value);
    }

    var columnasSql = string.Join(",", columnas);
    var valoresSql = string.Join(",", columnas.Select(c => "@" + c));

    var sql = $"INSERT INTO {model.NombreGabinete} ({columnasSql}) VALUES ({valoresSql})";

    _logger.LogInformation(
        "Insert gabinete iniciado gabinete={gabinete} idAlmacen={idAlmacen} columnas={columnas}",
        model.NombreGabinete,
        model.IdAlmacen,
        columnas.Count
    );

    var rows = await connection.ExecuteAsync(sql, parametros, transaction);

    if (rows != 1)
        throw new StorageTransactionException("Inserción en gabinete no afectó exactamente una fila");

    _logger.LogInformation(
        "Insert gabinete OK gabinete={gabinete} idAlmacen={idAlmacen}",
        model.NombreGabinete,
        model.IdAlmacen
    );

    return rows;
} } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — InventarioDocumentalRepository ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Responsabilidad: validar modelo insertar en registro_producion_documental retornar LAST_INSERT_ID usar transacción recibida no abrir conexión no hacer commit no hacer rollback Implementación esperada: public sealed class InventarioDocumentalRepository : IInventarioDocumentalRepository { private readonly ILogger<InventarioDocumentalRepository> _logger; public InventarioDocumentalRepository(ILogger<InventarioDocumentalRepository> logger)
{
    _logger = logger;
}

public async Task<long> InsertAsync(
    InventarioInsertModel model,
    IDbConnection connection,
    IDbTransaction transaction)
{
    if (model == null)
        throw new StorageTransactionException("InventarioInsertModel requerido");

    if (connection == null)
        throw new StorageTransactionException("Connection requerida");

    if (transaction == null)
        throw new StorageTransactionException("Transaction requerida");

    if (connection.State != ConnectionState.Open)
        throw new StorageTransactionException("Connection no está abierta");

    if (model.IdUsuarioGestion <= 0)
        throw new StorageTransactionException("IdUsuarioGestion inválido");

    if (model.IdEmpresa <= 0)
        throw new StorageTransactionException("IdEmpresa inválido");

    if (model.IdAlmacen <= 0)
        throw new StorageTransactionException("IdAlmacen inválido");

    if (model.NumeroFolios <= 0)
        throw new StorageTransactionException("NumeroFolios inválido");

    if (string.IsNullOrWhiteSpace(model.NombreGabinete))
        throw new StorageTransactionException("NombreGabinete requerido");

    if (model.FullText?.Length > 1000000)
    {
        model = model with
        {
            FullText = model.FullText.Substring(0, 1000000)
        };
    }

    const string sql = @" INSERT INTO registro_producion_documental ( ID_USUARIO_GESTION, ID_EMPRESA_DOCUMENTO, RADICADO_DOCUMENTO, FULTEXT_DOCUMENTO, NUMERO_FOLIOS, ID_DOCUMENTO_DOCUARCHI_ALMACEN, NOMBRE_GABINETE, FORMATO, TAMANO ) VALUES ( @IdUsuarioGestion, @IdEmpresa, @Radicado, @FullText, @NumeroFolios, @IdAlmacen, @NombreGabinete, @Formato, @Tamano ); SELECT LAST_INSERT_ID(); "; var id = await connection.ExecuteScalarAsync<long>(sql, model, transaction);

    if (id <= 0)
        throw new StorageTransactionException("No se obtuvo Id de inventario documental");

    _logger.LogInformation(
        "Inventario documental insertado idInventario={idInventario} idAlmacen={idAlmacen}",
        id,
        model.IdAlmacen
    );

    return id;
} } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN EN StorageTransactionCoordinator ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualizar StorageTransactionCoordinator para integrar: IGabineteStorageRepository IInventarioDocumentalRepository Dependencias adicionales: private readonly IGabineteStorageRepository _gabineteRepository; private readonly IInventarioDocumentalRepository _inventarioRepository; Flujo dentro de la misma transacción: Ejecutar IdentityAllocator. Construir GabineteInsertModel. Insertar gabinete. Construir InventarioInsertModel. Insertar inventario documental. Asignar IdRegistroProduccionDocumental. Commit al final. Ejemplo conceptual: var reservation = await _identityAllocator.ReserveAsync(context, connection, transaction); var gabineteModel = BuildGabineteInsertModel(context, reservation); await _gabineteRepository.InsertAsync(gabineteModel, connection, transaction); var inventarioModel = BuildInventarioInsertModel(context, reservation); var inventarioId = await _inventarioRepository.InsertAsync(inventarioModel, connection, transaction); transaction.Commit(); return new StorageTransactionResult { IdentityReservation = reservation, IdRegistroProduccionDocumental = inventarioId, Success = true, Estado = StorageDocumentState.Reserved, RequestId = context.RequestId, FechaEjecucion = DateTime.UtcNow, DuracionMs = stopwatch.ElapsedMilliseconds }; ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MÉTODOS BUILDER INTERNOS EN TransactionCoordinator ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Agregar métodos privados o extraer builder en fase posterior: private GabineteInsertModel BuildGabineteInsertModel( StorageContext context, StorageIdentityReservationResult reservation) Debe mapear: NombreGabinete IdAlmacen Disco Paginas TipoDocumento Carpeta Usuario Fecha CamposDinamicos private InventarioInsertModel BuildInventarioInsertModel( StorageContext context, StorageIdentityReservationResult reservation) Debe mapear: IdUsuarioGestion IdEmpresa Radicado FullText NumeroFolios IdAlmacen NombreGabinete Formato Tamano Reglas: No duplicar lógica compleja. Si crece, mover a builder dedicado en prompt posterior. No acceder a DB en builders. No abrir conexión en builders. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ NOTA DE PRECEDENCIA DAPPERCRUDENGINE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Cualquier bloque de SQL conceptual, INSERT INTO, SELECT, UPDATE o ejemplo con connection.ExecuteAsync / ExecuteScalarAsync incluido en este prompt es únicamente referencia funcional legacy. La implementación real debe hacerse exclusivamente con DapperCrudEngine + QueryOptions. Si existe contradicción entre un ejemplo SQL y la regla DapperCrudEngine, prevalece DapperCrudEngine. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DEPENDENCY INJECTION ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Registrar como Scoped: services.AddScoped<IGabineteStorageRepository, GabineteStorageRepository>(); services.AddScoped<IInventarioDocumentalRepository, InventarioDocumentalRepository>(); También debe mantenerse registrado: services.AddScoped<IStorageTransactionCoordinator, StorageTransactionCoordinator>(); No registrar como Singleton. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN SOLID ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SRP: GabineteStorageRepository: inserta únicamente en gabinete dinámico. InventarioDocumentalRepository: inserta únicamente en inventario documental. TransactionCoordinator: coordina transacción y orden de operaciones. IdentityAllocator: reserva identidad documental. OCP: nuevos repositorios transaccionales pueden agregarse sin modificar repositorios actuales. LSP: interfaces reemplazables por mocks. ISP: interfaces específicas por repositorio. DIP: TransactionCoordinator depende de abstracciones, no implementaciones. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DEUDA TÉCNICA CONTROLADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Documentar: confirmación de nombres reales de columnas de gabinete dinámico. confirmación de nombre legacy registro_producion_documental . límite real de FULTEXT_DOCUMENTO . si FullText debe ir completo, truncado o externo. si Formato y Tamano se calculan en fase posterior. si CamposDinamicos debe venir de metadata validada en BD. si se requiere whitelist por gabinete. si debe implementarse IStorageGabineteMetadataProvider . Clasificar deuda como: crítica media menor No dejar deuda crítica sin resolver. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Logs obligatorios: INFO: inicio insert gabinete fin insert gabinete inicio insert inventario fin insert inventario WARNING: FullText truncado CamposDinamicos vacío si aplica número alto de columnas dinámicas ERROR: error insert gabinete error insert inventario columnas inválidas nombre de gabinete inválido Campos mínimos: requestId si está disponible desde TransactionCoordinator alias usuarioId nombreGabinete idAlmacen idInventario cantidadColumnasDinamicas numeroFolios fase duraciónMs si aplica NO loguear: FullText contenido de documentos valores sensibles de campos SQL completo con valores rutas físicas Métricas recomendadas: storage_gabinete_insert_success_count storage_gabinete_insert_error_count storage_inventory_insert_success_count storage_inventory_insert_error_count storage_dynamic_columns_count storage_fulltext_truncated_count ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Unitarias — GabineteStorageRepository: model null → StorageTransactionException connection null → StorageTransactionException transaction null → StorageTransactionException connection cerrada → StorageTransactionException NombreGabinete vacío → error NombreGabinete con SQL injection → error IdAlmacen <= 0 → error Disco <= 0 → error Paginas <= 0 → error TipoDocumento <= 0 → error Carpeta <= 0 → error Usuario vacío → error Fecha default → error CamposDinamicos null → error columna dinámica vacía → error columna dinámica con SQL injection → error columna dinámica duplicada con base → error columna dinámica duplicada entre dinámicas → error insert devuelve rows != 1 → error insert válido → success Unitarias — InventarioDocumentalRepository: model null → error connection null → error transaction null → error connection cerrada → error IdUsuarioGestion <= 0 → error IdEmpresa <= 0 → error IdAlmacen <= 0 → error NumeroFolios <= 0 → error NombreGabinete vacío → error FullText > límite → truncado LAST_INSERT_ID <= 0 → error insert válido → retorna id Integración: insert real en tabla gabinete de prueba insert real en registro_producion_documental rollback elimina ambos inserts commit persiste ambos inserts transacción compartida por ambos repositories error en inventario provoca rollback de gabinete error en gabinete impide inventario columnas dinámicas válidas insertan correctamente columnas dinámicas inválidas bloqueadas Concurrencia: dos ejecuciones con TransactionCoordinator no colisionan IdAlmacen inserciones simultáneas respetan transacción fallo de una transacción no afecta otra QT / Calidad: validar logs sin datos sensibles validar que no se usa SELECT * validar que no hay SQL libre desde request validar que los nombres dinámicos están protegidos validar que no se abren conexiones en repositories validar que no se hace commit/rollback en repositories Regresión: no rompe Prompt 5 SCRUMCORE-166 no rompe Prompt 6 SCRUMCORE-167 no introduce filesystem no introduce XML no introduce expediente no introduce unidad de conservación Pruebas adicionales obligatorias por regla DapperCrudEngine: verificar que el repository invoca DapperCrudEngine y no IDbConnection directo. verificar que se construye QueryOptions con TableName correcto. verificar que se usan filtros parametrizados. verificar que LockMode = ForUpdate se usa cuando aplique. verificar que InsertValues/UpdateValues se usan cuando aplique. verificar que no existe ExecuteAsync/QueryAsync/ExecuteScalarAsync directo en el repository. verificar que no existe SQL concatenado. verificar que el contexto transaccional se propaga al DapperCrudEngine. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ruta: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Crear: SCRUM-[ID]-Arquitectura-Gabinete-Inventario.md Debe incluir: objetivo arquitectónico flujo TransactionCoordinator → GabineteRepository → InventarioRepository diagrama de clases diagrama de secuencia diagrama de estados: identity_reserved gabinete_inserted inventory_inserted rolled_back error relación con Prompt 5   SCRUMCORE-166 relación con Prompt 6   SCRUMCORE-167 relación con Prompt 8   SCRUMCORE-169 reglas de SQL dinámico seguro por qué los repositories no abren conexión por qué los repositories no hacen commit/rollback validación SOLID riesgos técnicos SCRUM-[ID]-Implementacion-Detallada-Gabinete-Inventario.md Debe incluir: archivos creados rutas exactas interfaces creadas clases creadas modelos creados método InsertAsync de gabinete método InsertAsync de inventario validaciones del modelo validación de nombre de gabinete validación de columnas dinámicas manejo de duplicados SQL generado conceptualmente parámetros usados integración con TransactionCoordinator DI agregado decisiones técnicas limitaciones actuales SCRUM-[ID]-Pruebas-Gabinete-Inventario.md Debe incluir: matriz de pruebas unitarias matriz de integración matriz de concurrencia matriz QT pruebas de SQL injection pruebas de rollback pruebas de commit pruebas de FullText truncado casos cubiertos casos no cubiertos riesgos residuales SCRUM-[ID]-Observabilidad-Gabinete-Inventario.md Debe incluir: logs agregados niveles de log campos capturados métricas recomendadas errores comunes troubleshooting: nombre de gabinete inválido columna inválida columna duplicada fallo de insert gabinete fallo de insert inventario rollback por error posterior SCRUM-[ID]-Seguridad-SQL-Dinamico.md Debe incluir: threat model básico riesgo de SQL injection validación regex usada ejemplos bloqueados ejemplos permitidos justificación de DynamicParameters restricciones de nombres deuda técnica sobre whitelist por metadata SCRUM-[ID]-Metadata.md Debe incluir: identificador del ticket usuario que creó el ticket fecha módulo relación con Prompt 2 SCRUMCORE-163 relación con Prompt 5 SCRUMCORE-166 relación con Prompt 6 SCRUMCORE-167 relación con Prompt 8 SCRUMCORE-169 estado del ticket Documentar adicionalmente por regla DapperCrudEngine: método del DapperCrudEngine usado. QueryOptions construido. filtros aplicados. columnas explícitas. LockMode / InsertValues / UpdateValues usados. cómo se propaga el contexto transaccional. extensiones agregadas al DapperCrudEngine si fueron necesarias. evidencia de que no se usa Dapper directo ni SQL manual. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENTREGABLES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Código: GabineteInsertModel.cs InventarioInsertModel.cs IGabineteStorageRepository.cs GabineteStorageRepository.cs IInventarioDocumentalRepository.cs InventarioDocumentalRepository.cs actualización de StorageTransactionCoordinator métodos builders internos o builder documentado registro DI tests unitarios tests integración tests concurrencia tests QT/calidad Documentación: SCRUM-[ID]-Arquitectura-Gabinete-Inventario.md SCRUM-[ID]-Implementacion-Detallada-Gabinete-Inventario.md SCRUM-[ID]-Pruebas-Gabinete-Inventario.md SCRUM-[ID]-Observabilidad-Gabinete-Inventario.md SCRUM-[ID]-Seguridad-SQL-Dinamico.md SCRUM-[ID]-Metadata.md ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Existe IGabineteStorageRepository. Existe GabineteStorageRepository. Existe IInventarioDocumentalRepository. Existe InventarioDocumentalRepository. Existen GabineteInsertModel e InventarioInsertModel. GabineteRepository valida nombre de tabla. GabineteRepository valida columnas dinámicas. GabineteRepository bloquea SQL injection. GabineteRepository bloquea columnas duplicadas. InventarioRepository valida modelo. InventarioRepository controla FullText grande. Ambos usan conexión recibida. Ambos usan transacción recibida. Ninguno abre conexión. Ninguno hace commit. Ninguno hace rollback. TransactionCoordinator integra ambos repositories. Si inventario falla, gabinete se revierte por rollback. Si gabinete falla, inventario no se ejecuta. StorageTransactionResult incluye IdRegistroProduccionDocumental. Hay logs estructurados. Hay pruebas unitarias completas. Hay pruebas de integración. Hay pruebas de rollback/commit. Hay pruebas QT/calidad. Hay documentación enterprise completa. Queda listo para Prompt 8 — Expediente + Unidad de Conservación + Índice Electrónico. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ NO filesystem. NO XML. NO CompensationManager. NO expediente. NO unidad de conservación. NO índice electrónico. NO log workflow. NO abrir conexiones en repositories. NO abrir transacciones en repositories. NO commit en repositories. NO rollback en repositories. NO SQL libre desde request. NO columnas sin validar. NO tabla sin validar. NO SELECT *. NO loguear FullText. NO loguear valores sensibles. NO cambiar contratos del Prompt 2  SCRUMCORE-163sin justificación. NO usar Dapper directo desde repositories. NO usar IDbConnection.ExecuteAsync / QueryAsync / ExecuteScalarAsync desde repositories. NO omitir DapperCrudEngine. NO implementar SQL manual si puede modelarse con QueryOptions. NO usar SQL directo para FOR UPDATE; usar LockMode o extensión centralizada del DapperCrudEngine. NO usar SQL directo para INSERT/UPDATE; usar InsertValues/UpdateValues o extensión centralizada del DapperCrudEngine. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar la persistencia principal del Storage Engine con: modelos interfaces repositories integración en TransactionCoordinator validaciones seguridad de acceso a datos con DapperCrudEngine dinámico DI pruebas documentación Debe quedar alineado con: Prompt 2 SCRUMCORE-163 Prompt 5 SCRUMCORE-166 Prompt 6 SCRUMCORE-167

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-168-crea-funcion-insertcion-gabinete-almacen.