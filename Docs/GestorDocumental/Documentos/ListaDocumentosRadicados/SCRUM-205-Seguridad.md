# SCRUM-205 Seguridad ListaDocumentosRadicados

## 1. Objetivo
Definir controles de seguridad para `query` y `action` en `ListaDocumentosRadicados`.

## 2. Claims obligatorios
- `defaulalias`:
  - requerido
  - origen exclusivo desde claim autenticado
- `usuarioid`:
  - requerido
  - numerico y mayor a cero

Comportamiento:
- Claims invalidos -> `BadRequest` o error de seguridad controlado en `AppResponses`.

## 3. Validaciones de entrada
- `ViewMode` solo permite `hierarchical` o `flatDocuments`.
- `ActionId` solo permite acciones soportadas.
- `Page` y `PageSize` con limites de saneamiento.
- `SortField` validado contra lista permitida.
- `SortDir` limitado a `ASC`/`DESC`.
- `RowId`, `ParentRowId`, `NodeType` validados por formato y coherencia.

## 4. Seguridad de acceso a datos
- Uso obligatorio de `DapperCrudEngine` + `QueryOptions`.
- SQL siempre parametrizado.
- Prohibido SQL string concatenado.
- Prohibido `QueryAsync`/`ExecuteAsync` manual fuera de motor estandar.

## 5. Manejo de errores
- `try/catch` en capas criticas.
- Respuesta uniforme `AppResponses<T>`.
- Sin exponer stacktrace interno ni datos sensibles.

## 6. Superficie de ataque cubierta
- SQL Injection:
  - mitigado por parametros y `QueryOptions`.
- Elevacion de privilegios por claims falsos:
  - mitigado por validacion de claims en controller.
- Input tampering en acciones:
  - mitigado por whitelist de `ActionId` y validacion de payload.

## 7. Checklist de revision de seguridad
- Validacion de claims implementada y probada.
- Validacion de `ViewMode` y `ActionId` implementada y probada.
- Confirmacion de consultas parametrizadas.
- Confirmacion de errores controlados sin stacktrace.
- Confirmacion de no regresion en endpoints existentes.

