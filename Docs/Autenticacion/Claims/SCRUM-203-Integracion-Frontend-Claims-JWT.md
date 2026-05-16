# SCRUM-203 - Integracion Frontend Claims JWT

## Objetivo
Incorporar en el JWT 4 claims de relacion multi-modulo, manteniendo compatibilidad con claims legacy.

## Claims Nuevos
| Claim | Origen DB (`remit_dest_interno`) | Tipo | Obligatorio en JWT | Fallback |
|---|---|---|---|---|
| `IdUsuarioWorkflow` | `Relacion_Workflow` | `string` (numero serializado) | Si | `0` |
| `IdUsuarioWorkflowExt` | `Relacion_Workflow_Extend` | `string` (numero serializado) | Si | `0` |
| `IdUsuarioRadicador` | `Relacion_Id_Usuario_Radicacion` | `string` (numero serializado) | Si | `0` |
| `IdUsuarioDa` | `Relacion_Da` | `string` (numero serializado) | Si | `0` |

## Claims Legacy (sin cambios)
- `usuarioid`
- `uid`
- `defaulalias`
- `defaulaliaswf`
- `perm` (repetible)

## Contrato de Emision
- Login normal: el token final incluye claims legacy + claims nuevos.
- Login con 2FA: el token final incluye claims legacy + claims nuevos (mismo contrato del login normal).
- El frontend no debe enviar estos claims manualmente; vienen en el JWT emitido por backend.

## Ejemplo Payload JWT (Login Normal)
```json
{
  "sub": "1:2:141",
  "emp": "1",
  "mod": "2",
  "modName": "GESTOR DOCUMENTAL",
  "uid": "141",
  "usuarioid": "141",
  "login": "musuario",
  "name": "Usuario Demo",
  "defaulalias": "DA",
  "defaulaliaswf": "WF",
  "IdUsuarioWorkflow": "137",
  "IdUsuarioWorkflowExt": "0",
  "IdUsuarioRadicador": "52",
  "IdUsuarioDa": "141",
  "perm": ["WORKFLOW", "GESTION_DOCUMENTAL"]
}
```

## Ejemplo Payload JWT (2FA)
```json
{
  "sub": "1:2:141",
  "emp": "1",
  "mod": "2",
  "uid": "141",
  "usuarioid": "141",
  "defaulalias": "DA",
  "defaulaliaswf": "WF",
  "IdUsuarioWorkflow": "137",
  "IdUsuarioWorkflowExt": "0",
  "IdUsuarioRadicador": "52",
  "IdUsuarioDa": "141",
  "perm": ["WORKFLOW", "GESTION_DOCUMENTAL"]
}
```

## Consumo Frontend
1. Decodificar JWT autenticado.
2. Leer claims nuevos por nombre exacto.
3. Convertir a entero localmente.
4. Tratar `0` como "sin relacion configurada".

## Archivos Implementados
- `MiApp.DTOs/DTOs/Autenticacion/UsuarioAutenticadoDTO.cs`
- `MiApp.Services/Service/Mapping/GestorDocumental/Usuario/RemitDestInternoMapping.cs`
- `MiApp.Services/Service/Autenticacion/TokenService.cs`
