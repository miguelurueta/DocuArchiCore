## Context

- Jira issue key: SCRUM-184
- Jira summary: IMPLEMENTACION-EXPEDIENTE-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-184

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 18 — Expediente / Unidad Legacy-Compatible (FASE CRÍTICA — PARIDAD ARCHIVÍSTICA VB → C#) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy gestión de expedientes unidades de conservación TRD acumulación de folios electrónicos/digitalizados reglas archivísticas Dapper / SQL transaccional SELECT ... FOR UPDATE Clean Architecture migración VB → C# con paridad funcional consistencia documental ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Restaurar la paridad funcional del bloque legacy relacionado con: Expediente
Unidad de conservación
Clase documental
Tipo de unidad documental
Folios electrónicos
Folios digitalizados La implementación nueva debe corregir las brechas detectadas: - C# solo ejecuta fase si vienen ambos IDs.
- VB trataba expediente y unidad como casos separados.
- C# actualiza un campo diferente en unidad.
- VB distinguía NUMERO_ELECTRONICO_CONTENIDO y NUMERO_DIGITALIZADO_CONTENIDO.
- Estado expediente electrónico tiene semántica distinta. Este prompt NO implementa todavía el XML del índice de expediente. Ese será cubierto por: PROMPT 19 — XML índice expediente legacy-compatible ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ BRECHA FUNCIONAL DETECTADA Comparativo actual: VB legacy:
- Puede operar solo con expediente.
- Puede operar solo con unidad de conservación.
- Valida clase documental si existe expediente o unidad.
- Si expediente existe:
  - valida estado expediente
  - lee estado_expediente_electronico
  - actualiza folios de expediente
- Si unidad existe:
  - actualiza folios según unidad_conserva_tipo:
    - DIGITALIZADO → NUMERO_DIGITALIZADO_CONTENIDO
    - ELECTRONICO → NUMERO_ELECTRONICO_CONTENIDO
- Determina ID_TIPO_UNIDAD_DOCUMENTAL:
  - 1 = unidad conservación
  - 2 = expediente

C# actual:
- Solo ejecuta si vienen expediente y unidad.
- No respeta casos separados.
- Actualiza NUMERO_FOLIO_UNIDAD_CONSERVACION.
- No distingue electrónico/digitalizado. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO LEGAZY VB D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\Architecture\Almacenamiento\funcion-almacena-consolidad.txt CONTEXTO TABLAS D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\DataModel\StorageEngine-Tables.md REFERENCIA LEGACY Reglas VB relevantes: If id_expediente <> 0 Then
    If id_clase_documento = 0 Then
        "Por favor seleccione la clase de documento si quiere asignar el expediente"
    End If

    SolicitaDatosEstructuraExpediente(...)
    If ESTADO_EXPEDIENTE <> 1 Then
        "El expediente no esta disponible..."
    End If

    estado_expediente_electronico = ...
End If If id_unidad_conservacion <> 0 Then
    If id_clase_documento = 0 Then
        "Por favor seleccione la clase de documento si quiere asignar la unidad de conservación"
    End If
End If If id_expediente <> 0 Then
    id_tipo_unidad_documental = 2
End If

If id_unidad_conservacion <> 0 Then
    id_tipo_unidad_documental = 1
End If If unidad_conserva_tipo = "DIGITALIZADO" Then
    NUMERO_DIGITALIZADO_CONTENIDO += pagi
End If

If unidad_conserva_tipo = "ELECTRONICO" Then
    NUMERO_ELECTRONICO_CONTENIDO += pagi
End If ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA Services / Expediente MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Expediente/ Archivos esperados: IExpedienteUnidadLegacyService.cs
ExpedienteUnidadLegacyService.cs
IExpedienteUnidadLegacyBuilder.cs
ExpedienteUnidadLegacyBuilder.cs Repository MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/Expediente/ Archivos esperados: IExpedienteLegacyRepository.cs
ExpedienteLegacyRepository.cs
IUnidadConservacionLegacyRepository.cs
UnidadConservacionLegacyRepository.cs
IClaseDocumentoLegacyRepository.cs
ClaseDocumentoLegacyRepository.cs Models MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Expediente/ Archivos esperados: ExpedienteLegacyInfoModel.cs
UnidadConservacionLegacyInfoModel.cs
ExpedienteUnidadLegacyPlan.cs
ExpedienteUnidadLegacyResult.cs
UnidadConservaTipoEnum.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS UnidadConservaTipoEnum public enum UnidadConservaTipoEnum
{
    Ninguna = 0,
    Digitalizado = 1,
    Electronico = 2
} ExpedienteLegacyInfoModel public sealed class ExpedienteLegacyInfoModel
{
    public int IdExpediente { get; init; }

    public int EstadoExpediente { get; init; }

    public int EstadoExpedienteElectronico { get; init; }

    public int NumeroDigitalizadoContenido { get; init; }

    public int NumeroElectronicoContenido { get; init; }

    public int OrdenIndice { get; init; }

    public int UltimaPaginaIndice { get; init; }
} UnidadConservacionLegacyInfoModel public sealed class UnidadConservacionLegacyInfoModel
{
    public int IdUnidadConservacion { get; init; }

    public int NumeroDigitalizadoContenido { get; init; }

    public int NumeroElectronicoContenido { get; init; }
} ExpedienteUnidadLegacyPlan public sealed class ExpedienteUnidadLegacyPlan
{
    public bool TieneExpediente { get; init; }

    public bool TieneUnidadConservacion { get; init; }

    public int? IdExpediente { get; init; }

    public int? IdUnidadConservacion { get; init; }

    public int? IdClaseDocumento { get; init; }

    public int NumeroFolios { get; init; }

    public int? IdTipoUnidadDocumental { get; init; }

    public UnidadConservaTipoEnum UnidadConservaTipo { get; init; }
} ExpedienteUnidadLegacyResult public sealed class ExpedienteUnidadLegacyResult
{
    public bool Ejecutado { get; init; }

    public bool TieneExpediente { get; init; }

    public bool TieneUnidadConservacion { get; init; }

    public int? IdTipoUnidadDocumental { get; init; }

    public int? EstadoExpedienteElectronico { get; init; }

    public int NumeroFolios { get; init; }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACES IExpedienteUnidadLegacyBuilder public interface IExpedienteUnidadLegacyBuilder
{
    ExpedienteUnidadLegacyPlan Build(StorageContext context);
} IExpedienteUnidadLegacyService public interface IExpedienteUnidadLegacyService
{
    Task<ExpedienteUnidadLegacyResult> ExecuteAsync(
        StorageContext context,
        IDbConnection connection,
        IDbTransaction transaction
    );
} IExpedienteLegacyRepository public interface IExpedienteLegacyRepository
{
    Task<ExpedienteLegacyInfoModel?> LockByIdAsync(
        int idExpediente,
        IDbConnection connection,
        IDbTransaction transaction
    );

    Task<int> UpdateFoliosElectronicosAsync(
        int idExpediente,
        int nuevoTotal,
        IDbConnection connection,
        IDbTransaction transaction
    );

    Task<int> UpdateFoliosDigitalizadosAsync(
        int idExpediente,
        int nuevoTotal,
        IDbConnection connection,
        IDbTransaction transaction
    );
} IUnidadConservacionLegacyRepository public interface IUnidadConservacionLegacyRepository
{
    Task<UnidadConservacionLegacyInfoModel?> LockByIdAsync(
        int idUnidadConservacion,
        IDbConnection connection,
        IDbTransaction transaction
    );

    Task<int> UpdateFoliosElectronicosAsync(
        int idUnidadConservacion,
        int nuevoTotal,
        IDbConnection connection,
        IDbTransaction transaction
    );

    Task<int> UpdateFoliosDigitalizadosAsync(
        int idUnidadConservacion,
        int nuevoTotal,
        IDbConnection connection,
        IDbTransaction transaction
    );
} IClaseDocumentoLegacyRepository public interface IClaseDocumentoLegacyRepository
{
    Task<UnidadConservaTipoEnum> GetUnidadConservaTipoAsync(
        int idClaseDocumento,
        string defaultDbAlias
    );
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ BUILDER — ExpedienteUnidadLegacyBuilder Responsabilidad Construir el plan de ejecución basado en: StorageContext
ResolvedOptions
ExpedienteStorageDto
PhysicalMetadata
IdClaseDocumento Reglas Si AplicaUnidadConservacion = false , no ejecutar. Si no hay expediente ni unidad, no ejecutar. Si hay expediente o unidad, IdClaseDocumento es obligatorio. Si hay expediente → IdTipoUnidadDocumental = 2 . Si hay unidad → IdTipoUnidadDocumental = 1 . Si vienen ambos expediente y unidad, debe generar error o aplicar regla explícita. NumeroFolios debe venir de PhysicalMetadata.NumeroPaginas . Implementación esperada public sealed class ExpedienteUnidadLegacyBuilder : IExpedienteUnidadLegacyBuilder
{
    public ExpedienteUnidadLegacyPlan Build(StorageContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (context.ResolvedOptions?.AplicaUnidadConservacion != true)
        {
            return new ExpedienteUnidadLegacyPlan
            {
                TieneExpediente = false,
                TieneUnidadConservacion = false,
                NumeroFolios = 0,
                UnidadConservaTipo = UnidadConservaTipoEnum.Ninguna
            };
        }

        var expediente = context.Command.Expediente;

        if (expediente == null)
            throw new InvalidOperationException("Expediente o unidad de conservación requerida");

        var tieneExpediente = expediente.IdExpediente.HasValue && expediente.IdExpediente > 0;
        var tieneUnidad = expediente.IdUnidadConservacion.HasValue && expediente.IdUnidadConservacion > 0;

        if (!tieneExpediente && !tieneUnidad)
            throw new InvalidOperationException("Debe seleccionar expediente o unidad de conservación");

        if (tieneExpediente && tieneUnidad)
            throw new InvalidOperationException("Ambigüedad: no se permite expediente y unidad simultáneamente");

        if (!expediente.IdClaseDocumento.HasValue || expediente.IdClaseDocumento <= 0)
            throw new InvalidOperationException("Clase de documento requerida");

        var physical = context.PhysicalMetadata
            ?? throw new InvalidOperationException("Metadata física requerida");

        if (physical.NumeroPaginas <= 0)
            throw new InvalidOperationException("Número de folios inválido");

        return new ExpedienteUnidadLegacyPlan
        {
            TieneExpediente = tieneExpediente,
            TieneUnidadConservacion = tieneUnidad,
            IdExpediente = expediente.IdExpediente,
            IdUnidadConservacion = expediente.IdUnidadConservacion,
            IdClaseDocumento = expediente.IdClaseDocumento,
            NumeroFolios = physical.NumeroPaginas,
            IdTipoUnidadDocumental = tieneExpediente ? 2 : 1,
            UnidadConservaTipo = UnidadConservaTipoEnum.Ninguna
        };
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SERVICE — ExpedienteUnidadLegacyService Responsabilidad Ejecutar dentro de la misma transacción: - lock expediente si aplica
- validar expediente si aplica
- lock unidad si aplica
- obtener tipo de unidad según clase documental
- actualizar folios correctos
- retornar resultado para inventario/índice Implementación esperada public sealed class ExpedienteUnidadLegacyService : IExpedienteUnidadLegacyService
{
    private readonly IExpedienteUnidadLegacyBuilder _builder;
    private readonly IExpedienteLegacyRepository _expedienteRepository;
    private readonly IUnidadConservacionLegacyRepository _unidadRepository;
    private readonly IClaseDocumentoLegacyRepository _claseRepository;
    private readonly ILogger<ExpedienteUnidadLegacyService> _logger;

    public ExpedienteUnidadLegacyService(
        IExpedienteUnidadLegacyBuilder builder,
        IExpedienteLegacyRepository expedienteRepository,
        IUnidadConservacionLegacyRepository unidadRepository,
        IClaseDocumentoLegacyRepository claseRepository,
        ILogger<ExpedienteUnidadLegacyService> logger)
    {
        _builder = builder;
        _expedienteRepository = expedienteRepository;
        _unidadRepository = unidadRepository;
        _claseRepository = claseRepository;
        _logger = logger;
    }

    public async Task<ExpedienteUnidadLegacyResult> ExecuteAsync(
        StorageContext context,
        IDbConnection connection,
        IDbTransaction transaction)
    {
        var plan = _builder.Build(context);

        if (!plan.TieneExpediente && !plan.TieneUnidadConservacion)
        {
            return new ExpedienteUnidadLegacyResult
            {
                Ejecutado = false,
                NumeroFolios = 0
            };
        }

        var tipo = await _claseRepository.GetUnidadConservaTipoAsync(
            plan.IdClaseDocumento!.Value,
            context.DefaultDbAlias
        );

        if (tipo == UnidadConservaTipoEnum.Ninguna)
            throw new InvalidOperationException("Tipo de unidad de conservación no determinado");

        if (plan.TieneExpediente)
        {
            var expediente = await _expedienteRepository.LockByIdAsync(
                plan.IdExpediente!.Value,
                connection,
                transaction
            );

            if (expediente == null)
                throw new InvalidOperationException("Expediente no existe");

            if (expediente.EstadoExpediente != 1)
                throw new InvalidOperationException("Expediente cerrado o inactivo");

            var nuevoTotal = expediente.NumeroElectronicoContenido + plan.NumeroFolios;

            if (nuevoTotal < expediente.NumeroElectronicoContenido)
                throw new InvalidOperationException("Overflow de folios electrónicos expediente");

            var rows = await _expedienteRepository.UpdateFoliosElectronicosAsync(
                expediente.IdExpediente,
                nuevoTotal,
                connection,
                transaction
            );

            if (rows != 1)
                throw new InvalidOperationException("No se pudo actualizar folios del expediente");

            _logger.LogInformation(
                "Expediente actualizado requestId={requestId} idExpediente={idExpediente} folios={folios}",
                context.RequestId,
                expediente.IdExpediente,
                plan.NumeroFolios
            );

            return new ExpedienteUnidadLegacyResult
            {
                Ejecutado = true,
                TieneExpediente = true,
                TieneUnidadConservacion = false,
                IdTipoUnidadDocumental = 2,
                EstadoExpedienteElectronico = expediente.EstadoExpedienteElectronico,
                NumeroFolios = plan.NumeroFolios
            };
        }

        if (plan.TieneUnidadConservacion)
        {
            var unidad = await _unidadRepository.LockByIdAsync(
                plan.IdUnidadConservacion!.Value,
                connection,
                transaction
            );

            if (unidad == null)
                throw new InvalidOperationException("Unidad de conservación no existe");

            if (tipo == UnidadConservaTipoEnum.Digitalizado)
            {
                var nuevoTotal = unidad.NumeroDigitalizadoContenido + plan.NumeroFolios;

                if (nuevoTotal < unidad.NumeroDigitalizadoContenido)
                    throw new InvalidOperationException("Overflow de folios digitalizados unidad");

                var rows = await _unidadRepository.UpdateFoliosDigitalizadosAsync(
                    unidad.IdUnidadConservacion,
                    nuevoTotal,
                    connection,
                    transaction
                );

                if (rows != 1)
                    throw new InvalidOperationException("No se pudo actualizar folios digitalizados de unidad");
            }

            if (tipo == UnidadConservaTipoEnum.Electronico)
            {
                var nuevoTotal = unidad.NumeroElectronicoContenido + plan.NumeroFolios;

                if (nuevoTotal < unidad.NumeroElectronicoContenido)
                    throw new InvalidOperationException("Overflow de folios electrónicos unidad");

                var rows = await _unidadRepository.UpdateFoliosElectronicosAsync(
                    unidad.IdUnidadConservacion,
                    nuevoTotal,
                    connection,
                    transaction
                );

                if (rows != 1)
                    throw new InvalidOperationException("No se pudo actualizar folios electrónicos de unidad");
            }

            _logger.LogInformation(
                "Unidad conservación actualizada requestId={requestId} idUnidad={idUnidad} tipo={tipo} folios={folios}",
                context.RequestId,
                unidad.IdUnidadConservacion,
                tipo,
                plan.NumeroFolios
            );

            return new ExpedienteUnidadLegacyResult
            {
                Ejecutado = true,
                TieneExpediente = false,
                TieneUnidadConservacion = true,
                IdTipoUnidadDocumental = 1,
                EstadoExpedienteElectronico = null,
                NumeroFolios = plan.NumeroFolios
            };
        }

        return new ExpedienteUnidadLegacyResult
        {
            Ejecutado = false
        };
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REPOSITORIOS — SQL ESPERADO ExpedienteLegacyRepository Debe usar: SELECT
    ID_EXPEDIENTE AS IdExpediente,
    ESTADO_EXPEDIENTE AS EstadoExpediente,
    estado_expediente_electronico AS EstadoExpedienteElectronico,
    NUMERO_DIGITALIZADO_CONTENIDO AS NumeroDigitalizadoContenido,
    NUMERO_ELECTRONICO_CONTENIDO AS NumeroElectronicoContenido,
    ORDEN_INDICE AS OrdenIndice,
    ULTIMA_PAGINA_INDICE AS UltimaPaginaIndice
FROM expediente_archivo
WHERE ID_EXPEDIENTE = @IdExpediente
FOR UPDATE; Updates: UPDATE expediente_archivo
SET NUMERO_ELECTRONICO_CONTENIDO = @NuevoTotal
WHERE ID_EXPEDIENTE = @IdExpediente; UPDATE expediente_archivo
SET NUMERO_DIGITALIZADO_CONTENIDO = @NuevoTotal
WHERE ID_EXPEDIENTE = @IdExpediente; UnidadConservacionLegacyRepository Debe usar: SELECT
    ID_UNIDAD_CONSERVACION AS IdUnidadConservacion,
    NUMERO_DIGITALIZADO_CONTENIDO AS NumeroDigitalizadoContenido,
    NUMERO_ELECTRONICO_CONTENIDO AS NumeroElectronicoContenido
FROM unidad_conservacion
WHERE ID_UNIDAD_CONSERVACION = @IdUnidadConservacion
FOR UPDATE; Updates: UPDATE unidad_conservacion
SET NUMERO_DIGITALIZADO_CONTENIDO = @NuevoTotal
WHERE ID_UNIDAD_CONSERVACION = @IdUnidadConservacion; UPDATE unidad_conservacion
SET NUMERO_ELECTRONICO_CONTENIDO = @NuevoTotal
WHERE ID_UNIDAD_CONSERVACION = @IdUnidadConservacion; ClaseDocumentoLegacyRepository Debe replicar: Retorna_unidad_conserva_tipo_documento(id_clase_documento, unidad_conserva_tipo) Retorno esperado: DIGITALIZADO → UnidadConservaTipoEnum.Digitalizado
ELECTRONICO → UnidadConservaTipoEnum.Electronico No hardcodear sin consultar configuración real. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN EN TRANSACTION COORDINATOR Agregar después de inventario, antes de índice electrónico: var expedienteUnidadResult = await _expedienteUnidadLegacyService.ExecuteAsync(
    context,
    connection,
    transaction
); Guardar resultado en StorageTransactionResult : public ExpedienteUnidadLegacyResult? ExpedienteUnidadResult { get; init; } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN CON INVENTARIO El InventarioDocumentalBuilder debe usar: ExpedienteUnidadLegacyResult.IdTipoUnidadDocumental Si el resultado existe, debe tener prioridad frente a cálculo duplicado. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN CON ÍNDICE ELECTRÓNICO El índice electrónico solo debe ejecutarse cuando: TieneExpediente = true
EstadoExpedienteElectronico indica flujo electrónico legacy
IdRegistroProduccionDocumental existe La regla exacta de EstadoExpedienteElectronico debe definirse en: PROMPT 19 — XML índice expediente legacy-compatible ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS Expediente y unidad se tratan como casos separados. No exigir ambos IDs. No permitir ambos IDs simultáneamente, salvo que se documente equivalencia legacy. IdClaseDocumento obligatorio si hay expediente o unidad. Usar FOR UPDATE . Todo dentro de la misma transacción. Actualizar NUMERO_ELECTRONICO_CONTENIDO o NUMERO_DIGITALIZADO_CONTENIDO , no campos alternos. No actualizar NUMERO_FOLIO_UNIDAD_CONSERVACION . Respetar unidad_conserva_tipo . No escribir en expediente cerrado. No ejecutar si AplicaUnidadConservacion = false . No abrir conexión aquí. No crear transacción aquí. No generar XML aquí. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DEPENDENCY INJECTION services.AddScoped<IExpedienteUnidadLegacyBuilder, ExpedienteUnidadLegacyBuilder>();
services.AddScoped<IExpedienteUnidadLegacyService, ExpedienteUnidadLegacyService>();

services.AddScoped<IExpedienteLegacyRepository, ExpedienteLegacyRepository>();
services.AddScoped<IUnidadConservacionLegacyRepository, UnidadConservacionLegacyRepository>();
services.AddScoped<IClaseDocumentoLegacyRepository, ClaseDocumentoLegacyRepository>(); ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS Unitarias — Builder opción unidad apagada → no ejecuta expediente null con opción activa → error sin expediente y sin unidad → error expediente sin clase → error unidad sin clase → error expediente y unidad juntos → error solo expediente → plan con IdTipoUnidadDocumental = 2 solo unidad → plan con IdTipoUnidadDocumental = 1 sin PhysicalMetadata → error folios <= 0 → error Unitarias — Service expediente no existe → error expediente cerrado → error expediente válido → actualiza electrónico unidad no existe → error unidad digitalizada → actualiza NUMERO_DIGITALIZADO_CONTENIDO unidad electrónica → actualiza NUMERO_ELECTRONICO_CONTENIDO tipo unidad no determinado → error overflow folios expediente → error overflow folios unidad → error Integración lock expediente real con FOR UPDATE lock unidad real con FOR UPDATE update real expediente update real unidad digitalizada update real unidad electrónica rollback revierte folios commit conserva folios Regresión Legacy Comparar contra VB: id_expediente
id_unidad_conservacion
id_clase_documento
estado_expediente
estado_expediente_electronico
unidad_conserva_tipo
NUMERO_ELECTRONICO_CONTENIDO
NUMERO_DIGITALIZADO_CONTENIDO
ID_TIPO_UNIDAD_DOCUMENTAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD Logs obligatorios: requestId
idExpediente
idUnidadConservacion
idClaseDocumento
idTipoUnidadDocumental
numeroFolios
tipoUnidadConserva
estadoExpedienteElectronico
estado
duración No loguear: rutas físicas completas
fulltext
contenido documental
datos sensibles ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN ENTERPRISE Ruta: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Crear: SCRUM-[ID]-Arquitectura-ExpedienteUnidadLegacy.md
SCRUM-[ID]-Implementacion-Detallada-ExpedienteUnidadLegacy.md
SCRUM-[ID]-Pruebas-ExpedienteUnidadLegacy.md
SCRUM-[ID]-Observabilidad-ExpedienteUnidadLegacy.md
SCRUM-[ID]-Regresion-Legacy-ExpedienteUnidad.md
SCRUM-[ID]-Metadata.md Debe incluir: comparación VB vs C# diferencia expediente vs unidad regla IdTipoUnidadDocumental regla unidad_conserva_tipo tabla de campos actualizados justificación de FOR UPDATE relación con inventario relación con índice electrónico riesgos de concurrencia casos borde ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN Expediente y unidad se procesan por separado. No exige ambos IDs. Clase documento obligatoria si aplica. Expediente cerrado bloquea escritura. Se actualizan folios correctos. No se usa campo alterno incorrecto. Se respeta unidad_conserva_tipo . Se mantiene transacción existente. Se usa FOR UPDATE . Se retorna resultado consumible por inventario/índice. Hay pruebas unitarias. Hay pruebas de integración. Hay regresión comparativa con VB. Hay documentación enterprise. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES No implementar XML índice expediente aquí. No implementar FileSystem aquí. No implementar Naming aquí. No implementar inventario aquí. No modificar función legacy. No abrir conexión. No crear transacción. No usar valores hardcode sin consulta real. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL Implementar: ExpedienteUnidadLegacyBuilder
ExpedienteUnidadLegacyService
ExpedienteLegacyRepository
UnidadConservacionLegacyRepository
ClaseDocumentoLegacyRepository
Modelos
DI
Integración TransactionCoordinator
Pruebas
Documentación El resultado debe quedar listo para: PROMPT 19 — XML índice expediente legacy-compatible

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-184-implementacion-expediente-almacenamiento.