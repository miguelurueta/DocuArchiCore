# SCRUM-176 - Diccionario de Datos

## Tabla: tipo_doc_entrante

Campos destacados para la API:
- `id_Tipo_Doc_Entrante` (PK)
- `Descripcion_Doc`
- `system_plantilla_radicado_id_plantilla`
- `estado_tipo_documento`
- `flow_tipo`
- `requiere_respuesta`
- `codigo_gabinete_workflow`

Campos nuevos incluidos en la respuesta:
- `util_envio_correo_certificado` (INT, default 0)
- `util_firma_digital_protocolo_respuesta` (INT, default 0)
- `util_agrega_digital_protocolo_respuesta` (INT, default 0)

## Observación
La API de SCRUM-176 retorna la estructura completa del modelo `TipoDocEntrante`.
