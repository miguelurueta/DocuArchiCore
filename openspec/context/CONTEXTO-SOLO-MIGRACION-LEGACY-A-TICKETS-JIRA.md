# Contexto acotado (solo este tipo de solicitudes)

Alcance:
Este contexto aplica UNICAMENTE para solicitudes de:
- analisis de repositorio legacy (VB.NET monolitico),
- generacion de tickets Jira de migracion,
- definicion de base tecnica para implementar en entorno actual.

No aplicar este contexto a:
- solicitudes generales del proyecto,
- tareas no relacionadas con migracion legacy,
- cambios funcionales fuera del flujo de radicacion/migracion.

Reglas acordadas:
1. No ejecutar opsxj:new.
2. Generar ticket/documentacion en la ruta que indique el usuario.
3. Priorizar separacion por tickets por etapas para funciones monoliticas.
4. No corregir legacy en esta fase; documentar migracion y plan de implementacion.
5. Atomicidad obligatoria en operaciones multi-tabla: si no se completan todos los registros del bloque transaccional, no se confirma la transaccion (rollback total).
6. Toda migracion debe generar obligatoriamente tres artefactos: Plan de Migracion, Especificacion Consolidada y Ticket Jira consolidado basado en ambos documentos.

## Definiciones cerradas para implementar (3 puntos)

### 1) Schema / base de datos objetivo por funcion legacy

Usar la conexion del contexto actual y mapear la base objetivo segun el conector legado detectado en cada funcion:
- Si la funcion usa `conect.Dbase_Conction_Mysql` -> apunta a BD `workflowtconta`.
- Si la funcion usa `conect.Dbase_Conction_Mysql_RA` -> apunta a BD `docuarchi`.
- Si la funcion usa `conect.Dbase_Conction_Mysql_DA` -> apunta a BD `docuarchi`.

Equivalencia esperada en entorno actual (aliases):
- `workflowtconta` -> alias/connection string tipo `MySqlConnection_WF`.
- `docuarchi` -> alias/connection string tipo `MySqlConnection_DA`.

### 2) Queries / reglas exactas de negocio por funcion

Para cada funcion migrada:
1. Identificar primero el conector legacy usado (`Dbase_Conction_Mysql`, `Dbase_Conction_Mysql_RA`, `Dbase_Conction_Mysql_DA`).
2. Determinar BD destino con el mapeo anterior.
3. Extraer query y reglas desde el codigo legacy respetando la BD asignada.
4. Implementar query parametrizada en repository usando `QueryOptions.DefaultAlias` (sin SQL injection).
5. En persistencia multi-tabla, aplicar regla de atomicidad todo-o-nada: cualquier fallo implica rollback completo sin registros parciales.

### 3) Ruta / capa destino definitiva en repo actual

La implementacion se ejecuta obligatoriamente por capas:
- `Controller -> Service -> Repository -> DTO/Model/Mapping -> Tests/Docs`.

La organizacion y orden de construccion queda regida por los tickets creados:
- `SCRUM-38-01` a `SCRUM-38-07` en `C:\tike-jira\Radicacion`.

## Nota operativa

Este contexto se usa como regla base para nuevas solicitudes de migracion del flujo `Registra_Radicacion_entrante` y funciones relacionadas.

## Referencia de frontend en contexto actual

- Proyecto frontend actual de referencia: `D:\imagenesda\GestorDocumental\DocuArchiCore.react`

## Regla para funciones legacy con `As Page`

- Para toda funcion legacy cuya firma reciba `As Page` (ejemplo: `ByRef Page1 As Page`), se debe migrar el origen de datos a contrato API enviado por frontend React.
- Queda prohibido depender en backend de `FindControl`, controles WebForms (`Hidden`, `CheckBox`, `DropDownList`, etc.) o estado UI del servidor.
- El backend debe recibir datos exclusivamente por:
  1. Request DTO del endpoint (body),
  2. Claims/sesion de autenticacion,
  3. Servicios/repositorios internos.

## Regla estricta sobre `Session.Item` y reemplazos legacy

