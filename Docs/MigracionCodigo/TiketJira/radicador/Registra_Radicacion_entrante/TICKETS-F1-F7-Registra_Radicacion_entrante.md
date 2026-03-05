# Documento Maestro SCRUM-38 (Fuente unica para Jira + opsxj:new)

Funcion objetivo: Registra_Radicacion_entrante
Estado: APROBADO PLAN
Fecha consolidacion: 2026-03-05

Este archivo consolida el contexto operativo y el detalle completo de migracion para evitar depender de multiples archivos durante la creacion/ejecucion del ticket.

## 1) Resumen operativo por fases (F1-F7)

# Tickets por Fase (F1-F7)

Funcion objetivo: `Registra_Radicacion_entrante`  
Estado: Plan aprobado (`APROBADO PLAN`)  
Fecha de corte: 2026-03-05

## Reglas transversales obligatorias

1. SQL parametrizado en el 100% de consultas.
2. Uso obligatorio de `QueryOptions.DefaultAlias = defaultDbAlias`.
3. Arquitectura por capas: `Controller -> Service -> Repository -> DTO/Mapping -> Tests/Docs`.
4. Atomicidad obligatoria en persistencia multi-tabla (todo o nada): si falla cualquier registro del bloque transaccional, se ejecuta rollback completo y no se confirma la transaccion.
5. Respuesta estandar en `AppResponses<T>` con manejo controlado de errores.

## F1 - Inventario legacy y dependencias del flujo

- Titulo sugerido Jira: `SCRUM-38-F1 Inventario completo de funciones legacy y dependencias`
- Objetivo: cerrar inventario al 100% del flujo de radicacion entrante (principal, auxiliares, hermanas y disparadores).
- Entregables:
  1. Matriz de funciones legacy con archivo, evento disparador y tipo de responsabilidad.
  2. Dependencias directas e indirectas identificadas por capa objetivo.
  3. Riesgos funcionales de cobertura incompleta.
- Criterios de aceptacion:
  1. Cobertura 100% del flujo inventariado.
  2. Cada funcion tiene trazabilidad a destino target.
  3. No quedan dependencias sin clasificar.
- Referencia documental: `PLAN-MIGRACION-Registra_Radicacion_entrante.md` (Fase 1 y regla de cierre).

## F2 - Reconstruccion del flujo funcional end-to-end

- Titulo sugerido Jira: `SCRUM-38-F2 Reconstruccion de flujo funcional real`
- Objetivo: documentar flujo real desde UI legacy hasta persistencia y retorno funcional.
- Entregables:
  1. Diagrama secuencial del flujo real.
  2. Listado de validaciones funcionales por etapa.
  3. Puntos de decision por tipo de radicacion y ramas condicionales.
- Criterios de aceptacion:
  1. Flujo completo con evidencia de origen (UI, evento, funcion VB).
  2. Validaciones separadas por precondicion, dato y negocio.
  3. Identificacion de reemplazo target por componente.
- Referencia documental: `PLAN-MIGRACION-Registra_Radicacion_entrante.md` (Fase 2).

## F3 - Conversion de parametros legacy a DTO

- Titulo sugerido Jira: `SCRUM-38-F3 Contrato API y mapeo de parametros a DTO canonico`
- Objetivo: cerrar request/response canonico sin dependencia de `FindControl`, `Hidden` ni `Session`.
- Entregables:
  1. Mapeo final `legacy -> DTO`.
  2. Contrato de `RegistrarRadicacionRequest` y `RegistrarRadicacionResponse`.
  3. Compatibilidad de `Campos[]` con mapping interno en Service.
- Criterios de aceptacion:
  1. `TipoRadicacion` queda como campo obligatorio funcional.
  2. Claims (`usuarioid`, `defaultDbAlias`) definidos como fuente de contexto.
  3. Compatibilidad frontend actual documentada.
- Referencia documental: `PLAN-MIGRACION-Registra_Radicacion_entrante.md` (Fase 3).

## F4 - Mapeo SQL a repositories y transaccion atomica

