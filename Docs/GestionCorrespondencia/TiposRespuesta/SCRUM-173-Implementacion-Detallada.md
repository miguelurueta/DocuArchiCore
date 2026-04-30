# SCRUM-173 - Implementacion Detallada

## Archivos creados
- `DocuArchi.Api/Controllers/GestionCorrespondencia/TiposRespuesta/SolicitaListaTiposRespuestaController.cs`
- `MiApp.Services/Service/GestionCorrespondencia/TiposRespuesta/ServiceSolicitaListaTiposRespuesta.cs`
- `MiApp.Repository/Repositorio/GestionCorrespondencia/TiposRespuesta/SolicitaListaTiposRespuestaRepository.cs`

## Archivos actualizados
- `DocuArchi.Api/Program.cs` (registro DI de service/repository)

## Decisiones clave
- Se reutiliza `ResponseDropdownDto` existente para no romper compatibilidad.
- Repository retorna `List<ResponseDropdownDto>`; service es dueño de `AppResponses`.
- Cache implementada en service con TTL de 10 minutos y fallback seguro a DB.
- Controller responde `Ok` si `success=true`; `BadRequest` si `success=false`.

## Seguridad
- Claim obligatorio: `defaulalias`.
- Alias no se recibe por body/querystring.
