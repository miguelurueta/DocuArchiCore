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

### 3) Ruta / capa destino definitiva en repo actual
La implementacion se ejecuta obligatoriamente por capas:
- `Controller -> Service -> Repository -> DTO/Model/Mapping -> Tests/Docs`.

La organizacion y orden de construccion queda regida por los tickets creados:
- `SCRUM-38-01` a `SCRUM-38-07` en `C:\tike-jira\Radicacion`.

## Nota operativa
Este contexto se usa como regla base para nuevas solicitudes de migracion del flujo `Registra_Radicacion_entrante` y funciones relacionadas.

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
9. Agregar diagramas de casos de uso, clases, secuencia y estado en `/Docs/Radicacion/Tramite`.
10. Incluir documentacion tecnica para frontend: descripcion del DTO, parametros de envio y direccion de la API.
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
- Model: `/Models/Radicacion/Tramite/`
- Mapping: `/Services/Mapping/Radicacion/Tramite/`
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

Aplicar `camelCase` en:
- variables.

## Regla para APIs de registro

- En tickets/especificaciones de APIs de registro NO especificar un `SCRUM-*` fijo.
- El identificador oficial del ticket lo asigna Jira.
- Usar titulo/descripcion funcional neutral hasta que Jira asigne el codigo.

## Regla para funciones que llenan tablas (frontend MUI)

- Toda funcion que retorne datos para llenar tablas debe integrar:
  1. `DynamicUiTable` (estructura/metadata de tabla para frontend),
  2. `DapperCrudEngine` (consulta de datos en backend).
- Aplica para frontend tipo tabla o `data-grid` MUI.
