## PLAN DE MIGRACIÓN PROPUESTO

Función objetivo: Registra_Radicacion_entrante
Estado: Previo a tickets (solo planificación técnica)

## FASE 1 — Inventario de funciones legacy relacionadas

1. Registra_Radicacion_entrante
   Archivo: D:\imagenesda\GestorDocumental\Desarrollo\GestionDocumental-Docuarchi.net\GestionDocumental-Docuarchi.net\radicador\ClassRadicador.vb
   Evento disparador: Buttonradicar_entrante_Click en WebFormRadicacionEntrante.aspx.vb
   Tipo: Registro + validación + orquestación de flujo.

2. Registra_Radicacion_entrante_interna
   Archivo: ...radicador\ClassRadicador.vb
   Evento disparador: WebFormRadicacionEntranteInterna.aspx.vb
   Tipo: Registro interno + validación + workflow.

3. Registra_Radicacion_saliente
   Archivo: ...radicador\ClassRadicador.vb
   Evento disparador: mismo botón con rama RADICACION SALIENTE
   Tipo: Registro saliente.

4. Funciones auxiliares invocadas desde el flujo principal (clases de radicador/plantilla/workflow)
   Tipo: consultas, validaciones de reglas, persistencia y trazabilidad.

## FASE 2 — Reconstrucción del flujo funcional real

1. UI WebForms carga controles y scripts en WebFormRadicacionEntrante.aspx + js/radicacion/WebFormRadicacionEntrante.js.
2. Usuario acciona botón Buttonradicar_entrante.
3. Buttonradicar_entrante_Click valida presencia de controles por FindControl y llama Registra_Radicacion_entrante(...).
4. Registra_Radicacion_entrante ejecuta:
   - validaciones de controles requeridos (Hidden, CheckBox, DropDownList, TextBox);
   - validación de permisos por día/hora;
   - validación de flujo/actividad inicial;
   - carga de campos de plantilla + scripts;
   - validaciones de obligatorios, formatos y reglas de negocio;
   - verificaciones de expediente/radicados relacionados;
   - persistencia y operaciones auxiliares.
5. Resultado vuelve como string ("YES" o mensaje de error).
6. UI actualiza estado, tabs y flujos de impresión según hidden/session.

Validaciones detectadas:

1. Frontend JS: navegación de tabs, selección grid, disparo de acciones.
2. Backend WebForms/VB: validaciones funcionales fuertes (la mayoría en VB).
3. Transformaciones: lectura de campos dinámicos por nombre de control, parsing de valores y fechas.
4. Dependencias entre controles: check_nuevo_radicado, Check_anexo_radicado, CheckBox_relacionado_radicado, Hiddenid_expediente, Hiddentramiteseleccion, etc.
5. Comportamiento dinámico: ramas por tipo de radicación y por tipo de impresión.

## FASE 3 — Conversión de parámetros legacy a DTO

Mapeo inicial propuesto (sin suposiciones no confirmadas):

1. Page1.FindControl("Hiddentramiteseleccion").Value -> RegistrarRadicacionRequest.TramiteCodigo
2. Page1.FindControl("Hiddendestinatario").Value -> RegistrarRadicacionRequest.DestinatarioCodigo
3. Page1.FindControl("Hiddenid_expediente").Value -> RegistrarRadicacionRequest.IdExpediente
4. check_nuevo_radicado.Checked -> RegistrarRadicacionRequest.EsNuevo
5. Check_anexo_radicado.Checked -> RegistrarRadicacionRequest.EsAnexo
6. CheckBox_relacionado_radicado.Checked -> RegistrarRadicacionRequest.EsRelacionado
7. Dropdowlis_sel_val_radciacion (selección) -> RegistrarRadicacionRequest.RadicadosRelacionados[]
8. Session.Item("RA_MODULO_SELECCIONADO") -> RegistrarRadicacionRequest.ModuloSeleccionado o resolver en backend.
9. Session.Item("...usuario...") -> Claim usuarioid (target).
10. Session.Item("...alias...") -> Claim defaulalias (target).
11. Campos dinámicos RE_<CAMPO>-<CAMPO>-<TIPO> -> RegistrarRadicacionRequest.Campos[].

DTO request mínimo propuesto:

1. RegistrarRadicacionRequest
2. int IdPlantilla
3. string TipoPlantilla
4. string NombrePlantilla
5. bool EsNuevo
6. bool EsAnexo
7. bool EsRelacionado
8. int? IdExpediente
9. List<int> RadicadosRelacionados
10. List<CampoRadicacionDto> Campos
11. ContextoRadicacionDto Contexto (si aplica: flags de impresión/flujo)

