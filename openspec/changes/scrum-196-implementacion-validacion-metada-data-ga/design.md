## Context

- Jira issue key: SCRUM-196
- Jira summary: IMPLEMENTACION-VALIDACION-METADA-DATA-GABINETE
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-196

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT — Metadata real de gabinete + validación estructural backend (FASE CRÍTICA — BLOQUEANTE FUNCIONAL DEL STORAGE ENGINE) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy validación dinámica por gabinete metadata documental Clean Architecture Dapper / QueryOptions validaciones backend estructurales SGDEA / ECM migración VB → C# con paridad funcional ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PROBLEMA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualmente: POST /api/gestor-documental/almacenamiento falla con: GAB_FIELDS_NOT_FOUND porque: StorageGabineteMetadataProvider retorna: new List<GabineteFieldMetadata>() (placeholder vacío). Además: - no existe validación real de tipo
- no existe validación de longitud
- no existe validación estructural backend por metadata ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ EVIDENCIA TÉCNICA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Provider placeholder D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Metadata/StorageGabineteMetadataProvider.cs retorna: new List<GabineteFieldMetadata>() Validator actual D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Validation/GabineteRequiredFieldsValidator.cs dispara: GAB_FIELDS_NOT_FOUND Fuente de metadata Existe tabla legacy: detalle_gabienete Archivo de evidencia: C:\cert\resultado-detalle.txt ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar resolución real de metadata documental por gabinete y validación backend estructural antes de insertar. Debe validar: ✔ existencia del campo ✔ obligatoriedad backend ✔ longitud ✔ compatibilidad de tipo ✔ consistencia con estructura física del gabinete ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ALCANCE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar: 1. Provider real de metadata.
2. Repositorio real basado en detalle_gabienete.
3. Validación backend por tipo y longitud.
4. Validación de existencia real del campo.
5. Cache de metadata por gabinete.
6. Integración con ValidationPipeline. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS FUNCIONALES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Resolución de metadata Resolver por: GABINETE = NombreGabinete Case-insensitive. Tipos soportados mínimos VARCHAR(n)
CHAR(n)
INT
INTEGER
DATE
DATETIME
TEXT
LONGTEXT Validaciones backend Campo inexistente GAB_FIELD_UNKNOWN Longitud inválida GAB_FIELD_LENGTH_INVALID Tipo inválido GAB_FIELD_TYPE_INVALID Metadata inexistente GAB_FIELDS_NOT_FOUND Metadata inconsistente GAB_METADATA_INCONSISTENT Tipo no soportado GAB_TYPE_UNSUPPORTED Desalineación física GAB_SCHEMA_MISMATCH ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE OBLIGATORIEDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Nuevo campo: IS_REQUIRED_BACKEND Reglas: 1 → campo obligatorio
0 → campo opcional IMPORTANTE CAMPO_ENABLE_DISABLE NO controla validación backend. Solo representa comportamiento UI: - habilitar textbox
- readonly
- bloquear edición NO debe: - eliminar validación backend
- permitir inconsistencias estructurales Separación obligatoria: UI state ≠ integridad backend ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SQL BASE SUGERIDO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SELECT
    GABINETE,
    CAMPO,
    TIPO,
    ESTADO,
    CAMPO_ENABLE_DISABLE,
    IS_REQUIRED_BACKEND
FROM detalle_gabienete
WHERE UPPER(GABINETE) = UPPER(@gabinete); ⚠️ IMPORTANTE: No asumir nombres si existen diferencias entre ambientes. Debe validarse contra el esquema real. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN DE ESTRUCTURA FÍSICA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Debe validarse opcionalmente contra: INFORMATION_SCHEMA.COLUMNS Objetivo: evitar metadata de campos inexistentes físicamente Debe detectar: - campo definido en metadata pero inexistente en tabla
- tipo incompatible
- longitud incompatible ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CACHE OBLIGATORIO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar cache por: alias + gabinete Reglas: - TTL configurable
- cache hit/miss logueado
- invalidación posible Objetivo: no consultar metadata en cada request ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Services MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Metadata/ Archivos: IProveedorMetadataGabinete.cs
ProveedorMetadataGabinete.cs

