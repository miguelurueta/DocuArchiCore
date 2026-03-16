# CLAIMS Arquitectura

## Objetivo

Documentar el manejo actual de claims en `DocuArchiCore` sin inferir comportamiento no implementado.

## Arquitectura observada

El proyecto usa autenticacion JWT en la API y expone los claims dentro de `HttpContext.User`.

Componentes principales identificados:

- Configuracion JWT en [Program.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Program.cs)
- Emision de token en [TokenService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Autenticacion/TokenService.cs)
- Fachada de emision en [TokenIssuer.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Autenticacion/SecondFactor/TokenIssuer.cs)
- Construccion de claims de permisos en [PermissionClaimsOrchestrator.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/Constructoras/PermissionClaimsOrchestrator.cs)
- Tipos de claims funcionales en [DocuArchiClaimTypes.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs)
- Lectura de claims en [CurrentUserService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/CurrentUserService.cs)
- Validacion de claims requeridos en [ClaimValidationService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/ClaimValidationService.cs)
- Politicas por permiso en [AuthorizationExtensions.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/Extensiones/AuthorizationExtensions.cs)

## Flujo general

1. El login entra por [AccountController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/Account/AccountController.cs) y delega en [AutenticacionApplicationService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Account/AutenticacionApplicationService.cs).
2. El servicio valida empresa, modulo, credenciales y flujo de segundo factor si aplica.
3. Para modulos de negocio, se resuelven permisos por perfil y modulo usando servicios de inicio como:
   - [InicioModuloGestorL.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/GestorDocumental/Inicio/InicioModuloGestorL.cs)
   - [IncioModuloWorkflowL.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Workflow/Inicio/IncioModuloWorkflowL.cs)
4. Esos servicios generan claims de permisos mediante `PermissionClaimsOrchestrator`.
5. `TokenService.CrearToken(...)` construye el JWT final y agrega claims base y permisos.
6. La API valida el token con `AddJwtBearer(...)` y deja los claims disponibles en `HttpContext.User`.
7. Controllers y servicios consumen los claims a traves de `IClaimValidationService` o `ICurrentUserService`.

## Claims observados en el token

En [TokenService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Autenticacion/TokenService.cs) se agregan estos claims base:

- `sub`: compuesto `empresa:modulo:usuario`
- `ClaimTypes.NameIdentifier`: id de usuario
- `emp`: id de empresa
- `mod`: id de modulo
- `modName`: nombre del modulo
- `uid`: id de usuario
- `login`: login del usuario
- `name`: nombre visible del usuario
- `usuarioid`: id de usuario usado por endpoints funcionales
- `defaulaliaswf`: alias de base de workflow, cuando aplica
- `defaulalias`: alias de base principal del modulo, cuando aplica
- `email`: correo del usuario, cuando existe
- `perm`: permisos funcionales deduplicados

## Tipos de claims funcionales

En [DocuArchiClaimTypes.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs) se centralizan estos nombres:

- `permisos = "perm"`
- `usuarioId = "uid"`
- `empresaId = "emp"`
- `modulo = "mod"`
- `dbAlias = "db"`
- `login = "login"`
- `email = "email"`
- `nombre = "name"`

Observacion importante:

- El token usa `defaulalias` y `defaulaliaswf` como claims directos.
- `DocuArchiClaimTypes.dbAlias` existe, pero en los controladores revisados no es el claim usado para resolver conexiones.

## Estructura de autenticacion JWT

La configuracion actual en [appsettings.json](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/appsettings.json) define:

- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Minutes`

Y [Program.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Program.cs) configura:

- `ValidateIssuerSigningKey = true`
- `ValidateIssuer = true`
- `ValidateAudience = true`
- `ClockSkew = TimeSpan.Zero`

## Ejemplo de payload representativo

```json
{
  "sub": "1:3:25",
  "emp": "1",
  "mod": "3",
  "modName": "GESTOR DOCUMENTAL",
  "uid": "25",
  "usuarioid": "25",
  "login": "murueta",
  "name": "Miguel Urueta",
  "defaulalias": "DA",
  "defaulaliaswf": "WF",
  "email": "usuario@empresa.com",
  "perm": [
    "wf.diagramador",
    "ra.interna.radicacion"
  ]
}
```

Ese JSON es ilustrativo. La forma y nombres de los claims estan basados en el código real; los valores concretos dependen del usuario, modulo y permisos resueltos.

## Recomendaciones arquitectonicas

- Mantener un solo catalogo de nombres de claims y evitar mezclar aliases como `uid` y `usuarioid` salvo cuando sea estrictamente necesario.
- Consolidar el uso de alias de base de datos, porque hoy conviven `defaulalias`, `defaulaliaswf` y `DocuArchiClaimTypes.dbAlias`.
- Priorizar `[Authorize]` en endpoints que dependen de claims; algunos controladores revisados consumen claims pero tienen el atributo comentado.
