# Ticket 5 - Integraciones y procesos auxiliares

Objetivo:
Migrar integraciones auxiliares del flujo (asignaciones, estados, adjuntos/notificaciones si aplican).

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
