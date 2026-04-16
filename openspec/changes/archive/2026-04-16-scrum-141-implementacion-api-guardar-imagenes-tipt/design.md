## Context

- Jira issue key: SCRUM-141
- Jira summary: IMPLEMENTACION-API-GUARDAR-IMAGENES-TIPTAP
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-141

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — Ticket BE Crear API + Service + Repository: Guardar imágenes del editor Tiptap Rol esperado: Arquitecto de software senior backend (.NET, C#, Core, controllers, services, repositorios, Dapper, manejo de archivos, seguridad, performance, pruebas y documentación técnica). ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar el caso de uso backend para recibir, validar y almacenar imágenes insertadas desde el editor Tiptap, persistiéndolas en la tabla ra_editor_document_images, manteniendo consistencia con la arquitectura actual del proyecto: Controller Service Repository IDapperCrudEngine AppResponses<T> validaciones tempranas manejo controlado de errores validación de claim defaulalias preparación para almacenamiento DB o STORAGE La solución debe permitir: subir imágenes desde el frontend validar tipo y tamaño almacenar metadata de imagen almacenar binario (DB) o ruta (STORAGE) devolver identificador y URL usable por el editor ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO EXISTENTE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Patrón backend actual: Controllers con IClaimValidationService Services con try/catch + AppResponses Repositories con IDapperCrudEngine Models con atributos [Table], [Column] Tabla objetivo: ra_editor_document_images ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Controller: DocuArchi.Api/Controllers/GestorDocumental/Editor/GuardaEditorImageController.cs Service: MiApp.Services/Service/GestorDocumental/Editor/ServiceGuardaEditorImage.cs Repositorio: MiApp.Repository/Repositorio/GestorDocumental/Editor/GuardaEditorImageRepository.cs Modelo: MiApp.Models/Models/GestorDocumental/Editor/RaEditorDocumentImage.cs DTO: MiApp.DTOs/DTOs/GestorDocumental/Editor/ Documentación: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\Editor\ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CASO DE USO FUNCIONAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Escenario: usuario inserta imagen en Tiptap frontend envía archivo al backend backend valida archivo backend almacena imagen backend retorna metadata frontend inserta URL en el HTML ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ TABLA OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ra_editor_document_images Campos: image_id image_uid file_name content_type file_size storage_type_code image_bytes storage_path public_url created_by created_at deleted_at ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELO OFICIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Archivo: MiApp.Models/Models/GestorDocumental/Editor/RaEditorDocumentImage.cs using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiApp.Models.Models.GestorDocumental.Editor
{
    [Table("ra_editor_document_images")]
    public class RaEditorDocumentImage
    {
        [Key]
        [Column("image_id")]
        public long ImageId { get; set; }

        [Required]
        [Column("image_uid")]
        public string ImageUid { get; set; } = null!;

        [Required]
        [Column("file_name")]
        public string FileName { get; set; } = null!;

        [Required]
        [Column("content_type")]
        public string ContentType { get; set; } = null!;

        [Column("file_size")]
        public long FileSize { get; set; }

        [Required]
        [Column("storage_type_code")]
        public string StorageTypeCode { get; set; } = null!;

        [Column("image_bytes")]
        public byte[]? ImageBytes { get; set; }

        [Column("storage_path")]
        public string? StoragePath { get; set; }

        [Column("public_url")]
        public string? PublicUrl { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTRATO OFICIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Request: Multipart/form-data: file (obligatorio) storageType (opcional) Response: AppResponses<RaEditorDocumentImage> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RUTA OFICIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Base: api/gestor-documental/editor Endpoint: POST /api/gestor-documental/editor/image ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE SEGURIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ validar claim defaulalias validar archivo no permitir archivos vacíos NOTA: antivirus NO incluido en este ticket sanitización avanzada se maneja en ticket de seguridad ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — CONTROLLER ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ validar claim validar archivo validar tamaño validar content-type llamar service error → BadRequest éxito → Ok ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — SERVICE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Interface: public interface IServiceGuardaEditorImage
{
    Task<AppResponses<RaEditorDocumentImage>> GuardaEditorImageAsync(
        IFormFile file,
        string defaultDbAlias
    );
} Responsabilidades: validar archivo validar tamaño validar tipo MIME generar image_uid (GUID) definir storage_type_code convertir a byte[] si aplica delegar a repository manejar errores ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — REPOSITORY ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Interface: public interface IGuardaEditorImageRepository
{
    Task<AppResponses<RaEditorDocumentImage>> GuardaEditorImageAsync(
        RaEditorDocumentImage entity,
        string defaultDbAlias
    );
} Reglas: insertar en ra_editor_document_images respetar storage_type_code no SQL concatenado usar engine existente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SEMÁNTICA DE RESPUESTA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Éxito: success = true data = imagen Error: success = false AppError ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIONES OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Archivo: requerido tamaño > 0 tamaño máximo configurable Content-Type permitido: image/png image/jpeg image/jpg image/webp ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE NEGOCIO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ image_uid debe ser único soportar DB o STORAGE no eliminar físicamente en este ticket permitir futura asociación con documentos ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PATRÓN ARQUITECTÓNICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Controller ↓ Service ↓ Repository ↓ IDapperCrudEngine ↓ DB ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Unitarias Controller: claim inválido archivo null archivo vacío Unitarias Service: archivo inválido tipo inválido tamaño inválido success Unitarias Repository: insert correcto error engine Integración: persistencia real validación de datos QT: subida real respuesta válida ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN SOLID ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SRP: controller valida service procesa repository persiste OCP: extensible a storage externo DIP: dependencias por interfaces ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN DE DEUDA TÉCNICA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Evaluar: falta de antivirus manejo de storage externo límites de tamaño duplicación validaciones ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Integración Frontend Arquitectura Implementación Pruebas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENTREGABLES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Código: Controller Service Repository Model DTO DI Tests Documentación: 4 documentos estándar ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ endpoint funcional valida archivo persiste imagen retorna metadata usa AppResponses pruebas completas documentación completa

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-141-implementacion-api-guardar-imagenes-tipt.