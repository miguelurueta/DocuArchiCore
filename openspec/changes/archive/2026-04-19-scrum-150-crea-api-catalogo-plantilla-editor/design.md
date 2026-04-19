## Context

- Jira issue key: SCRUM-150
- Jira summary: CREA-API-CATALOGO-PLANTILLA-EDITOR
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-150

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — Ticket BE Crear API + Service + Repository: Catálogo de plantillas HTML del editor Rol esperado: Arquitecto de software senior backend (.NET, C#, Core, controllers, services, repositorios, Dapper, modelado documental, versionado, pruebas y documentación técnica). ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar el catálogo base de plantillas HTML del editor Tiptap para permitir que el sistema gestione múltiples plantillas reutilizables por proceso de negocio, evitando hardcodeo en frontend o backend y dejando la solución preparada para crecimiento funcional. La solución debe contemplar: definición lógica de plantilla versionado de plantilla activación/inactivación persistencia del HTML base desacoplamiento respecto al módulo de negocio preparación para selección dinámica por contexto ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO EXISTENTE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Situación previa: el editor Tiptap requiere un HTML inicial para comenzar edición ese HTML debe salir de una plantilla, no de contenido hardcodeado la aplicación puede crecer hacia múltiples tipos de respuesta ya existe un modelo de contexto de negocio desacoplado del documento ( ra_editor_context_definitions , ra_editor_document_context ) se requiere una infraestructura reusable para múltiples plantillas Problema: si el HTML base queda quemado en código, cada nueva plantilla obliga a cambiar frontend/backend si no existe versionado, será difícil mantener evolución de estructura documental Solución: crear catálogo formal de plantillas separar definición de plantilla y versión de plantilla permitir que una plantilla tenga múltiples versiones ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO FUNCIONAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Permitir registrar y consultar plantillas HTML del editor que puedan ser usadas luego por el motor de hidratación para generar el contenido inicial del documento. La plantilla debe representar únicamente la estructura base HTML y no el documento ya diligenciado. Ejemplo de uso: plantilla de respuesta a correspondencia plantilla de memorando interno plantilla de nota a expediente plantilla de comentario enriquecido de workflow ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Controller: DocuArchi.Api/Controllers/GestorDocumental/Editor/TemplateDefinitionsController.cs Service: MiApp.Services/Service/GestorDocumental/Editor/ServiceTemplateDefinitions.cs Repositorio: MiApp.Repository/Repositorio/GestorDocumental/Editor/TemplateDefinitionsRepository.cs Modelos: MiApp.Models/Models/GestorDocumental/Editor/RaEditorTemplateDefinition.cs MiApp.Models/Models/GestorDocumental/Editor/RaEditorTemplateVersion.cs DTO: MiApp.DTOs/DTOs/GestorDocumental/Editor/ Documentación: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\Editor\ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ TABLAS OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ra_editor_template_definitions ra_editor_template_versions Campos esperados en definición: template_definition_id template_code template_name module_name description is_active created_by created_at Campos esperados en versión: template_version_id template_definition_id version_number template_html is_active is_published created_by created_at ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS OFICIALES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Archivo: MiApp.Models/Models/GestorDocumental/Editor/RaEditorTemplateDefinition.cs using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiApp.Models.Models.GestorDocumental.Editor
{
    [Table("ra_editor_template_definitions")]
    public class RaEditorTemplateDefinition
    {
        [Key]
        [Column("template_definition_id")]
        public long TemplateDefinitionId { get; set; }

        [Required]
        [Column("template_code")]
        public string TemplateCode { get; set; } = null!;

        [Required]
        [Column("template_name")]
        public string TemplateName { get; set; } = null!;

        [Column("module_name")]
        public string? ModuleName { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
} Archivo: MiApp.Models/Models/GestorDocumental/Editor/RaEditorTemplateVersion.cs using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiApp.Models.Models.GestorDocumental.Editor
{
    [Table("ra_editor_template_versions")]
    public class RaEditorTemplateVersion
    {
        [Key]
        [Column("template_version_id")]
        public long TemplateVersionId { get; set; }

        [Column("template_definition_id")]
        public long TemplateDefinitionId { get; set; }

        [Column("version_number")]
        public int VersionNumber { get; set; }

        [Required]
        [Column("template_html")]
        public string TemplateHtml { get; set; } = null!;

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("is_published")]
        public bool IsPublished { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTRATOS OFICIALES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DTO Request — Crear definición: public sealed class CreaEditorTemplateDefinitionDto
{
    public string TemplateCode { get; set; } = null!;
    public string TemplateName { get; set; } = null!;
    public string? ModuleName { get; set; }
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }
} DTO Request — Crear versión: public sealed class CreaEditorTemplateVersionDto
{
    public string TemplateCode { get; set; } = null!;
    public int VersionNumber { get; set; }
    public string TemplateHtml { get; set; } = null!;
    public bool IsPublished { get; set; }
    public string? CreatedBy { get; set; }
} Response: AppResponses<RaEditorTemplateDefinition> AppResponses<RaEditorTemplateVersion> AppResponses<List<RaEditorTemplateVersion>> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RUTAS OFICIALES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Base: api/gestor-documental/editor/templates Endpoints: POST /api/gestor-documental/editor/templates POST /api/gestor-documental/editor/templates/version GET  /api/gestor-documental/editor/templates/{templateCode} GET  /api/gestor-documental/editor/templates/{templateCode}/versions ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE SEGURIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ validar claim defaulalias validar request validar TemplateCode validar TemplateName validar TemplateHtml en versión no permitir duplicar template_code no permitir duplicar version_number para la misma plantilla ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — CONTROLLER ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Nombre: TemplateDefinitionsController Responsabilidades: validar claim defaulalias validar request nulo validar campos obligatorios invocar service retornar BadRequest en errores controlados retornar Ok en éxito Restricciones: no consultar repositorio directamente no contener lógica de negocio no decidir publicación/activación fuera del patrón establecido ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — SERVICE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Interface: public interface IServiceTemplateDefinitions
{
    Task<AppResponses<RaEditorTemplateDefinition?>> CreaTemplateDefinitionAsync(
        CreaEditorTemplateDefinitionDto request,
        string defaultDbAlias
    );

    Task<AppResponses<RaEditorTemplateVersion?>> CreaTemplateVersionAsync(
        CreaEditorTemplateVersionDto request,
        string defaultDbAlias
    );

    Task<AppResponses<RaEditorTemplateDefinition?>> SolicitaTemplateByCodeAsync(
        string templateCode,
        string defaultDbAlias
    );

    Task<AppResponses<List<RaEditorTemplateVersion>>> SolicitaTemplateVersionsAsync(
        string templateCode,
        string defaultDbAlias
    );
} Responsabilidades: validar defaultDbAlias validar TemplateCode validar TemplateName validar TemplateHtml homologar strings con Trim() resolver definición de plantilla por código cuando aplique crear definición crear versión consultar definición y versiones manejar errores con try/catch ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — REPOSITORY ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Interface: public interface ITemplateDefinitionsRepository
{
    Task<AppResponses<RaEditorTemplateDefinition?>> CreaTemplateDefinitionAsync(
        CreaEditorTemplateDefinitionDto request,
        string defaultDbAlias
    );

    Task<AppResponses<RaEditorTemplateVersion?>> CreaTemplateVersionAsync(
        long templateDefinitionId,
        CreaEditorTemplateVersionDto request,
        string defaultDbAlias
    );

    Task<AppResponses<RaEditorTemplateDefinition?>> SolicitaTemplateByCodeAsync(
        string templateCode,
        string defaultDbAlias
    );

    Task<AppResponses<List<RaEditorTemplateVersion>>> SolicitaTemplateVersionsAsync(
        long templateDefinitionId,
        string defaultDbAlias
    );
} Responsabilidades: crear definición de plantilla validar unicidad de template_code consultar definición por template_code crear versión de plantilla validar unicidad de (template_definition_id, version_number) consultar versiones por plantilla Reglas: no usar SQL concatenado manualmente permitir múltiples versiones por plantilla la versión debe quedar asociada a una definición existente no crear versión si la plantilla no existe soportar consultas ordenadas por versión ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE NEGOCIO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ template_code debe ser único. Una plantilla puede tener múltiples versiones. version_number debe ser único por plantilla. Solo las versiones publicadas/activas podrán ser usadas por el motor de hidratación. El HTML aquí almacenado es plantilla base, no documento diligenciado. La solución debe ser transversal y no acoplada a un solo módulo. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIONES OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ defaultDbAlias: requerido TemplateCode: requerido no null no empty no whitespace TemplateName: requerido no null no empty no whitespace VersionNumber: requerido mayor a 0 TemplateHtml: requerido no null no empty no whitespace ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS UNITARIAS — Controller claim defaulalias ausente → 400 request null → 400 TemplateCode vacío → 400 TemplateName vacío → 400 TemplateHtml vacío → 400 success crear definición → 200 success crear versión → 200 success consultar definición → 200 success consultar versiones → 200 PRUEBAS UNITARIAS — Service defaultDbAlias inválido → error controlado TemplateCode inválido → error controlado TemplateName inválido → error controlado VersionNumber inválido → error controlado TemplateHtml inválido → error controlado crear definición → success true crear versión → success true duplicado → error controlado excepción del repositorio → error controlado PRUEBAS UNITARIAS — Repository inserta definición nueva evita duplicado de TemplateCode inserta versión nueva evita duplicado de VersionNumber por plantilla consulta definición por código consulta versiones por plantilla PRUEBAS DE INTEGRACIÓN flujo completo controller → service → repository inserción real en ra_editor_template_definitions inserción real en ra_editor_template_versions consulta real de definición consulta real de versiones PRUEBAS QT / PUNTA A PUNTA crear plantilla desde endpoint crear versión desde endpoint consultar plantilla consultar versiones validar códigos HTTP validar estructura de response ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN SOLID ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SRP Controller valida y delega Service orquesta Repository persiste/consulta OCP el diseño permite múltiples plantillas y versiones sin romper consumers LSP interfaces reemplazables ISP interfaces específicas del catálogo de plantillas DIP controller depende de service por interface service depende de repository por interface ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN DE DEUDA TÉCNICA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Evaluar y documentar: si el HTML de plantilla requiere validación adicional si debe existir una versión “vigente” por contexto si conviene auditoría más detallada de cambios consistencia de nombres físicos posibilidad futura de administración UI de plantillas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ubicación: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\Editor\ Crear: SCRUM-[ID]-Arquitectura-TemplateDefinitions.md SCRUM-[ID]-Implementacion-Detallada-TemplateDefinitions.md SCRUM-[ID]-Pruebas-TemplateDefinitions.md SCRUM-[ID]-Integracion-Frontend-TemplateDefinitions.md En el documento de arquitectura crea los siguientes diagramas: Casos de uso Diagramas de estado Diagramas de secuencia Diagrama de clases Diagramas de tablas relacionadas e implementadas por la Api. El diagrama de secuencia debe tener una descripción literal paso a paso. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENTREGABLES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Código: TemplateDefinitionsController.cs ServiceTemplateDefinitions.cs ITemplateDefinitionsRepository TemplateDefinitionsRepository.cs modelos de definición y versión DTOs DI pruebas Base de datos: scripts de ra_editor_template_definitions scripts de ra_editor_template_versions ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ existen tablas de definición y versión existe API de creación y consulta existe soporte para múltiples plantillas existe soporte de versionado existen pruebas existe documentación

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-150-crea-api-catalogo-plantilla-editor.