- Titulo sugerido Jira: `SCRUM-38-F4 Persistencia por repositorio y transaccion atomica`
- Objetivo: implementar matriz Q01-Q09 en repositorios con seguridad SQL y transaccion de aplicacion.
- Entregables:
  1. Matriz de queries Q01-Q09 asignada a interfaces/metodos repository.
  2. Estrategia transaccional para Q01-Q08 con commit/rollback.
  3. Regla de whitelist para tabla dinamica `{nombre_plantilla}`.
- Criterios de aceptacion:
  1. Todas las consultas parametrizadas.
  2. `defaultDbAlias` aplicado en el 100% de repositories.
  3. Atomicidad obligatoria: si falla cualquier registro de Q01-Q08, rollback total sin persistencia parcial.
- Referencia documental: `PLAN-MIGRACION-Registra_Radicacion_entrante.md` (Fase 4).

## F5 - Mapeo legacy a arquitectura target

- Titulo sugerido Jira: `SCRUM-38-F5 Orquestacion por capas y reemplazo de dependencias legacy`
- Objetivo: traducir comportamiento monolitico a componentes desacoplados por capa.
- Entregables:
  1. Definicion de responsabilidades por Controller, Service y Repository.
  2. Politica de reemplazo `Session/ViewState/FindControl`.
  3. Estandar de errores y respuestas.
- Criterios de aceptacion:
  1. Sin logica de negocio en Controller.
  2. Sin SQL en Service.
  3. Dependencias legacy con sustituto target explicito.
- Referencia documental: `PLAN-MIGRACION-Registra_Radicacion_entrante.md` (Fase 5).

## F6 - Definicion y cierre de endpoints API

- Titulo sugerido Jira: `SCRUM-38-F6 Endpoints de radicacion entrante y contratos de transporte`
- Objetivo: cerrar rutas y contratos de los endpoints funcionales del flujo.
- Entregables:
  1. `POST /api/radicacion/registrar-entrante`.
  2. `POST /api/radicacion/validar-entrante`.
  3. `GET /api/radicacion/flujo-inicial`.
- Criterios de aceptacion:
  1. Request/response por endpoint documentado y versionable.
  2. `flujo-inicial` queda separado de `validar-entrante`.
  3. Pendiente `onFinish` React queda marcado como tecnico y no bloqueante del plan.
- Referencia documental: `PLAN-MIGRACION-Registra_Radicacion_entrante.md` (Fase 6).

## F7 - Dependencias legacy detectadas y plan de reemplazo

- Titulo sugerido Jira: `SCRUM-38-F7 Eliminacion de dependencias legacy de UI server-side`
- Objetivo: cerrar estrategia de reemplazo de acoplamientos WebForms/JS legacy.
- Entregables:
  1. Matriz `dependencia legacy -> mecanismo target`.
  2. Lineamientos de estado React y contratos API.
  3. Riesgos residuales por dependencia.
- Criterios de aceptacion:
  1. Todas las dependencias criticas tienen reemplazo definido.
  2. Sin dependencia obligatoria a mecanismos server-side legacy.
  3. Riesgos documentados para implementacion y QA.
- Referencia documental: `PLAN-MIGRACION-Registra_Radicacion_entrante.md` (Fase 7).

## Mapeo con documentos SCRUM-38 existentes

1. F3 se alinea con las secciones `-01` y `-02` del consolidado `SCRUM-38-ticket-consolidado-Registra_Radicacion_entrante.md`.
2. F4 se alinea con la seccion `-04` del consolidado `SCRUM-38-ticket-consolidado-Registra_Radicacion_entrante.md`.
3. F6 se alinea con la seccion `-01` y contratos del consolidado `SCRUM-38-ticket-consolidado-Registra_Radicacion_entrante.md`.
4. F1/F2/F5/F7 quedan como desglose operativo del plan maestro aprobado.


## 2) Plan detallado de migracion (base tecnica aprobada)

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


## 3) Ticket consolidado SCRUM-38 (especificacion integral)

