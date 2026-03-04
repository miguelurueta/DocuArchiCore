# Ticket 2 - Validaciones funcionales

Objetivo:
Migrar validaciones funcionales de entrada del flujo legacy.

## Configuración técnica cerrada (aplica a todos los tickets)

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
4. Queda listo para integrar autorizaciones/workflow del Ticket 3.

## Correcciones incluidas

1. El ticket no replica defectos del legacy; define comportamiento corregido y verificable.
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
