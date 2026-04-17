## Context

- Jira issue key: SCRUM-142
- Jira summary: CREAR-API-SINCRONIZA-IMAGENES-EDITOR-TIPTAP
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-142

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — Ticket BE Crear API + Service + Repository: Sincronizar imágenes asociadas a documento del editor Tiptap Rol esperado: Arquitecto de software senior backend (.NET, C#, Core, controllers, services, repositorios, Dapper, integridad transaccional, consistencia de datos, performance, pruebas y documentación técnica). ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar el caso de uso backend para sincronizar las imágenes utilizadas en un documento HTML del editor Tiptap, persistiendo la relación en la tabla ra_editor_document_image_links. Este ticket es responsable de: asociar imágenes a un documento eliminar asociaciones obsoletas garantizar consistencia entre HTML y base de datos evitar imágenes huérfanas preparar el sistema para limpieza futura ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO EXISTENTE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Tickets previos: Guarda documento HTML Guarda imágenes Tablas involucradas: ra_editor_documents ra_editor_document_images ra_editor_document_image_links Patrón backend: Controller Service Repository IDapperCrudEngine AppResponses<T> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Controller: DocuArchi.Api/Controllers/GestorDocumental/Editor/SincronizaEditorDocumentImagesController.cs Service: MiApp.Services/Service/GestorDocumental/Editor/ServiceSincronizaEditorDocumentImages.cs Repositorio: MiApp.Repository/Repositorio/GestorDocumental/Editor/SincronizaEditorDocumentImagesRepository.cs DTO: MiApp.DTOs/DTOs/GestorDocumental/Editor/ Documentación: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\Editor\ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CASO DE USO FUNCIONAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Escenario: usuario edita documento en Tiptap HTML contiene imágenes (URLs o identificadores) frontend envía lista de image_uid utilizadas backend sincroniza: inserta nuevas relaciones elimina relaciones no usadas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ TABLA OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ra_editor_document_image_links Campos: link_id document_id image_id created_at ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTRATO OFICIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Request: public class SincronizaEditorDocumentImagesRequest
{
    public long DocumentId { get; set; }
    public List<string> ImageUids { get; set; } = new();
} Response: AppResponses<bool> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RUTA OFICIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Base: api/gestor-documental/editor Endpoint: POST /api/gestor-documental/editor/document/images/sync ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE SEGURIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ validar claim defaulalias validar DocumentId validar lista de imágenes ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — CONTROLLER ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ validar claim validar request validar DocumentId > 0 validar lista llamar service error → BadRequest éxito → Ok ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — SERVICE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Interface: public interface IServiceSincronizaEditorDocumentImages
{
    Task<AppResponses<bool>> SincronizaAsync(
        SincronizaEditorDocumentImagesRequest request,
        string defaultDbAlias
    );
} Responsabilidades: validar parámetros obtener imágenes existentes del documento obtener imágenes válidas por UID calcular diferencias: nuevas relaciones relaciones a eliminar ejecutar operación transaccional delegar a repository ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — REPOSITORY ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Interface: public interface ISincronizaEditorDocumentImagesRepository
{
    Task<AppResponses<bool>> SincronizaAsync(
        long documentId,
        List<long> imageIds,
        string defaultDbAlias
    );
} Responsabilidades: eliminar relaciones no presentes insertar nuevas relaciones garantizar UNIQUE(document_id, image_id) operación debe ser transaccional ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SEMÁNTICA DE RESPUESTA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Éxito: success = true data = true Error: success = false AppError ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIONES OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DocumentId: requerido ImageUids: lista no null permitir lista vacía (limpia relaciones) Validar: existencia de imágenes no usar imágenes eliminadas (deleted_at) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE NEGOCIO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ una imagen puede estar en múltiples documentos un documento puede tener múltiples imágenes eliminar relación NO elimina imagen sincronización reemplaza estado completo operación debe ser idempotente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PATRÓN ARQUITECTÓNICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Controller ↓ Service ↓ Repository ↓ DB ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Unitarias Controller: claim inválido request inválido Unitarias Service: DocumentId inválido lista vacía nuevas relaciones eliminación relaciones Unitarias Repository: insert delete transacción correcta Integración: sincronización real consistencia datos QT: flujo completo editor guardado documento + imágenes ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN SOLID ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SRP: controller valida service orquesta repository ejecuta OCP: preparado para limpieza automática futura DIP: interfaces desacopladas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN DE DEUDA TÉCNICA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Evaluar: necesidad de limpieza de imágenes huérfanas performance en grandes volúmenes uso de transacciones ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Integración Frontend Arquitectura Implementación Pruebas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENTREGABLES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Código: Controller Service Repository DTO DI Tests Documentación: 4 documentos estándar ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ endpoint funcional sincroniza correctamente elimina relaciones obsoletas inserta nuevas no duplica respeta integridad usa AppResponses pruebas completas documentación completa

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-142-crear-api-sincroniza-imagenes-editor-tip.