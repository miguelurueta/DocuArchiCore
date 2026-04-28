## Context

- Jira issue key: SCRUM-157
- Jira summary: REUBICAR-SolicitaEstructuraRespuestaIdTarea
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-157

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — Reubicar Controller, Service y Repository de SolicitaEstructuraRespuestaIdTarea

Rol esperado: Arquitecto/desarrollador backend .NET con enfoque en refactor seguro (mover archivos), namespaces, DI, build y pruebas.

Objetivo: Reubicar archivos existentes a nuevas carpetas y ajustar `namespace`/`using`/referencias para que todo compile y funcione igual, sin cambios funcionales.

### Cambios requeridos (solo reubicación + namespaces)

1) Controller
- De: `DocuArchi.Api/Controllers/GestionCorrespondencia/SolicitaEstructuraRespuestaIdTareaController.cs`
- A: `DocuArchi.Api/Controllers/GestionCorrespondencia/GestionRespuesta/SolicitaEstructuraRespuestaIdTareaController.cs`

2) Service
- De: `MiApp.Services/Service/GestorDocumental/ServiceSolicitaEstructuraRespuesta.cs`
- A: `MiApp.Services/Service/GestionCorrespondencia/GestionRespuesta/ServiceSolicitaEstructuraRespuesta.cs`

3) Repository
- De: `MiApp.Repository/Repositorio/GestionCorrespondencia/SolicitaEstructuraRespuestaIdTareaRepository.cs`
- A: `MiApp.Repository/Repositorio/GestionCorrespondencia/GestionRespuesta/SolicitaEstructuraRespuestaIdTareaRepository.cs`

### Reglas (obligatorias)
- Mantener la misma ruta HTTP del controller (no cambiar `[Route]` / `[HttpGet]` ni strings).
- Mantener el mismo contrato request/response (shape de `AppResponses<T>` y firma de métodos).
- Mantener los mismos nombres de clases e interfaces (solo cambia ubicación física y `namespace`).
- No modificar lógica interna, validaciones, mensajes ni comportamiento.
- Actualizar `namespace` según nueva ruta, incluyendo `GestionRespuesta` donde aplique.
- Actualizar `using`/referencias en consumidores: controllers, services, repositories, `Program.cs`/DI, y tests.

### Validaciones obligatorias
- No deben quedar referencias al namespace anterior.
- DI debe seguir registrando las interfaces correctas (mismo tipo, nuevo `namespace`).
- El endpoint conserva exactamente la misma ruta pública.

### Build / pruebas
- Ejecutar `dotnet build`.
- Ejecutar `dotnet test` (si falla por causas no relacionadas, ejecutar pruebas focales del flujo afectado y documentar evidencia).

### Criterios de aceptación
- Archivos movidos a las rutas nuevas.
- `namespace`/`using` actualizados y compilación exitosa.
- Sin cambios funcionales: endpoint, contratos e interfaces intactos.
- Evidencia adjunta de build y pruebas.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar artefactos OpenSpec del change (según el flujo/herramienta disponible en el repo).
