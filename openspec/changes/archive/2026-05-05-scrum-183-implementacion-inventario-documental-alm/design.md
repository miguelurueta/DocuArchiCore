## Context

- Jira issue key: SCRUM-183
- Jira summary: IMPLEMENTACION-INVENTARIO-DOCUMENTAL-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-183

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 17 — Inventario Documental Completo Legacy-Compatible (FASE CRÍTICA — PARIDAD FUNCIONAL LEGACY) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy inventario documental producción documental TRD expedientes y unidades de conservación Dapper / SQL transaccional Clean Architecture migración VB → C# con paridad funcional sistemas ECM críticos integridad archivística ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Restaurar la paridad funcional completa de la inserción en: registro_producion_documental La implementación actual en C# inserta menos columnas que VB y en algunos casos intenta insertar inventario aunque la opción legacy no aplica. Este prompt debe corregir: ✔ Inserción condicionada por opción real de inventario ✔ Mapeo completo legacy-compatible ✔ Uso de metadata física real ✔ Uso de TRD si aplica ✔ Uso de expediente/unidad si aplica ✔ Uso de segundo nombre documental legacy ✔ Uso de tamaño/formato reales ✔ Uso de FullText consolidado ✔ Retorno correcto de IdRegistroProduccionDocumental ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO LEGAZY VB D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\Architecture\Almacenamiento\funcion-almacena-consolidad.txt CONTEXTO TABLAS D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\DataModel\StorageEngine-Tables.md BRECHA FUNCIONAL DETECTADA Comparativo actual: VB legacy:
- Inserta registro_producion_documental solo si opcion_inventario = 1
- Persiste muchas columnas archivísticas
- Usa id_usuario_gestion
- Usa id_empresa
- Usa TRD
- Usa expediente/unidad
- Usa clase documento
- Usa fecha elaboración
- Usa radicado
- Usa segundo nombre documento
- Usa tamaño y formato
- Retorna id_registro_producion

