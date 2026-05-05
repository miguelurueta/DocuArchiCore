# SCRUM-181 - Observabilidad Opciones TRD/Inventario

## Logs relevantes
- Inicio/final de validacion de pipeline.
- Errores funcionales por codigo:
  - `INV_REQUIRED`
  - `INV_USER_REQUIRED`
  - `INV_EMPRESA_REQUIRED`
  - `TRD_INVALID_AREA`
  - `TRD_INVALID_SERIE`
  - `TRD_INVALID_SUBSERIE`
  - `TRD_INVALID_TIPO_DOCUMENTO`
  - `EXP_CLASE_REQUIRED`
  - `UNI_CLASE_REQUIRED`

## Campos de trazabilidad recomendados
- `requestId`
- `usuarioId`
- `defaultDbAlias`
- `nombreGabinete`
- `tipoAlmacenamiento`

## Diagnostico rapido
- Si falla por inventario: revisar opcion de gabinete + payload `Inventario`.
- Si falla por TRD: revisar IDs negativos o incompletos.
- Si falla por expediente/unidad: validar `IdClaseDocumento`.
