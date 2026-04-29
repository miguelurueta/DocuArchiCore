# SCRUM-162 Implementacion Detallada

## resumen de implementación
Se implementó API GET para listar adjuntos de respuesta radicado por `idTareaWf` con autorización por claim y salida `AppResponses`.

## archivos creados
- `DocuArchi.Api/Controllers/GestionCorrespondencia/GestionRespuesta/SolicitaDocumentosAdjuntosRespuestaRadicadoController.cs`
- `MiApp.Services/Service/GestionCorrespondencia/GestionRespuesta/ServiceSolicitaDocumentosAdjuntosRespuestaRadicado.cs`
- `MiApp.Repository/Repositorio/GestionCorrespondencia/GestionRespuesta/SolicitaDocumentosAdjuntosRespuestaRadicadoRepository.cs`
- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/SolicitaDocumentosAdjuntosRespuestaRadicadoControllerTests.cs`
- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/ServiceSolicitaDocumentosAdjuntosRespuestaRadicadoTests.cs`

## archivos modificados
- `DocuArchi.Api/Program.cs` (registro DI)

## rutas exactas
- controller: `DocuArchi.Api/.../SolicitaDocumentosAdjuntosRespuestaRadicadoController.cs`
- service: `MiApp.Services/.../ServiceSolicitaDocumentosAdjuntosRespuestaRadicado.cs`
- repository: `MiApp.Repository/.../SolicitaDocumentosAdjuntosRespuestaRadicadoRepository.cs`
- DTO: definido en `ServiceSolicitaDocumentosAdjuntosRespuestaRadicado.cs`
- DI: `DocuArchi.Api/Program.cs`

## método del controller
`Get([FromQuery] long idTareaWf)`.

## método del service
`SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(long idTareaWf, string defaultDbAlias)`.

## método del repository
`SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(long idTareaWf, string defaultDbAlias)`.

## QueryOptions utilizado
No aplica.

## joins aplicados
No aplica; consulta directa sobre `ra_respuesta_radicado`.

## filtros aplicados
`ID_TAREA_WF = @idTareaWf` + `IdImagen IS NOT NULL` / `IdImagenRespuesta IS NOT NULL`.

## columnas seleccionadas
`ID_RESPUESTA_RADICADO`, `RADICADO`, `ASUNTO`, `IdImagen`, `Gabinete`, `IdImagenRespuesta`, `GabineteRespuesta`.

## deduplicación aplicada
Sí, en service por `(IdRespuestaRadicado, TipoAdjunto, IdImagen)`.

## tratamiento de nulls
`COALESCE` en SQL para strings; validaciones de alias/id en controller/service/repository.

## regla de límite 100
Aplicada en service con `Take(100)`.

## manejo de autorización
`[Authorize]` + `ClaimValidationService` sobre claim `defaulalias`.

## manejo de empty
`success=true`, `message=Sin resultados`, `meta.status=empty`, `data=[]`.

## manejo de errores
`try/catch` en service/repository y respuesta con `errors` tipado.

## logging implementado
Logs informativos en controller y repository (idTareaWf, alias, filas, ms).

## decisiones de naming
Se usa prefijo `Solicita...` alineado con convenciones existentes del módulo.

## registro DI
- `ISolicitaDocumentosAdjuntosRespuestaRadicadoRepository -> SolicitaDocumentosAdjuntosRespuestaRadicadoRepository`
- `IServiceSolicitaDocumentosAdjuntosRespuestaRadicado -> ServiceSolicitaDocumentosAdjuntosRespuestaRadicado`

## consideraciones de performance
Consulta acotada por `idTareaWf`, deduplicación en memoria y truncamiento a 100 para respuesta estable.

## observaciones técnicas finales
Pendiente validar índice en `ID_TAREA_WF` en entornos con alto volumen.
