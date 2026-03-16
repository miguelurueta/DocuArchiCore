# CLAIMS Uso Backend

## Consumo de claims en backend

Hay dos mecanismos principales de consumo:

1. Acceso general a claims del usuario actual por `ICurrentUserService`
2. Validacion de claims obligatorios por `IClaimValidationService`

## Servicio de acceso actual

El contrato esta en [CurrentUserService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/CurrentUserService.cs).

Capacidades observadas:

- `UserId`: toma `ClaimTypes.NameIdentifier`
- `uid`: toma el claim `"uid"`
- `UserIdInt`: convierte `uid` a entero
- `Permisos`: lee todos los claims `perm`
- `HasPermission(string)`: valida si un permiso existe en memoria
- `GetClaimValue(string)`: lectura genérica de cualquier claim

## Validacion de claims requeridos

El patrón más repetido está en [ClaimValidationService.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/CurrentClaim/ClaimValidationService.cs):

- Si el claim no existe o llega vacío:
  - `Success = false`
  - se construye `AppResponses<T>`
  - se devuelve un `AppError` de tipo `Claim`
- Si existe:
  - `Success = true`
  - `ClaimValue = value`

## Claims usados funcionalmente en controllers

### Alias de base de datos

Varios endpoints validan `defaulalias` antes de ejecutar servicios o repositorios.

Ejemplos:

- [RadicacionController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/Radicacion/Tramite/RadicacionController.cs)
- [PlantillaRadicacionController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/Radicacion/PlantillaRadicado/PlantillaRadicacionController.cs)
- [DynamicUiTableController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/UI/MuiTable/DynamicUiTableController.cs)

Patrón observado:

```csharp
var aliasValidation = _claimValidationService.ValidateClaim<string>("defaulalias");
if (!aliasValidation.Success || aliasValidation.ClaimValue == null)
{
    return BadRequest(aliasValidation.Response);
}
```

### Usuario autenticado

Para lógica transaccional se usa `usuarioid`.

Ejemplo real en [RadicacionController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/Radicacion/Tramite/RadicacionController.cs):

```csharp
var userValidation = _claimValidationService.ValidateClaim<string>("usuarioid");
if (!int.TryParse(userValidation.ClaimValue, out var idUsuarioGestion))
{
    throw new SecurityException("Claim invalido: usuarioid");
}
```

### Lista de permisos

En [MenuL.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Home/Menu/MenuL.cs), el menú se filtra usando:

- `_currentUser.Permisos`
- `_currentUser.HasPermission("wf.diagramador")`
- `_currentUser.HasPermission("ra.interna.radicacion")`

Esto muestra que los claims no solo protegen endpoints: también recortan opciones funcionales y estructura visible al usuario.

### Claims en requests dinámicos

[DynamicUiTableController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/UI/MuiTable/DynamicUiTableController.cs) extrae claims de permisos/roles y los inyecta al request:

- `req.DefaultDbAlias = aliasClaim.ClaimValue`
- `req.UserClaims = ExtractClaims()`

Impacto:

- la lógica dinámica puede adaptar consultas o acciones a permisos presentes en el token
- el controller normaliza solo algunos tipos aceptados: `permiso`, `permission`, `role`, `ClaimTypes.Role`

## Impacto en servicios

En los servicios revisados, los claims afectan:

- resolución del alias de base de datos que se pasa a repositorios
- identificación del usuario que ejecuta operaciones
- filtrado funcional de opciones y permisos

No se observó en esta revisión un patrón único de multi-tenant en repositorios basado directamente en `HttpContext.User`. Lo que sí existe es propagación del alias derivado del claim hacia capas inferiores.

## Impacto en repositories

El impacto observado es indirecto:

- controllers y servicios toman `defaulalias`
- ese valor se envía a servicios/repositorios
- repositorios lo usan como `defaultDbAlias` o `DefaultAlias` en consultas

Esto afecta:

- selección de conexión
- comportamiento por base o módulo
- alcance de datos consultados

No se documenta aquí auditoría directa por claim en repositories porque no se encontró un patrón explícito en los archivos revisados.

## Uso de políticas por permiso

En [AuthorizationExtensions.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Services/Service/Seguridad/Autorizacion/Extensiones/AuthorizationExtensions.cs) se crean políticas dinámicamente:

- por cada permiso del catálogo
- `policy.RequireClaim("perm", permiso)`

Esto habilita autorización declarativa cuando se use `Authorize(Policy = "...")`.

## Endpoint de validación de catálogo

[PermissionTestController.cs](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchi.Api/Controllers/Account/PermissionTestController.cs) expone `api/seguridad/TestUserClaim` para verificación controlada del sistema de permisos.

Características observadas:

- requiere un secreto de validación adicional
- ejecuta un servicio real de validación de claims/permisos
- puede exportar resultados en CSV

## Recomendaciones

- Estandarizar el nombre del claim de alias de base de datos.
- Unificar el uso de `uid` y `usuarioid` o documentar claramente su diferencia funcional.
- Aumentar el uso de políticas declarativas donde ya existe catálogo de permisos.
