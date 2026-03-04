# Ticket 4 - Persistencia y transacciones

Objetivo:
Migrar operaciones de persistencia del proceso de radicación con manejo transaccional.

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
