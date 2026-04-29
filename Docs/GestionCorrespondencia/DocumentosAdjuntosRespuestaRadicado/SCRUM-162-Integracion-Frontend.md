# SCRUM-162 Integracion Frontend

## descripción de API
Lista documentos adjuntos de respuesta radicado asociados a una tarea de workflow.

## método HTTP
GET

## ruta oficial
`/api/GestionCorrespondencia/solicita-documentos-adjuntos-respuesta-radicado`

## query params
- `idTareaWf` (long, requerido, > 0)

## origen esperado de idSolicitudAprobacion
No aplica en este endpoint. El identificador requerido es `idTareaWf` (tomado del contexto de tarea en bandeja/gestión respuesta).

## claims requeridos
- `defaulalias`

## response AppResponses<List<ResponseDropdownDto>>
Para este endpoint el contrato real es `AppResponses<List<DocumentoAdjuntoRespuestaRadicadoDto>>`.

## significado de
- success: indica resultado lógico de la operación.
- message: texto de estado (`YES`, `Sin resultados`, error).
- data: lista de adjuntos.
- meta.status: `success` o `empty`.
- errors: detalle técnico/validación.

## ejemplo request válido
`GET /api/GestionCorrespondencia/solicita-documentos-adjuntos-respuesta-radicado?idTareaWf=1250`

## ejemplo response success
```json
{
  "success": true,
  "message": "YES",
  "data": [
    {
      "IdRespuestaRadicado": 33,
      "Radicado": "2026-001",
      "Asunto": "Respuesta",
      "TipoAdjunto": "DocumentoPrincipal",
      "IdImagen": 9981,
      "Gabinete": "ARCHIVO"
    }
  ],
  "meta": { "Status": "success" },
  "errors": []
}
```

## ejemplo response empty
```json
{
  "success": true,
  "message": "Sin resultados",
  "data": [],
  "meta": { "Status": "empty" },
  "errors": []
}
```

## ejemplo response unauthorized/error
```json
{
  "success": false,
  "message": "DefaultDbAlias requerido",
  "data": [],
  "errors": [
    { "Type": "Validation", "Field": "defaultDbAlias", "Message": "DefaultDbAlias requerido" }
  ]
}
```

## reglas de consumo frontend
- manejar `success=false` como error funcional.
- no asumir orden fijo fuera de `IdRespuestaRadicado DESC`.
- respetar que la lista puede estar truncada a 100.

## cómo poblar dropdown/listado de firmas
No aplica firmas; poblar listado de adjuntos con `TipoAdjunto`, `Radicado`, `Asunto`.

## UX para lista vacía
Mostrar estado vacío con opción de reintento.

## UX para error
Mostrar mensaje amigable y log técnico en consola/telemetría.

## UX para no autorizado
Redirigir a login o mostrar bloqueo por permisos.

## consideraciones de seguridad
No exponer internamente rutas físicas; consumir solo `IdImagen`/`Gabinete` para posteriores APIs seguras.

## no asumir que siempre hay firmantes
Aplicable por analogía: no asumir que siempre hay adjuntos.