# Especificacion Consolidada
## Funcion objetivo
- Componente fuente: `radicador`
- Funcion legacy: `Registra_Radicacion_entrante`
- Archivo fuente: `D:\imagenesda\GestorDocumental\Desarrollo\Visual-2019\GestionDocumental-Docuarchi.net\GestionDocumental-Docuarchi.net\radicador\ClassRadicador.vb`

## Objetivo
Consolidar en un solo especificacion el plan completo de migracion + correccion para implementar en `DocuArchiCore` bajo arquitectura por capas y reglas de calidad del entorno.

## Configuracion de rutas destino (obligatorio para IA)

- `API (Controller)`: `/Controllers/Radicacion/Tramite/`
- `Service`: `/Services/Service/Radicacion/Tramite/`
- `Repository`: `/Repositorio/Radicador/Tramite/`
- `DTO`: `/DTOs/Radicacion/Tramite/`
- `Model`: `/Models/Radicacion/Tramite/`
- `Mapping`: `/Services/Mapping/Radicacion/Tramite/`
- `Tests unitarios/integracion`: `/tests/TramiteDiasVencimiento.Tests/`
- `Documentacion tecnica y diagramas`: `/Docs/Radicacion/Tramite/`

Reglas de uso de rutas:
1. Si la carpeta no existe, crearla respetando el patron del repositorio.
2. Toda clase nueva debe ubicarse solo en su capa correspondiente.
3. No mezclar implementacion de negocio en Controller ni SQL en Service.

---

## Seccion: -01-base-api-contratos.md

# especificacion 1 - Base API y contratos (Registra_Radicacion_entrante)

## Contexto

- Repositorio fuente: `D:\imagenesda\GestorDocumental\Desarrollo\Visual-2019\GestionDocumental-Docuarchi.net\GestionDocumental-Docuarchi.net`
- Archivo fuente: `/radicador/ClassRadicador.vb`
- Función fuente: `Registra_Radicacion_entrante`
- Repositorio destino: `D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore`

## Objetivo

Crear la base de migración para exponer el caso de uso de radicación entrante en arquitectura por capas (`Controller -> Service -> Repository`) con contratos estables y respuesta estándar (`AppResponses`).

## Reglas de migración aplicadas

1. No replicar errores del legacy; documentar y aplicar corrección.
2. Mantener consultas parametrizadas y validación de datos.
3. Mapeo de conexión legacy:
 - `conect.Dbase_Conction_Mysql` -> `workflowtconta` -> alias `WF`
 - `conect.Dbase_Conction_Mysql_RA` -> `docuarchi` -> alias `DA`
 - `conect.Dbase_Conction_Mysql_DA` -> `docuarchi` -> alias `DA`

## Alcance funcional

1. Definir endpoint base para registrar radicación entrante.
2. Definir DTO de entrada y DTO de salida mínimos.
3. Definir interfaz de servicio principal de migración.
4. Estandarizar contrato de respuesta `AppResponses`.
5. Manejo controlado de errores en Controller y Service (`try/catch`).

## Correcciones incluidas

1. Evitar ambigüedad de tipo/valor en parámetros de flujo (sin mezclar `"null"` y `0`).
2. Garantizar mensajes de error coherentes por etapa de validación.
3. Preparar diseño para separar validación, autorización y persistencia (sin método monolítico).

## Requerimientos técnicos obligatorios

1. Mantener SoC, bajo acoplamiento y alta cohesión.
2. Cumplir principios SOLID.
3. Comentarios en funciones con:
 - fecha de creación,
 - descripción de parámetros,
 - descripción de retorno.
4. Esta especificacion no ejecuta lógica SQL aún; prepara contratos para fases siguientes.

## Criterios de aceptación

1. Existe endpoint base documentado y callable.
2. Request/Response DTO definidos con campos mínimos del caso de uso.
3. Service interface definida y conectada al controller.
4. Todas las respuestas salen envueltas en `AppResponses`.
5. Casos cubiertos: éxito, validación fallida y excepción controlada.

