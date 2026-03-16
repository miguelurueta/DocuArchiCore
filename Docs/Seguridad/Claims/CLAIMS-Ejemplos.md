# CLAIMS Ejemplos

## Objetivo

Concentrar ejemplos prácticos del sistema de claims actual de `DocuArchiCore` para consulta rápida.

## Ejemplo 1: cómo funciona `ValidateClaim("defaulalias")`

### Rutas involucradas

- `DocuArchi.Api/Controllers/Radicacion/Tramite/RadicacionController.cs`
- `DocuArchi.Api/Controllers/UI/MuiTable/DynamicUiTableController.cs`
- `MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/ClaimValidationService.cs`
- `MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/CurrentUserService.cs`

### Uso en controller

```csharp
var aliasValidation = _claimValidationService.ValidateClaim<string>("defaulalias");
if (!aliasValidation.Success || aliasValidation.ClaimValue == null)
{
    return BadRequest(aliasValidation.Response);
}

var defaultDbAlias = aliasValidation.ClaimValue;
```

Ejemplos reales:

- [RadicacionController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/Radicacion/Tramite/RadicacionController.cs)
- [DynamicUiTableController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/UI/MuiTable/DynamicUiTableController.cs)

### Qué hace internamente

En [ClaimValidationService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/ClaimValidationService.cs):

```csharp
var value = _currentUserService.GetClaimValue(claimName) ?? string.Empty;
```

Si no existe:

- `Success = false`
- retorna `AppResponses<T>`
- arma un `AppError` tipo `Claim`

Si existe:

- `Success = true`
- retorna el valor en `ClaimValue`

### De dónde sale ese valor

En [CurrentUserService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/CurrentUserService.cs):

```csharp
return _httpContextAccessor.HttpContext?.User?
    .FindFirst(claimType)?.Value;
```

Eso significa que `ValidateClaim("defaulalias")` lee directamente desde `HttpContext.User.Claims`.

## Ejemplo 2: dónde se llena `defaulalias`

### Rutas involucradas

- `MiApp.Services/Service/Autenticacion/TokenService.cs`
- `MiApp.Services/Service/Account/AutenticacionApplicationService.cs`
- `DocuArchi.Api/Program.cs`

El claim se agrega en [TokenService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Autenticacion/TokenService.cs):

```csharp
if (!string.IsNullOrWhiteSpace(modulo.defaultDbAlias))
    claims.Add(new Claim("defaulalias", modulo.defaultDbAlias));
```

Por tanto:

- origen: `modulo.defaultDbAlias`
- empaquetado: `TokenService.CrearToken(...)`
- consumo: `HttpContext.User`

## Ejemplo 3: flujo completo de `defaulalias`

### Rutas involucradas

- `DocuArchi.Api/Controllers/Account/AccountController.cs`
- `MiApp.Services/Service/Account/AutenticacionApplicationService.cs`
- `MiApp.Services/Service/Autenticacion/TokenService.cs`
- `DocuArchi.Api/Program.cs`
- `MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/ClaimValidationService.cs`

```text
Login
  -> AutenticacionApplicationService
  -> obtiene modulo
  -> TokenService.CrearToken(...)
  -> agrega claim "defaulalias"
  -> cliente recibe JWT
  -> request con Bearer token
  -> JwtBearer middleware valida token
  -> claim disponible en HttpContext.User
  -> ClaimValidationService lo retorna al controller
```

## Ejemplo 4: agregar un claim de contexto

Caso ejemplo: `sede`

### Rutas involucradas

- `MiApp.DTOs/DTOs/Autenticacion/UsuarioAutenticadoDTO.cs`
- `MiApp.Services/Service/Mapping/GestorDocumental/Usuario/RemitDestInternoMapping.cs`
- `MiApp.Services/Service/Mapping/Workflow/Usuario/UsuarioWorkflowMapping.cs`
- `MiApp.Services/Service/Account/AutenticacionApplicationService.cs`
- `MiApp.Services/Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs`
- `MiApp.Services/Service/Autenticacion/TokenService.cs`

### Paso 1. Agregar propiedad al DTO de autenticación

En [UsuarioAutenticadoDTO.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.DTOs/DTOs/Autenticacion/UsuarioAutenticadoDTO.cs):

```csharp
public string? sede { get; set; }
```

### Paso 2. Llenar el dato antes de crear el token

Opciones:

- mapearlo desde el modelo origen en:
  - [RemitDestInternoMapping.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Mapping/GestorDocumental/Usuario/RemitDestInternoMapping.cs)
  - [UsuarioWorkflowMapping.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Mapping/Workflow/Usuario/UsuarioWorkflowMapping.cs)
- o asignarlo manualmente en:
  - [AutenticacionApplicationService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Account/AutenticacionApplicationService.cs)

### Paso 3. Registrar el nombre del claim

En [DocuArchiClaimTypes.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs):

```csharp
public const string sede = "sede";
```

### Paso 4. Agregarlo al JWT

En [TokenService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Autenticacion/TokenService.cs):

```csharp
if (!string.IsNullOrWhiteSpace(usuario.sede))
    claims.Add(new Claim(DocuArchiClaimTypes.sede, usuario.sede));
```

