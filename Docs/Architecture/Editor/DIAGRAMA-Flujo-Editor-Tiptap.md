# Diagrama — Flujo Editor Tiptap (validado)

```text
[1] Usuario inserta imagen
        │
        ▼
[2] Front → POST /api/gestor-documental/editor/guardar-imagen
        │
        │   Ticket 2
        │   API + Service + Repository:
        │   Guardar imágenes del editor Tiptap
        ▼
[3] Backend valida y guarda imagen
        │
        ├── valida claim `defaulalias`
        ├── valida archivo / MIME / tamaño
        ├── genera `image_uid`
        ├── guarda metadata / binario o storage_path
        └── genera `public_url`
        ▼
[4] Backend responde `public_url` + `image_uid`
        │
        ▼
[5] Tiptap inserta `public_url` en el HTML
        │
        ▼
[6] Usuario guarda documento
        │
        ▼
[7] Front arma request de Full Save
        │
        ├── documentHtml
        ├── documentTitle
        ├── statusCode
        ├── imageUids[]              (uids, luego se limpian/normalizan)
        ├── entityId                ← ej: id_Radicado
        └── contextCode             ← fijo por módulo
                                      ej: RAD_GESTION_RESPUESTA
        │
        ▼
[8] Front → POST /api/gestor-documental/editor/document/full-save
        │
        │   Ticket 9 (actualizado)
        │   API orquestadora Full Save atómico
        ▼
[9] Backend inicia transacción (una sola `conn` + `trans`)
        │
        ├── [9.1] Guardar HTML del documento (repo puro)
        │         │
        │         │   Ticket 1
        │         │   Repository: GuardaEditorDocumentRepository
        │         ▼
        │         ├── INSERT/UPDATE `ra_editor_documents`
        │         └── retorna `document_id`
        │
        ├── [9.2] Resolver catálogo de contexto (lookup)
        │         │
        │         │   Ticket 8 (actualizado)
        │         │   Repository catálogo: SolicitaEditorContextDefinitionRepository
        │         ▼
        │         ├── normaliza `contextCode` (Trim + Upper)
        │         ├── busca `context_code`
        │         ├── valida que exista
        │         ├── valida `is_active = 1`
        │         └── obtiene `context_definition_id`
        │
        ├── [9.3] Guardar relación documento ↔ contexto (idempotente)
        │         │
        │         │   Ticket 8 (actualizado)
        │         │   Service/Repository: GuardaEditorDocumentContext*
        │         │   (consumido internamente por DI, NO por HTTP)
        │         ▼
        │         ├── usa `document_id`
        │         ├── usa `context_definition_id`
        │         ├── usa `entity_id`
        │         ├── evita duplicado activo (idempotencia)
        │         └── persiste en `ra_editor_document_context`
        │
        ├── [9.4] Sincronizar imágenes del documento
        │         │
        │         │   Ticket 3
        │         │   Repository: SincronizaEditorDocumentImagesRepository
        │         ▼
        │         ├── limpia `imageUids[]` (Trim + distinct + remover vacíos)
        │         ├── resuelve `image_id` desde `ra_editor_document_images` (DeletedAt IS NULL)
        │         ├── inserta relaciones nuevas
        │         ├── elimina relaciones obsoletas
        │         └── actualiza `ra_editor_document_image_links`
        │
        ├── [9.5] Validar consistencia final
        │         │
        │         │   Parte del Ticket 9
        │         ▼
        │         ├── documento guardado correctamente
        │         ├── contexto guardado correctamente
        │         ├── imágenes sincronizadas correctamente
        │         └── si algo falla → ROLLBACK
        │
        ▼
[10] COMMIT
        │
        ▼
[11] Backend responde OK + `documentId`
        │
        ▼
[12] Reingreso posterior del usuario (otro día / otra sesión)
        │
        ▼
[13] Front ya NO depende de recordar `documentId`
        │
        ├── contextCode = fijo por módulo
        └── entityId = tomado del proceso de negocio
        │
        ▼
[14] Front → GET /api/gestor-documental/editor/document/by-context?contextCode=RAD_GESTION_RESPUESTA&entityId=911
        │
        │   Ticket 10 (actualizado)
        │   API + Service + Repository:
        │   Obtener documento por contexto (catálogo)
        ▼
[15] Backend resuelve (solo lectura)
        │
        ├── normaliza `contextCode` (Trim + Upper)
        ├── `context_definition_id` desde `ra_editor_context_definitions` (activo)
        ├── `document_id` desde `ra_editor_document_context` (is_active=1)
        │     └── si múltiples activos: tomar más reciente (ORDER BY created_at/context_id DESC LIMIT 1)
        ├── documento desde `ra_editor_documents`
        └── imágenes activas desde links + imágenes (DeletedAt IS NULL, sin bytes)
        │
        ├── si no hay contexto activo → success=true, "Sin resultados", data=null
        └── si hay contexto pero no hay documento → error controlado (inconsistencia)
        │
        ▼
[16] Backend retorna
        │
        ├── HTML persistido
        ├── metadatos del documento
        ├── contexto asociado (incluye ContextCode + EntityId + ContextDefinitionId)
        └── imágenes activas (sin `image_bytes`)
        │
        ▼
[17] Front rehidrata Tiptap con el HTML guardado
        │
        ▼
[18] Limpieza posterior de imágenes huérfanas
        │
        │   Ticket 6
        │   API + Service + Repository + Job:
        │   Eliminación lógica y limpieza de imágenes huérfanas
        ▼
[19] Job o proceso detecta
        │
        ├── imágenes sin relación activa (sin links)
        ├── con antigüedad válida (OlderThanMinutes)
        └── no marcadas previamente (DeletedAt IS NULL)
        │
        ▼
[20] Marca `deleted_at` en imágenes huérfanas (sin borrado físico en esta fase)
```

