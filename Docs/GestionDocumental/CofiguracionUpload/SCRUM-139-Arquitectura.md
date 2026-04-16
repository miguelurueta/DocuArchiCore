# SCRUM-139 — Arquitectura — Configuración Upload

## Descripción

Endpoint para consultar `ra_config_upload_gestion` filtrando únicamente por `NAME_PROCESO`.

## Flujo

Controller → Service → Repository → `IDapperCrudEngine` (`QueryOptions`)

## Decisiones

- Validación de claim `defaulalias` vía `IClaimValidationService`.
- `nameProceso` requerido (querystring).
- Repository filtra solo por `NAME_PROCESO`.

## SOLID / Deuda técnica

- Single Responsibility: controller valida y delega; service orquesta; repository consulta.
- Deuda: tests de contrato existentes fallan por `ObjectResult` vs `Ok/BadRequest` (fuera de alcance del ticket).

