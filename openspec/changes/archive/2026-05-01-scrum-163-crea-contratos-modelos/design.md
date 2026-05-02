## Context

- Jira issue key: SCRUM-163
- Jira summary: CREA-CONTRATOS-MODELOS
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-163

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 2 — Definición de contratos, DTOs, Models, Enums y excepciones base del Storage Engine (VERSIÓN ENTERPRISE FINAL — CONTRATOS UNIFICADOS) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET, especialista en: Clean Architecture diseño de contratos y modelos de dominio tipado fuerte sistemas documentales críticos Dapper migración de código legacy a arquitectura enterprise diseño de engines/orquestadores contratos extensibles observabilidad idempotencia resiliencia ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Diseñar y construir todos los contratos formales de entrada, salida y modelos internos del Storage Engine que reemplazará la función legacy Almacenamiento . Este prompt define la base estructural de todo el sistema. Debe incluir: DTOs externos Commands internos Context Models Enums Results Responses Validation models Exception models Estados del flujo Contratos base para futuras fases IMPORTANTE: NO implementar lógica de negocio NO acceder a base de datos NO acceder a filesystem NO generar XML NO crear repositories NO crear controllers SOLO definir contratos sólidos y compilables ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ La función legacy Almacenamiento usa: parámetros sueltos parámetros opcionales ByRef mezcla de responsabilidades acoplamiento a base de datos acoplamiento a filesystem acoplamiento a XML baja trazabilidad ausencia de idempotencia ausencia de contratos semánticos Debe reemplazarse por un flujo estructurado: DTO Request → Command → StorageContext → ValidationResult → IdentityReservation → StoragePlan → TransactionResult → PhysicalResult → AlmacenarDocumentoResult → AlmacenarDocumentoResponse ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN POR CAPAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DTOs: MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/ Models: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/ Enums: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Enums/ Exceptions: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Exceptions/ Documentation: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Si alguna ruta no existe, crearla. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENUMS OBLIGATORIOS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Enums/TipoAlmacenamientoEnum.cs public enum TipoAlmacenamientoEnum { BatchPreindex = 1, Manual = 2, Digitalizacion = 3, Workflow = 4 } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Enums/StorageDocumentState.cs public enum StorageDocumentState { Pending = 1, Reserved = 2, Completed = 3, PhysicalFailed = 4, Failed = 5 } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Enums/StoragePhase.cs public enum StoragePhase { RequestReceived = 1, Validation = 2, Transaction = 3, IdentityReservation = 4, GabineteInsert = 5, InventarioInsert = 6, ExpedienteIndex = 7, FileSystem = 8, Xml = 9, Compensation = 10, Completed = 11, Failed = 12 } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DTOs — CAPA EXTERNA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/AlmacenarDocumentoRequest.cs public sealed class AlmacenarDocumentoRequest { public required string NombreGabinete { get; init; } // Identificador lógico de la ruta temporal.
// NO debe representar una ruta física directa.
public required string RutaTemporalId { get; init; }

public required string NombreDocumento { get; init; }

public required List<DocumentoEntradaDto> Documentos { get; init; }
public required List<CampoIndexacionDto> CamposIndexacion { get; init; }

public TipoAlmacenamientoEnum TipoAlmacenamiento { get; init; }

public int NumeroPaginasDeclaradas { get; init; }

public int TipoDocumento { get; init; }
public int TipoDocumentoAnexo { get; init; }

public bool EvaluarCamposObligatorios { get; init; }

public InventarioDocumentalDto? Inventario { get; init; }
public TrdStorageDto? Trd { get; init; }
public ExpedienteStorageDto? Expediente { get; init; }
public WorkflowStorageDto? Workflow { get; init; } } Crear: MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/AlmacenarDocumentoResponse.cs public sealed class AlmacenarDocumentoResponse { public long IdAlmacen { get; init; } public long? IdRegistroProduccionDocumental { get; init; } public string NombreArchivoFinal { get; init; } = string.Empty;
public string RutaFisicaFinal { get; init; } = string.Empty;

public int NumeroPaginas { get; init; }
public string Tamano { get; init; } = string.Empty;
public string Formato { get; init; } = string.Empty;

public StorageDocumentState Estado { get; init; }

public string RequestId { get; init; } = string.Empty; } Crear: MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/DocumentoEntradaDto.cs public sealed class DocumentoEntradaDto { public required string IdDocumento { get; init; } // Identificador temporal controlado.
// NO debe ser una ruta física enviada libremente por el cliente.
public required string ArchivoTemporalId { get; init; }

public required string NombreOriginal { get; init; }

public long TamanoBytes { get; init; } } Crear: MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/CampoIndexacionDto.cs public sealed class CampoIndexacionDto { public required string NombreCampo { get; init; } public string? Valor { get; init; } public bool EsObligatorio { get; init; } } Crear: MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/InventarioDocumentalDto.cs public sealed class InventarioDocumentalDto { public int IdUsuarioGestion { get; init; } public int IdEmpresa { get; init; } public string? Radicado { get; init; }
public string? FechaElaboracion { get; init; } } Crear: MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/TrdStorageDto.cs public sealed class TrdStorageDto { public int? IdArea { get; init; } public int? IdSerie { get; init; } public int? IdSubSerie { get; init; } public int? IdTipoDocumento { get; init; } public string? NombreArea { get; init; }
public string? NombreSerie { get; init; }
public string? NombreSubSerie { get; init; }
public string? NombreTipoDocumento { get; init; } } Crear: MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/ExpedienteStorageDto.cs public sealed class ExpedienteStorageDto { public int? IdExpediente { get; init; } public int? IdTipoExpediente { get; init; } public int? IdUnidadConservacion { get; init; }
public int? IdTipoUnidadConservacion { get; init; }

public int? IdClaseDocumento { get; init; }

public string? NombreExpediente { get; init; }
public string? NombreUnidadConservacion { get; init; }
public string? ClaseDocumento { get; init; } } Crear: MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/WorkflowStorageDto.cs public sealed class WorkflowStorageDto { public long? IdTareaWorkflow { get; init; } public int? IdRutaWorkflow { get; init; } } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELS — CAPA DOMAIN/APPLICATION ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/AlmacenarDocumentoCommand.cs public sealed class AlmacenarDocumentoCommand { public required string NombreGabinete { get; init; } public required string RutaTemporalId { get; init; } public required string NombreDocumento { get; init; } public required IReadOnlyList<DocumentoEntradaDto> Documentos { get; init; }
public required IReadOnlyList<CampoIndexacionDto> Campos { get; init; }

public TipoAlmacenamientoEnum TipoAlmacenamiento { get; init; }

public int NumeroPaginasDeclaradas { get; init; }

public int TipoDocumento { get; init; }
public int TipoDocumentoAnexo { get; init; }

public bool EvaluarCamposObligatorios { get; init; }

public InventarioDocumentalDto? Inventario { get; init; }
public TrdStorageDto? Trd { get; init; }
public ExpedienteStorageDto? Expediente { get; init; }
public WorkflowStorageDto? Workflow { get; init; } } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageContext.cs public sealed class StorageContext { public required string DefaultDbAlias { get; init; } public required string Usuario { get; init; }

// Obligatorio. No nullable.
public int UsuarioId { get; init; }

// Debe ser único por intento de almacenamiento.
public required string RequestId { get; init; }

public required DateTime FechaEjecucion { get; init; }

public required AlmacenarDocumentoCommand Command { get; init; } } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageIdentityModel.cs public sealed class StorageIdentityModel { public long IdAlmacen { get; init; } public int Disco { get; init; } public int Carpeta { get; init; } public int NumeroPaginasCarpeta { get; init; } } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageMetadataModel.cs public sealed class StorageMetadataModel { public string Formato { get; init; } = string.Empty; public string Tamano { get; init; } = string.Empty; public int NumeroPaginas { get; init; } public string NombreFinalDocumento { get; init; } = string.Empty;

// Texto extraído para indexación.
// Puede estar vacío.
public string FullText { get; init; } = string.Empty; } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StoragePlanModel.cs public sealed class StoragePlanModel { public required StorageIdentityModel Identity { get; init; } public required StorageMetadataModel Metadata { get; init; } public required string RutaFinal { get; init; }
public required IReadOnlyList<string> ArchivosOrigen { get; init; }

public long TamanoTotalBytes { get; init; } } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/AlmacenarDocumentoResult.cs public sealed class AlmacenarDocumentoResult { public long IdAlmacen { get; init; } public long? IdRegistroProduccionDocumental { get; init; } public string NombreArchivoFinal { get; init; } = string.Empty;
public string RutaFisicaFinal { get; init; } = string.Empty;

public int NumeroPaginas { get; init; }
public string Tamano { get; init; } = string.Empty;
public string Formato { get; init; } = string.Empty;

public StorageDocumentState Estado { get; init; }

public string RequestId { get; init; } = string.Empty; } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDATION MODELS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageError.cs public sealed class StorageError { public string Code { get; init; } = string.Empty; public string Message { get; init; } = string.Empty; public StoragePhase? Phase { get; init; } } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageValidationResult.cs public sealed class StorageValidationResult { public bool IsValid { get; init; } public List<StorageError> Errors { get; init; } = new(); } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ EXCEPTIONS — JERARQUÍA OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Exceptions/StorageException.cs public abstract class StorageException : Exception { protected StorageException(string message) : base(message) { } protected StorageException(string message, Exception innerException)
    : base(message, innerException)
{
} } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Exceptions/StorageValidationException.cs public sealed class StorageValidationException : StorageException { public IReadOnlyList<StorageError> Errors { get; } public StorageValidationException(IReadOnlyList<StorageError> errors)
    : base("Errores de validación en Storage Engine")
{
    Errors = errors;
} } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Exceptions/StorageTransactionException.cs public sealed class StorageTransactionException : StorageException { public StorageTransactionException(string message) : base(message) { } public StorageTransactionException(string message, Exception innerException)
    : base(message, innerException)
{
} } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Exceptions/StoragePhysicalException.cs public sealed class StoragePhysicalException : StorageException { public StoragePhysicalException(string message) : base(message) { } public StoragePhysicalException(string message, Exception innerException)
    : base(message, innerException)
{
} } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS DE IDEMPOTENCIA Y ESTADO FÍSICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageIdempotencyModel.cs public sealed class StorageIdempotencyModel { public required string RequestId { get; init; } public int UsuarioId { get; init; } public string NombreGabinete { get; init; } = string.Empty; public StorageDocumentState Estado { get; init; } public long? IdAlmacen { get; init; } public DateTime FechaCreacion { get; init; } } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StoragePhysicalStatusModel.cs public sealed class StoragePhysicalStatusModel { public required string RequestId { get; init; } public long IdAlmacen { get; init; } public StorageDocumentState Estado { get; init; } public string? ErrorMessage { get; init; } public DateTime FechaActualizacion { get; init; } } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS PARA BUILDERS FUTUROS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageFilePlanModel.cs public sealed class StorageFilePlanModel { public required string StorageRoot { get; init; } public required string RutaFinal { get; init; } public required string NombreArchivoPrincipal { get; init; } public required List<string> ArchivosOrigen { get; init; } } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageXmlModel.cs public sealed class StorageXmlModel { public required long IdAlmacen { get; init; } public required int Disco { get; init; } public required int Carpeta { get; init; } public required int Paginas { get; init; } public required string Usuario { get; init; } public required Dictionary<string, string?> Metadata { get; init; } } Crear: MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageCompensationPlan.cs public sealed class StorageCompensationPlan { public List<string> ArchivosTemporales { get; init; } = new(); public List<string> ArchivosFinales { get; init; } = new(); public List<string> XmlTemporales { get; init; } = new(); public List<string> XmlFinales { get; init; } = new(); public List<string> DirectoriosCreados { get; init; } = new(); } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACES BASE PARA BUILDERS FUTUROS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Builders/IStoragePlanBuilder.cs public interface IStoragePlanBuilder { StorageFilePlanModel BuildFilePlan( StorageContext context, StorageTransactionResult transactionResult ); } Crear: MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Builders/IStorageXmlBuilder.cs public interface IStorageXmlBuilder { StorageXmlModel BuildXmlModel( StorageContext context, StorageTransactionResult transactionResult ); } IMPORTANTE: Si StorageTransactionResult aún no existe en esta fase, dejar las interfaces definidas en el prompt correspondiente o documentar dependencia con Prompt 6. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS ARQUITECTÓNICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DTOs no contienen lógica. Models representan dominio/application. Enums centralizan estados. Excepciones deben ser tipadas. No usar ByRef . No usar parámetros opcionales legacy. No usar rutas físicas directas desde el request. No usar RutaArchivo en DocumentoEntradaDto . Usar únicamente ArchivoTemporalId . StorageContext.UsuarioId debe ser obligatorio y no nullable. RequestId debe existir en todo el flujo. El estado físico debe contemplar PhysicalFailed . Los resultados deben incluir estado final. El Response debe incluir RequestId. La idempotencia debe quedar preparada desde contratos. Preparar integración con: ValidationPipeline TransactionCoordinator FileSystem XML Compensation Builders ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS FUNCIONALES IMPORTANTES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ NumeroPaginasDeclaradas debe validarse contra páginas reales en pipeline posterior. RutaTemporalId debe resolverse en infraestructura. ArchivoTemporalId debe resolverse en infraestructura. FullText puede estar vacío. FullText debe ser tratado como dato potencialmente pesado. RequestId debe evitar doble ejecución lógica. PhysicalFailed debe representar fallo posterior al commit DB. Completed solo puede asignarse cuando DB + FS + XML finalicen correctamente. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Incluir pruebas de compilación/contrato para: DTOs instanciables correctamente required properties obligatorias UsuarioId no nullable DocumentoEntradaDto usa ArchivoTemporalId AlmacenarDocumentoResponse contiene RequestId AlmacenarDocumentoResult contiene Estado StorageDocumentState contiene PhysicalFailed StorageValidationException conserva lista de errores StoragePhysicalException existe StorageTransactionException existe modelos de idempotencia existen builders base compilan si se incluyen en esta fase ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ruta: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Crear: SCRUM-[ID]-Arquitectura-Contratos-StorageEngine.md Debe incluir: objetivo de los contratos diagrama de flujo DTO → Command → Context → Result → Response diagrama de clases explicación de estados explicación de RequestId e idempotencia explicación de PhysicalFailed relación con prompts posteriores SCRUM-[ID]-Implementacion-Contratos-StorageEngine.md Debe incluir: archivos creados rutas exactas DTOs Models Enums Exceptions decisiones de tipado decisiones de compatibilidad legacy SCRUM-[ID]-Pruebas-Contratos-StorageEngine.md Debe incluir: matriz de pruebas de contrato pruebas de compilación pruebas de required properties pruebas de consistencia de nombres SCRUM-[ID]-Metadata.md Debe incluir: identificador del ticket usuario que creó el ticket fecha módulo relación con Prompt 3 relación con Prompt 4 relación con Prompt 10 estado del ticket ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ No existen parámetros sueltos. No existe ByRef . Existe Request estructurado. Existe Command separado. Existe StorageContext enriquecido. UsuarioId es obligatorio. DocumentoEntradaDto usa ArchivoTemporalId. No existe RutaArchivo en DocumentoEntradaDto. Existe Result con Estado. Existe Response con RequestId. Existe StorageDocumentState. Existe PhysicalFailed. Existe StoragePhase. Existe jerarquía de excepciones. Existe StoragePhysicalException. Existe StorageIdempotencyModel. Existe StoragePhysicalStatusModel. Existe StorageValidationResult con lista de errores. Tipado fuerte completo. El resultado compila. Queda listo para Prompt 3 — UseCase + Orchestrator. Queda listo para Prompt 4 — ValidationPipeline. Queda listo para Prompt 10 — API final. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ NO lógica de negocio. NO acceso a base de datos. NO filesystem. NO XML. NO repositories. NO controller. NO SQL. NO implementación de builders todavía si depende de fases futuras. NO usar nombres inconsistentes con este prompt. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Generar todos los DTOs, Models, Enums y Exceptions definidos, respetando: rutas nombres tipado fuerte separación de capas compatibilidad con prompts posteriores compilación consistencia arquitectónica El resultado debe quedar listo para continuar con: SCRUMCORE-164

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-163-crea-contratos-modelos.