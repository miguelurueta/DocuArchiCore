# SCRUM-175 - Actualizacion TipoDocEntrante

## Resumen

Se incorporan tres flags nuevos en `tipo_doc_entrante`:

- `util_envio_correo_certificado` (INT, default `0`)
- `util_firma_digital_protocolo_respuesta` (INT, default `0`)
- `util_agrega_digital_protocolo_respuesta` (INT, default `0`)

## Impacto Tecnico

- Modelo `TipoDocEntrante` actualizado para mapear los tres campos.
- DTO `TipoDocEntranteParametroDto` actualizado para exponer los tres campos a servicios consumidores.
- Servicio `SolicitaParametrosRadicadosService` actualizado para mapear los nuevos valores desde repositorio hacia DTO de salida.
- Fixtures SQL de pruebas actualizados (`schema.sql` y `seed.sql`) para reflejar el esquema de BD vigente.

## Validacion

- Se agregan/ajustan valores en tests de radicacion para mantener cobertura de mapeo y compatibilidad.
- El comportamiento existente del flujo de radicacion se conserva; los nuevos campos se propagan sin alterar reglas previas.

## Nota de Alcance

El archivo `RemitDestInterno.cs` no aplica a este cambio, porque los campos nuevos pertenecen a la tabla `tipo_doc_entrante`.
