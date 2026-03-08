## Context

- Jira issue key: SCRUM-42
- Jira summary: CREA-API-RADICACION
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-42

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

El flujo legacy `Registra_Radicacion_entrante` concentra validaciones, autorizaciones y persistencia multi-tabla en un metodo monolitico acoplado a WebForms (`Session`, `FindControl`, `HiddenField`, `ViewState`). Para migrarlo, se requiere una API desacoplada que mantenga comportamiento funcional, contratos estables y transaccion atomica.

## Scope

1. Endpoints:
   - `POST /api/radicacion/registrar-entrante`
   - `POST /api/radicacion/validar-entrante`
   - `GET /api/radicacion/flujo-inicial`
2. Mapeo de entrada legacy a DTO canonico con claims (`usuarioid`, `defaultDbAlias`).
3. Persistencia con matriz SQL Q01-Q09:
   - Q01-Q08 dentro de transaccion unica.
   - Q09 condicional segun rama funcional.
4. Pruebas de paridad funcional y reglas tecnicas obligatorias.

## Out Of Scope

1. Cambios funcionales ajenos al flujo `Registra_Radicacion_entrante`.
2. Reescritura de modulos no relacionados con radicacion/tramite.
3. Activar archivado OpenSpec (`opsxj:archive`) antes de merge de PRs impactados.

## Design Decisions

1. Patron de capas obligatorio: `Controller -> Service -> Repository -> DTO/Mapping`.
2. Contrato de salida uniforme: `AppResponses<T>`.
3. SQL 100% parametrizado (sin concatenacion).
4. `QueryOptions.DefaultAlias = defaultDbAlias` obligatorio en repositorios.
5. Atomicidad obligatoria:
   - Si falla cualquier paso Q01-Q08, rollback total y sin persistencia parcial.
6. Tabla dinamica de plantilla permitida solo con whitelist.

## Acceptance Baseline

1. Paridad funcional validada para escenarios criticos de legacy.
2. Endpoint `registrar-entrante` opera con transaccion atomica.
3. Endpoint `validar-entrante` separa validacion previa del registro.
4. Endpoint `flujo-inicial` entrega contrato minimo consumible por frontend.
5. Evidencia de pruebas unitarias/integracion/contract en PR.

## Approach

1. Tomar como referencia funcional el documento maestro:
   - `Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/TICKETS-F1-F7-Registra_Radicacion_entrante.md`
2. Implementar por etapas:
   - contratos API y validaciones
   - persistencia transaccional
   - pruebas de paridad
3. Coordinar implementacion multi-repo desde `DocuArchiCore` como orquestador.
4. Validar OpenSpec durante refinamiento e implementacion:
   - `openspec.cmd validate scrum-42-crea-api-radicacion`
