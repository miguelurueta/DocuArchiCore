# SCRUM-176 - Integracion Frontend

## Endpoint
- Metodo: `GET`
- Ruta: `/api/radicacion/tramite/tipo-doc-entrante/{idTipoDocEntrante}`

## Seguridad
- Requiere claim `defaulalias` en sesión/token.

## Request
- Parametro de ruta: `idTipoDocEntrante` (int).

## Contrato de Respuesta

Tipo:
- `AppResponses<TipoDocEntrante>`

Forma esperada:
```json
{
  "success": "bool",
  "message": "string",
  "data": "TipoDocEntrante | null",
  "errors": "array|null",
  "meta": "object|null"
}
```

`data` corresponde al modelo completo `TipoDocEntrante`.

## Ejemplo success
```json
{
  "success": true,
  "message": "YES",
  "data": {
    "id_Tipo_Doc_Entrante": 302,
    "Descripcion_Doc": "TRAMITE",
    "system_plantilla_radicado_id_plantilla": 100,
    "numero_dias_vence": 5,
    "estado_tipo_documento": 1,
    "flow_tipo": 1,
    "requiere_respuesta": 1,
    "codigo_gabinete_workflow": 23,
    "nombre_gabinete_workflow": "BANDEJA TRAMITE",
    "resp_correo_fisico_electronico": 1,
    "id_ruta": 7,
    "ID_RA_WF_GRUPOS_WORKFLOW_TRAMITE": 12,
    "ID_WF_FLUJOS_TRABAJO": 99,
    "tipo_activo_rad_interno": 1,
    "nombre_tramite": "Solicitud General",
    "tipo_registro": "ENTRANTE",
    "descripcion_tramite": "Radicacion de solicitud general",
    "tipo_tramite": 1,
    "estado_ruta_open_close": 1,
    "obliga_exp_radicado": 0,
    "activo_modulo_respuesta": 1,
    "util_tipo_modulo_envio": 2,
    "util_producion_documental": 0,
    "util_envio_correo_certificado": 1,
    "util_firma_digital_protocolo_respuesta": 0,
    "util_agrega_digital_protocolo_respuesta": 1,
    "tipo_tramite_entrante_saliente": 1,
    "ra_auto_registro_expediente_id_auto_registro": null,
    "wf_copia_doc_expediente_actualiza_exped_gabinete": 0,
    "wf_auto_vincula_doc_expediente_actualiza_exped_gabinete": 0,
    "wf_copia_doc_expediente_produc_actualiza_exped_gabinete": 0,
    "util_auto_vincula_migracion": 0,
    "id_gabinete": 5,
    "util_radicacion_simple": 1,
    "util_nivel_padre_auto_vincula": 0,
    "util_opcion_auto_vincula": 0,
    "util_Estado_Crea_ExpedienteSII": 0,
    "util_Estado_Multiple_expedienteSII": 0,
    "Id_ser_servicioIntegracion": null
  },
  "errors": [],
  "meta": null
}
```

## Ejemplo error
```json
{
  "success": false,
  "message": "Id de tipo documento entrante requerido",
  "data": null
}
```
