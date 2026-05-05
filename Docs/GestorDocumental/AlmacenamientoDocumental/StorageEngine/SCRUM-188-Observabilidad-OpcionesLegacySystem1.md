# SCRUM-188 - Observabilidad Opciones Legacy System1

## Objetivo
Definir el minimo observable para validar que las opciones `system1` gobiernan el flujo de almacenamiento sin ambiguedad.

## Campos de log esperados
- `requestId`
- `nombreGabinete`
- `aplicaInventarioDocumental`
- `aplicaTrd`
- `aplicaUnidadConservacion`
- `estadoResolucion`
- `duracionMs`

## Datos que no se deben loguear
- Fulltext documental
- Contenido del documento
- Datos sensibles del usuario

## Estado SCRUM-188
- Sin nuevo cambio de instrumentacion.
- Se mantiene el estandar definido en implementacion previa y se deja evidencia documental para auditoria.
