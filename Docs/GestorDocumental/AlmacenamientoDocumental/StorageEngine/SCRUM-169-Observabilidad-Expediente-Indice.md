# SCRUM-169 Observabilidad Expediente Indice

## Eventos clave
- lock expediente OK / fallo.
- lock unidad OK / fallo.
- calculo indice OK.
- update expediente OK / conflicto.
- update unidad OK / conflicto.
- insert indice electronico OK / fallo.

## Campos sugeridos
- `requestId`
- `idExpediente`
- `idUnidadConservacion`
- `idRegistroProduccionDocumental`
- `nuevoOrden`
- `paginaInicial`
- `paginaFinal`
- `numeroFolios`
- `fase`
- `duracionMs`

## No registrar
- `FullText` documental.
- payload binario.
- SQL crudo con datos sensibles.
