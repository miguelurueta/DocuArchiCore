# SCRUM-42 Implementation Playbook

## Objective

Implementar la migracion de `Registra_Radicacion_entrante` con contratos API, persistencia transaccional y paridad funcional, siguiendo `Controller -> Service -> Repository`.

## Execution Order

1. Contratos y rutas API.
2. Servicios de validacion/flujo/registro.
3. Repositorios Q01-Q09 con transaccion Q01-Q08.
4. Registro DI y mapeos.
5. Pruebas unitarias/integracion/contract.

## Repo-by-Repo File Map

### DocuArchi.Api

1. Controllers:
   - `Controllers/Radicacion/Tramite/TramiteController.cs`
   - `Controllers/Radicacion/ConsultaRadicacion/ConsultaRadicacionController.cs` (si aplica por separacion existente)
2. Startup/DI:
   - `Program.cs`

### MiApp.DTOs

1. Requests:
   - `DTOs/Radicacion/Tramite/RegistrarRadicacionRequest*.cs`
   - `DTOs/Radicacion/Tramite/ValidarRadicacionRequest*.cs`
2. Responses:
   - `DTOs/Radicacion/Tramite/RegistrarRadicacionResponse*.cs`
   - `DTOs/Radicacion/Tramite/ValidarRadicacionResponse*.cs`
   - `DTOs/Radicacion/Tramite/FlujoInicialDto*.cs`

### MiApp.Services

1. Services:
   - `Service/Radicacion/Tramite/*Registrar*`
   - `Service/Radicacion/Tramite/*Validar*`
   - `Service/Radicacion/Tramite/*Flujo*`
2. Mapping:
   - `Service/Mapping/Radicacion/Tramite/*`

### MiApp.Repository

1. Repositories:
   - `Repositorio/Radicador/Tramite/*Consecutivo*` (Q01, Q02)
   - `Repositorio/Radicador/Tramite/*Registro*` (Q03, Q05)
   - `Repositorio/Radicador/Tramite/*Relacion*` (Q04)
   - `Repositorio/Radicador/Tramite/*Respuesta*` (Q06, Q07)
   - `Repositorio/Radicador/Tramite/*Estado*` (Q08)
   - `Repositorio/Radicador/Tramite/*Pqr*` (Q09 condicional)
2. Regla obligatoria:
   - `QueryOptions.DefaultAlias = defaultDbAlias` en todas las consultas.

### MiApp.Models

1. Modelos nuevos/ajustados para tablas de radicacion:
   - `Models/Radicacion/Tramite/*`

## Technical Rules (Non-negotiable)

1. SQL parametrizado en 100% de queries.
2. Q01-Q08 en transaccion unica.
3. Si falla cualquier paso Q01-Q08: rollback total sin persistencia parcial.
4. Q09 se ejecuta solo en ramas funcionales definidas.
5. `AppResponses<T>` en todas las respuestas API.

## Definition of Done (Application Stage)

1. Endpoints implementados y conectados a servicios:
   - `POST /api/radicacion/registrar-entrante`
   - `POST /api/radicacion/validar-entrante`
   - `GET /api/radicacion/flujo-inicial`
2. Reglas de atomicidad demostradas por pruebas.
3. DI registrado en `Program.cs`.
4. Compilacion y pruebas en verde (o evidencia de skip justificado).
