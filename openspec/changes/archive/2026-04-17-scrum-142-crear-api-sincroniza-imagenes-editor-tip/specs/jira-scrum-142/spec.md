## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-142.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-142

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Sincronizar imágenes usadas por documento
El sistema MUST exponer un endpoint para sincronizar (insertar/eliminar) las relaciones entre un documento del editor y las imágenes que el HTML/JSON actualmente referencia.

#### Contrato
- **RUTA**: `POST /api/gestor-documental/editor/document/images/sync`
- **Request**: `SincronizaEditorDocumentImagesRequestDto { long DocumentId, List<string> ImageUids }`
- **Response**: `AppResponses<bool>` con `data=true` cuando la sincronización fue aplicada.

#### Validaciones
- **WHEN** el claim `defaulalias` no existe o es inválido **THEN** retorna `400 BadRequest`.
- **WHEN** `DocumentId <= 0` **THEN** retorna `400 BadRequest`.
- **WHEN** `ImageUids` es `null` **THEN** retorna `400 BadRequest`.
- **WHEN** `ImageUids` contiene uids inexistentes o eliminados (`ra_editor_document_images.DeletedAt IS NOT NULL`) **THEN** retorna `400 BadRequest`.

#### Reglas de negocio
- **WHEN** `ImageUids` viene vacío **THEN** elimina todas las relaciones existentes para el documento (limpia).
- **WHEN** `ImageUids` contiene elementos repetidos **THEN** la operación es idempotente (sin duplicar relaciones).
- La sincronización reemplaza el estado completo de relaciones del documento.
- Eliminar relaciones NO elimina registros de `ra_editor_document_images`.

#### Persistencia y consistencia
- La operación MUST ser transaccional.
- La relación se persiste en `ra_editor_document_image_links (link_id, document_id, image_id, created_at)`.
