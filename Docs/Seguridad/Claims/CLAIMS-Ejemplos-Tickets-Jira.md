# CLAIMS Ejemplos Tickets Jira

## Objetivo

Proveer ejemplos de redacción de tickets Jira para cambios relacionados con claims, basados en los casos prácticos documentados en `CLAIMS-Ejemplos.md`.

## Ejemplo 1: documentar cómo funciona `ValidateClaim("defaulalias")`

### Título sugerido

`DOCUMENTAR-FLUJO-VALIDACION-CLAIM-DEFAULALIAS`

### Descripción sugerida

```text
Analizar y documentar el flujo actual del claim defaulalias en DocuArchiCore.

Objetivo:
- identificar dónde se genera el claim
- identificar cómo llega a HttpContext.User
- explicar cómo ClaimValidationService lo valida
- documentar ejemplos reales de consumo en controllers

Rutas a revisar:
- MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/ClaimValidationService.cs
- MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/CurrentUserService.cs
- MiApp.Services/Service/Autenticacion/TokenService.cs
- DocuArchi.Api/Controllers/Radicacion/Tramite/RadicacionController.cs
- DocuArchi.Api/Controllers/UI/MuiTable/DynamicUiTableController.cs

Restricción:
- no modificar comportamiento, solo documentar el flujo real existente
```

## Ejemplo 2: agregar un claim de contexto `sede`

### Título sugerido

`AGREGAR-CLAIM-CONTEXTO-SEDE-AL-JWT`

### Descripción sugerida

```text
Implementar un nuevo claim de contexto llamado sede en el flujo de autenticación de DocuArchiCore.

Objetivo:
- agregar la propiedad sede al DTO de usuario autenticado
- llenar el dato desde el origen correcto del login
- agregar el claim sede al JWT
- permitir su lectura con IClaimValidationService e ICurrentUserService

Rutas potencialmente impactadas:
- MiApp.DTOs/DTOs/Autenticacion/UsuarioAutenticadoDTO.cs
- MiApp.Services/Service/Mapping/GestorDocumental/Usuario/RemitDestInternoMapping.cs
- MiApp.Services/Service/Mapping/Workflow/Usuario/UsuarioWorkflowMapping.cs
- MiApp.Services/Service/Account/AutenticacionApplicationService.cs
- MiApp.Services/Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs
- MiApp.Services/Service/Autenticacion/TokenService.cs

Requerimientos:
- no romper claims existentes
- documentar de dónde sale el dato
- validar el claim con token nuevo
```

## Ejemplo 3: agregar un claim de contexto `idsede`

### Título sugerido

`AGREGAR-CLAIM-IDSede-DESDE-REMITDESTINTERNO`

### Descripción sugerida

```text
Agregar al JWT un claim idsede aprovechando el dato Id_Sedes_Empresa ya disponible en RemitDestInterno.

Objetivo:
- exponer idsede en UsuarioAutenticadoDTO
- mapear Id_Sedes_Empresa desde RemitDestInterno
- agregar idsede al token
- permitir lectura del claim en endpoints y servicios

Rutas impactadas:
- MiApp.Models/Models/GestorDocumental/usuario/RemitDestInterno.cs
- MiApp.DTOs/DTOs/Autenticacion/UsuarioAutenticadoDTO.cs
- MiApp.Services/Service/Mapping/GestorDocumental/Usuario/RemitDestInternoMapping.cs
- MiApp.Services/Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs
- MiApp.Services/Service/Autenticacion/TokenService.cs

Ventaja:
- no requiere nueva consulta a base de datos durante login
```

## Ejemplo 4: agregar un claim de permiso

### Título sugerido

`AGREGAR-PERMISO-RA-DOCUMENTO-APROBAR`

### Descripción sugerida

```text
Agregar un nuevo permiso funcional al sistema de claims basado en el patrón actual de claim type perm.

Objetivo:
- definir el permiso ra.documento.aprobar
- agregarlo en el builder correspondiente
- asegurar que viaje en el token JWT
- permitir validación imperativa o por policy

Rutas impactadas:
- MiApp.Services/Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs
- MiApp.Services/Service/Seguridad/Autorizacion/Constructoras/*
- MiApp.Services/Service/Seguridad/Autorizacion/Constructoras/PermissionClaimsOrchestrator.cs
- MiApp.Services/Service/Seguridad/Autorizacion/Extensiones/AuthorizationExtensions.cs
- MiApp.Services/Service/Autenticacion/TokenService.cs

Requerimientos:
- mantener el patrón de claim type perm
- no crear claims de permiso fuera del catálogo existente
```

## Ejemplo 5: exponer un claim obligatorio en un endpoint

### Título sugerido

`VALIDAR-CLAIM-OBLIGATORIO-SEDE-EN-ENDPOINT`

### Descripción sugerida

```text
Actualizar un endpoint para requerir el claim sede antes de ejecutar la lógica funcional.

Objetivo:
- validar el claim sede con IClaimValidationService
- retornar BadRequest con AppResponses cuando el claim falte
- pasar el valor validado al service correspondiente

Rutas impactadas:
- DocuArchi.Api/Controllers/<modulo>/<controller>.cs
- MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/ClaimValidationService.cs

Requerimientos:
- mantener el patrón existente usado con defaulalias y usuarioid
- no leer claims directamente en repository
```

## Ejemplo 6: documentar la arquitectura de claims

### Título sugerido

`DOCUMENTAR-ARQUITECTURA-CLAIMS-DOCUARCHICORE`

### Descripción sugerida

```text
Crear documentación técnica del sistema de claims actual de DocuArchiCore.

Objetivo:
- describir generación del token
- describir claims base y claims de permisos
- explicar consumo en backend
- documentar middleware JWT y autorización
- incluir rutas reales de archivos

Ubicación de documentación:
- Docs/Seguridad/Claims

Archivos esperados:
- CLAIMS-Arquitectura.md
- CLAIMS-UsoBackend.md
- CLAIMS-FlujoAutorizacion.md
- CLAIMS-Ejemplos.md
```

## Plantilla base reusable

```text
Título:
<VERBO>-<OBJETO>-<CONTEXTO>

Descripción:
Objetivo:
- ...

Rutas impactadas:
- ...

Requerimientos:
- ...

Restricciones:
- no romper claims existentes
- usar el patrón actual del proyecto
- validar con token nuevo después del cambio
```