IValidadorCamposGabinete.cs
ValidadorCamposGabinete.cs Repository MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/GabineteMetadata/ Archivos: IRepositorioMetadataGabinete.cs
RepositorioMetadataGabinete.cs Models MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Metadata/ Archivos: MetadataCampoGabineteModel.cs
ResultadoMetadataGabineteModel.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLA DE NOMENCLATURA (OBLIGATORIA) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PROHIBIDO Resolve
Handler
Manager
Helper
Utils
GenericService
GenericRepository OBLIGATORIO Los nombres deben representar exactamente la funcionalidad. Ejemplos válidos: ConsultarMetadataCamposGabineteAsync
ValidarLongitudCampoGabinete
ValidarTipoDatoCampoGabinete
ConsultarCamposGabinetePorNombreAsync IMPORTANTE NO usar términos en inglés Toda clase, interfaz y función debe escribirse en español técnico consistente con el proyecto. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS TÉCNICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - usar DapperCrudEngine por defecto
- SQL parametrizado obligatorio
- no concatenar SQL
- no romper contratos del ValidationPipeline
- no loguear datos sensibles
- no confiar en frontend ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UNITARIAS — PARSER DE TIPOS Validar: VARCHAR(20)
varchar(20)
VARCHAR (20)
CHAR(10)
INT
INTEGER
DATE
DATETIME
TEXT
LONGTEXT Casos: - tipo soportado → OK
- tipo inválido → GAB_TYPE_UNSUPPORTED
- longitud parseada correctamente
- espacios/casing legacy soportados UNITARIAS — VALIDACIÓN DE LONGITUD Casos: - VARCHAR(20) con 20 chars → OK
- VARCHAR(20) con 21 chars → GAB_FIELD_LENGTH_INVALID
- CHAR(10) exacto → OK
- CHAR(10) excedido → error
- TEXT/LONGTEXT → no validar longitud fija UNITARIAS — VALIDACIÓN DE TIPO Casos: - INT válido → OK
- INT con texto → GAB_FIELD_TYPE_INVALID
- DATE válida → OK
- DATE inválida → GAB_FIELD_TYPE_INVALID
- DATETIME válido → OK UNITARIAS — VALIDACIÓN DE EXISTENCIA Casos: - campo existente → OK
- campo inexistente → GAB_FIELD_UNKNOWN
- metadata vacía → GAB_FIELDS_NOT_FOUND
- metadata inconsistente → GAB_SCHEMA_MISMATCH UNITARIAS — VALIDACIÓN IS_REQUIRED_BACKEND Casos: - IS_REQUIRED_BACKEND = 1 + vacío → error
- IS_REQUIRED_BACKEND = 1 + valor → OK
- IS_REQUIRED_BACKEND = 0 + vacío → OK UNITARIAS — CAMPO_ENABLE_DISABLE Casos: - CAMPO_ENABLE_DISABLE = 0 → backend sigue validando
- CAMPO_ENABLE_DISABLE = 1 → backend sigue validando Regla: CAMPO_ENABLE_DISABLE NO altera integridad backend. UNITARIAS — CACHE Casos: - primer request → cache miss
- segundo request → cache hit
- expiración TTL → recarga metadata
- invalidación manual → recarga metadata INTEGRACIÓN — BASE DE DATOS REAL Validar: - lectura real de detalle_gabienete
- lectura real de INFORMATION_SCHEMA.COLUMNS
- gabinete CONTABIL
- metadata física consistente
- metadata física inconsistente INTEGRACIÓN — VALIDATION PIPELINE Casos: - pipeline válido → continúa
- metadata inexistente → corta flujo
- tipo inválido → corta flujo
- longitud inválida → corta flujo REGRESIÓN LEGACY Comparar comportamiento contra: Consulta_Campos_Obligatorio Validar: - obligatoriedad
- orden de campos
- consistencia gabinete
- mensajes de error equivalentes PERFORMANCE Validar: - cache reduce consultas repetidas
- no degradación significativa del pipeline CRITERIOS DE ÉXITO DE PRUEBAS ✔ parser legacy-compatible
✔ tipos legacy soportados
✔ longitud validada
✔ metadata consistente
✔ cache funcionando
✔ pipeline estable
✔ sin regresiones ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Logs obligatorios: requestId
gabinete
campo
tipo
resultado
cache hit/miss
duración NO loguear: valores sensibles
fulltext
datos documentales ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualizar en: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\Arquitectura-Final\ Archivos: SCRUM-189-Arquitectura-Metadata-Gabinete.md
SCRUM-189-Implementacion-Metadata-Gabinete.md
SCRUM-189-Pruebas-Metadata-Gabinete.md
SCRUM-189-Observabilidad-Metadata-Gabinete.md
SCRUM-189-Regresion-Legacy-Metadata-Gabinete.md
SCRUM-189-Metadata.md Debe incluir: - estructura detalle_gabienete
- separación UI/backend
- parser de tipos
- validaciones
- cache
- validación INFORMATION_SCHEMA
- evidencia de pruebas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ✔ metadata real desde DB
✔ no más placeholder
✔ validación de longitud
✔ validación de tipo
✔ validación de existencia
✔ validación backend requerida
✔ cache funcionando
✔ pruebas en verde
✔ documentación actualizada
✔ nombres técnicos en español
✔ sin nombres genéricos ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RIESGOS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - inconsistencia metadata vs tabla física
- tipos legacy no estandarizados
- cache desactualizado ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRIORIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ALTA / BLOQUEANTE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar resolución real de metadata de gabinete y validación backend estructural usando detalle_gabienete como fuente principal, garantizando consistencia documental y compatibilidad con el Storage Engine legacy.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-196-implementacion-validacion-metada-data-ga.