DTO response propuesto:

1. RegistrarRadicacionResponse
2. string ConsecutivoRadicado
3. string EstadoAsignacion
4. List<string> Alertas
5. object MetadataOperativa

Complemento FASE 3 con evidencia del formulario frontend (React entregado por el usuario):

1. `name="nuevoRadicado"` / `id="nuevoRadicado"` -> `RegistrarRadicacionRequest.EsNuevo`
2. `name="relacionarRadicado"` / `id="relacionarRadicado"` -> `RegistrarRadicacionRequest.EsRelacionado`
3. `name="buscarRadicado"` / `id="buscarRadicado"` -> `RegistrarRadicacionRequest.BuscarRadicado`
4. `name="expedienteRelacionado"` -> `RegistrarRadicacionRequest.IdExpediente`
5. `name="tramite"` -> `RegistrarRadicacionRequest.TramiteCodigo`
6. `name="flujo"` -> `RegistrarRadicacionRequest.FlujoCodigo`
7. `name="fechaLimite"` -> `RegistrarRadicacionRequest.FechaLimiteRespuesta`
8. `name="asunto"` -> `RegistrarRadicacionRequest.Asunto`
9. `name="remitente"` -> `RegistrarRadicacionRequest.RemitenteId`
10. `name="destinatario"` -> `RegistrarRadicacionRequest.DestinatarioId`
11. `name="anexos"` -> `RegistrarRadicacionRequest.Anexos`
12. `name="tipo-radicacion"` -> `RegistrarRadicacionRequest.TipoRadicacion`

Contrato de transporte detectado en frontend actual:

1. `RadicacionPayloadDTO { Campos: [{ IdDetallePlantillaRadicado, NombreCampo, Valor }] }`
2. Recomendación de migración: compatibilizar `Campos[]` con `RegistrarRadicacionRequest` en capa Service (mapping interno).

Pendientes sin suposición (requieren decisión):

1. El botón `Radicar` dispara `form.submit()`, pero se debe confirmar en qué punto exacto se ejecuta el `onFinish`/mutación API final del flujo.

## FASE 4 — Mapeo de consultas DB a Repository

Patrón obligatorio:

1. Query parametrizada.
2. QueryOptions.DefaultAlias = defaultDbAlias.
3. Método repository por responsabilidad.

Matriz SQL detallada (extraída del flujo de `Registra_Radicacion_entrante`):

1. Q01 — Lectura de consecutivo con lock  
   SQL legacy: `SELECT Consecutivo_Rad,Consecutivo_CodBarra FROM system_plantilla_consecutivo_radicado FOR UPDATE`  
   Variante: `SELECT Consecutivo_Rad,Consecutivo_CodBarra FROM system_plantilla_Radicado WHERE id_plantilla = ? FOR UPDATE`  
   Tablas: `system_plantilla_consecutivo_radicado`, `system_plantilla_radicado`  
   Parámetros: `id_plantilla` (variante por plantilla)  
   Alias: DA (`docuarchi`)  
   Repository: `IRadicacionConsecutivoRepository.LockAndGetAsync`

2. Q02 — Actualización de consecutivo  
   SQL legacy: `UPDATE system_plantilla_consecutivo_radicado SET Consecutivo_Rad=?, Consecutivo_CodBarra=?`  
   Variante: `UPDATE system_plantilla_radicado SET Consecutivo_Rad=?, Consecutivo_CodBarra=? WHERE id_plantilla=?`  
   Tablas: `system_plantilla_consecutivo_radicado`, `system_plantilla_radicado`  
   Parámetros: `consecutivoRad`, `consecutivoCodBarra`, `id_plantilla`  
   Alias: DA  
   Repository: `IRadicacionConsecutivoRepository.UpdateAsync`

3. Q03 — Inserción principal del radicado en tabla de plantilla (dinámica)  
   SQL legacy: `INSERT INTO {nombre_plantilla} (...) VALUES (...)`  
   Tablas: tabla dinámica `{nombre_plantilla}`  
   Parámetros: destinatario, remitente, usuario, plantilla, consecutivos, fecha, sede, área, tipo radicado, cargo, campos dinámicos, flags, tipo trámite, flujo  
   Alias: DA  
   Repository: `IRadicacionRegistroRepository.InsertRadicadoPlantillaAsync`