## Pruebas requeridas

1. Prueba unitaria de validación de request inválido.
2. Prueba unitaria de flujo exitoso simulado.
3. Prueba unitaria de excepción controlada.

## Evidencia esperada en PR

1. Código de Controller/Service/DTO base.
2. Tests unitarios en verde.
3. Documento técnico actualizado en `/Docs/MigracionCodigo`.

## Reglas obligatorias del entorno de migracion (actualizado)

1. Mantener SoC, bajo acoplamiento y alta cohesion.
2. Cumplir principios SOLID.
3. Agregar diagramas de casos de uso, clases, secuencia y estado en `/Docs/Radicacion/Tramite`.
4. Incluir documentacion tecnica para frontend: descripcion del DTO, parametros de envio y direccion de la API.
5. Agregar comentarios en todas las funciones con:
 - fecha de creacion,
 - descripcion de parametros,
 - descripcion de retorno.

---

## Seccion: -02-validaciones-funcionales.md

# especificacion 2 - Validaciones funcionales

Objetivo:
Migrar validaciones funcionales de entrada del flujo legacy.

## Configuración técnica cerrada (aplica a todos los fases)

- Función origen: `Registra_Radicacion_entrante` en `/radicador/ClassRadicador.vb`.
- Mapeo de conexión legacy -> base de datos destino:
 - `conect.Dbase_Conction_Mysql` -> `workflowtconta` -> alias `WF` (`MySqlConnection_WF`).
 - `conect.Dbase_Conction_Mysql_RA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
 - `conect.Dbase_Conction_Mysql_DA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
- Regla de implementación Repository:
 - Identificar conector usado en función legacy.
 - Traducir a alias (`WF` o `DA`).
 - Ejecutar consultas parametrizadas con `QueryOptions.DefaultAlias = defaultDbAlias`.
 - Retornar `AppResponses` en éxito/sin resultados/error.
- Patrón obligatorio: `Controller -> Service -> Repository -> DTO/Model/Mapping -> Tests/Docs`.

Alcance:
1. Validaciones de campos obligatorios.
2. Validaciones de formato/fechas.
3. Mensajes funcionales consistentes por regla incumplida.
4. Respuesta normalizada en AppResponses.
5. Separar validaciones por etapa para trazabilidad (precondición, dato, negocio).

Criterios de aceptación:
1. Cada regla de validación tiene prueba unitaria.
2. Escenarios inválido/válido cubiertos.
3. Mensajes de error trazables por regla.
4. Queda listo para integrar autorizaciones/workflow del especificacion 3.

## Correcciones incluidas

1. El especificacion no replica defectos del legacy; define comportamiento corregido y verificable.
2. Toda inconsistencia detectada en validaciones, flujo o persistencia se transforma en requisito de corrección técnica.
3. Se exige implementación parametrizada, segura y trazable (sin SQL Injection, manejo controlado de errores).

## Especificación de corrección

1. Describir regla legacy actual + problema detectado.
2. Definir regla objetivo corregida en el entorno nuevo.
3. Incluir criterios de aceptación para comprobar la corrección.
4. Incluir pruebas unitarias/integración asociadas a la corrección.

## Criterio de calidad aplicado

1. Clean Architecture y principios SOLID.
2. Separación de responsabilidades y bajo acoplamiento.
3. Código testeable y reusable.
4. Validación y seguridad de datos obligatoria.

## Reglas obligatorias del entorno de migracion (actualizado)

1. Mantener SoC, bajo acoplamiento y alta cohesion.
2. Cumplir principios SOLID.
3. Agregar diagramas de casos de uso, clases, secuencia y estado en `/Docs/Radicacion/Tramite`.
4. Incluir documentacion tecnica para frontend: descripcion del DTO, parametros de envio y direccion de la API.
5. Agregar comentarios en todas las funciones con:
 - fecha de creacion,
 - descripcion de parametros,
 - descripcion de retorno.

---

## Seccion: -03-autorizacion-workflow.md

