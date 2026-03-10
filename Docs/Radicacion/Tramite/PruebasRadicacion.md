# Pruebas Funcionales - Flujo Registrar Entrante

## Endpoint

- `POST /api/radicacion/registrar-entrante`

## Casos Cubiertos

1. Exito con datos validos:
   - retorna `success=true`
   - retorna DTO con `ConsecutivoRadicado`
2. Sin resultados:
   - retorna `success=true`
   - `data=null`
   - `message="Sin resultados"`
3. Error de validacion:
   - retorna `success=false`
   - retorna lista de errores en `errors/data`
4. Excepcion simulada:
   - controller retorna `500` con `AppResponses` controlado

## Ajustes De Validacion De Campos

En `ValidaCamposObligatorios` se excluyen de obligatorios los campos generados por backend:

- `Usuario_Radicador_id_usuario`
- `Consecutivo_Rad`
- `Consecutivo_CodBarra`
- `Fecha_Radicado`
- `Codigo_Sede`
- `Id_area_remit_dest_interno`
- `Area_remit_dest_interno`
- `CARGO_DESTINATARIO`

Estos valores se completan dentro del backend durante el registro.
