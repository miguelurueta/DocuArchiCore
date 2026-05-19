# SCRUM-206 Seguridad Permisos Visor PDF

## 1. Claims obligatorios
- `defaulalias`: alias/tenant para resolver conexion.
- `usuarioid`: identificador del usuario autenticado.

Si falta alguno:
- No ejecutar acceso a datos.
- Responder `AppResponses` controlado con `success=false`.

## 2. Endpoints con control administrativo
Requieren permiso administrativo explicito:
- `GET /implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos`
- `PUT /implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides`
- `DELETE /implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}`

## 3. Validaciones de entrada
- `codigoImpl`: regex `^[A-Za-z0-9_]+$`.
- `idUsuario`: entero positivo.
- `codigoPermiso`: debe existir en catalogo activo `ra_vis_per_permiso`.
- `permitido`: solo `0` o `1`.
- `motivo`: maximo recomendado `255` caracteres.

## 4. Politica de acceso a datos
- Uso obligatorio de `DapperCrudEngine + QueryOptions`.
- Criterios parametrizados.
- Prohibido SQL concatenado con entradas de usuario.

## 5. Riesgos mitigados
- Escalamiento horizontal de permisos por implementacion.
- Evitar elevacion no autorizada por endpoints admin.
- Evitar inyeccion SQL por validacion + parametrizacion.
- Evitar exposicion de stacktrace en respuestas.

## 6. Reglas de trazabilidad
Registrar en logs:
- `requestId`
- `usuarioid`
- `alias`
- `codigoImpl`
- `idUsuarioObjetivo`
- `operacion` (`query`, `upsert_override`, `delete_override`)

