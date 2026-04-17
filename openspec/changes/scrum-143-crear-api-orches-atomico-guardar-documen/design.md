## Context

- Jira issue key: SCRUM-143
- Jira summary: CREAR-API-ORCHES-ATOMICO-GUARDAR-DOCUMENTO-EDITOR
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-143

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — Ticket BE Crear API + Service + Repository: Full Save atómico (Documento + Imágenes) del editor Tiptap Rol esperado: Arquitecto de software senior backend (.NET, C#, Core, controllers, services, repositorios, Dapper, transacciones, consistencia de datos, diseño enterprise, pruebas y documentación técnica). ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar un endpoint orquestador que permita guardar de forma atómica: el documento HTML del editor Tiptap la sincronización de imágenes asociadas garantizando que ambas operaciones se ejecuten dentro de una misma transacción. Este endpoint resuelve el problema de inconsistencia cuando: el documento se guarda pero la sincronización de imágenes falla ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO EXISTENTE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Tickets previos implementados: Guarda documento HTML Guarda imágenes Sincroniza imágenes Problema actual: las APIs están separadas no existe atomicidad riesgo de inconsistencia Solución: crear API orquestadora (Full Save) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Controller: DocuArchi.Api/Controllers/GestorDocumental/Editor/FullSaveEditorDocumentController.cs Service: MiApp.Services/Service/GestorDocumental/Editor/ServiceFullSaveEditorDocument.cs Repositorio: (se reutilizan los existentes) DTO: MiApp.DTOs/DTOs/GestorDocumental/Editor/ Documentación: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\Editor\ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CASO DE USO FUNCIONAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Escenario completo: usuario edita documento usuario sube imágenes (ya guardadas previamente) Tiptap genera HTML con URLs frontend envía: HTML lista de image_uid backend ejecuta: guardar documento sincronizar imágenes todo en una sola transacción ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTRATO OFICIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Request: public class FullSaveEditorDocumentRequest
{
    public long? DocumentId { get; set; }
    public long? TemplateId { get; set; }
    public int? TemplateVersion { get; set; }
    public string? DocumentTitle { get; set; }
    public string DocumentHtml { get; set; } = null!;
    public string? StatusCode { get; set; }
    public List<string> ImageUids { get; set; } = new();
} Response: AppResponses<RaEditorDocument> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RUTA OFICIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Base: api/gestor-documental/editor Endpoint: POST /api/gestor-documental/editor/document/full-save ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE SEGURIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ validar claim defaulalias validar request validar HTML validar lista de imágenes ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — CONTROLLER ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ validar claim validar request validar DocumentHtml llamar service FullSave error → BadRequest éxito → Ok ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — SERVICE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Interface: public interface IServiceFullSaveEditorDocument
{
    Task<AppResponses<RaEditorDocument>> FullSaveAsync(
        FullSaveEditorDocumentRequest request,
        string defaultDbAlias
    );
} Responsabilidades: validar parámetros iniciar transacción ejecutar: guardar documento (Ticket 1) sincronizar imágenes (Ticket 3) hacer commit si todo ok hacer rollback si falla ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — REPOSITORY ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ No crear repositorio nuevo. Reutilizar: GuardaEditorDocumentRepository SincronizaEditorDocumentImagesRepository IMPORTANTE: ambos deben soportar ejecución dentro de la misma transacción ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ TRANSACCIÓN (CRÍTICO) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Flujo: BEGIN TRANSACTION guardar documento obtener document_id sincronizar imágenes COMMIT Si falla cualquiera: ROLLBACK ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SEMÁNTICA DE RESPUESTA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Éxito: success = true data = documento Error: success = false rollback ejecutado AppError ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIONES OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DocumentHtml obligatorio defaultDbAlias obligatorio DocumentId válido si update ImageUids puede ser vacío ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE NEGOCIO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ operación es atómica HTML y relaciones siempre consistentes lista vacía elimina relaciones no se eliminan imágenes físicas idempotente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PATRÓN ARQUITECTÓNICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Controller ↓ Service (orquestador) ↓ Repositories existentes ↓ DB ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Unitarias Controller: claim inválido request inválido Unitarias Service: create success update success falla en sync → rollback falla en save → rollback Integración: transacción completa consistencia final QT: flujo completo editor guardar con imágenes error controlado ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN SOLID ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SRP: service orquesta OCP: extensible a validaciones futuras DIP: usa repositorios existentes ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN DE DEUDA TÉCNICA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Evaluar: soporte transaccional del engine manejo de rollback logs de error performance ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Integración Frontend Arquitectura Implementación Pruebas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENTREGABLES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Código: Controller FullSave Service FullSave DTO DI Tests Documentación: 4 documentos estándar ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ endpoint full-save funcional transacción implementada rollback funcional no hay inconsistencias usa AppResponses reutiliza repositorios pruebas completas documentación completa

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-143-crear-api-orches-atomico-guardar-documen.