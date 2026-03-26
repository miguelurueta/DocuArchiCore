# SCRUM-101 - Migracion de Contrato

## DTO actualizado

`ReturnRegistraRadicacionDto` ahora contiene:

- `ConsecutivoRadicado`
- `IdRadicado`
- `IdEstadoRadicado`

## Compatibilidad

Si consumidores antiguos reciben una respuesta donde `ReturnRegistraRadicacion.IdEstadoRadicado` viene vacio pero `MetadataOperativa["idEstadoRadicado"]` existe, el servicio rellena el valor antes de retornar.