- Nunca utilizar variables `Session.Item` en implementaciones migradas.
- No inventar ni asumir reemplazos de valores legacy sin fuente confirmada.
- Cuando aparezca una dependencia legacy (`Session.Item`, `HttpContext.Current.Session`, `FindControl`, hidden fields), se debe detener el reemplazo y preguntar al usuario el origen real del dato.
- La pregunta debe entregarse siempre con contexto funcional:
  1. variable legacy exacta detectada,
  2. uso en la logica (para que se necesita),
  3. punto de entrada propuesto (claim, DTO, servicio, repositorio),
  4. decision requerida al usuario.

## Regla de Reconstruccion de Flujo Funcional (WebForms / JavaScript)

Antes de generar el ticket de migracion o definir requerimientos funcionales, se debe reconstruir el flujo completo de la funcionalidad a migrar e identificar el contexto real desde donde se ejecuta.

### 1) Identificacion del origen de la llamada

Se debe identificar el punto real donde se dispara la funcionalidad, incluyendo:
- Eventos del WebForm:
  - `Page_Load`
  - `Button_Click`
  - `SelectedIndexChanged`
  - eventos de `GridView`
  - `PostBack`
- Eventos JavaScript asociados al WebForm:
  - `onclick`
  - `onchange`
  - `submit`
  - llamadas `ajax`
  - funciones JavaScript vinculadas al formulario

### 2) Reconstruccion del flujo completo

Se debe analizar el recorrido completo de ejecucion:

UI (WebForm / JavaScript)
-> Evento del formulario
-> Llamada a metodo backend
-> Logica de negocio
-> Consulta a base de datos
-> Respuesta backend
-> Actualizacion de UI

El analisis debe identificar:
- validaciones en frontend,
- validaciones en backend,
- transformacion de datos,
- dependencias entre controles del formulario,
- comportamiento dinamico de la interfaz.

### 3) Comprension del objetivo funcional

Se debe explicar el proposito funcional de la funcion dentro del flujo del formulario, incluyendo:
- que problema resuelve,
- en que momento del proceso del formulario se ejecuta,
- que datos consume,
- que datos produce,
- que componentes dependen del resultado.

### 4) Regla para generacion del ticket

El ticket no debe basarse solo en el codigo del metodo; debe basarse en el flujo completo reconstruido.

Esto garantiza:
- comprension correcta de la funcionalidad original,
- preservacion del comportamiento esperado durante la migracion,
- eliminacion de dependencias implicitas de WebForms.

### 5) Integracion con OpenSpec

El ticket generado debe integrarse completamente con el modelo OpenSpec del proyecto, incluyendo:
- contexto funcional basado en flujo reconstruido,
- requerimientos funcionales derivados del flujo,
- requerimientos tecnicos alineados con la nueva arquitectura,
- pruebas basadas en el comportamiento real del flujo.

## Reglas de calidad para migracion de codigo

### 1) Analisis previo del codigo
- Revisar arquitectura, dependencias, flujo de datos y logica de negocio implementada.
- Evaluar coherencia del codigo legacy con estandares del proyecto actual.

### 2) Validacion mediante pruebas
- Diseñar o ejecutar pruebas (unitarias o de comportamiento) para validar funcionamiento del codigo analizado.
- Identificar errores funcionales, inconsistencias y comportamientos inesperados.

### 3) No replicar errores existentes
- Si se detectan errores, malas practicas, deuda tecnica o violaciones de arquitectura, NO convertirlos en requerimientos del ticket.
- Los errores del codigo actual NO se trasladan ni se replican en la nueva especificacion.

### 4) Propuesta de correccion
- Cuando se identifiquen problemas en el codigo legacy, el ticket debe describir la solucion correcta y mejorada.
- Aplicar buenas practicas de ingenieria de software en la propuesta.

### 5) Mejora arquitectonica obligatoria

El ticket debe promover implementacion alineada con:
- Clean Architecture.
- Principios SOLID.
- Separacion de responsabilidades.
- Reutilizacion de componentes.
- Codigo testeable.
- Seguridad y validacion adecuada de datos.

### Regla final

La especificacion debe reflejar la version optimizada y arquitectonicamente correcta del codigo migrado, no una copia literal del codigo existente.

## Regla de salida de tickets generados en exploracion