# especificacion 3 - Autorización y workflow

Objetivo:
Migrar reglas de autorización de radicación y selección de flujo de trabajo.

## Configuración técnica cerrada (aplica a todos los fases)

- Función origen: `Registra_Radicacion_entrante` en `/radicador/ClassRadicador.vb`.
- Mapeo de conexión legacy -> base de datos destino:
 - `conect.Dbase_Conction_Mysql` -> `workflowtconta` -> alias `WF` (`MySqlConnection_WF`).
 - `conect.Dbase_Conction_Mysql_RA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
 - `conect.Dbase_Conction_Mysql_DA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
- Regla de implementación Repository:
 - Identificar conector usado en función legacy.
 - Traducir a alias (`WF` o `DA`).
 - Ejecutar consultas parametrizadas con `QueryOptions.DefaultAlias = defaultDbAlias`.
 - Retornar `AppResponses` en éxito/sin resultados/error.
- Patrón obligatorio: `Controller -> Service -> Repository -> DTO/Model/Mapping -> Tests/Docs`.

Alcance:
1. Validar autorización por días/horas de radicación.
2. Resolver actividad inicial de workflow.
3. Validar consistencia de flujo seleccionado.
4. Retornar errores de negocio controlados.
5. Definir contrato de continuidad del flujo hacia persistencia (especificacion 4).

Criterios de aceptación:
1. Reglas de autorización probadas.
2. Flujo inválido retorna mensaje funcional.
3. Flujo válido permite continuar proceso.
4. Se identifican funciones legacy dependientes y su alias (WF/DA).

## Correcciones incluidas

1. El especificacion no replica defectos del legacy; define comportamiento corregido y verificable.
2. Toda inconsistencia detectada en validaciones, flujo o persistencia se transforma en requisito de corrección técnica.
3. Se exige implementación parametrizada, segura y trazable (sin SQL Injection, manejo controlado de errores).

## Especificación de corrección

1. Describir regla legacy actual + problema detectado.
2. Definir regla objetivo corregida en el entorno nuevo.
3. Incluir criterios de aceptación para comprobar la corrección.
4. Incluir pruebas unitarias/integración asociadas a la corrección.

## Criterio de calidad aplicado

1. Clean Architecture y principios SOLID.
2. Separación de responsabilidades y bajo acoplamiento.
3. Código testeable y reusable.
4. Validación y seguridad de datos obligatoria.

## Reglas obligatorias del entorno de migracion (actualizado)

1. Mantener SoC, bajo acoplamiento y alta cohesion.
2. Cumplir principios SOLID.
3. Agregar diagramas de casos de uso, clases, secuencia y estado en `/Docs/Radicacion/Tramite`.
4. Incluir documentacion tecnica para frontend: descripcion del DTO, parametros de envio y direccion de la API.
5. Agregar comentarios en todas las funciones con:
 - fecha de creacion,
 - descripcion de parametros,
 - descripcion de retorno.

---

## Seccion: -04-persistencia-transacciones.md

# especificacion 4 - Persistencia y transacciones

Objetivo:
Migrar operaciones de persistencia del proceso de radicación con manejo transaccional.

## Configuración técnica cerrada (aplica a todos los fases)

- Función origen: `Registra_Radicacion_entrante` en `/radicador/ClassRadicador.vb`.
- Mapeo de conexión legacy -> base de datos destino:
 - `conect.Dbase_Conction_Mysql` -> `workflowtconta` -> alias `WF` (`MySqlConnection_WF`).
 - `conect.Dbase_Conction_Mysql_RA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
 - `conect.Dbase_Conction_Mysql_DA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
- Regla de implementación Repository:
 - Identificar conector usado en función legacy.
 - Traducir a alias (`WF` o `DA`).
 - Ejecutar consultas parametrizadas con `QueryOptions.DefaultAlias = defaultDbAlias`.
 - Retornar `AppResponses` en éxito/sin resultados/error.
- Patrón obligatorio: `Controller -> Service -> Repository -> DTO/Model/Mapping -> Tests/Docs`.

