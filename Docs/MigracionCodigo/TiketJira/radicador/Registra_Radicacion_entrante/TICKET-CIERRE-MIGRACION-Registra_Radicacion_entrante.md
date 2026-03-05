# Ticket Jira - Cierre de migracion parcial `Registra_Radicacion_entrante`

## Titulo sugerido

`[SCRUM-42] Cerrar brechas de migracion de Registra_Radicacion_entrante (claims, SQL real Q01-Q09 y paridad final)`

## Tipo

Historia tecnica / Hardening

## Problema actual

La migracion de `Registra_Radicacion_entrante` quedo parcial:

1. En `POST /api/radicacion/registrar-entrante` se usan valores hardcodeados (`141`, `"DA"`) en lugar de claims.
2. La matriz Q01-Q09 esta simulada por pasos y no implementa SQL funcional de negocio.
3. El mapeo canónico de `Campos[]` en service no esta cerrado segun plan.
4. Existe desalineacion potencial entre comportamiento final del controller y pruebas de contrato.

## Objetivo

Cerrar la migracion al 100% contra el plan aprobado F1-F8, preservando atomicidad, trazabilidad y paridad funcional del flujo legacy.

## Alcance

1. Activar lectura real de claims `defaulalias` y `usuarioid` en `RegistrarEntrante`.
2. Eliminar hardcode en controller y propagar alias/usuario reales a service/repository.
3. Implementar SQL real Q01-Q09 con transaccion unica Q01-Q08 y rollback total.
4. Ejecutar Q09 solo cuando aplique por regla funcional (ej. `TipoRadicacion = PQR`).
5. Implementar/explicitar mapping de `Campos[]` a modelo canónico de persistencia.
6. Alinear pruebas unitarias/integracion/contrato con comportamiento final.
7. Entregar evidencia de `dotnet build` y `dotnet test`.

## No alcance

1. Nuevas reglas funcionales fuera de `Registra_Radicacion_entrante`.
2. Cambios de UI que no impacten el contrato del endpoint.
3. Refactor global no relacionado con radicacion entrante.

## Tareas tecnicas

- [ ] Controller: reactivar validacion de claims y parseo de `usuarioid`.
- [ ] Service: mantener validaciones y cerrar mapping canónico de `Campos[]`.
- [ ] Repository: reemplazar `ExecuteStep` simulado por SQL real Q01-Q09.
- [ ] Repository: mantener transaccion unica en Q01-Q08 + rollback total ante fallo.
- [ ] Repository: ejecutar Q09 condicional por rama funcional.
- [ ] DI: validar/ajustar registro de interfaces concretas usadas por el flujo.
- [ ] Tests: actualizar contratos de controller (claims requeridos en registrar).
- [ ] Tests: validar propagacion de `defaultDbAlias` en toda la cadena.
- [ ] Tests: validar atomicidad y no persistencia parcial.
- [ ] Evidencia: anexar salida de build/test del repo impactado.

## Criterios de aceptacion

1. `POST /api/radicacion/registrar-entrante` falla con `BadRequest` si faltan claims requeridos.
2. `POST /api/radicacion/registrar-entrante` usa `defaulalias` y `usuarioid` reales; sin hardcode.
3. Q01-Q08 se ejecutan en una sola transaccion de aplicacion.
4. Si falla cualquier paso entre Q01-Q08 se ejecuta rollback completo.
5. Q09 solo se ejecuta cuando la condicion funcional lo exige.
6. Todas las consultas del flujo usan alias DB propagado correctamente.
7. Las pruebas automatizadas de servicio/repositorio/controller pasan y reflejan el comportamiento final.
8. Se adjunta evidencia de compilacion y pruebas del estado final.

## Evidencia esperada en PR

1. Diff de controller/service/repository con referencias a Q01-Q09.
2. Tests actualizados (unit, integration, contract).
3. Resultado de `dotnet build` y `dotnet test`.
4. Nota de compatibilidad con plan de migracion F1-F8.

## Referencias

1. `Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/PLAN-MIGRACION-Registra_Radicacion_entrante.md`
2. `Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/TICKETS-F1-F7-Registra_Radicacion_entrante.md`
3. `openspec/specs/jira-scrum-42/spec.md`
