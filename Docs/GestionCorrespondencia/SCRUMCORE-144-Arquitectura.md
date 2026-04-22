# SCRUMCORE-144 — Arquitectura

## Objetivo

Eliminar el flicker en `GestionRespuesta` corrigiendo la inconsistencia contractual del endpoint que consulta la estructura por `idTareaWf`.

## Ruta canónica (fuente de verdad)

`GET /api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea?idTareaWf={idTareaWf}`

## Contrato (aditivo, no breaking)

Se preserva el shape `AppResponses<List<RaRespuestaRadicado>>` y se agrega `meta.status` como fuente canónica de estado:

- `success`: existen datos utilizables (`data.length > 0`)
- `empty`: ausencia definitiva
- `pending`: solo con señal objetiva de procesamiento/consistencia eventual

Regla de compatibilidad:
- si `meta.status` no está presente, el consumidor aplica semántica legacy basada en `success + data.length`.

## Diagramas (texto)

### Diagrama de clases (alto nivel)

- `SolicitaEstructuraRespuestaIdTareaController`
  - depende de `IClaimValidationService`
  - depende de `IServiceSolicitaEstructuraRespuesta`
- `ServiceSolicitaEstructuraRespuesta`
  - depende de `ISolicitaEstructuraRespuestaIdTareaRepository`
- `SolicitaEstructuraRespuestaIdTareaRepository`
  - depende de `IDbConnectionFactory`
- DTO/Contrato:
  - `AppResponses<T>` con `meta: AppMeta`
  - `AppMeta.Status` (nuevo, aditivo)

### Diagrama de secuencia (resumen)

1) UI → API: `GET ...?idTareaWf=...`
2) Controller:
   - valida claim `defaulalias`
   - valida `idTareaWf > 0`
   - ejecuta service
3) Service:
   - delega al repository
   - normaliza `meta.status` si falta
4) Repository:
   - abre conexión con `defaultDbAlias`
   - consulta (incluye `TRAMITE_DOCUMENTO`)
   - retorna AppResponses con `meta.status`
5) UI:
   - decide render por `meta.status` (cuando exista)

### Diagrama de estados (UI)

- `pending` → mostrar skeleton (no bloqueo definitivo)
- `success` → renderizar master-detail
- `empty` → bloqueo definitivo (sin transición visual a detalle)

## Decisiones SOLID

- SoC: decisión de estado contractual se mantiene en backend (service/repo) y UI solo consume.
- Open/Closed: contrato se amplía de forma aditiva (no se rompe el consumidor legacy).