4. Q04 — Inserción de relación de radicados (cuando EsRelacionado)  
   SQL legacy: `INSERT INTO ra_entr_relacion_radicados (...) VALUES (...)` (multi-row)  
   Tabla: `ra_entr_relacion_radicados`  
   Parámetros: plantilla padre/hijo, id radicado plantilla, consecutivo padre, consecutivos relacionados  
   Alias: DA  
   Repository: `IRadicacionRelacionRepository.InsertRelacionesAsync`

5. Q05 — Registro general de radicado  
   SQL legacy: `INSERT INTO ra_registro_general_radicacion (...) VALUES (...)`  
   Tabla: `ra_registro_general_radicacion`  
   Parámetros: `system_plantilla_radicado_id_plantilla`, `id_radicado_plantilla`, `nombre_plantilla`, `consecutivo`, `codbarra`, `flag_flow`  
   Alias: DA  
   Repository: `IRadicacionRegistroGeneralRepository.InsertAsync`

6. Q06 — Registro de respuesta al trámite (condicional)  
   SQL legacy: `INSERT INTO ra_respuesta_radicado (...) VALUES (...)`  
   Tabla: `ra_respuesta_radicado`  
   Parámetros: destinatario interno, área, plantilla, radicado, fecha registro, fecha vence, destinatario externo, trámite, asunto, responsable, tipo respuesta obligatoria  
   Alias: DA  
   Repository: `IRadicacionRespuestaRepository.InsertSolicitudRespuestaAsync`

7. Q07 — Log de respuesta  
   SQL legacy: `INSERT INTO ra_log_respuesta_radicado (...) VALUES (...)`  
   Tabla: `ra_log_respuesta_radicado`  
   Parámetros: desc operación, usuario, id user, fecha/hora, id respuesta, ip, módulo, campos  
   Alias: DA  
   Repository: `IRadicacionRespuestaRepository.InsertLogAsync`

8. Q08 — Estado inicial de módulo radicación (condicional por tipo envío)  
   SQL legacy: `INSERT INTO ra_rad_estados_modulo_radicacion (...) VALUES (...)`  
   Tabla: `ra_rad_estados_modulo_radicacion`  
   Parámetros: plantilla, id radicado, consecutivo, fecha registro, estado, remitente, id usuario, id tarea workflow, tipo trámite  
   Alias: DA  
   Repository: `IRadicacionEstadoRepository.InsertEstadoInicialAsync`

9. Q09 — Inserción PQR (en ramas de trámite específicas)  
   SQL legacy: `INSERT INTO ra_registro_pqr (...) VALUES (...)`  
   Tabla: `ra_registro_pqr`  
   Parámetros: radicado, remitente/destinatario, fecha, externo, usuario workflow, actividad workflow, asunto, texto  
   Alias: DA  
   Repository: `IRadicacionPqrRepository.InsertAsync`

Reglas técnicas de cierre Fase 4:

1. Todas las queries anteriores deben ejecutarse parametrizadas (sin concatenación SQL).
2. Usar `QueryOptions.DefaultAlias = defaultDbAlias` en el 100% de repositorios.
3. Ejecutar Q01-Q08 dentro de una misma transacción de aplicación.
4. Atomicidad obligatoria del bloque Q01-Q08: si falla cualquier registro, se debe ejecutar rollback completo y no confirmar cambios parciales.
5. La tabla dinámica `{nombre_plantilla}` solo se permite con whitelist de nombres válidos.

Pendiente único para cierre completo de implementación:

1. Desglosar en ticket técnico la lista de funciones auxiliares con SQL indirecto (horarios, flujo inicial, validaciones de destinatario externo y obligatorios por trámite) para completar cobertura 100% del flujo.

## FASE 5 — Mapeo Legacy -> Arquitectura Target

1. WebForm `Buttonradicar_entrante_Click` -> `POST /api/radicacion/registrar-entrante` (Controller API).
2. `ClassRadicador.Registra_Radicacion_entrante` -> `RadicacionRegistroService.RegistrarEntranteAsync`.
3. `FindControl + Hidden + Session` -> `Request DTO + Claims` (`usuarioid`, `defaulalias`).
4. SQL legacy concatenado/mixto -> Repositories Dapper con query parametrizada y `QueryOptions.DefaultAlias`.
5. Resultado `"YES"` o texto error -> `AppResponses<RegistrarRadicacionResponse>`.
6. `UpdatePanel/PostBack` -> estado cliente React (hooks + mutation/query + render).

Paralelismo de capas para implementación:

1. Controller: validación de contrato + resolución de claims + llamada a service.
2. Service: orquestación de reglas + transacción + coordinación de repositories.
3. Repository: persistencia por responsabilidad (consecutivo, registro, relación, estado, respuesta, log).
4. DTO/Mapping: contrato API estable y mapeo de `Campos[]` -> DTO canónico.
5. Tests/Docs: pruebas de paridad y documentación técnica por ticket.

