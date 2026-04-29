# SCRUM-162 Arquitectura

## METADATA
- identificador del ticket: SCRUM-162
- usuario que creó el ticket: N/D (fuente Jira no visible localmente)
- fecha de creación: 2026-04-29
- módulo afectado: GestionCorrespondencia/GestionRespuesta
- objetivo resumido: exponer API para listar adjuntos asociados a respuesta radicado por `idTareaWf`
- relación con tickets previos: continuidad técnica de SCRUM-159/160/161 (patrón Controller-Service-Repository)
- autor de implementación: Codex + equipo backend
- autor de documentación: Codex
- estado del ticket: En implementación

## objetivo arquitectónico
Crear endpoint seguro, desacoplado y testeable para obtener documentos adjuntos (principal y respuesta) con contrato estándar `AppResponses`.

## contexto funcional
Frontend requiere poblar lista de documentos de una respuesta radicado usando `idTareaWf`.

## alcance y no alcance
- alcance: API GET, service, repository SQL, DI, tests unitarios, documentación técnica.
- no alcance: carga binaria del documento, cambios de esquema BD, UI frontend.

## componentes involucrados
- Controller: `SolicitaDocumentosAdjuntosRespuestaRadicadoController`
- Service: `ServiceSolicitaDocumentosAdjuntosRespuestaRadicado`
- Repository: `SolicitaDocumentosAdjuntosRespuestaRadicadoRepository`
- DTO: `DocumentoAdjuntoRespuestaRadicadoDto`
- ClaimValidationService: validación claim `defaulalias`
- QueryOptions: no aplica (consulta SQL directa por llave)

## diagrama de clases
Controller -> Service -> Repository -> DB (`ra_respuesta_radicado`)

## diagrama de secuencia
Cliente -> Controller (valida claim + query) -> Service (reglas) -> Repository (SQL) -> Service (dedup/límite) -> Controller (`Ok/BadRequest`).

## diagrama de casos de uso
- success: usuario autorizado consulta adjuntos.
- empty: usuario autorizado sin adjuntos.
- unauthorized: claim inválido/no presente.
- error: excepción en acceso a datos.

## diagrama de estados
- success: `meta.status=success`, `data>0`
- empty: `meta.status=empty`, `data=[]`
- unauthorized: `BadRequest` por claim inválido
- error: `BadRequest` con `errors[type=Exception]`

## flujo end-to-end
JWT/Session -> claim `defaulalias` -> GET con `idTareaWf` -> query union adjuntos -> deduplicación -> límite 100 -> respuesta normalizada.

## reglas de seguridad
- endpoint con `[Authorize]`
- no exposición de rutas internas de almacenamiento

## reglas de autorización
- claim obligatorio `defaulalias`
- parámetro `idTareaWf > 0`

## regla de deduplicación
Clave `(IdRespuestaRadicado, TipoAdjunto, IdImagen)`.

## regla de cardinalidad
Máximo 100 registros en salida.

## validación SOLID
- SRP: responsabilidades separadas por capa.
- OCP: ampliable con nuevos tipos de adjunto sin romper contrato.
- DIP: controller/service dependen de interfaces.

## deuda técnica identificada
`Program.cs` contiene ruido de registros repetidos previos no abordados en este ticket.

## riesgos técnicos
- heterogeneidad de datos legacy (`null`/vacíos)
- variabilidad de volumen de resultados

## decisiones arquitectónicas
- mantener `AppResponses<List<T>>`
- aplicar deduplicación en service (regla de negocio)
- usar `UNION ALL` y normalización con `COALESCE` en repository
