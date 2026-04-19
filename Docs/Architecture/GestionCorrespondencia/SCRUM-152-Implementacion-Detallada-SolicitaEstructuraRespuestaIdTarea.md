# SCRUM-152 — Implementación Detallada: SolicitaEstructuraRespuestaIdTarea

## Contexto
El flujo `SolicitaEstructuraRespuestaIdTareaAsync(idTareaWf, defaultDbAlias)` consulta la tabla `ra_respuesta_radicado` filtrando por `ID_TAREA_WF`.

Se detectó un problema donde, aun existiendo datos en BD, el modelo materializado (`RaRespuestaRadicado`) retornaba `IdTareaWf` vacío/null. Esto rompe flujos aguas abajo (por ejemplo, hidratación de plantillas/tokens del editor).

## Causa raíz
La tabla usa columnas legacy en formato `UPPER_SNAKE_CASE` (ej. `ID_TAREA_WF`) mientras el modelo C# usa propiedades PascalCase (ej. `IdTareaWf`) y atributos `[Column("IdTareaWf")]`.

En este caso, el acceso de datos no estaba garantizando el mapeo por atributo ni realizando conversión automática de nombres, por lo que el mapeo quedaba inconsistente.

## Decisión técnica
Usar un `SELECT` explícito con alias (`AS`) para mapear de forma determinística columnas legacy a las propiedades del modelo:

- `rr.ID_TAREA_WF AS IdTareaWf` (obligatorio)
- Alias explícitos para campos usados por tokens/plantillas (`Radicado`, `Destinatario`, `DireccionDestinatario`, `Asunto`, `UsuarioResponsable`, `CargoResponsable`, `AreaResponsable`).

Esto evita depender de convenciones implícitas o de soporte no confirmado de `[Column]` en el materializador.

## SQL (extracto)
La consulta se implementa con parámetros (sin concatenación) y filtro:

- `WHERE rr.ID_TAREA_WF = @idTareaWf`

Y retorna un set mínimo de columnas (incluyendo campos no-nullables/requeridos del modelo) para evitar valores por defecto inconsistentes.

## Guardrail técnico
Si la consulta retorna filas pero `IdTareaWf` llega vacío/null/0, se retorna un error controlado tipo “Mapping” para evitar fallas silenciosas en flujos posteriores.

## Pruebas / Evidencia
Pendiente según infraestructura de pruebas del repositorio:

- Validar caso con fila real `ID_TAREA_WF = X` y comprobar `IdTareaWf == X`.
- Validar “Sin resultados” devuelve `success=true`, `data=[]`, `message="Sin resultados"`.

