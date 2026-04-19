# SCRUM-148 — Arquitectura: Full Save Editor Document (Catálogo)

## Propósito

El Full Save es el **service orquestador** que garantiza atomicidad (una sola transacción) entre:

1) Guardar documento HTML  
2) Resolver catálogo de contexto (`ContextCode`)  
3) Guardar relación documento ↔ contexto  
4) Sincronizar imágenes

## Capas

Controller → Service (orquestador) → Repositorios → BD

### Controller

- Valida claim `defaulalias`
- Valida request y campos mínimos
- Delegación al service

### Service Full Save (orquestador)

- Abre `IDbConnection` y `IDbTransaction`
- Ejecuta repositorios reutilizados pasando `conn/trans`
- Ejecuta `COMMIT` solo si todo OK
- Ejecuta `ROLLBACK` ante cualquier error o excepción

## Repositorios reutilizados

- `IGuardaEditorDocumentRepository` (documento)
- `ISolicitaEditorContextDefinitionRepository` (catálogo)
- `IGuardaEditorDocumentContextRepository` (contexto)
- `ISincronizaEditorDocumentImagesRepository` (imágenes)

## Invariantes

- No debe persistir documento sin contexto cuando el request exige contexto.
- No debe persistir documento/contexto si falla sincronización de imágenes.
- No debe haber commit parcial.

