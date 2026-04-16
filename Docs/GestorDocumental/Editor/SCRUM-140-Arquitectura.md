# SCRUM-140 — Arquitectura — Guardar documento (Editor)

## Flujo

Controller → Service → Repository → `IDapperCrudEngine` (`InsertAsync`/`UpdateAsync`/`GetAllAsync`)

## Seguridad

- Claim obligatorio: `defaulalias` (`IClaimValidationService`)

## Persistencia

- Tabla: `ra_editor_documents`
- Create: `InsertAsync` y luego `GetAllAsync` por `DocumentId`
- Update: validar existencia (`GetAllAsync`) y luego `UpdateAsync`

## SOLID / Deuda técnica

- SRP: controller valida request/claim; service orquesta; repository persiste/consulta.
- Deuda: definición final de nombres de columnas (snake_case vs PascalCase) debe confirmarse con el esquema real.

