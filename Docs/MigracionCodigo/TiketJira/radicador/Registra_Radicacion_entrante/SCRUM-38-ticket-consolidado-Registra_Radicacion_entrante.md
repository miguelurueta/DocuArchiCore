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


