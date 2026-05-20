# SCRUM-206 Reglas de Permisos Efectivos y Errores

## 1. Regla funcional principal
- El frontend siempre recibe un mapa único `Permissions` por implementación/usuario.
- La resolución efectiva aplica precedencia:
  - `usuario_override`
  - `perfil_activo`
  - `default_implementacion`
  - `fallback_deny`

## 2. Detección de origen del permiso
- `usuario_override`: existe override activo en `ra_vis_per_usuario_override`.
- `perfil_activo`: existe asignación vigente en `ra_vis_per_usuario_perfil` + regla en `ra_vis_per_perfil_permiso`.
- `default_implementacion`: valor por defecto en `ra_vis_per_impl_perm_default`.
- `fallback_deny`: no existe definición previa, se retorna `false`.

## 3. Contrato de salida esperado (`mis-permisos`)
- `Permissions`: `Dictionary<string,bool>`.
- `Sources`: `Dictionary<string,string>` con el origen de cada permiso.
- `GeneratedAt`: timestamp de generación.
- `meta.Status`: `success` en operación OK.

## 4. Política de seguridad y acceso
- `defaulalias` obligatorio para todas las operaciones.
- `usuarioid` obligatorio para `mis-permisos`.
- Endpoints admin autorizan por:
  - `HasPermission("pdf.permissions.admin")`, o
  - claim `esadmin/isadmin/admin/role/rol` con valor `1|true|admin|superadmin`.
- `codigoImpl` y `codigoPermiso` validados por regex segura `^[A-Za-z0-9_\.]+$`.

## 5. Estrategia de persistencia de overrides
- `PUT overrides` procesa item a item:
  - si existe registro: `update`
  - si no existe: `insert`
- `DELETE override` elimina por clave funcional (`id_usuario`, `id_impl`, `id_perm`).
- Implementación actual: sin transacción explícita de lote en `PUT`.

## 6. Errores y comportamiento esperado
- `400 Validation`:
  - claims faltantes/inválidos (`defaulalias`, `usuarioid`)
  - request inválido (`codigoImpl`, `idUsuario`, `codigoPermiso`, `Permitido`)
  - implementación/permiso no encontrado
- `401`:
  - consumo de endpoint admin sin privilegios
- `500`:
  - excepción técnica no controlada

## 7. Observabilidad mínima recomendada
- Registrar inicio/fin de operaciones service.
- Registrar parámetros clave (`usuarioId`, `codigoImpl`, `success`, `total/procesados`).
- Registrar excepciones en `catch`.
- Como mejora pendiente: correlación por `requestId` transversal.