Alcance:
1. Implementar Repository con consultas parametrizadas por función migrada.
2. Integrar IDapperCrudEngine y QueryOptions.DefaultAlias.
3. Manejar commit/rollback según resultado de negocio.
4. Registrar interfaces e implementaciones en Program.cs.
5. Aplicar mapeo legacy->alias (WF/DA) por cada consulta.

Criterios de aceptación:
1. Sin SQL Injection (consultas parametrizadas).
2. Errores de DB retornan AppResponses de fallo.
3. Pruebas de integración para commit/rollback.
4. Cada método repository declara alias esperado y tabla objetivo.

## Correcciones incluidas

1. El especificacion no replica defectos del legacy; define comportamiento corregido y verificable.
2. Toda inconsistencia detectada en validaciones, flujo o persistencia se transforma en requisito de corrección técnica.
3. Se exige implementación parametrizada, segura y trazable (sin SQL Injection, manejo controlado de errores).

## Especificación de corrección

1. Describir regla legacy actual + problema detectado.
2. Definir regla objetivo corregida en el entorno nuevo.
3. Incluir criterios de aceptación para comprobar la corrección.
4. Incluir pruebas unitarias/integración asociadas a la corrección.

## Criterio de calidad aplicado

1. Clean Architecture y principios SOLID.
2. Separación de responsabilidades y bajo acoplamiento.
3. Código testeable y reusable.
4. Validación y seguridad de datos obligatoria.

## Reglas obligatorias del entorno de migracion (actualizado)

1. Mantener SoC, bajo acoplamiento y alta cohesion.
2. Cumplir principios SOLID.
3. Agregar diagramas de casos de uso, clases, secuencia y estado en `/Docs/Radicacion/Tramite`.
4. Incluir documentacion tecnica para frontend: descripcion del DTO, parametros de envio y direccion de la API.
5. Agregar comentarios en todas las funciones con:
 - fecha de creacion,
 - descripcion de parametros,
 - descripcion de retorno.

---

## Seccion: -05-integraciones-auxiliares.md

# especificacion 5 - Integraciones y procesos auxiliares

Objetivo:
Migrar integraciones auxiliares del flujo (asignaciones, estados, adjuntos/notificaciones si aplican).

## Configuración técnica cerrada (aplica a todos los fases)

- Función origen: `Registra_Radicacion_entrante` en `/radicador/ClassRadicador.vb`.
- Mapeo de conexión legacy -> base de datos destino:
 - `conect.Dbase_Conction_Mysql` -> `workflowtconta` -> alias `WF` (`MySqlConnection_WF`).
 - `conect.Dbase_Conction_Mysql_RA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
 - `conect.Dbase_Conction_Mysql_DA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
- Regla de implementación Repository:
 - Identificar conector usado en función legacy.
 - Traducir a alias (`WF` o `DA`).
 - Ejecutar consultas parametrizadas con `QueryOptions.DefaultAlias = defaultDbAlias`.
 - Retornar `AppResponses` en éxito/sin resultados/error.
- Patrón obligatorio: `Controller -> Service -> Repository -> DTO/Model/Mapping -> Tests/Docs`.

Alcance:
1. Integrar llamadas auxiliares dependientes del flujo.
2. Estandarizar manejo de errores por integración.
3. Definir contratos de salida para frontend.
4. Declarar por integración qué alias de BD utiliza (WF o DA).

Criterios de aceptación:
1. Integraciones críticas cubiertas por pruebas.
2. Excepción externa no rompe proceso sin control.
3. Contratos de respuesta documentados.
4. Dependencias legacy trazadas a servicios/repositorios nuevos.

## Correcciones incluidas

1. El especificacion no replica defectos del legacy; define comportamiento corregido y verificable.
2. Toda inconsistencia detectada en validaciones, flujo o persistencia se transforma en requisito de corrección técnica.
3. Se exige implementación parametrizada, segura y trazable (sin SQL Injection, manejo controlado de errores).

