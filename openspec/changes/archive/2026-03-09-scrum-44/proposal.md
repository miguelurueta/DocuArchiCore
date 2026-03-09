## Why

Se requiere cerrar una brecha funcional del flujo de radicacion entrante creando un servicio dedicado de validacion de campos obligatorios (fijos y dinamicos), desacoplado y reutilizable por otros modulos.

## What Changes

- Crear servicio `ValidaCamposObligatorios` en `MiApp.Services/Service/Radicacion/Tramite`.
- Definir interfaz en el mismo archivo de la clase para facilitar pruebas unitarias.
- Evaluar obligatorios usando:
  - Campos fijos centralizados como constante compartible.
  - Estructura dinamica `DetallePlantillaRadicado`.
  - Datos de entrada `RegistrarRadicacionEntranteRequestDto`.
- Retornar `AppResponses<List<ValidationError>>` con errores construidos vía `ValidationHelper`.
- Registrar servicio en `Program.cs` de `DocuArchi.Api`.
- Agregar pruebas unitarias e integración (MySQL con Testcontainers/Docker).
- Documentar en `DocuArchiCore/Docs/Radicacion/Tramite` con diagramas y guía para frontend.

## Capabilities

### New Capabilities
- `jira-scrum-44`: validación centralizada de campos obligatorios para radicación entrante.

### Modified Capabilities
- `radicacion-tramite`: incorpora validación previa reusable y documentada.

## Impact

- Repos impactados: `MiApp.Services`, `MiApp.Repository`, `MiApp.DTOs`, `DocuArchi.Api`, `DocuArchiCore`.
- Ruta principal de implementación: `/Service/Radicacion/Tramite`.
- Referencias:
  - `openspec/context/multi-repo-context.md`
  - `openspec/context/OPSXJ_BACKEND_RULES.md`
