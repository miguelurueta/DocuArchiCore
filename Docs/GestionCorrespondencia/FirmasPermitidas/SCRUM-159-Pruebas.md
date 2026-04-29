# SCRUM-159 - Pruebas

## Estrategia de pruebas
- Unitarias como primera barrera para validar reglas del endpoint y service.
- Integración sugerida para validar query real contra DB de prueba.
- Contrato/consumo para validar estructura `AppResponses<List<ResponseDropdownDto>>`.

## Matriz unitaria
- Controller:
  - claims inválidos -> `BadRequest`
  - parámetro inválido -> `BadRequest`
  - éxito -> `Ok`
- Service:
  - alias vacío -> validación error
  - sin datos -> `success=true`, `meta.status=empty`
  - duplicados + nulls -> dedupe y fallback correcto

## Matriz integración
- Caso con filas reales autorizadas (`ESTADO_AUTORIZACION_FIRMA=1`).
- Caso sin filas para solicitud.
- Validación de join con `remit_dest_interno`.
- Validación de orden y límite 100.

## Matriz QT
- Flujo UI consulta firmas por solicitud válida.
- Flujo UI sin firmas (estado vacío).
- Flujo UI con token inválido/claims faltantes.
- Flujo UI con error backend controlado.

## Matriz regresión
- No romper endpoint existente `usuario-principal-respuesta`.
- No romper contrato `ResponseDropdownDto`.
- No romper DI general en `Program.cs`.

## Pruebas de autorización
- Claim `defaulalias` faltante.
- Claim `usuarioid` faltante.
- Claim `usuarioid` no numérico.
- Claim `usuarioid <= 0`.

## Pruebas de deduplicación
- Múltiples filas con mismo `Id` deben retornar una sola entrada.

## Pruebas de nulls
- `NombreRemitente` nulo/vacío -> `"Sin nombre"`.
- `CargoRemite` nulo/vacío -> no concatenar guion/cargo.

## Pruebas de límite 100
- Validar en integración que nunca excede 100 registros.

## Casos cubiertos (implementados)
- `SolicitaListaFirmasPermitidasSolicitudAprobacionControllerTests`:
  - claim alias inválido
  - parámetro inválido
  - success
- `ServiceSolicitaListaFirmasPermitidasSolicitudAprobacionTests`:
  - alias vacío
  - sin datos (empty)
  - deduplicación + nulls

## Casos no cubiertos
- Integración real con DB para este endpoint (pendiente).
- Prueba explícita de truncamiento a 100 en ambiente de integración.
- Prueba e2e frontend.

## Hallazgos
- Se detectó y corrigió incompatibilidad de tipo en repository (`IEnumerable` vs `List`).

## Riesgos residuales
- Variaciones de datos reales (espacios, collation, caracteres) pueden impactar formato visual.
- Si cambia esquema de columnas/join, puede fallar consulta.
