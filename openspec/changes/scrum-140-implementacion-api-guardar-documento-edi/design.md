## Context

- Jira issue key: SCRUM-140
- Jira summary: IMPLEMENTACION-API-GUARDAR-DOCUMENTO-EDITOR
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-140

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — Ticket BE Crear API + Service + Repository: Guardar documento HTML del editor Tiptap Rol esperado: Arquitecto de software senior backend (.NET, C#, Core, controllers, services, repositorios, Dapper, contratos DTO/Models, seguridad por claims, integridad transaccional, pruebas y documentación técnica). ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar el caso de uso backend para crear y actualizar documentos HTML generados desde Tiptap, persistiéndolos en la tabla ra_editor_documents, manteniendo consistencia con la arquitectura actual del proyecto: Controller Service Repository IDapperCrudEngine + QueryOptions AppResponses<T> validaciones tempranas y defensivas manejo controlado de errores con AppError validación de claim defaulalias documentación técnica y de arquitectura La solución debe permitir: crear un documento nuevo actualizar un documento existente persistir el HTML del editor persistir metadatos funcionales mínimos devolver una respuesta consistente para consumo frontend ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO EXISTENTE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Patrón backend actual: Controllers con IClaimValidationService, validación de parámetros, BadRequest para errores y Ok para éxito Services con interface + implementación, try/catch, validación defensiva Repositories con IDapperCrudEngine, QueryOptions, AppResponses<List<T>> Models con [Table], [Key], [Column] Base de datos: ra_editor_documents ra_editor_document_formats ra_editor_document_statuses ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Controller: DocuArchi.Api/Controllers/GestorDocumental/Editor/GuardaEditorDocumentController.cs Service: MiApp.Services/Service/GestorDocumental/Editor/ServiceGuardaEditorDocument.cs Repositorio: MiApp.Repository/Repositorio/GestorDocumental/Editor/GuardaEditorDocumentRepository.cs Modelo: MiApp.Models/Models/GestorDocumental/Editor/RaEditorDocument.cs DTO: MiApp.DTOs/DTOs/GestorDocumental/Editor/ Documentación: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\Editor\ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CASO DE USO FUNCIONAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Escenario A — Crear DocumentId = null → crear nuevo documento Escenario B — Actualizar DocumentId > 0 → actualizar documento existente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ TABLA OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ra_editor_documents Campos: document_id template_id template_version document_title format_code document_html status_code created_by updated_by created_at updated_at ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELO OFICIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Archivo: MiApp.Models/Models/GestorDocumental/Editor/RaEditorDocument.cs using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiApp.Models.Models.GestorDocumental.Editor
{
    [Table("ra_editor_documents")]
    public class RaEditorDocument
    {
        [Key]
        [Column("document_id")]
        public long DocumentId { get; set; }

        [Column("template_id")]
        public long? TemplateId { get; set; }

        [Column("template_version")]
        public int? TemplateVersion { get; set; }

        [Column("document_title")]
        public string? DocumentTitle { get; set; }

        [Required]
        [Column("format_code")]
        public string FormatCode { get; set; } = null!;

        [Required]
        [Column("document_html")]
        public string DocumentHtml { get; set; } = null!;

        [Required]
        [Column("status_code")]
        public string StatusCode { get; set; } = null!;

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTRATO OFICIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DTO Request: public class GuardaEditorDocumentRequest
{
    public long? DocumentId { get; set; }
    public long? TemplateId { get; set; }
    public int? TemplateVersion { get; set; }
    public string? DocumentTitle { get; set; }
    public string DocumentHtml { get; set; } = null!;
    public string? StatusCode { get; set; }
} Response: AppResponses<RaEditorDocument> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RUTA OFICIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Base: api/gestor-documental/editor Endpoint: POST /api/gestor-documental/editor/document ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE SEGURIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ validar claim defaulalias usar IClaimValidationService si falla → BadRequest(validation.Response) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — CONTROLLER ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ validar claim validar request validar DocumentHtml llamar service error → BadRequest éxito → Ok ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — SERVICE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Interface: public interface IServiceGuardaEditorDocument
{
    Task<AppResponses<RaEditorDocument>> GuardaEditorDocumentAsync(
        GuardaEditorDocumentRequest request,
        string defaultDbAlias
    );
} Responsabilidades: validar defaultDbAlias validar DocumentHtml set FormatCode = "html" set StatusCode default "saved" decidir create/update llamar repository try/catch retornar AppResponses ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — REPOSITORY ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Interface: public interface IGuardaEditorDocumentRepository
{
    Task<AppResponses<RaEditorDocument>> GuardaEditorDocumentAsync(
        GuardaEditorDocumentRequest request,
        string defaultDbAlias
    );
} Reglas: persistir en ra_editor_documents soportar insert y update validar existencia en update no permitir HTML vacío usar IDapperCrudEngine no SQL concatenado ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SEMÁNTICA DE RESPUESTA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Error: success = false data = null Éxito: success = true data = documento Sin documento en update: success = false Excepción: success = false AppError tipo Exception ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIONES OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ defaultDbAlias: requerido DocumentHtml: requerido no vacío DocumentId: si update > 0 StatusCode: default "saved" FormatCode: siempre "html" ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE NEGOCIO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ solo formato html no guardar HTML vacío soportar create/update no manejar imágenes en este ticket ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PATRÓN ARQUITECTÓNICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Controller ↓ Service ↓ Repository ↓ IDapperCrudEngine ↓ DB ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Unitarias Controller: claim inválido request inválido html vacío éxito create éxito update Unitarias Service: defaultDbAlias inválido html inválido create success update success excepción Unitarias Repository: insert correcto update correcto html vacío error engine Integración: persistencia real create y read update y read QT: flujo completo frontend validación HTTP estructura AppResponses ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN SOLID ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SRP: controller valida service orquesta repository persiste OCP: extensible a imágenes LSP: interfaces reemplazables ISP: interfaces específicas DIP: dependencias por interface ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN DE DEUDA TÉCNICA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Evaluar: soporte de escritura en IDapperCrudEngine duplicación de validaciones naming DTO necesarios separación create/update Clasificar: crítica media menor ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ruta: Docs\GestorDocumental\Editor\ Archivos: Integración Frontend Arquitectura Implementación detallada Pruebas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENTREGABLES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Código: Controller Service Interface Service Repository Interface Repository Model DTO DI Tests Documentación: Integración Frontend Arquitectura Implementación Pruebas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ endpoint funcional valida claim usa service usa repository persiste en DB permite create/update no permite html vacío respuesta AppResponses pruebas completas documentación completa validación SOLID deuda técnica documentada

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-140-implementacion-api-guardar-documento-edi.