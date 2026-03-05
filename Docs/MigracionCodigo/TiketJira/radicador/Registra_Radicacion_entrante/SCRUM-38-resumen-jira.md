# SCRUM-38 Resumen para Jira

Objetivo: migrar `Registra_Radicacion_entrante` del legacy VB/WebForms a arquitectura por capas en API moderna.

## Alcance

1. F1 Inventario completo de funciones y dependencias del flujo.
2. F2 Reconstrucción end-to-end del flujo funcional real.
3. F3 Contrato DTO/request-response y mapeo `legacy -> target`.
4. F4 Persistencia por repositorios con matriz SQL Q01-Q09.
5. F5 Orquestación por capas (`Controller -> Service -> Repository`).
6. F6 Endpoints: `POST /api/radicacion/registrar-entrante`, `POST /api/radicacion/validar-entrante`, `GET /api/radicacion/flujo-inicial`.
7. F7 Reemplazo de dependencias legacy (`Session`, `FindControl`, `ViewState`, `Hidden`).

## Reglas obligatorias

1. SQL 100% parametrizado.
2. `QueryOptions.DefaultAlias = defaultDbAlias` en todos los repositorios.
3. Respuestas estándar `AppResponses<T>`.
4. Atomicidad obligatoria Q01-Q08: si falla un paso, rollback total y sin persistencia parcial.
5. `Q09` condicional según rama funcional (ej. PQR) dentro de control transaccional.

## Criterios de aceptación

1. Paridad funcional en casos críticos.
2. Validaciones y autorizaciones cubiertas por pruebas.
3. Commit solo con bloque transaccional completo; fallos con rollback total.
4. Contratos API y documentación técnica actualizados.

## Referencia técnica completa en repo

1. `Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/TICKETS-F1-F7-Registra_Radicacion_entrante.md`
2. `Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/PLAN-MIGRACION-Registra_Radicacion_entrante.md`
3. `Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/SCRUM-38-ticket-consolidado-Registra_Radicacion_entrante.md`
