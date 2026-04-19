# SCRUM-151 — Pruebas: Initial Content Editor

## Unitarias — Controller

Casos mínimos:

- claim inválido → 400
- `idTareaWf <= 0` → 400
- `contextCode` vacío → 400
- `entityId <= 0` → 400
- service OK → 200

## Unitarias — Service

Casos mínimos:

- alias inválido → Validation
- ContextCode inválido/inactivo → Validation
- plantilla inexistente → Validation
- tokens faltantes → Validation
- OK → retorna `htmlInicial` y metadata

## Integración (pendiente)

Recomendado con MySQL Testcontainers/Docker:

- catálogo de contextos + plantillas + versiones
- lectura de estructura por `idTareaWf`
- render final del HTML

