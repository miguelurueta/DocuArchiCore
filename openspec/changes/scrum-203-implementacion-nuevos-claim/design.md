## Context

- Jira issue key: SCRUM-203
- Jira summary: IMPLEMENTACION-NUEVOS-CLAIM
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-203
- Change: `scrum-203-implementacion-nuevos-claim`

## Context Reference

- `openspec/context/multi-repo-context.md`
- `openspec/context/OPSXJ_BACKEND_RULES.md`

## Problem Statement

El sistema de autenticación emite JWT válidos para login normal y login con segundo factor, pero hoy no incluye
los claims de relación multi-módulo que frontend y APIs workflow/radicación necesitan leer de forma consistente.

Se requiere incorporar los claims:

- `IdUsuarioWorkflow`
- `IdUsuarioWorkflowExt`
- `IdUsuarioRadicador`
- `IdUsuarioDa`

usando los campos existentes de `remit_dest_interno`, manteniendo compatibilidad total con claims actuales.

## Scope

### In Scope

- Extender `UsuarioAutenticadoDTO` con 4 IDs nuevos.
- Ajustar mapping `RemitDestInterno -> UsuarioAutenticadoDTO`.
- Emitir los 4 claims en `TokenService.CrearToken(...)`.
- Verificar paridad de emisión en flujo 2FA.
- Actualizar contrato técnico/documentación de claims para frontend.
- Pruebas unitarias y de integración focalizadas en emisión de claims.

### Out of Scope

- Cambios de reglas de autorización/permissions (`perm`).
- Nuevos endpoints de autenticación.
- Cambios de estructura en tablas de base de datos.
- Cambio de naming de claims legacy (`usuarioid`, `uid`, `defaulalias`, `defaulaliaswf`).

## Canonical Mapping

- `IdUsuarioWorkflow` <- `remit_dest_interno.Relacion_Workflow`
- `IdUsuarioWorkflowExt` <- `remit_dest_interno.Relacion_Workflow_Extend`
- `IdUsuarioRadicador` <- `remit_dest_interno.Relacion_Id_Usuario_Radicacion`
- `IdUsuarioDa` <- `remit_dest_interno.Relacion_Da`

Regla de normalización: valores nulos/no definidos se emiten como `0`.

## Architecture Decisions

### AD-01: Punto único de emisión de claims

Se mantiene `TokenService.CrearToken(...)` como único punto de emisión del JWT.
El flujo 2FA debe seguir cerrando en el mismo servicio para no duplicar contratos.

### AD-02: Contrato intermedio en DTO autenticado

Los 4 campos se agregan a `UsuarioAutenticadoDTO` para:

- transportar datos desde login/mapping,
- serializar correctamente en payload de segundo factor,
- mantener consistencia entre login normal y 2FA.

### AD-03: Compatibilidad hacia atrás

- No remover claims existentes.
- Emitir siempre los nuevos claims (incluyendo `0`).
- Evitar romper consumidores que aún solo usan claims legacy.

## Impacted Repositories and Files

- `MiApp.DTOs`
  - `DTOs/Autenticacion/UsuarioAutenticadoDTO.cs`
- `MiApp.Services`
  - `Service/Mapping/GestorDocumental/Usuario/RemitDestInternoMapping.cs`
  - `Service/Autenticacion/TokenService.cs`
  - `Service/Autenticacion/SecondFactor/SecondFactorService.cs` (validación de continuidad de payload)
  - `Service/Autenticacion/Providers/EmailSecondFactorProvider.cs` (validación serialización payload)
  - opcional: `Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs`
- `DocuArchiCore` (coordinador/documentación)
  - `Docs/...` de contrato técnico de claims para integración frontend.

## Observability

Registrar logs en nivel `Information` para:

- emisión de token con nuevos claims (sin exponer token),
- flujo (normal vs 2FA),
- IDs emitidos (enmascarando datos sensibles cuando aplique).

Registrar `Warning` cuando algún ID llegue en `0` por datos de relación faltantes.

## Risk Assessment

- Riesgo de regresión por renombrar claims existentes: mitigado al no modificar claims legacy.
- Riesgo de inconsistencia entre login normal y 2FA: mitigado al centralizar emisión en `TokenService`.
- Riesgo frontend por contratos parciales: mitigado con documentación explícita y fallback `0`.

## Validation Strategy

- Unit tests mapping `RemitDestInterno` -> `UsuarioAutenticadoDTO`.
- Unit tests de `TokenService` verificando presencia/valor de los 4 claims.
- Tests de flujo 2FA confirmando que el JWT final contiene también los 4 claims.
- Test de no regresión sobre claims legacy.

## Acceptance Summary

- JWT de login normal contiene los 4 nuevos claims.
- JWT de login 2FA contiene los 4 nuevos claims.
- Claims legacy permanecen intactos.
- Contrato técnico/frontend actualizado con nombres exactos y fallback.
