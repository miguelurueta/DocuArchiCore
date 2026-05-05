## Context

- Jira issue key: SCRUM-186
- Jira summary: IMPLEMENTACION-LOG-DOCUARCHI-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-186

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 20 — Logdocuarchi Workflow Legacy-Compatible (FASE CRÍTICA — PARIDAD FUNCIONAL VB → C#) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy auditoría documental workflow documental trazabilidad de operaciones logs transaccionales legacy Dapper / SQL transaccional Clean Architecture migración VB → C# con paridad funcional observabilidad enterprise ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Restaurar la paridad funcional del registro legacy en: logdocuarchi La nueva implementación debe registrar el log de workflow equivalente al VB cuando aplique: INSERT INTO logdocuarchi
(
    id_tran,
    desc_op,
    USER_OPER,
    DATE_TRANS,
    RUT_DOCU,
    MODULO_REGISTRO,
    GABINETE,
    CAMPOS,
    IP_TRANS,
    HORA_REGISTRO,
    RADICADO,
    ID_TAREA_WF,
    ID_RUTA_WF,
    USER_PROPIETARIO,
    TIPOLOGIA_DOCUMENTAL
) Debe corregir las brechas actuales: - IP_TRANS vacío.
- Tipología usa ID string en lugar de descripción legacy.
- Campos/log no replica fultex_log.
- Ruta documento no respeta naming/ruta legacy.
- No hay paridad completa con flujo workflow VB. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ BRECHA FUNCIONAL DETECTADA Comparativo actual: VB legacy:
- Inserta log solo si id_tarea_workflow <> 0.
- Usa idAlmacen como id_tran.
- desc_op = "Registra".
- USER_OPER = useral.
- DATE_TRANS = date1al.
- RUT_DOCU = ruta del archivo DIG.
- MODULO_REGISTRO = "WORKFLOW".
- GABINETE = nombre gabinete.
- CAMPOS = fultex_log concatenado con "|".
- IP_TRANS = sesión ip_host_name.
- HORA_REGISTRO = hora actual.
- RADICADO = radicado si existe.
- ID_TAREA_WF = id_tarea_workflow.
- ID_RUTA_WF = Id_Ruta_Workflow.
- USER_PROPIETARIO = useral.
- TIPOLOGIA_DOCUMENTAL = descripción del tipo documental.

C# actual:
- Inserta parcialmente.
- IP_TRANS vacío.
- Tipología desalineada.
- Campos incompletos.
- Ruta puede no ser la legacy final. Impacto: - Auditoría incompleta.
- Workflow no trazable.
- Reportes legacy inconsistentes.
- Dificultad para soporte.
- Brecha funcional con VB. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO LEGAZY VB D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\Architecture\Almacenamiento\funcion-almacena-consolidad.txt CONTEXTO TABLAS D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\DataModel\StorageEngine-Tables.md UBICACIÓN ESPERADA Services / Workflow MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Workflow/ Archivos esperados: IWorkflowStorageLogBuilder.cs
WorkflowStorageLogBuilder.cs
IWorkflowStorageLogService.cs
WorkflowStorageLogService.cs Repository MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/Workflow/ Archivos esperados: IWorkflowStorageLogRepository.cs
WorkflowStorageLogRepository.cs Models MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Workflow/ Archivos esperados: WorkflowStorageLogModel.cs
WorkflowStorageLogBuildResult.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS WorkflowStorageLogModel public sealed class WorkflowStorageLogModel
{
    public long IdTran { get; init; }

    public string DescOp { get; init; } = "Registra";

    public string UserOper { get; init; } = "";

    public DateTime DateTrans { get; init; }

    public string RutDocu { get; init; } = "";

    public string ModuloRegistro { get; init; } = "WORKFLOW";

    public string Gabinete { get; init; } = "";

    public string Campos { get; init; } = "";

    public string IpTrans { get; init; } = "";

    public string HoraRegistro { get; init; } = "";

    public string? Radicado { get; init; }

    public long IdTareaWorkflow { get; init; }

    public int IdRutaWorkflow { get; init; }

    public string UserPropietario { get; init; } = "";

    public string? TipologiaDocumental { get; init; }
} WorkflowStorageLogBuildResult public sealed class WorkflowStorageLogBuildResult
{
    public bool ShouldInsert { get; init; }

    public WorkflowStorageLogModel? Model { get; init; }

    public string Estado { get; init; } = "";
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACES IWorkflowStorageLogBuilder public interface IWorkflowStorageLogBuilder
{
    WorkflowStorageLogBuildResult Build(
        StorageContext context,
        StorageIdentityModel identity,
        StorageNamingResult naming,
        StoragePhysicalPathModel physicalPath
    );
} IWorkflowStorageLogService public interface IWorkflowStorageLogService
{
    Task ExecuteAsync(
        StorageContext context,
        StorageIdentityModel identity,
        StorageNamingResult naming,
        StoragePhysicalPathModel physicalPath,
        IDbConnection connection,
        IDbTransaction transaction
    );
} IWorkflowStorageLogRepository public interface IWorkflowStorageLogRepository
{
    Task<int> InsertAsync(
        WorkflowStorageLogModel model,
        IDbConnection connection,
        IDbTransaction transaction
    );
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLA LEGACY DE ACTIVACIÓN El log se inserta solo si: IdTareaWorkflow > 0 No debe insertarse si: Workflow == null
IdTareaWorkflow == null
IdTareaWorkflow <= 0 Regla: if (context.Command.Workflow?.IdTareaWorkflow is null or <= 0)
{
    return new WorkflowStorageLogBuildResult
    {
        ShouldInsert = false,
        Estado = "NO_WORKFLOW"
    };
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ BUILDER — WorkflowStorageLogBuilder Responsabilidad Construir el modelo legacy de logdocuarchi usando: StorageContext
StorageIdentityModel
StorageNamingResult
StoragePhysicalPathModel
WorkflowStorageDto
CamposIndexacion
TRD
Inventario/Radicado
Usuario
IP Reglas IdTran = identity.IdAlmacen DescOp = "Registra" UserOper = context.Usuario DateTrans = context.FechaEjecucion RutDocu = ruta final + nombre DIG ModuloRegistro = "WORKFLOW" Gabinete = NombreGabinete Campos = fultex_log legacy ("|valor1|valor2|...") IpTrans debe venir del contexto/request, no quedar vacío. HoraRegistro = HH:mm:ss Radicado = Inventario.Radicado IdTareaWorkflow = Workflow.IdTareaWorkflow IdRutaWorkflow = Workflow.IdRutaWorkflow UserPropietario = context.Usuario TipologiaDocumental = descripción del tipo documental Ajuste requerido en StorageContext Si aún no existe, agregar: public string? IpTrans { get; set; } El Controller/UseCase debe poblarlo desde: HttpContext.Connection.RemoteIpAddress
header X-Forwarded-For si aplica No usar sesión legacy. Implementación esperada public sealed class WorkflowStorageLogBuilder : IWorkflowStorageLogBuilder
{
    public WorkflowStorageLogBuildResult Build(
        StorageContext context,
        StorageIdentityModel identity,
        StorageNamingResult naming,
        StoragePhysicalPathModel physicalPath)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (identity == null)
            throw new ArgumentNullException(nameof(identity));

        if (naming == null)
            throw new ArgumentNullException(nameof(naming));

        if (physicalPath == null)
            throw new ArgumentNullException(nameof(physicalPath));

        var workflow = context.Command.Workflow;

        if (workflow?.IdTareaWorkflow is null or <= 0)
        {
            return new WorkflowStorageLogBuildResult
            {
                ShouldInsert = false,
                Estado = "NO_WORKFLOW"
            };
        }

        var rutaDocumento = Path.Combine(
            physicalPath.RutaFinal,
            naming.NombreArchivoPrincipal
        );

        var campos = BuildCamposLegacy(context.Command.Campos);

        var tipologia = context.Command.Trd?.NombreTipoDocumento;

        var hora = context.FechaEjecucion.ToString("HH:mm:ss");

        var model = new WorkflowStorageLogModel
        {
            IdTran = identity.IdAlmacen,
            DescOp = "Registra",
            UserOper = context.Usuario,
            DateTrans = context.FechaEjecucion,
            RutDocu = rutaDocumento,
            ModuloRegistro = "WORKFLOW",
            Gabinete = context.Command.NombreGabinete,
            Campos = campos,
            IpTrans = context.IpTrans ?? "",
            HoraRegistro = hora,
            Radicado = context.Command.Inventario?.Radicado,
            IdTareaWorkflow = workflow.IdTareaWorkflow.Value,
            IdRutaWorkflow = workflow.IdRutaWorkflow ?? 0,
            UserPropietario = context.Usuario,
            TipologiaDocumental = tipologia
        };

        return new WorkflowStorageLogBuildResult
        {
            ShouldInsert = true,
            Model = model,
            Estado = "READY"
        };
    }

    private static string BuildCamposLegacy(IReadOnlyList<CampoIndexacionDto> campos)
    {
        if (campos == null || campos.Count == 0)
            return "";

        var values = campos
            .Where(c => !string.IsNullOrWhiteSpace(c.Valor))
            .Select(c => c.Valor!.Trim());

        return "|" + string.Join("|", values);
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REPOSITORY — WorkflowStorageLogRepository Reglas Usar SQL parametrizado. No abrir conexión. No crear transacción. Retornar filas afectadas. Validar modelo. Insertar dentro de transacción DB. No loguear campos completos si contienen datos sensibles. Implementación esperada public sealed class WorkflowStorageLogRepository : IWorkflowStorageLogRepository
{
    private readonly ILogger<WorkflowStorageLogRepository> _logger;

    public WorkflowStorageLogRepository(
        ILogger<WorkflowStorageLogRepository> logger)
    {
        _logger = logger;
    }

    public async Task<int> InsertAsync(
        WorkflowStorageLogModel model,
        IDbConnection connection,
        IDbTransaction transaction)
    {
        Validate(model);

        const string sql = @"
INSERT INTO logdocuarchi
(
    id_tran,
    desc_op,
    USER_OPER,
    DATE_TRANS,
    RUT_DOCU,
    MODULO_REGISTRO,
    GABINETE,
    CAMPOS,
    IP_TRANS,
    HORA_REGISTRO,
    RADICADO,
    ID_TAREA_WF,
    ID_RUTA_WF,
    USER_PROPIETARIO,
    TIPOLOGIA_DOCUMENTAL
)
VALUES
(
    @IdTran,
    @DescOp,
    @UserOper,
    @DateTrans,
    @RutDocu,
    @ModuloRegistro,
    @Gabinete,
    @Campos,
    @IpTrans,
    @HoraRegistro,
    @Radicado,
    @IdTareaWorkflow,
    @IdRutaWorkflow,
    @UserPropietario,
    @TipologiaDocumental
);
";

        var rows = await connection.ExecuteAsync(
            sql,
            model,
            transaction
        );

        if (rows != 1)
            throw new InvalidOperationException("No se pudo insertar logdocuarchi");

        _logger.LogInformation(
            "logdocuarchi insertado idTran={idTran} idTareaWorkflow={idTareaWorkflow} gabinete={gabinete}",
            model.IdTran,
            model.IdTareaWorkflow,
            model.Gabinete
        );

        return rows;
    }

    private static void Validate(WorkflowStorageLogModel model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (model.IdTran <= 0)
            throw new InvalidOperationException("IdTran inválido");

        if (string.IsNullOrWhiteSpace(model.DescOp))
            throw new InvalidOperationException("DescOp requerido");

        if (string.IsNullOrWhiteSpace(model.UserOper))
            throw new InvalidOperationException("UserOper requerido");

        if (model.DateTrans == default)
            throw new InvalidOperationException("DateTrans inválido");

        if (string.IsNullOrWhiteSpace(model.RutDocu))
            throw new InvalidOperationException("RutDocu requerido");

        if (string.IsNullOrWhiteSpace(model.ModuloRegistro))
            throw new InvalidOperationException("ModuloRegistro requerido");

        if (string.IsNullOrWhiteSpace(model.Gabinete))
            throw new InvalidOperationException("Gabinete requerido");

        if (model.IdTareaWorkflow <= 0)
            throw new InvalidOperationException("IdTareaWorkflow inválido");

        if (model.IdRutaWorkflow < 0)
            throw new InvalidOperationException("IdRutaWorkflow inválido");

        if (string.IsNullOrWhiteSpace(model.UserPropietario))
            throw new InvalidOperationException("UserPropietario requerido");
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SERVICE — WorkflowStorageLogService public sealed class WorkflowStorageLogService : IWorkflowStorageLogService
{
    private readonly IWorkflowStorageLogBuilder _builder;
    private readonly IWorkflowStorageLogRepository _repository;
    private readonly ILogger<WorkflowStorageLogService> _logger;

    public WorkflowStorageLogService(
        IWorkflowStorageLogBuilder builder,
        IWorkflowStorageLogRepository repository,
        ILogger<WorkflowStorageLogService> logger)
    {
        _builder = builder;
        _repository = repository;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        StorageContext context,
        StorageIdentityModel identity,
        StorageNamingResult naming,
        StoragePhysicalPathModel physicalPath,
        IDbConnection connection,
        IDbTransaction transaction)
    {
        var build = _builder.Build(
            context,
            identity,
            naming,
            physicalPath
        );

        if (!build.ShouldInsert || build.Model == null)
        {
            _logger.LogInformation(
                "logdocuarchi no aplica requestId={requestId} estado={estado}",
                context.RequestId,
                build.Estado
            );

            return;
        }

        await _repository.InsertAsync(
            build.Model,
            connection,
            transaction
        );
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN EN TRANSACTION COORDINATOR Agregar dentro de la transacción DB, después de que exista: identity
naming
physicalPath Ejemplo: await _workflowStorageLogService.ExecuteAsync(
    context,
    reservation.Identity,
    naming,
    physicalPath,
    connection,
    transaction
); Reglas: - Debe ejecutarse dentro de transacción.
- Si falla, debe hacer rollback.
- Solo aplica si IdTareaWorkflow > 0. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN EN CONTROLLER / USECASE PARA IP Agregar extracción de IP en Controller: var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? ""; Pasar al UseCase o incluir en request/contexto interno. Actualizar StorageContext : public string? IpTrans { get; set; } No usar HttpContext.Current.Session.Item("ip_host_name") . ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS Insertar solo si IdTareaWorkflow > 0 . IP no debe quedar vacía si el contexto HTTP la tiene. Tipología debe ser descripción, no ID. Ruta documento debe ser la ruta legacy final + nombre DIG. Campos debe usar formato legacy con | . No loguear CAMPOS completo. No loguear rutas completas si son sensibles. Insertar dentro de transacción DB. Si falla log y workflow aplica, rollback. No usar sesión legacy. No usar valores hardcode sin justificación. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DEPENDENCY INJECTION services.AddScoped<IWorkflowStorageLogBuilder, WorkflowStorageLogBuilder>();
services.AddScoped<IWorkflowStorageLogService, WorkflowStorageLogService>();
services.AddScoped<IWorkflowStorageLogRepository, WorkflowStorageLogRepository>(); ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS Unitarias — Builder workflow null → no inserta IdTareaWorkflow null → no inserta IdTareaWorkflow <= 0 → no inserta workflow válido → construye modelo IdTran = IdAlmacen DescOp = Registra ModuloRegistro = WORKFLOW Campos usa |valor1|valor2 Tipologia usa NombreTipoDocumento Radicado desde Inventario IpTrans desde contexto RutDocu usa ruta final + DIG Unitarias — Repository modelo null → error IdTran inválido → error UserOper vacío → error RutDocu vacío → error IdTareaWorkflow inválido → error insert OK → rows = 1 rows != 1 → error Integración insert real en logdocuarchi rollback revierte log commit conserva log workflow apagado no inserta workflow activo inserta Regresión Legacy Comparar contra VB: id_tran
desc_op
USER_OPER
DATE_TRANS
RUT_DOCU
MODULO_REGISTRO
GABINETE
CAMPOS
IP_TRANS
HORA_REGISTRO
RADICADO
ID_TAREA_WF
ID_RUTA_WF
USER_PROPIETARIO
TIPOLOGIA_DOCUMENTAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD Logs obligatorios: requestId
idTran
idTareaWorkflow
idRutaWorkflow
gabinete
estado
duración No loguear: CAMPOS completo
rutas físicas completas sensibles
fulltext
contenido documental
datos sensibles ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN ENTERPRISE Ruta: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Crear: SCRUM-[ID]-Arquitectura-LogdocuarchiWorkflow.md
SCRUM-[ID]-Implementacion-Detallada-LogdocuarchiWorkflow.md
SCRUM-[ID]-Pruebas-LogdocuarchiWorkflow.md
SCRUM-[ID]-Observabilidad-LogdocuarchiWorkflow.md
SCRUM-[ID]-Regresion-Legacy-LogdocuarchiWorkflow.md
SCRUM-[ID]-Metadata.md Debe incluir: comparación VB vs C# tabla de equivalencia de columnas regla de activación por IdTareaWorkflow estrategia para IP estrategia para tipología documental relación con Naming relación con ruta física legacy relación con TransactionCoordinator riesgos de auditoría ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN Inserta logdocuarchi solo si workflow aplica. IP_TRANS se llena desde contexto HTTP. Tipología documental es descripción. Ruta documento usa ruta legacy final + DIG. CAMPOS usa formato legacy. Se ejecuta dentro de la transacción. Rollback revierte log. No usa sesión legacy. Hay pruebas unitarias. Hay pruebas de integración. Hay regresión comparativa con VB. Hay documentación enterprise. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES No implementar FileSystem aquí. No implementar XML aquí. No implementar Naming aquí. No implementar Inventario aquí. No abrir conexión. No crear transacción. No modificar función legacy. No loguear CAMPOS completo. No loguear contenido documental. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL Implementar: WorkflowStorageLogModel
WorkflowStorageLogBuildResult
IWorkflowStorageLogBuilder
WorkflowStorageLogBuilder
IWorkflowStorageLogService
WorkflowStorageLogService
IWorkflowStorageLogRepository
WorkflowStorageLogRepository
StorageContext.IpTrans
Integración Controller/UseCase
Integración TransactionCoordinator
DI
Pruebas
Documentación El resultado debe quedar listo para: PROMPT 21 — Pruebas de paridad VB vs C#

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-186-implementacion-log-docuarchi-almacenamie.