## FASE 6 — Endpoints API propuestos

1. POST /api/radicacion/registrar-entrante
   Controller: RadicacionController
   Service: IRadicacionRegistroService
   Repository: IRadicacionRegistroRepository + auxiliares
   Request: RegistrarRadicacionRequest
   Response: AppResponses<RegistrarRadicacionResponse>

2. POST /api/radicacion/validar-entrante
   Controller: RadicacionController
   Service: IRadicacionValidacionService
   Repository: lectura de reglas/plantilla
   Request: ValidarRadicacionRequest
   Response: AppResponses<ValidarRadicacionResponse>

3. GET /api/radicacion/flujo-inicial
   Controller: RadicacionController
   Service: IFlujoTrabajoService
   Repository: flujo/actividad
   Request: query params (idTipoTramite, etc.)
   Response: AppResponses<FlujoInicialDto>

## FASE 7 — Dependencias legacy detectadas y reemplazo

1. Session.Item -> claims + servicios de contexto + parámetros explícitos.
2. ViewState -> estado cliente React.
3. HiddenField -> propiedades DTO explícitas.
4. FindControl -> binding tipado en React form.
5. IsPostBack/UpdatePanel -> ciclo request/response REST + estado local.
6. Variables globales JS -> estado controlado por hooks/context.
7. Utilidades compartidas VB monolíticas -> servicios separados por dominio.

## FASE 8 — Riesgos técnicos y mitigaciones

1. SQL no parametrizado heredado
   Mitigación: parametrización obligatoria y revisión de queries.

2. Dependencia de UI server controls
   Mitigación: DTO canónico + validación de contrato.

3. Reglas de negocio embebidas en método monolítico
   Mitigación: descomponer en servicios de validación/orquestación/persistencia.

4. Inconsistencia de tipos/nulls
   Mitigación: normalización de tipos y validadores explícitos.

5. Diferencias de fecha/hora/formato
   Mitigación: definir política única de timezone y formato ISO.

6. Riesgo transaccional en registro multi-tabla
   Mitigación: una sola transacción de aplicación para Q01-Q08 con rollback completo ante error.

## Estado de decisiones del plan

Decisiones confirmadas:

1. Mantener plan en formato detallado.
2. Fase 3 incorpora estructura real del formulario React.
3. `tipo-radicacion` se considera campo de entrada del DTO (`TipoRadicacion`).
4. Cierre de Fase 4 sin modificar contexto general.

Pendientes abiertos:

1. Confirmar punto exacto de `onFinish`/mutación API final tras `form.submit()` en React.
2. Definir si `POST /validar-entrante` será obligatorio antes de `POST /registrar-entrante` o solo validación opcional.
3. Confirmar contrato final de `FlujoInicialDto` (campos mínimos requeridos por frontend).

## Criterios para aprobar el plan (`APROBADO PLAN`)

1. Fase 3 aceptada con mapeo final de campos/claims.
2. Fase 4 aceptada con matriz SQL Q01-Q09 + reglas de transacción/parametrización.
3. Endpoints de Fase 6 aceptados (rutas + contratos request/response).
4. Pendientes abiertos identificados como decisiones de implementación y no bloqueantes de planificación.

## Revisión rápida (checklist de aprobación)

1. Confirmar que `TipoRadicacion` queda en request como campo obligatorio funcional.
2. Confirmar que `Campos[]` se conserva por compatibilidad y se mapea a DTO canónico en Service.
3. Confirmar uso de `defaultDbAlias` (claims) en todos los repositories de Fase 4.
4. Confirmar transacción única para bloque principal de registro (Q01-Q08).
5. Confirmar atomicidad obligatoria (todo o nada): sin completar todos los registros de Q01-Q08 no se confirma la transacción y se aplica rollback total.
6. Confirmar endpoint `GET /api/radicacion/flujo-inicial` como soporte de UI (separado de validación).
7. Dejar explícito que `onFinish` React queda pendiente técnico y no bloquea aprobación del plan.

Estado recomendado para cerrar revisión:

1. Si los 7 puntos anteriores están de acuerdo: responder `APROBADO PLAN`.
2. Si hay ajustes: responder con `AJUSTAR` + número de punto(s) a cambiar.

Plan completado.

Siguiente paso, solo con tu aprobación:

1. convertir este plan en tickets Jira por etapas;
2. generar especificaciones OpenSpec por ticket;
3. fijar rutas definitivas por capa para implementación.
