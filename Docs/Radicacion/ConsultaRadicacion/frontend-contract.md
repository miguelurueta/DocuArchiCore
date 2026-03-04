# Contrato Frontend - Consulta Radicacion

## Endpoint

- Metodo: `POST`
- URL: `/api/tramite/consulta-radicacion/apListaCoinsidenciaRadicados`
- Auth: JWT requerido
- Claims obligatorios: `defaulalias`, `usuarioid`

## Request DTO

`ParametroCoinsidenciaRadicadoDTO`

- `TextoBuscado`: `string`
- `TipoModuloDeConsulta`: `int`
  - `1`: validacion
  - `2`: consulta radicados

## Response DTO

`AppResponses<DynamicUiTableDto>`

- `success`: bandera de exito
- `message`: `OK`, `Sin resultados` o detalle de error
- `data`: tabla dinamica compatible con MUI
  - `Columns`: configuracion de columnas
  - `Rows`: registros con `id` y `Values`
  - `RowActions`: acciones por fila segun `TipoModuloDeConsulta`

## Nota SCRUM-38

- La columna `estado_validacion` ya no se incluye en `Columns` cuando `TipoModuloDeConsulta = 1`.
- El frontend no debe depender de esa clave para renderizado o validacion.

## Manejo en UI

1. Enviar `TextoBuscado` con debounce.
2. Renderizar columnas y filas desde `DynamicUiTableDto`.
3. Si `data == null` y `success == true`, mostrar estado vacio.
4. Si `success == false`, mostrar `message` y `errors`.
