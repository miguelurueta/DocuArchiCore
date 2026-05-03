# SCRUM-172 Observabilidad Workflow Log

## Eventos clave
- inicio de construccion de workflow log.
- workflow log omitido por `IdTareaWorkflow` invalido.
- insercion workflow log exitosa.
- fallo en insercion workflow log (con rollback global).

## Campos recomendados
- `requestId`
- `idAlmacen`
- `idTareaWorkflow`
- `idRutaWorkflow`
- `nombreGabinete`
- `radicado`
- `fase`
- `duracionMs`

## Controles de seguridad
- no registrar SQL crudo.
- no registrar `FullText` de indexacion sin sanitizacion.
- no exponer rutas fisicas sensibles ni payload binario documental.

## Troubleshooting operativo
- si no hay log workflow, validar condicion `IdTareaWorkflow > 0`.
- si hay rollback, revisar primero validaciones de repository y `rows != 1`.
- verificar que DI registre builder y repository en `DocuArchi.Api/Program.cs`.

