# Ticket 6 - Pruebas de paridad funcional

Objetivo:
Validar paridad funcional entre comportamiento legacy y comportamiento migrado.

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
