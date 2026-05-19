# SCRUM-206 Diagramas Permisos Visor PDF

## 1. Secuencia: mis permisos
```text
Frontend
  | GET /implementaciones/{codigoImpl}/mis-permisos
  v
PermisosVisorPdfController
  | valida claims (defaulalias, usuarioid)
  | valida codigoImpl
  v
PermisosVisorPdfService
  | solicita catalogo activo de permisos
  | resuelve override usuario
  | resuelve perfil activo (si existe)
  | resuelve default implementacion
  | aplica fallback deny
  v
PermisosVisorPdfRepository
  | QueryOptions + DapperCrudEngine
  v
DB (ra_vis_per_*)
  ^
  | resultado consolidado
PermisosVisorPdfService
  | arma Dictionary<string,bool> + sources
  v
PermisosVisorPdfController
  | AppResponses<VisorPdfPermissionsResponseDto>
  v
Frontend
```

## 2. Secuencia: upsert override (admin)
```text
Admin FE
  | PUT /implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides
  v
PermisosVisorPdfController
  | valida claims + autorizacion admin
  | valida payload overrides
  v
PermisosVisorPdfService
  | valida codigos de permiso
  | delega persistencia transaccional
  v
PermisosVisorPdfRepository
  | upsert por (id_usuario,id_impl,id_perm)
  v
DB (ra_vis_per_usuario_override)
```

## 3. Secuencia: delete override (admin)
```text
Admin FE
  | DELETE /implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}
  v
PermisosVisorPdfController
  | valida claims + autorizacion admin + input
  v
PermisosVisorPdfService
  | valida existencia de permiso
  v
PermisosVisorPdfRepository
  | delete parametrizado
  v
DB (ra_vis_per_usuario_override)
```

## 4. Entidades y relacion simplificada
```text
ra_vis_per_implementacion (id_impl, codigo_impl)
  1 --- * ra_vis_per_impl_perm_default (* id_perm)
  1 --- * ra_vis_per_perfil
  1 --- * ra_vis_per_usuario_perfil (* id_usuario)
  1 --- * ra_vis_per_usuario_override (* id_usuario, * id_perm)

ra_vis_per_permiso (id_perm, codigo_perm)
  1 --- * ra_vis_per_impl_perm_default
  1 --- * ra_vis_per_perfil_permiso (* id_perfil)
  1 --- * ra_vis_per_usuario_override

ra_vis_per_perfil (id_perfil, id_impl)
  1 --- * ra_vis_per_perfil_permiso

remit_dest_interno (id_Remit_Dest_Int)
  1 --- * ra_vis_per_usuario_perfil
  1 --- * ra_vis_per_usuario_override
```

