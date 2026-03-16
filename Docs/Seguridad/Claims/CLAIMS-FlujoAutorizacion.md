# CLAIMS Flujo Autorizacion

## Flujo completo observado

```text
Login
  -> AutenticacionApplicationService.ValidarLogin
  -> validacion de modulo y credenciales
  -> resolucion de permisos por modulo/perfil
  -> TokenService.CrearToken
  -> JWT firmado
  -> cliente envia Authorization: Bearer <token>
  -> JwtBearer middleware valida token
  -> claims disponibles en HttpContext.User
  -> Controller consume IClaimValidationService o ICurrentUserService
  -> Service usa alias/usuario/permisos
  -> logica funcional o consulta se ejecuta
```

## Diagrama de autenticacion

```text
Cliente
  -> AccountController.ValidaUserAplicacion
  -> AutenticacionApplicationService
  -> InicioSesion / GestorModulo / InicioModulo*
  -> PermissionClaimsOrchestrator
  -> TokenService
  -> JWT con claims
  -> Cliente
```

## Diagrama de autorizacion

```text
Request con Bearer Token
  -> Middleware JWT
  -> HttpContext.User
  -> [Authorize] si existe
  -> Controller
      -> ClaimValidationService.ValidateClaim("defaulalias")
      -> ClaimValidationService.ValidateClaim("usuarioid") cuando aplica
  -> Service
      -> CurrentUserService / permisos
  -> Ejecucion o rechazo
```

## Puntos de validacion identificados

### Middleware JWT

Configurado en [Program.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Program.cs):

- firma
- issuer
- audience
- expiración

### Authorize en controllers

Se observan controllers con `[Authorize]`, por ejemplo:

- [PlantillaRadicacionController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/Radicacion/PlantillaRadicado/PlantillaRadicacionController.cs)
- [DynamicUiTableController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/UI/MuiTable/DynamicUiTableController.cs)

También se observaron endpoints que consumen claims con el atributo comentado, por ejemplo:

- [RadicacionController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/Radicacion/Tramite/RadicacionController.cs)

### Validacion manual de claims

Patrón actual:

- si falta `defaulalias`, el endpoint responde `BadRequest`
- si falta `usuarioid`, el endpoint responde `BadRequest`
- si `usuarioid` no convierte a entero, se lanza `SecurityException`

### Validacion por permisos

Se identifican dos mecanismos:

1. Imperativo:
   - `CurrentUserService.HasPermission(...)`
   - aplicado por ejemplo en [MenuL.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Home/Menu/MenuL.cs)
2. Declarativo:
   - políticas creadas en [AuthorizationExtensions.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/Extensiones/AuthorizationExtensions.cs)

## Claims y efecto funcional

```text
Claim defaulalias
  -> seleccion de alias de conexion
  -> servicios/repositorios consultan la base correcta

Claim usuarioid / uid
  -> identifica al usuario ejecutor
  -> usado en trazabilidad y operaciones funcionales

Claim perm
  -> habilita o restringe acciones
  -> filtra menu
  -> puede respaldar policies
```

## Ejemplo de autorizacion funcional

En [MenuL.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Home/Menu/MenuL.cs), si el usuario no tiene `wf.diagramador`, se remueven nodos del menú. Eso muestra que la autorización no se limita al acceso HTTP; también modela la experiencia funcional dentro del sistema.

## Evidencia usada

- [Program.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Program.cs)
- [TokenService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Autenticacion/TokenService.cs)
- [AutenticacionApplicationService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Account/AutenticacionApplicationService.cs)
- [CurrentUserService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/CurrentUserService.cs)
- [ClaimValidationService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/ClaimValidationService.cs)
- [PermissionClaimsOrchestrator.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/Constructoras/PermissionClaimsOrchestrator.cs)
- [MenuL.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Home/Menu/MenuL.cs)
