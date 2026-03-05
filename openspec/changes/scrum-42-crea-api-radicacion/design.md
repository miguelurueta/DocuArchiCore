## Context

- Jira issue key: SCRUM-42
- Jira summary: CREA-API-RADICACION
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-42

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Resumen para Jira Objetivo: migrar Registra_Radicacion_entrante del legacy VB/WebForms a arquitectura por capas en API moderna. Alcance F1 Inventario completo de funciones y dependencias del flujo. F2 Reconstrucción end-to-end del flujo funcional real. F3 Contrato DTO/request-response y mapeo legacy -> target . F4 Persistencia por repositorios con matriz SQL Q01-Q09. F5 Orquestación por capas ( Controller -> Service -> Repository ). F6 Endpoints: POST /api/radicacion/registrar-entrante , POST /api/radicacion/validar-entrante , GET /api/radicacion/flujo-inicial . F7 Reemplazo de dependencias legacy ( Session , FindControl , ViewState , Hidden ). Reglas obligatorias SQL 100% parametrizado. QueryOptions.DefaultAlias = defaultDbAlias en todos los repositorios. Respuestas estándar AppResponses<T> . Atomicidad obligatoria Q01-Q08: si falla un paso, rollback total y sin persistencia parcial. Q09 condicional según rama funcional (ej. PQR) dentro de control transaccional. Criterios de aceptación Paridad funcional en casos críticos. Validaciones y autorizaciones cubiertas por pruebas. Commit solo con bloque transaccional completo; fallos con rollback total. Contratos API y documentación técnica actualizados. Referencia técnica completa en repo Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/TICKETS-F1-F7-Registra_Radicacion_entrante.md Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/PLAN-MIGRACION-Registra_Radicacion_entrante.md Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/SCRUM-38-ticket-consolidado-Registra_Radicacion_entrante.md

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-42-crea-api-radicacion.