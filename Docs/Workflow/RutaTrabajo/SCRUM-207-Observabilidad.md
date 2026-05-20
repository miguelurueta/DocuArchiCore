# SCRUM-207 - Observabilidad y Troubleshooting

## 1. Qué monitorear
1. Tasa de `400` por claim `defaulaliaswf` faltante.
2. Tasa de `400` por validaciones (`consecutivoRadicado`, `idTareaWorkflow`, `Nombre_Ruta`).
3. Tasa de `success=true` con `EstadoExistenciaRadicado=NO` (sin registro en ruta).
4. Latencia de consulta en tabla dinámica.

## 2. Eventos clave del flujo
1. Inicio de solicitud por radicado/tarea.
2. Resultado de resolución de ruta activa.
3. Resultado de consulta `dat_adic_tar{ruta}`.
4. Resultado de consulta `configuracion_gabinete`.
5. Fin de solicitud con estado funcional (`YES/NO`).

## 3. Fallas comunes y diagnóstico

## 3.1 `400` Claim faltante
- Verificar JWT:
  - `defaulaliaswf`

## 3.2 `400` `Nombre_Ruta` inválido
- Verificar valor en `rutas_workflow.Nombre_Ruta`.
- Solo se permiten: letras, números y `_`.

## 3.3 `success=true` y `EstadoExistenciaRadicado=NO`
- No existe fila en tabla dinámica para el radicado o id de tarea.
- Validar que el registro sí esté en `dat_adic_tar{ruta}` correcta.

## 3.4 Error `NombreGabinete requerido`
- `ID_GABINETE` en fila workflow existe, pero no se resolvió `Nombre_Gabinete`.
- `id_Gabinete` no existe en `configuracion_gabinete`.
- Falla de consulta en la base workflow para `configuracion_gabinete`.

## 4. Recomendaciones operativas
1. Registrar `nombreRuta` resuelta en logs de depuración.
2. Registrar alias workflow usado para tabla dinámica y `configuracion_gabinete`.
3. Correlacionar por `requestId` cuando el gateway lo provea.
