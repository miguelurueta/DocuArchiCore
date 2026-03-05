# SCRUM-42 - Documentacion Tecnica

## Objetivo

Implementar API para radicacion entrante migrando el flujo legacy `Registra_Radicacion_entrante` a arquitectura por capas (`Controller -> Service -> Repository`), manteniendo atomicidad transaccional y trazabilidad.

## Endpoints implementados

- `POST /api/radicacion/registrar-entrante`
- `POST /api/radicacion/validar-entrante`
- `GET /api/radicacion/flujo-inicial`

## Regla tecnica clave

- Atomicidad obligatoria de base de datos:
  - Q01-Q08 en una sola transaccion.
  - Si falla cualquier paso, se ejecuta rollback total.
  - Q09 se ejecuta de forma condicional segun rama funcional.

## Artefactos OpenSpec del cambio

Ruta:

- `openspec/changes/scrum-42-crea-api-radicacion/`

Archivos principales:

- `proposal.md`
- `design.md`
- `specs/jira-scrum-42/spec.md`
- `implementation-playbook.md`
- `application-checkpoint.md`
- `tasks.md`
- `sync.md`

## Implementacion por repositorio

### DocuArchi.Api

- `Controllers/Radicacion/Tramite/RadicacionController.cs`
- `Program.cs` (registro DI para servicios/repositorio de SCRUM-42)

### MiApp.Services

- `Service/Radicacion/Tramite/RegistrarRadicacionEntranteService.cs`
- `Service/Radicacion/Tramite/ValidarRadicacionEntranteService.cs`
- `Service/Radicacion/Tramite/FlujoInicialRadicacionService.cs`

### MiApp.Repository

- `Repositorio/Radicador/Tramite/RegistrarRadicacionEntranteRepository.cs`

### MiApp.DTOs

- `DTOs/Radicacion/Tramite/RegistrarRadicacionEntranteDtos.cs`

### DocuArchiCore (orquestador)

- `tests/TramiteDiasVencimiento.Tests/RegistrarRadicacionEntranteServiceTests.cs`
- `openspec/changes/scrum-42-crea-api-radicacion/tasks.md`

## Verificacion tecnica

- `dotnet build DocuArchi.Api.csproj -v minimal` ejecutado con resultado exitoso (0 errores).
- `openspec.cmd validate scrum-42-crea-api-radicacion` valido.

## Notas

- Existen advertencias historicas del repositorio no bloqueantes (por ejemplo NU1504 y NU1510) fuera del alcance funcional de SCRUM-42.
