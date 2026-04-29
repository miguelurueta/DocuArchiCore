# SCRUM-159 - Implementación Detallada

## Resumen de implementación
Se implementó el endpoint para listar firmas permitidas por solicitud de aprobación, incluyendo validación de claims, validación de parámetros, consulta con join, deduplicación, fallback de nulls, límite de registros y respuestas estandarizadas.

## Archivos creados
- `DocuArchi.Api/Controllers/GestionCorrespondencia/Firmas/SolicitaListaFirmasPermitidasSolicitudAprobacionController.cs`
- `MiApp.Services/Service/GestionCorrespondencia/Firmas/ServiceSolicitaListaFirmasPermitidasSolicitudAprobacion.cs`
- `MiApp.Repository/Repositorio/GestionCorrespondencia/Firmas/SolicitaListaFirmasPermitidasSolicitudAprobacionRepository.cs`
- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/SolicitaListaFirmasPermitidasSolicitudAprobacionControllerTests.cs`
- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/ServiceSolicitaListaFirmasPermitidasSolicitudAprobacionTests.cs`

## Archivos modificados
- `DocuArchi.Api/Program.cs`

## Rutas exactas
- Controller:
  - `D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\GestionCorrespondencia\Firmas\SolicitaListaFirmasPermitidasSolicitudAprobacionController.cs`
- Service:
  - `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\GestionCorrespondencia\Firmas\ServiceSolicitaListaFirmasPermitidasSolicitudAprobacion.cs`
- Repository:
  - `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\GestionCorrespondencia\Firmas\SolicitaListaFirmasPermitidasSolicitudAprobacionRepository.cs`
- DTO:
  - `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\Common\ResponseDropdownDto.cs`
  - `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\Utilidades\AppResponses.cs`
- DI:
  - `D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Program.cs`

## Método del controller
- `Get([FromQuery] long idSolicitudAprobacion)`
- Ruta: `GET api/gestion-correspondencia/firmas/permitidas-por-solicitud`

## Método del service
- `SolicitaListaFirmasPermitidasPorSolicitudAsync(long idSolicitudAprobacion, int usuarioId, string defaultDbAlias)`

## Método del repository
- `SolicitaListaFirmasPermitidasPorSolicitudAsync(long idSolicitudAprobacion, string defaultDbAlias)`

## QueryOptions utilizado
- `TableName`: `ra_cd_usuarios_solicitudes_aprobacion`
- `DefaultAlias`: claim `defaulalias`
- `Limit`: `100`
- `OrderBy`: `rdi.Nombre_Remitente ASC`

## Joins aplicados
- `INNER JOIN remit_dest_interno AS rdi ON rdi.id_Remit_Dest_Int = ra_cd_usuarios_solicitudes_aprobacion.Remit_Dest_Interno_id_remit_dest_Int`

## Filtros aplicados
- `RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION = idSolicitudAprobacion`
- `ESTADO_AUTORIZACION_FIRMA = 1`

## Columnas seleccionadas
- `ra_cd_usuarios_solicitudes_aprobacion.Remit_Dest_Interno_id_remit_dest_Int AS Id`
- `rdi.Nombre_Remitente AS NombreRemitente`
- `rdi.Cargo_Remite AS CargoRemite`

## Deduplicación aplicada
- Agrupación en service por `Id` (`GroupBy(x => x.Id)`), tomando la primera ocurrencia.

## Tratamiento de nulls
- `NombreRemitente` nulo/vacío -> `"Sin nombre"`.
- `CargoRemite` nulo/vacío -> omitido en descripción.
- Descripción final:
  - Con cargo: `"Nombre - Cargo"`
  - Sin cargo: `"Nombre"`

## Regla de límite 100
- En repository (`QueryOptions.Limit = 100`).
- Defensa adicional en service (`Take(100)`).

## Manejo de autorización
- `[Authorize]` en controller.
- Validación de claims `defaulalias` y `usuarioid`.
- `usuarioid` inválido -> `BadRequest` con error de validación.

## Manejo de empty
- Si no hay filas, `success=true`, `data=[]`, `meta.status="empty"`, `message="Sin resultados"`.

## Manejo de errores
- `try/catch` en service.
- Excepción controlada con `success=false`, `meta.status="error"` y `errors` tipado.

## Logging implementado
- Repository registra warning cuando query no retorna datos o falla (`_logger.LogWarning(...)`).

## Decisiones de naming
- Nombre explícito orientado a caso de uso:
  - `SolicitaListaFirmasPermitidasSolicitudAprobacion*`
- DTO de salida reutiliza estándar existente: `ResponseDropdownDto`.

## Registro DI
- `IServiceSolicitaListaFirmasPermitidasSolicitudAprobacion -> ServiceSolicitaListaFirmasPermitidasSolicitudAprobacion`
- `ISolicitaListaFirmasPermitidasSolicitudAprobacionRepository -> SolicitaListaFirmasPermitidasSolicitudAprobacionRepository`

## Consideraciones de performance
- Filtros selectivos en DB.
- Límite duro de 100 elementos.
- Proyección de columnas necesarias (sin SELECT *).

## Observaciones técnicas finales
- Se corrigió retorno de repository a `List` explícita (`ToList`) para compatibilidad de contrato.
- Pruebas unitarias enfocadas en claims inválidos, parámetros inválidos, deduplicación, fallback nulls, empty y success.
