# SCRUM-161 Implementacion Detallada

## resumen de implementación
Se creó endpoint orquestado que consolida `usuario-principal-respuesta` y `autorizadas-documento`.

## archivos creados
- DocuArchi.Api/.../SolicitaFirmasDocumentoRespuestaOrquestadoController.cs
- MiApp.Services/.../ServiceSolicitaFirmasDocumentoRespuestaOrquestado.cs
- tests unitarios controller/service

## archivos modificados
- DocuArchi.Api/Program.cs (DI)

## rutas exactas
- controller: DocuArchi.Api/Controllers/GestionCorrespondencia/Firmas/SolicitaFirmasDocumentoRespuestaOrquestadoController.cs
- service: MiApp.Services/Service/GestionCorrespondencia/Firmas/ServiceSolicitaFirmasDocumentoRespuestaOrquestado.cs
- repository: reutilizado vía servicios existentes
- DTO: ResponseDropdownDto (reutilizado)
- DI: DocuArchi.Api/Program.cs

## método del controller
`Get([FromQuery] int idUsuarioGestion)`

## método del service
`SolicitaFirmasDocumentoRespuestaOrquestadoAsync(...)`

## método del repository
No nuevo; se consume existente vía `ServiceSolicitaListaFirmasAutorizadasDocumento`.

## QueryOptions utilizado
Indirecto (service de firmas autorizadas).

## joins aplicados
Indirecto (service de firmas autorizadas).

## filtros aplicados
Indirecto + `idUsuarioGestion` validado en controller/service.

## columnas seleccionadas
Indirecto (Id/Nombre/Cargo).

## deduplicación aplicada
Consolidación por `Id` al combinar listas.

## tratamiento de nulls
Heredado de servicios base.

## regla de límite 100
`Take(100)` en orquestador.

## manejo de autorización
Claim obligatorio + validación en servicio dependiente de autorizadas.

## manejo de empty
`meta.status=empty` y `data=[]`.

## manejo de errores
`try/catch` con `AppError`.

## logging implementado
Reutiliza logging de servicios base.

## decisiones de naming
Sufijo `DocumentoRespuestaOrquestado`.

## registro DI
Se agregó `IServiceSolicitaFirmasDocumentoRespuestaOrquestado`.

## consideraciones de performance
Se evita doble llamada desde frontend; dedupe en memoria sobre dataset acotado.

## observaciones técnicas finales
No se introdujeron cambios en persistencia.

## METADATA
- identificador del ticket: SCRUM-161
- usuario que creó el ticket: (según Jira)
- fecha de creación: (según Jira)
- módulo afectado: Gestión Correspondencia / Firmas
- objetivo resumido: orquestar firmas de documento respuesta
- relación con tickets previos: SCRUM-159, SCRUM-160
- autor de implementación: Codex + equipo backend
- autor de documentación: Codex
- estado del ticket: En implementación
