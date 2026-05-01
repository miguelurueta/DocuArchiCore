## Context

- Jira issue key: SCRUM-164
- Jira summary: CREA-FUNCION-OQUESTADOR-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-164

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 3 — UseCase + Orchestrator base (ALINEADO CON PROMPT 2   SCRUMCORE-163 ) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Arquitecto Master Backend .NET especialista en: Clean Architecture Application Layer (UseCase) Orchestrator / Engine separación estricta de responsabilidades diseño desacoplado observabilidad manejo de errores tipados idempotencia contratos enterprise ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Construir la estructura base del caso de uso y el orquestador del Storage Engine. Este prompt define: IAlmacenarDocumentoUseCase AlmacenarDocumentoUseCase IDocumentStorageOrchestrator DocumentStorageOrchestrator ⚠️ IMPORTANTE: NO implementar lógica real de almacenamiento NO acceder a base de datos NO acceder a filesystem NO escribir XML NO usar repositories NO usar SQL Este es un orquestador base preparado para pipeline completo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DEPENDENCIA CRÍTICA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Este prompt depende de: 👉 PROMPT 2 (contratos finalizados) SCRUMCORE-163 Debe usar EXACTAMENTE: StorageContext AlmacenarDocumentoCommand AlmacenarDocumentoResult AlmacenarDocumentoResponse StorageDocumentState StorageException hierarchy NO redefinir contratos. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/ Archivos: IAlmacenarDocumentoUseCase.cs AlmacenarDocumentoUseCase.cs IDocumentStorageOrchestrator.cs DocumentStorageOrchestrator.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACE — USE CASE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ public interface IAlmacenarDocumentoUseCase { Task<AppResponses<AlmacenarDocumentoResponse>> ExecuteAsync( AlmacenarDocumentoRequest request, string defaultDbAlias, string usuario, int usuarioId ); } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACE — ORCHESTRATOR ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ public interface IDocumentStorageOrchestrator { Task<AlmacenarDocumentoResult> ExecuteAsync(StorageContext context); } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — USE CASE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ public sealed class AlmacenarDocumentoUseCase : IAlmacenarDocumentoUseCase { private readonly IDocumentStorageOrchestrator _orchestrator; private readonly ILogger<AlmacenarDocumentoUseCase> _logger; public AlmacenarDocumentoUseCase(
    IDocumentStorageOrchestrator orchestrator,
    ILogger<AlmacenarDocumentoUseCase> logger)
{
    _orchestrator = orchestrator;
    _logger = logger;
}

public async Task<AppResponses<AlmacenarDocumentoResponse>> ExecuteAsync(
    AlmacenarDocumentoRequest request,
    string defaultDbAlias,
    string usuario,
    int usuarioId)
{
    var requestId = Guid.NewGuid().ToString("N");

    try
    {
        // ============================
        // VALIDACIÓN DEFENSIVA MÍNIMA
        // ============================

        if (request == null)
            return BuildValidationError("Request requerido", requestId);

        if (string.IsNullOrWhiteSpace(defaultDbAlias))
            return BuildValidationError("Alias requerido", requestId);

        if (string.IsNullOrWhiteSpace(usuario))
            return BuildValidationError("Usuario requerido", requestId);

        if (usuarioId <= 0)
            return BuildValidationError("UsuarioId inválido", requestId);

        if (request.Documentos == null || request.Documentos.Count == 0)
            return BuildValidationError("Debe existir al menos un documento", requestId);

        if (request.CamposIndexacion == null)
            return BuildValidationError("CamposIndexacion requerido", requestId);

        if (string.IsNullOrWhiteSpace(request.NombreGabinete))
            return BuildValidationError("NombreGabinete requerido", requestId);

        if (string.IsNullOrWhiteSpace(request.RutaTemporalId))
            return BuildValidationError("RutaTemporalId requerido", requestId);

        if (string.IsNullOrWhiteSpace(request.NombreDocumento))
            return BuildValidationError("NombreDocumento requerido", requestId);

        // ============================
        // MAPEO REQUEST → COMMAND
        // ============================

        var command = new AlmacenarDocumentoCommand
        {
            NombreGabinete = request.NombreGabinete,
            RutaTemporalId = request.RutaTemporalId,
            NombreDocumento = request.NombreDocumento,
            Documentos = request.Documentos,
            Campos = request.CamposIndexacion,
            TipoAlmacenamiento = request.TipoAlmacenamiento,
            NumeroPaginasDeclaradas = request.NumeroPaginasDeclaradas,
            TipoDocumento = request.TipoDocumento,
            TipoDocumentoAnexo = request.TipoDocumentoAnexo,
            EvaluarCamposObligatorios = request.EvaluarCamposObligatorios,
            Inventario = request.Inventario,
            Trd = request.Trd,
            Expediente = request.Expediente,
            Workflow = request.Workflow
        };

        // ============================
        // CREACIÓN CONTEXT
        // ============================

        var context = new StorageContext
        {
            DefaultDbAlias = defaultDbAlias,
            Usuario = usuario,
            UsuarioId = usuarioId,
            RequestId = requestId,
            FechaEjecucion = DateTime.UtcNow,
            Command = command
        };

        _logger.LogInformation(
            "Inicio almacenamiento requestId={requestId} usuarioId={usuarioId}",
            requestId,
            usuarioId
        );

        // ============================
        // EJECUCIÓN ORCHESTRATOR
        // ============================

        var result = await _orchestrator.ExecuteAsync(context);

        // ============================
        // MAPEO RESULT → RESPONSE
        // ============================

        var response = new AlmacenarDocumentoResponse
        {
            IdAlmacen = result.IdAlmacen,
            IdRegistroProduccionDocumental = result.IdRegistroProduccionDocumental,
            NombreArchivoFinal = result.NombreArchivoFinal,
            RutaFisicaFinal = result.RutaFisicaFinal,
            NumeroPaginas = result.NumeroPaginas,
            Tamano = result.Tamano,
            Formato = result.Formato,
            Estado = result.Estado,
            RequestId = requestId
        };

        return new AppResponses<AlmacenarDocumentoResponse>
        {
            Success = true,
            Message = "OK",
            Data = response,
            Meta = AppMetaBuilder.Build(null, 0, 0, "StorageEngine", "")
        };
    }
    catch (StorageValidationException ex)
    {
        return BuildValidationError("Error de validación", requestId, ex.Errors);
    }
    catch (StorageTransactionException ex)
    {
        return BuildError("Error transaccional", requestId, ex.Message);
    }
    catch (StoragePhysicalException ex)
    {
        return BuildError("Error físico (FS/XML)", requestId, ex.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error inesperado StorageEngine {requestId}", requestId);

        return BuildError("Error inesperado", requestId, ex.Message);
    }
}

private AppResponses<AlmacenarDocumentoResponse> BuildValidationError(
    string message,
    string requestId,
    IReadOnlyList<StorageError>? errors = null)
{
    return new AppResponses<AlmacenarDocumentoResponse>
    {
        Success = false,
        Message = message,
        Data = null,
        Meta = null,
        ErrorMessage = message
    };
}

private AppResponses<AlmacenarDocumentoResponse> BuildError(
    string message,
    string requestId,
    string error)
{
    return new AppResponses<AlmacenarDocumentoResponse>
    {
        Success = false,
        Message = message,
        Data = null,
        Meta = null,
        ErrorMessage = error
    };
} } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — ORCHESTRATOR BASE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ public sealed class DocumentStorageOrchestrator : IDocumentStorageOrchestrator { private readonly ILogger<DocumentStorageOrchestrator> _logger; public DocumentStorageOrchestrator(
    ILogger<DocumentStorageOrchestrator> logger)
{
    _logger = logger;
}

public async Task<AlmacenarDocumentoResult> ExecuteAsync(StorageContext context)
{
    if (context == null)
        throw new ArgumentNullException(nameof(context));

    if (context.Command == null)
        throw new ArgumentException("Command requerido");

    if (string.IsNullOrWhiteSpace(context.RequestId))
        throw new ArgumentException("RequestId requerido");

    _logger.LogInformation(
        "Orchestrator iniciado requestId={requestId}",
        context.RequestId
    );

    // ============================
    // PIPELINE FUTURO
    // ============================

    /*
    ValidationPipeline
    TransactionCoordinator
    FileSystemWriter
    XmlWriter
    CompensationManager
    */

    await Task.CompletedTask;

    return new AlmacenarDocumentoResult
    {
        IdAlmacen = 0,
        IdRegistroProduccionDocumental = null,
        NombreArchivoFinal = string.Empty,
        RutaFisicaFinal = string.Empty,
        NumeroPaginas = 0,
        Tamano = string.Empty,
        Formato = string.Empty,
        Estado = StorageDocumentState.Pending,
        RequestId = context.RequestId
    };
} } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DEPENDENCY INJECTION ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ services.AddScoped<IAlmacenarDocumentoUseCase, AlmacenarDocumentoUseCase>(); services.AddScoped<IDocumentStorageOrchestrator, DocumentStorageOrchestrator>(); ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ✔ Compila ✔ Usa contratos del Prompt 2 SCRUMCORE-163 ✔ No redefine modelos ✔ Maneja errores tipados ✔ Genera RequestId ✔ Crea StorageContext correctamente ✔ Orchestrator desacoplado ✔ Sin acceso a DB ✔ Sin filesystem ✔ Sin XML ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar UseCase y Orchestrator base alineados al Prompt 2. SCRUMCORE-163

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-164-crea-funcion-oquestador-almacenamiento.