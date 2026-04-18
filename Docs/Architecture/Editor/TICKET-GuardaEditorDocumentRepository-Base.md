# Ticket — Ajustar `GuardaEditorDocumentRepository` como repositorio base (NO orquestador)

## Objetivo
Ajustar `GuardaEditorDocumentRepository` para mantener SRP y ser reutilizable por el orquestador Full Save, corrigiendo consistencia técnica (fechas, existencia en update, mapeo columnas) y preservando soporte de transacción externa.

## Requisitos técnicos validados
- NO convertir el repositorio en orquestador.
- Soportar transacción externa: `IDbConnection? conn`, `IDbTransaction? trans`.
- Fechas:
  - INSERT: `created_at=now` y `updated_at=now`.
  - UPDATE: no tocar `created_at`; solo `updated_at=now`.
  - El código no debe setear `CreatedAt=now` en update.
- Update con existencia:
  - Si `DocumentId>0` y no existe → error controlado (pre-check o `rowsAffected==0`).
- Consistencia de columnas:
  - Asegurar mapeo consistente `document_id` ↔ `DocumentId`, etc.; evitar dependencia implícita de `SELECT *` si el mapping no es explícito.

## Pruebas mínimas
- Insert correcto (fechas).
- Update correcto (created no cambia, updated cambia).
- Update inexistente → error controlado.

## Documentación
Actualizar la implementación detallada de “Guardar documento” para agregar sección:
- “Uso como repositorio base dentro del Full Save” (invocación con `conn/trans`, sin orquestación).

