# CLAIMS Matriz

## Objetivo

Tener una vista rápida de los claims más relevantes del proyecto, indicando origen, uso y validación observada en el código actual.

## Matriz de claims

| Claim | Tipo funcional | Origen observado | Dónde se llena | Dónde se valida/lee | Uso principal |
|---|---|---|---|---|---|
| `sub` | Identidad compuesta | `empresaId`, `modulo.ID_MODULO`, `usuario.usuarioId` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | Middleware JWT | Identidad compuesta del token |
| `emp` | Contexto | `empresaId` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | `HttpContext.User` | Contexto de empresa |
| `mod` | Contexto | `modulo.ID_MODULO` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | `HttpContext.User` | Contexto de módulo |
| `modName` | Contexto | `modulo.NOMBRE_MODULO` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | `HttpContext.User` | Nombre del módulo activo |
| `uid` | Identidad | `usuario.usuarioId` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | `CurrentUserService.uid` | Id actual de usuario |
| `usuarioid` | Identidad funcional | `usuario.usuarioId` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | `ClaimValidationService.ValidateClaim("usuarioid")` | Validaciones funcionales en controllers |
| `login` | Contexto | `usuario.login` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | `HttpContext.User` | Login del usuario |
| `name` | Contexto | `usuario.nombre` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | `HttpContext.User` | Nombre visible del usuario |
| `email` | Contexto | `usuario.email` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | `HttpContext.User` | Correo del usuario |
| `defaulalias` | Contexto técnico | `modulo.defaultDbAlias` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | `ClaimValidationService.ValidateClaim("defaulalias")` | Alias de conexión principal |
| `defaulaliaswf` | Contexto técnico | `usuario.defaulaliaswf` | `MiApp.Services/Service/Autenticacion/TokenService.cs` | `HttpContext.User` | Alias workflow |
| `perm` | Permiso | `usuario.permisos` / builders de permisos | `TokenService` y flujo de inicio de módulo | `CurrentUserService.Permisos`, `HasPermission`, policies | Autorización funcional |

## Archivos clave asociados

### Generación

- [TokenService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Autenticacion/TokenService.cs)
- [AutenticacionApplicationService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Account/AutenticacionApplicationService.cs)

### Validación y lectura

- [ClaimValidationService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/ClaimValidationService.cs)
- [CurrentUserService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/CurrentUserService.cs)

### Construcción de permisos

- [PermissionClaimsOrchestrator.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/Constructoras/PermissionClaimsOrchestrator.cs)
- builders bajo `MiApp.Services/Service/Seguridad/Autorizacion/Constructoras`

### Middleware

- [Program.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Program.cs)

## Observaciones importantes

- `uid` y `usuarioid` representan el mismo identificador base, pero hoy se usan en puntos distintos del sistema.
- `defaulalias` es el claim más utilizado para resolver alias de base de datos en endpoints funcionales.
- `perm` es el claim de autorización más importante; no conviene reemplazarlo por claims nuevos de permiso fuera del patrón actual.
- `DocuArchiClaimTypes.dbAlias` existe como constante, pero en la práctica el proyecto revisado usa `defaulalias` y `defaulaliaswf`.