## Especificación de corrección

1. Describir regla legacy actual + problema detectado.
2. Definir regla objetivo corregida en el entorno nuevo.
3. Incluir criterios de aceptación para comprobar la corrección.
4. Incluir pruebas unitarias/integración asociadas a la corrección.

## Criterio de calidad aplicado

1. Clean Architecture y principios SOLID.
2. Separación de responsabilidades y bajo acoplamiento.
3. Código testeable y reusable.
4. Validación y seguridad de datos obligatoria.

## Reglas obligatorias del entorno de migracion (actualizado)

1. Mantener SoC, bajo acoplamiento y alta cohesion.
2. Cumplir principios SOLID.
3. Agregar diagramas de casos de uso, clases, secuencia y estado en `/Docs/Radicacion/Tramite`.
4. Incluir documentacion tecnica para frontend: descripcion del DTO, parametros de envio y direccion de la API.
5. Agregar comentarios en todas las funciones con:
 - fecha de creacion,
 - descripcion de parametros,
 - descripcion de retorno.

---

## Seccion: -06-pruebas-paridad-funcional.md

# especificacion 6 - Pruebas de paridad funcional

Objetivo:
Validar paridad funcional entre comportamiento legacy y comportamiento migrado.

## Configuración técnica cerrada (aplica a todos los fases)

- Función origen: `Registra_Radicacion_entrante` en `/radicador/ClassRadicador.vb`.
- Mapeo de conexión legacy -> base de datos destino:
 - `conect.Dbase_Conction_Mysql` -> `workflowtconta` -> alias `WF` (`MySqlConnection_WF`).
 - `conect.Dbase_Conction_Mysql_RA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
 - `conect.Dbase_Conction_Mysql_DA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
- Regla de implementación Repository:
 - Identificar conector usado en función legacy.
 - Traducir a alias (`WF` o `DA`).
 - Ejecutar consultas parametrizadas con `QueryOptions.DefaultAlias = defaultDbAlias`.
 - Retornar `AppResponses` en éxito/sin resultados/error.
- Patrón obligatorio: `Controller -> Service -> Repository -> DTO/Model/Mapping -> Tests/Docs`.

Alcance:
1. Matriz de casos legacy vs nuevo (por etapa y por alias de BD).
2. Pruebas unitarias por capa.
3. Pruebas de integración con MySQL/Testcontainers.
4. Escenarios: éxito, sin resultados, error de validación, excepción.

Criterios de aceptación:
1. Cobertura mínima acordada por módulo.
2. Evidencia de paridad en casos críticos.
3. Reporte de brechas funcionales pendientes.
4. Evidencia de consultas ejecutadas en alias correcto (WF/DA).

## Correcciones incluidas

1. El especificacion no replica defectos del legacy; define comportamiento corregido y verificable.
2. Toda inconsistencia detectada en validaciones, flujo o persistencia se transforma en requisito de corrección técnica.
3. Se exige implementación parametrizada, segura y trazable (sin SQL Injection, manejo controlado de errores).

## Especificación de corrección

1. Describir regla legacy actual + problema detectado.
2. Definir regla objetivo corregida en el entorno nuevo.
3. Incluir criterios de aceptación para comprobar la corrección.
4. Incluir pruebas unitarias/integración asociadas a la corrección.

## Criterio de calidad aplicado

1. Clean Architecture y principios SOLID.
2. Separación de responsabilidades y bajo acoplamiento.
3. Código testeable y reusable.
4. Validación y seguridad de datos obligatoria.

## Reglas obligatorias del entorno de migracion (actualizado)

1. Mantener SoC, bajo acoplamiento y alta cohesion.
2. Cumplir principios SOLID.
3. Agregar diagramas de casos de uso, clases, secuencia y estado en `/Docs/Radicacion/Tramite`.
4. Incluir documentacion tecnica para frontend: descripcion del DTO, parametros de envio y direccion de la API.
5. Agregar comentarios en todas las funciones con:
 - fecha de creacion,
 - descripcion de parametros,
 - descripcion de retorno.

