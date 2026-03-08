## Context

- Jira issue key: SCRUM-43
- Jira summary: ACTUALIZA-API-RADICACION
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-43

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Ticket Jira - Cierre de migracion parcial Registra_Radicacion_entrante Titulo sugerido [SCRUM-42] Cerrar brechas de migracion de Registra_Radicacion_entrante (claims, SQL real Q01-Q09 y paridad final) Tipo Historia tecnica / Hardening Problema actual La migracion de Registra_Radicacion_entrante quedo parcial: En POST /api/radicacion/registrar-entrante se usan valores hardcodeados ( 141 , "DA" ) en lugar de claims. La matriz Q01-Q09 esta simulada por pasos y no implementa SQL funcional de negocio. El mapeo canónico de Campos[] en service no esta cerrado segun plan. Existe desalineacion potencial entre comportamiento final del controller y pruebas de contrato. Objetivo Cerrar la migracion al 100% contra el plan aprobado F1-F8, preservando atomicidad, trazabilidad y paridad funcional del flujo legacy. Alcance Activar lectura real de claims defaulalias y usuarioid en RegistrarEntrante . Eliminar hardcode en controller y propagar alias/usuario reales a service/repository. Implementar SQL real Q01-Q09 con transaccion unica Q01-Q08 y rollback total. Ejecutar Q09 solo cuando aplique por regla funcional (ej. TipoRadicacion = PQR ). Implementar/explicitar mapping de Campos[] a modelo canónico de persistencia. Alinear pruebas unitarias/integracion/contrato con comportamiento final. Entregar evidencia de dotnet build y dotnet test . No alcance Nuevas reglas funcionales fuera de Registra_Radicacion_entrante . Cambios de UI que no impacten el contrato del endpoint. Refactor global no relacionado con radicacion entrante. Tareas tecnicas [ ] Controller: reactivar validacion de claims y parseo de usuarioid . [ ] Service: mantener validaciones y cerrar mapping canónico de Campos[] . [ ] Repository: reemplazar ExecuteStep simulado por SQL real Q01-Q09. [ ] Repository: mantener transaccion unica en Q01-Q08 + rollback total ante fallo. [ ] Repository: ejecutar Q09 condicional por rama funcional. [ ] DI: validar/ajustar registro de interfaces concretas usadas por el flujo. [ ] Tests: actualizar contratos de controller (claims requeridos en registrar). [ ] Tests: validar propagacion de defaultDbAlias en toda la cadena. [ ] Tests: validar atomicidad y no persistencia parcial. [ ] Evidencia: anexar salida de build/test del repo impactado. Criterios de aceptacion POST /api/radicacion/registrar-entrante falla con BadRequest si faltan claims requeridos. POST /api/radicacion/registrar-entrante usa defaulalias y usuarioid reales; sin hardcode. Q01-Q08 se ejecutan en una sola transaccion de aplicacion. Si falla cualquier paso entre Q01-Q08 se ejecuta rollback completo. Q09 solo se ejecuta cuando la condicion funcional lo exige. Todas las consultas del flujo usan alias DB propagado correctamente. Las pruebas automatizadas de servicio/repositorio/controller pasan y reflejan el comportamiento final. Se adjunta evidencia de compilacion y pruebas del estado final. Evidencia esperada en PR Diff de controller/service/repository con referencias a Q01-Q09. Tests actualizados (unit, integration, contract). Resultado de dotnet build y dotnet test . Nota de compatibilidad con plan de migracion F1-F8. Referencias Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/PLAN-MIGRACION-Registra_Radicacion_entrante.md Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/TICKETS-F1-F7-Registra_Radicacion_entrante.md openspec/specs/jira-scrum-42/spec.md

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-43-actualiza-api-radicacion.