- Toda salida de tickets generados en modo exploracion debe guardarse dentro del repositorio `DocuArchiCore` en la ruta:
  - `Docs/MigracionCodigo/TiketJira/<ComponenteFuente>/<NombreFuncion>/`
- `<ComponenteFuente>`: nombre del modulo o carpeta origen en el legacy (ejemplo: `radicador`).
- `<NombreFuncion>`: nombre exacto de la funcion origen (ejemplo: `Registra_Radicacion_entrante`).

## Regla de confirmacion antes de PR/Merge

- Cuando se terminen de crear/actualizar todos los tickets, se debe solicitar confirmacion explicita del usuario antes de:
  1. crear PR,
  2. ejecutar merge,
  3. usar la conexion del entorno para operaciones de repositorio remoto.
- Sin confirmacion explicita, no se ejecuta PR ni merge.

## Requerimientos obligatorios para tickets de migracion

7. Mantener SoC, bajo acoplamiento y alta cohesion.
8. Cumplir principios SOLID.
11. Agregar comentarios en todas las funciones con:
   - fecha de creacion,
   - descripcion de parametros,
   - descripcion del retorno de la funcion.

## Especificacion guardada: rutas obligatorias de implementacion

Referencia principal:
- `Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/SCRUM-38-ticket-consolidado-Registra_Radicacion_entrante.md`

Configuracion de rutas destino (obligatorio para IA):
- API (Controller): `/Controllers/Radicacion/Tramite/`
- Service: `/Services/Service/Radicacion/Tramite/`
- Repository: `/Repositorio/Radicador/Tramite/`
- DTO: `/DTOs/Radicacion/Tramite/`
- Tests unitarios/integracion: `/tests/TramiteDiasVencimiento.Tests/`
- Documentacion tecnica y diagramas: `/Docs/Radicacion/Tramite/`

Reglas de uso:
1. Si la carpeta no existe, crearla respetando el patron del repositorio.
2. Toda clase nueva debe ubicarse solo en su capa correspondiente.
3. No mezclar implementacion de negocio en Controller ni SQL en Service.

## Convencion de nomenclatura para funciones migradas

Aplicar `PascalCase` en:
- clases,
- metodos,
- propiedades,
- DTOs,
- interfaces.
- variables.

- En tickets/especificaciones de APIs de registro NO especificar un `SCRUM-*` fijo.
- El identificador oficial del ticket lo asigna Jira.
- Usar titulo/descripcion funcional neutral hasta que Jira asigne el codigo.

## Regla para funciones que llenan tablas (frontend MUI)

- Toda funcion que retorne datos para llenar tablas debe integrar:
  1. `DynamicUiTable` (estructura/metadata de tabla para frontend),
  2. `DapperCrudEngine` (consulta de datos en backend).
- Aplica para frontend tipo tabla o `data-grid` MUI.

## Regla principal de planificacion previa (obligatoria)

Antes de generar cualquier ticket Jira u OpenSpec de implementacion, se debe entregar primero un `PLAN DE MIGRACION PROPUESTO` y esperar aprobacion explicita del usuario.

## Regla de conservación del plan (obligatoria)

1. El `PLAN DE MIGRACION PROPUESTO` debe conservarse siempre en version detallada.
2. No reemplazar el plan detallado por resúmenes o versiones reducidas.
3. Toda ampliación del plan debe integrarse sobre la versión detallada existente, manteniendo fases, matrices y pendientes explícitos.
4. Si el usuario solicita visualizar el plan, se debe mostrar en formato detallado completo.

## Fases minimas obligatorias del plan de migracion

1. Inventario de funciones legacy del flujo.
2. Reconstruccion del flujo funcional end-to-end.
3. Conversion de parametros legacy a DTO.
4. Mapeo de queries a repository (con alias DB).
5. Mapeo legacy -> arquitectura target.
6. Endpoints API propuestos.
7. Dependencias legacy y reemplazo.
8. Riesgos tecnicos y mitigaciones.

## Regla obligatoria para Fase 1 (Inventario)

El inventario debe cubrir el 100% de funciones relacionadas al flujo migrado (funcion principal, dependencias directas e indirectas de negocio/infra). No se permite cerrar el plan con inventario parcial.