---

## Seccion: -07-hardening-documentacion.md

# especificacion 7 - Hardening y documentación

Objetivo:
Cerrar migración con estabilización técnica y documentación para backend/frontend.

## Configuración técnica cerrada (aplica a todos los fases)

- Función origen: `Registra_Radicacion_entrante` en `/radicador/ClassRadicador.vb`.
- Mapeo de conexión legacy -> base de datos destino:
 - `conect.Dbase_Conction_Mysql` -> `workflowtconta` -> alias `WF` (`MySqlConnection_WF`).
 - `conect.Dbase_Conction_Mysql_RA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
 - `conect.Dbase_Conction_Mysql_DA` -> `docuarchi` -> alias `DA` (`MySqlConnection_DA`).
- Regla de implementación Repository:
 - Identificar conector usado en función legacy.
 - Traducir a alias (`WF` o `DA`).
 - Ejecutar consultas parametrizadas con `QueryOptions.DefaultAlias = defaultDbAlias`.
 - Retornar `AppResponses` en éxito/sin resultados/error.
- Patrón obligatorio: `Controller -> Service -> Repository -> DTO/Model/Mapping -> Tests/Docs`.

Alcance:
1. Ajustes de rendimiento y observabilidad.
2. Documentación API y DTOs para frontend.
3. Diagramas: casos de uso, clases, secuencia, estado.
4. Checklist final de salida a QA.
5. Documentar mapeo final de funciones legacy -> componentes nuevos.

Criterios de aceptación:
1. Documentación técnica completa y vigente.
2. Checklist QA aprobado.
3. Riesgos residuales registrados.
4. Guía de operación con alias de BD y diagnóstico de errores.

## Correcciones incluidas

1. El especificacion no replica defectos del legacy; define comportamiento corregido y verificable.
2. Toda inconsistencia detectada en validaciones, flujo o persistencia se transforma en requisito de corrección técnica.
3. Se exige implementación parametrizada, segura y trazable (sin SQL Injection, manejo controlado de errores).

## Especificación de corrección

1. Describir regla legacy actual + problema detectado.
2. Definir regla objetivo corregida en el entorno nuevo.
3. Incluir criterios de aceptación para comprobar la corrección.
4. Incluir pruebas unitarias/integración asociadas a la corrección.

## Criterio de calidad aplicado

1. Clean Architecture y principios SOLID.
2. Separación de responsabilidades y bajo acoplamiento.
3. Código testeable y reusable.
4. Validación y seguridad de datos obligatoria.

## Reglas obligatorias del entorno de migracion (actualizado)

1. Mantener SoC, bajo acoplamiento y alta cohesion.
2. Cumplir principios SOLID.
3. Agregar diagramas de casos de uso, clases, secuencia y estado en `/Docs/Radicacion/Tramite`.
4. Incluir documentacion tecnica para frontend: descripcion del DTO, parametros de envio y direccion de la API.
5. Agregar comentarios en todas las funciones con:
 - fecha de creacion,
 - descripcion de parametros,
 - descripcion de retorno.

---

## Actualizacion por contexto vigente (obligatorio)

### Convencion de nomenclatura para codigo migrado

Aplicar `PascalCase` en:
- clases
- metodos
- propiedades
- DTOs
- interfaces

Aplicar `camelCase` en:
- variables

### Reglas de aplicacion

1. Esta convención aplica a todo código nuevo generado por migración.
2. No trasladar nombres legacy inconsistentes al nuevo entorno.
3. Si un nombre legacy no cumple la convención, se renombra en la capa nueva manteniendo trazabilidad funcional.




## 4) Regla de uso

1. Para opsxj:new, usar este documento como contexto principal.
2. Los archivos PLAN-MIGRACION-Registra_Radicacion_entrante.md y SCRUM-38-ticket-consolidado-Registra_Radicacion_entrante.md se mantienen como respaldo historico y trazabilidad.
3. Si hay cambios funcionales o tecnicos, actualizar primero este documento maestro.