C# actual:
- Intenta inventario siempre si falta inventario falla
- Inserta menos columnas
- No usa todas las columnas legacy
- Tamano/Formato venían vacíos antes del Prompt 16
- SegundoNombre puede quedar vacío Esta brecha afecta: - consulta documental
- auditoría
- expediente electrónico
- índice
- trazabilidad
- visor documental
- reportes archivísticos ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REFERENCIA LEGACY Fragmento conceptual VB: insert into registro_producion_documental
(
    remit_dest_interno_idremit_dest_interno,
    ID_USUARIO_GESTION,
    FECHA_DOCUMENTO,
    ID_AREA_DEPARTAMENTO,
    ID_SERIE_DOCUMENTO,
    SERIE_DOCUMENTO,
    ID_SUBSERIE_DOCUMENTO,
    SUBSERIE_DOCUMENTO,
    ID_TIPO_DOCUMENTO,
    DESCRIPCION_TIPO_DOCUMENTO,
    FULTEXT_DOCUMENTO,
    ID_DOCUMENTO_DOCUARCHI_ALMACEN,
    ESTADO_DOCUMENTO_ARCHIVO,
    NOMBRE_GABINETE,
    NUMERO_FOLIOS,
    EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,
    EXPEDIENTE,
    ID_TIPO_EXPEDIENTE,
    ID_TIPO_UNIDAD_CONSERVACION,
    ID_UNIDAD_CONSERVACION,
    ID_CLASE_DOCUMENTO,
    CLASEDOCUMENTO,
    FECHA_ELABORACION,
    UNIDADCONSERVA,
    NOMBRE_AREA_DEPARTAMENTO,
    ID_TIPO_UNIDAD_DOCUMENTAL,
    ID_EMPRESA_DOCUMENTO,
    RADICADO_DOCUMENTO,
    SEGUNDO_NOMBRE_DOCUMENTO,
    DOCUMENTO_PRODUCION_DOCUMENTAL,
    TAMANO,
    FORMATO
) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA Repository MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/Inventario/ Archivos esperados: IInventarioDocumentalRepository.cs
InventarioDocumentalRepository.cs Services / Builders MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Inventario/ Archivos esperados: IInventarioDocumentalBuilder.cs
InventarioDocumentalBuilder.cs Models MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Inventario/ Archivos esperados: InventarioDocumentalInsertModel.cs
InventarioDocumentalBuildResult.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS InventarioDocumentalInsertModel public sealed class InventarioDocumentalInsertModel
{
    public int? RemitDestInternoId { get; init; }

    public int IdUsuarioGestion { get; init; }
    public DateTime FechaDocumento { get; init; }

    public int? IdAreaDepartamento { get; init; }
    public int? IdSerieDocumento { get; init; }
    public string? SerieDocumento { get; init; }

    public int? IdSubserieDocumento { get; init; }
    public string? SubserieDocumento { get; init; }

    public int? IdTipoDocumento { get; init; }
    public string? DescripcionTipoDocumento { get; init; }

    public string? FullTextDocumento { get; init; }

    public long IdDocumentoDocuarchiAlmacen { get; init; }

    public int EstadoDocumentoArchivo { get; init; }

    public string NombreGabinete { get; init; } = "";

    public int NumeroFolios { get; init; }

    public int? IdExpediente { get; init; }
    public string? Expediente { get; init; }
    public int? IdTipoExpediente { get; init; }

    public int? IdTipoUnidadConservacion { get; init; }
    public int? IdUnidadConservacion { get; init; }

    public int? IdClaseDocumento { get; init; }
    public string? ClaseDocumento { get; init; }

    public DateTime? FechaElaboracion { get; init; }

    public string? UnidadConserva { get; init; }

    public string? NombreAreaDepartamento { get; init; }

    public int? IdTipoUnidadDocumental { get; init; }

    public int IdEmpresaDocumento { get; init; }

    public string? RadicadoDocumento { get; init; }

    public string? SegundoNombreDocumento { get; init; }

    public int DocumentoProduccionDocumental { get; init; }

    public string Tamano { get; init; } = "";

    public string Formato { get; init; } = "";
} InventarioDocumentalBuildResult public sealed class InventarioDocumentalBuildResult
{
    public bool ShouldInsert { get; init; }

    public InventarioDocumentalInsertModel? Model { get; init; }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACES IInventarioDocumentalBuilder public interface IInventarioDocumentalBuilder
{
    InventarioDocumentalBuildResult Build(
        StorageContext context,
        StorageIdentityModel identity,
        StorageNamingResult naming
    );
} IInventarioDocumentalRepository public interface IInventarioDocumentalRepository
{
    Task<long> InsertAsync(
        InventarioDocumentalInsertModel model,
        IDbConnection connection,
        IDbTransaction transaction
    );
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — InventarioDocumentalBuilder Responsabilidad Construir el modelo completo de inventario usando: StorageContext
ResolvedOptions
PhysicalMetadata
TRD
Expediente
Naming
Identity
FullText Reglas Si AplicaInventarioDocumental = false , no construir modelo. Si aplica inventario, validar Inventario . Validar IdUsuarioGestion > 0 . Validar IdEmpresa > 0 . Usar PhysicalMetadata.NumeroPaginas . Usar PhysicalMetadata.TamanoLegacy . Usar PhysicalMetadata.Formato . Usar naming.SegundoNombre . Usar FullText consolidado desde campos/preindex. Determinar EstadoDocumentoArchivo . Determinar IdTipoUnidadDocumental . Implementación esperada public sealed class InventarioDocumentalBuilder : IInventarioDocumentalBuilder
{
    public InventarioDocumentalBuildResult Build(
        StorageContext context,
        StorageIdentityModel identity,
        StorageNamingResult naming)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (identity == null)
            throw new ArgumentNullException(nameof(identity));

        if (naming == null)
            throw new ArgumentNullException(nameof(naming));

        var options = context.ResolvedOptions;

        if (options == null || !options.AplicaInventarioDocumental)
        {
            return new InventarioDocumentalBuildResult
            {
                ShouldInsert = false,
                Model = null
            };
        }

        var command = context.Command;
        var inventario = command.Inventario;

        if (inventario == null)
            throw new InvalidOperationException("Inventario documental requerido");

        if (inventario.IdUsuarioGestion <= 0)
            throw new InvalidOperationException("IdUsuarioGestion inválido");

        if (inventario.IdEmpresa <= 0)
            throw new InvalidOperationException("IdEmpresa inválido");

        var physical = context.PhysicalMetadata
            ?? throw new InvalidOperationException("Metadata física requerida para inventario");

        var trd = command.Trd;
        var expediente = command.Expediente;

        var tieneExpediente = expediente?.IdExpediente.HasValue == true && expediente.IdExpediente > 0;
        var tieneUnidad = expediente?.IdUnidadConservacion.HasValue == true && expediente.IdUnidadConservacion > 0;

        var estadoArchivo = tieneExpediente || tieneUnidad ? 0 : 1;

        int? idTipoUnidadDocumental = null;

        if (tieneUnidad)
            idTipoUnidadDocumental = 1;

        if (tieneExpediente)
            idTipoUnidadDocumental = 2;

        var fullText = BuildFullText(command.Campos);

        var model = new InventarioDocumentalInsertModel
        {
            RemitDestInternoId = inventario.IdUsuarioGestion, // paridad con VB si aplica

            IdUsuarioGestion = inventario.IdUsuarioGestion,
            FechaDocumento = context.FechaEjecucion,

            IdAreaDepartamento = trd?.IdArea,
            IdSerieDocumento = trd?.IdSerie,
            SerieDocumento = trd?.NombreSerie,

            IdSubserieDocumento = trd?.IdSubSerie,
            SubserieDocumento = trd?.NombreSubSerie,

            IdTipoDocumento = trd?.IdTipoDocumento,
            DescripcionTipoDocumento = trd?.NombreTipoDocumento,

            FullTextDocumento = fullText,

            IdDocumentoDocuarchiAlmacen = identity.IdAlmacen,

            EstadoDocumentoArchivo = estadoArchivo,

            NombreGabinete = command.NombreGabinete,

            NumeroFolios = physical.NumeroPaginas,

            IdExpediente = expediente?.IdExpediente,
            Expediente = expediente?.NombreExpediente,
            IdTipoExpediente = expediente?.IdTipoExpediente,

            IdTipoUnidadConservacion = expediente?.IdTipoUnidadConservacion,
            IdUnidadConservacion = expediente?.IdUnidadConservacion,

            IdClaseDocumento = expediente?.IdClaseDocumento,
            ClaseDocumento = expediente?.ClaseDocumento,

            FechaElaboracion = TryParseDate(inventario.FechaElaboracion),

            UnidadConserva = expediente?.NombreUnidadConservacion,

            NombreAreaDepartamento = trd?.NombreArea,

            IdTipoUnidadDocumental = idTipoUnidadDocumental,

            IdEmpresaDocumento = inventario.IdEmpresa,

            RadicadoDocumento = inventario.Radicado,

            SegundoNombreDocumento = naming.SegundoNombre,

            DocumentoProduccionDocumental = 0,

            Tamano = physical.TamanoLegacy,

            Formato = physical.Formato
        };

        return new InventarioDocumentalBuildResult
        {
            ShouldInsert = true,
            Model = model
        };
    }

    private static string BuildFullText(IReadOnlyList<CampoIndexacionDto> campos)
    {
        if (campos == null || campos.Count == 0)
            return "";

        return string.Join(
            Environment.NewLine,
            campos
                .Where(c => !string.IsNullOrWhiteSpace(c.Valor))
                .Select(c => c.Valor!.Trim())
        );
    }

    private static DateTime? TryParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (DateTime.TryParse(value, out var result))
            return result;

        return null;
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — InventarioDocumentalRepository Reglas Debe usar SQL parametrizado. No debe abrir conexión. No debe crear transacción. Debe retornar LAST_INSERT_ID() . Debe insertar todas las columnas legacy necesarias. Debe validar modelo antes de ejecutar. Implementación esperada public sealed class InventarioDocumentalRepository : IInventarioDocumentalRepository
{
    private readonly ILogger<InventarioDocumentalRepository> _logger;

    public InventarioDocumentalRepository(
        ILogger<InventarioDocumentalRepository> logger)
    {
        _logger = logger;
    }

    public async Task<long> InsertAsync(
        InventarioDocumentalInsertModel model,
        IDbConnection connection,
        IDbTransaction transaction)
    {
        Validate(model);

        const string sql = @"
INSERT INTO registro_producion_documental
(
    remit_dest_interno_idremit_dest_interno,
    ID_USUARIO_GESTION,
    FECHA_DOCUMENTO,
    ID_AREA_DEPARTAMENTO,
    ID_SERIE_DOCUMENTO,
    SERIE_DOCUMENTO,
    ID_SUBSERIE_DOCUMENTO,
    SUBSERIE_DOCUMENTO,
    ID_TIPO_DOCUMENTO,
    DESCRIPCION_TIPO_DOCUMENTO,
    FULTEXT_DOCUMENTO,
    ID_DOCUMENTO_DOCUARCHI_ALMACEN,
    ESTADO_DOCUMENTO_ARCHIVO,
    NOMBRE_GABINETE,
    NUMERO_FOLIOS,
    EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,
    EXPEDIENTE,
    ID_TIPO_EXPEDIENTE,
    ID_TIPO_UNIDAD_CONSERVACION,
    ID_UNIDAD_CONSERVACION,
    ID_CLASE_DOCUMENTO,
    CLASEDOCUMENTO,
    FECHA_ELABORACION,
    UNIDADCONSERVA,
    NOMBRE_AREA_DEPARTAMENTO,
    ID_TIPO_UNIDAD_DOCUMENTAL,
    ID_EMPRESA_DOCUMENTO,
    RADICADO_DOCUMENTO,
    SEGUNDO_NOMBRE_DOCUMENTO,
    DOCUMENTO_PRODUCION_DOCUMENTAL,
    TAMANO,
    FORMATO
)
VALUES
(
    @RemitDestInternoId,
    @IdUsuarioGestion,
    @FechaDocumento,
    @IdAreaDepartamento,
    @IdSerieDocumento,
    @SerieDocumento,
    @IdSubserieDocumento,
    @SubserieDocumento,
    @IdTipoDocumento,
    @DescripcionTipoDocumento,
    @FullTextDocumento,
    @IdDocumentoDocuarchiAlmacen,
    @EstadoDocumentoArchivo,
    @NombreGabinete,
    @NumeroFolios,
    @IdExpediente,
    @Expediente,
    @IdTipoExpediente,
    @IdTipoUnidadConservacion,
    @IdUnidadConservacion,
    @IdClaseDocumento,
    @ClaseDocumento,
    @FechaElaboracion,
    @UnidadConserva,
    @NombreAreaDepartamento,
    @IdTipoUnidadDocumental,
    @IdEmpresaDocumento,
    @RadicadoDocumento,
    @SegundoNombreDocumento,
    @DocumentoProduccionDocumental,
    @Tamano,
    @Formato
);
SELECT LAST_INSERT_ID();
";

        var id = await connection.ExecuteScalarAsync<long>(
            sql,
            model,
            transaction
        );

        if (id <= 0)
            throw new InvalidOperationException("No se pudo insertar inventario documental");

        _logger.LogInformation(
            "Inventario documental insertado idRegistroProduccion={id} idAlmacen={idAlmacen} gabinete={gabinete}",
            id,
            model.IdDocumentoDocuarchiAlmacen,
            model.NombreGabinete
        );

        return id;
    }

    private static void Validate(InventarioDocumentalInsertModel model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (model.IdUsuarioGestion <= 0)
            throw new InvalidOperationException("IdUsuarioGestion inválido");

        if (model.IdEmpresaDocumento <= 0)
            throw new InvalidOperationException("IdEmpresaDocumento inválido");

        if (model.IdDocumentoDocuarchiAlmacen <= 0)
            throw new InvalidOperationException("IdDocumentoDocuarchiAlmacen inválido");

        if (string.IsNullOrWhiteSpace(model.NombreGabinete))
            throw new InvalidOperationException("NombreGabinete requerido");

        if (model.NumeroFolios <= 0)
            throw new InvalidOperationException("NumeroFolios inválido");

        if (string.IsNullOrWhiteSpace(model.Tamano))
            throw new InvalidOperationException("Tamano requerido");

        if (string.IsNullOrWhiteSpace(model.Formato))
            throw new InvalidOperationException("Formato requerido");
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN EN TRANSACTION COORDINATOR Actualizar flujo: var inventarioBuild = _inventarioBuilder.Build(
    context,
    reservation.Identity,
    naming
);

long? idRegistroProduccion = null;

if (inventarioBuild.ShouldInsert && inventarioBuild.Model != null)
{
    idRegistroProduccion = await _inventarioRepository.InsertAsync(
        inventarioBuild.Model,
        connection,
        transaction
    );
} Actualizar resultado: IdRegistroProduccionDocumental = idRegistroProduccion ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN CON ÍNDICE ELECTRÓNICO Si se inserta índice electrónico, debe usar: IdRegistroProduccionDocumental Regla: No crear índice electrónico si no existe IdRegistroProduccionDocumental. Si el expediente electrónico requiere índice, pero inventario no aplica, documentar decisión funcional: - o se bloquea
- o se crea índice sin inventario si legacy lo permitía No asumir. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS Inventario solo si ResolvedOptions.AplicaInventarioDocumental = true . No fallar por inventario faltante si la opción está apagada. Fallar si la opción está encendida y falta inventario. Insertar columnas legacy necesarias. Usar PhysicalMetadata . Usar Naming . Usar TRD si aplica. Usar Expediente/Unidad si aplica. Retornar IdRegistroProduccionDocumental . No hardcodear valores sin justificar. No usar SQL concatenado. Todo dentro de la transacción DB. No acceder a filesystem aquí. No generar XML aquí. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS Unitarias — InventarioDocumentalBuilder options null → no inserta inventario apagado → no inserta inventario activo sin dto → error inventario activo sin usuario gestión → error inventario activo sin empresa → error sin PhysicalMetadata → error con TRD → mapea campos TRD con expediente → estado documento archivo = 0 sin expediente/unidad → estado documento archivo = 1 con unidad → IdTipoUnidadDocumental = 1 con expediente → IdTipoUnidadDocumental = 2 FullText desde campos SegundoNombre desde Naming Tamano/Formato desde PhysicalMetadata Unitarias — InventarioDocumentalRepository modelo null → error usuario gestión inválido → error empresa inválida → error idAlmacen inválido → error folios inválidos → error tamaño vacío → error formato vacío → error insert OK retorna id LAST_INSERT_ID <= 0 → error Integración insert real en registro_producion_documental columnas TRD persistidas columnas expediente/unidad persistidas tamaño/formato persistidos segundo nombre persistido rollback elimina registro commit conserva registro Regresión Legacy Comparar contra VB: ID_USUARIO_GESTION
FECHA_DOCUMENTO
ID_AREA_DEPARTAMENTO
ID_SERIE_DOCUMENTO
SERIE_DOCUMENTO
ID_SUBSERIE_DOCUMENTO
SUBSERIE_DOCUMENTO
ID_TIPO_DOCUMENTO
DESCRIPCION_TIPO_DOCUMENTO
FULTEXT_DOCUMENTO
ID_DOCUMENTO_DOCUARCHI_ALMACEN
ESTADO_DOCUMENTO_ARCHIVO
NOMBRE_GABINETE
NUMERO_FOLIOS
EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE
EXPEDIENTE
ID_TIPO_EXPEDIENTE
ID_TIPO_UNIDAD_CONSERVACION
ID_UNIDAD_CONSERVACION
ID_CLASE_DOCUMENTO
CLASEDOCUMENTO
FECHA_ELABORACION
UNIDADCONSERVA
NOMBRE_AREA_DEPARTAMENTO
ID_TIPO_UNIDAD_DOCUMENTAL
ID_EMPRESA_DOCUMENTO
RADICADO_DOCUMENTO
SEGUNDO_NOMBRE_DOCUMENTO
DOCUMENTO_PRODUCION_DOCUMENTAL
TAMANO
FORMATO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD Logs obligatorios: requestId
idAlmacen
idRegistroProduccionDocumental
nombreGabinete
aplicaInventario
numeroFolios
formato
tamano
estado
duración No loguear: FullText completo
contenido documental
rutas físicas completas
datos sensibles ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN ENTERPRISE Ruta: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Crear: SCRUM-[ID]-Arquitectura-InventarioDocumental.md
SCRUM-[ID]-Implementacion-Detallada-InventarioDocumental.md
SCRUM-[ID]-Pruebas-InventarioDocumental.md
SCRUM-[ID]-Observabilidad-InventarioDocumental.md
SCRUM-[ID]-Regresion-Legacy-InventarioDocumental.md
SCRUM-[ID]-Metadata.md Debe incluir: comparación VB vs C# tabla de equivalencia de columnas reglas de activación por opciones system1 dependencia de PhysicalMetadata dependencia de Naming dependencia de TRD dependencia de Expediente/Unidad impacto en índice electrónico decisiones funcionales pendientes si inventario no aplica ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN Inventario se inserta solo si aplica. No falla si inventario no aplica. Falla si inventario aplica y faltan datos obligatorios. Inserta columnas legacy necesarias. Usa tamaño real. Usa formato real. Usa número de folios real. Usa segundo nombre legacy. Usa TRD si aplica. Usa expediente/unidad si aplica. Retorna IdRegistroProduccionDocumental . TransactionCoordinator propaga el id. Índice electrónico puede consumir el id. Hay pruebas unitarias. Hay pruebas de integración. Hay regresión comparativa con VB. Hay documentación enterprise. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES No implementar XML aquí. No implementar FileSystem aquí. No implementar Naming aquí. No implementar PhysicalMetadata aquí. No implementar opciones system1 aquí. No abrir conexión aquí. No crear transacción aquí. No modificar función legacy. No loguear FullText completo. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL Implementar: InventarioDocumentalInsertModel
InventarioDocumentalBuildResult
IInventarioDocumentalBuilder
InventarioDocumentalBuilder
IInventarioDocumentalRepository
InventarioDocumentalRepository
Integración TransactionCoordinator
DI
Pruebas
Documentación

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-183-implementacion-inventario-documental-alm.