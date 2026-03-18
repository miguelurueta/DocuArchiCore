## Context

- Jira issue key: SCRUM-77
- Jira summary: Desacoplar DocuArchi.Api de DocuArchiCore para corregir publish y dejar DocuArchiCore como repositorio coordinador
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-77

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Scope

- This coordinator repo documents the change, tracks impacted repositories and keeps the current `SesionActual*` source as migration input.
- Source-code implementation is expected in `DocuArchi.Api` and potentially a shared non-web location if the session implementation is extracted further.
- `DocuArchiCore.Abstractions` remains contract-only unless hidden implementation dependencies are discovered.
- `MiApp.DTOs`, `MiApp.Models`, `MiApp.Repository`, and `MiApp.Services` stay out of scope unless the migration reveals compile-time dependencies.

## Problem Statement

Problema actual La publicación de DocuArchi.Api falla por colisión de archivos de salida de publish porque el proyecto referencia a DocuArchiCore como ProjectReference , y ambos son proyectos Microsoft.NET.Sdk.Web . Esto provoca duplicados en rutas relativas como: appsettings.json appsettings.Development.json Tools/jira-open/package.json Además, la referencia web-a-web hace que DocuArchi.Api arrastre contenido MVC y ApplicationPart de DocuArchiCore , aunque ese repositorio no debe seguir participando en runtime. Hallazgo técnico DocuArchi.Api no necesita a DocuArchiCore como aplicación web completa. La dependencia real identificada es la implementación de sesión: Infrastructure/Security/SesionActual.cs Infrastructure/Security/SesionActual.DocuArchi.cs Infrastructure/Security/SesionActual.General.cs Infrastructure/Security/SesionActual.Gestion.cs Infrastructure/Security/SesionActual.Radicacion.cs Infrastructure/Security/SesionActual.Workflow.cs Las interfaces ya están correctamente separadas en DocuArchiCore.Abstractions y no requieren migración. Objetivo Dejar: DocuArchi.Api como única aplicación web/runtime/publicable DocuArchiCore como repositorio coordinador para Docs , openspec , Tools/jira-open y artefactos de documentación sin dependencia web-a-web entre DocuArchi.Api y DocuArchiCore Solución propuesta Mover la implementación SesionActual* fuera de DocuArchiCore web. Reubicarla en DocuArchi.Api o en una librería compartida. Mantener las interfaces en DocuArchiCore.Abstractions . Actualizar DocuArchi.Api/Program.cs para registrar la nueva ubicación de SesionActual . Eliminar la referencia: DocuArchi.Api -> DocuArchiCore.csproj Validar compilación, ejecución y publish de DocuArchi.Api . Impacto esperado Corrige el error de publish por archivos duplicados. Evita que DocuArchi.Api arrastre controladores, vistas y contenido estático de DocuArchiCore . Alinea la arquitectura con el objetivo de dejar DocuArchiCore como coordinador/documentación. Criterios de aceptación DocuArchi.Api compila sin referencia a DocuArchiCore.csproj DocuArchi.Api publica correctamente sin duplicados de appsettings ni Tools/jira-open DocuArchi.Api mantiene funcionando autenticación/sesión basada en SesionActual DocuArchiCore deja de participar como dependencia runtime DocuArchiCore.Abstractions sigue siendo el contrato compartido para sesión Riesgos Dependencias indirectas no detectadas dentro de SesionActual* Registros DI faltantes en DocuArchi.Api Pérdida de endpoints MVC heredados implícitamente desde DocuArchiCore si alguno todavía se usa Subtareas sugeridas Analizar dependencias de SesionActual* Migrar implementación de sesión Actualizar DI en DocuArchi.Api Eliminar referencia a DocuArchiCore.csproj Validar build y publish Validar endpoints y sesión

## Impacted Repositories

- `DocuArchi.Api`: remove the web-to-web project reference, relocate `SesionActual*`, update DI, build and publish.
- `DocuArchiCore`: keep coordinator artifacts and provide the current `Infrastructure/Security/SesionActual*` implementation as source material during migration.
- `DocuArchiCore.Abstractions`: no planned changes; contracts are already correctly separated.

## Implementation Notes

- Current evidence confirms `DocuArchi.Api.csproj` still references `..\DocuArchiCore\DocuArchiCore.csproj`.
- Current evidence confirms `DocuArchi.Api/Program.cs` imports `DocuArchiCore.Infrastructure.Security` and registers `SesionActual` plus all session interfaces from that implementation.
- The minimum viable fix is to move the `SesionActual*` implementation to `DocuArchi.Api` and remove the project reference. A second extraction into a non-web shared library is optional if duplication control requires it.

## Verification Plan

- Build `DocuArchi.Api` without `DocuArchiCore.csproj` reference.
- Publish `DocuArchi.Api` and verify no duplicate web asset/configuration conflicts remain.
- Exercise one endpoint depending on `ISesionActual` to confirm DI/runtime behavior is unchanged.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-77-desacoplar-docuarchi-api-de-docuarchicor.