### Paso 5. Consumirlo

```csharp
var sedeValidation = _claimValidationService.ValidateClaim<string>("sede");
if (!sedeValidation.Success || sedeValidation.ClaimValue == null)
{
    return BadRequest(sedeValidation.Response);
}
```

## Ejemplo 5: agregar un claim de contexto más seguro en este proyecto

Caso sugerido: `idsede`

### Rutas involucradas

- `MiApp.Models/Models/GestorDocumental/usuario/RemitDestInterno.cs`
- `MiApp.DTOs/DTOs/Autenticacion/UsuarioAutenticadoDTO.cs`
- `MiApp.Services/Service/Mapping/GestorDocumental/Usuario/RemitDestInternoMapping.cs`
- `MiApp.Services/Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs`
- `MiApp.Services/Service/Autenticacion/TokenService.cs`

Razón:

- [RemitDestInterno.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Models/Models/GestorDocumental/usuario/RemitDestInterno.cs) ya tiene `Id_Sedes_Empresa`
- no requiere consulta extra al login

### DTO

```csharp
public int? idSede { get; set; }
```

### Mapping

En [RemitDestInternoMapping.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Mapping/GestorDocumental/Usuario/RemitDestInternoMapping.cs):

```csharp
.ForMember(d => d.idSede,
    o => o.MapFrom(s => s.Id_Sedes_Empresa))
```

### Claim type

```csharp
public const string idSede = "idsede";
```

### Token

```csharp
if (usuario.idSede.HasValue && usuario.idSede.Value > 0)
    claims.Add(new Claim("idsede", usuario.idSede.Value.ToString()));
```

### Lectura

```csharp
var sedeValidation = _claimValidationService.ValidateClaim<string>("idsede");
```

## Ejemplo 6: agregar un claim de permiso

Caso ejemplo: `ra.documento.aprobar`

### Rutas involucradas

- `MiApp.Services/Service/Seguridad/Autorizacion/Constructoras/PermissionClaimsOrchestrator.cs`
- `MiApp.Services/Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs`
- `MiApp.Services/Service/Seguridad/Autorizacion/Extensiones/AuthorizationExtensions.cs`
- `MiApp.Services/Service/Autenticacion/TokenService.cs`

### Cuándo aplica

Usa un claim de permiso cuando responde a la pregunta:

- `¿el usuario puede ejecutar esta acción?`

No lo uses para contexto como alias, sede o tenant.

### Patrón actual

El proyecto usa:

- tipo de claim: `perm`
- valor del claim: permiso concreto

Ejemplo actual:

```csharp
claims.Add(new Claim("perm", p));
```

### Dónde se construye

Los permisos salen de builders y del orquestador:

- [PermissionClaimsOrchestrator.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/Constructoras/PermissionClaimsOrchestrator.cs)

### Ejemplo de agregado

En el builder correcto:

```csharp
if (perfil.PUEDE_APROBAR_DOCUMENTO == 1)
    claims.Add(new Claim(DocuArchiClaimTypes.permisos, "ra.documento.aprobar"));
```

### Consumo imperativo

```csharp
if (!_currentUserService.HasPermission("ra.documento.aprobar"))
{
    return Forbid();
}
```

### Consumo declarativo

Si el permiso está en el catálogo de policies:

```csharp
[Authorize(Policy = "ra.documento.aprobar")]
```

## Errores comunes

- agregar el claim pero probar con un token viejo
- usar nombres distintos, por ejemplo `defaultalias` en vez de `defaulalias`
- meter permisos como claim nuevo en vez de reutilizar `perm`
- depender de `HttpContext.User` en capas bajas en vez de pasar el valor como parámetro

## Recomendación práctica

- contexto: usa claims como `defaulalias`, `usuarioid`, `uid`, `idsede`
- permisos: sigue el patrón `perm`

## Resumen rápido de archivos por categoría

### Generación del token

- `MiApp.Services/Service/Autenticacion/TokenService.cs`
- `MiApp.Services/Service/Account/AutenticacionApplicationService.cs`
- `DocuArchi.Api/Controllers/Account/AccountController.cs`

### Lectura y validación de claims

- `MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/CurrentUserService.cs`
- `MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/ClaimValidationService.cs`

### Catálogo y autorización

- `MiApp.Services/Service/Seguridad/Autorizacion/Claims/DocuArchiClaimTypes.cs`
- `MiApp.Services/Service/Seguridad/Autorizacion/Extensiones/AuthorizationExtensions.cs`
- `MiApp.Services/Service/Seguridad/Autorizacion/Constructoras/PermissionClaimsOrchestrator.cs`

### Mappings y datos de autenticación

- `MiApp.DTOs/DTOs/Autenticacion/UsuarioAutenticadoDTO.cs`
- `MiApp.Services/Service/Mapping/GestorDocumental/Usuario/RemitDestInternoMapping.cs`
- `MiApp.Services/Service/Mapping/Workflow/Usuario/UsuarioWorkflowMapping.cs`
- `MiApp.Models/Models/GestorDocumental/usuario/RemitDestInterno.cs`
