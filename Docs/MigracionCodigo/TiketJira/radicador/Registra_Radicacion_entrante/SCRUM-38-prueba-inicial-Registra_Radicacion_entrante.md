# Ticket Jira Propuesto: Correccion inicial de `Registra_Radicacion_entrante`

## Proyecto origen
- Ruta analizada: `D:\imagenesda\GestorDocumental\Desarrollo\Visual-2019\GestionDocumental-Docuarchi.net\GestionDocumental-Docuarchi.net`
- Modulo: `radicador`
- Archivo: `radicador\ClassRadicador.vb`
- Funcion objetivo: `Registra_Radicacion_entrante` (inicia en linea 939)

## Objetivo funcional
Corregir fallas funcionales iniciales en el flujo de radicacion entrante para evitar falsos positivos/negativos de validacion, mensajes de error engañosos y ramas de ejecucion inconsistentes por tipado debil.

## Prueba inicial ejecutada
Se ejecuto una prueba estatica de consistencia sobre el bloque funcional y dio **4/4 casos FAIL**:

1. **T1 FAIL**: Validacion de `UpdatePanel_imp_impresion` usa variable equivocada.
   - Evidencia: lineas 992-995.
   - Defecto: se valida `Hiddendatoradicacion` en lugar de `UpdatePanel_imp_impresion`.

2. **T2 FAIL**: Mensaje de error no corresponde al control validado.
   - Evidencia: lineas 998-1000.
   - Defecto: al fallar `_ValidacionConsulta_val_radicacion`, el mensaje indica `Hiddendatoradicacion`.

3. **T3 FAIL**: Inconsistencia de tipo/valor en `id_flujo_trabajo`.
   - Evidencia: lineas 1030, 1043 y 1064.
   - Defecto: mezcla de comparacion contra string (`"null"`) y numerico (`0`) sobre variable `Object`.

4. **T4 FAIL**: Evaluacion de `Result` sin capturar retorno de llamada.
   - Evidencia: lineas 1065-1070.
   - Defecto: se invoca `Solicita_datos_actividad_inicio_flujo(...)` sin asignar retorno y luego se evalua `Result` previo.

## Alcance de correccion (inmerso en el ticket)
- Ajustar validacion de controles ASP.NET para que cada control se valide contra su propia referencia.
- Corregir mensajes de error para reflejar exactamente el control ausente.
- Normalizar tipo de `id_flujo_trabajo` (recomendado: `Integer?` o `String` con contrato unico) y unificar condicionales.
- Capturar y evaluar retorno real de `Solicita_datos_actividad_inicio_flujo`.
- Mantener compatibilidad funcional del flujo actual de radicacion externa.

## Requerimientos tecnicos
- No alterar contratos publicos de la funcion ni firma actual.
- Evitar regresiones en:
  - validaciones de campos obligatorios,
  - autorizacion de dias/horas,
  - flujo de trabajo documental,
  - transaccion y rollback.
- Registrar errores con mensajes trazables por control y etapa del flujo.

## Criterios de aceptacion
1. Si falta `UpdatePanel_imp_impresion`, la funcion retorna mensaje explicito de ese control y no de otro.
2. Si falta `_ValidacionConsulta_val_radicacion`, el mensaje refleja exactamente ese control.
3. La evaluacion de `id_flujo_trabajo` usa una sola semantica (sin mezclar `"null"` y `0`).
4. `Result` se evalua solo despues de asignar el retorno de cada llamada de negocio.
5. Se conserva comportamiento esperado en escenarios validos de radicacion.

## Casos de prueba minimos a ejecutar
1. Radicacion con controles completos y flujo valido -> resultado `YES`.
2. Faltante de `UpdatePanel_imp_impresion` -> error especifico y salida controlada.
3. Faltante de `_ValidacionConsulta_val_radicacion` -> error especifico y salida controlada.
4. `RE_flujo_trabajo` en valor vacio/0 -> no debe entrar en rama inconsistente.
5. Falla simulada en `Solicita_datos_actividad_inicio_flujo` -> debe propagar error real de retorno.

## Definicion de terminado (DoD)
- Codigo corregido y compilable en solucion actual.
- Pruebas minimas documentadas con resultado.
- Mensajes de error trazables y coherentes con el control validado.
- Evidencia adjunta en Jira: diff y resultados de prueba.

## Prompt IA sugerido para generar subtareas tecnicas
"Analiza `radicador\ClassRadicador.vb` en la funcion `Registra_Radicacion_entrante` y crea subtareas de implementacion para: (1) corregir validacion de controles equivocada, (2) alinear mensajes de error con control faltante, (3) normalizar manejo de `id_flujo_trabajo` sin tipado ambiguo, (4) capturar/validar retornos de llamadas de negocio, (5) definir pruebas de regresion funcional del flujo de radicacion. Incluye criterios de aceptacion verificables por subtarea y estimacion tecnica." 
