# SCRUM-85 - Integracion Backend

## Objetivo

Ajustar `ValidarActividadInicioFlujoAsync` dentro de `RegistrarRadicacionEntranteAsync` para que retorne el modelo `SolicitaDatosActividadInicioFlujo`, permita almacenar el resultado en el flujo principal y maneje excepciones con respuesta controlada.

## Cambios aplicados

- `RegistrarRadicacionEntranteService` ahora recibe el resultado de `ValidarActividadInicioFlujoAsync` como `AppResponses<SolicitaDatosActividadInicioFlujo?>`.
- Cuando el flujo workflow aplica y no existe actividad inicial valida, `RegistrarRadicacionEntranteAsync` detiene el proceso con respuesta de validacion controlada.
- `ValidarActividadInicioFlujoAsync` encapsula su propio `try/catch` y retorna error controlado si falla la lectura de claims o la consulta previa.
- Se ampliaron las pruebas unitarias para cubrir la excepcion controlada del helper antes de la validacion workflow y del registro final.

## Comportamiento funcional

- Si el request no trae `RE_flujo_trabajo.id_tipo_flujo_workflow`, la validacion retorna `success=true` con `data=null` y el flujo continua sin validacion workflow inicial.
- Si el request workflow trae un flujo pero no existe actividad inicial valida, el servicio responde `success=false` y no llama el repositorio final de registro.
- Si una dependencia del helper falla, el servicio responde `Error al validar actividad inicial workflow` y tampoco continua con prevalidacion ni registro.

## Evidencia tecnica

- `tests/TramiteDiasVencimiento.Tests/RegistrarRadicacionEntranteServiceTests.cs`
- Escenario nuevo: `RegistrarRadicacionEntranteAsync_CuandoValidarActividadInicioFlujoAsyncLanzaExcepcion_RetornaErrorControlado`
