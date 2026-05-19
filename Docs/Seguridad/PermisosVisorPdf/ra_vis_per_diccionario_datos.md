# Diccionario De Datos - Permisos Visor PDF (`ra_vis_per_*`)

## 1) Tabla: `ra_vis_per_implementacion`
Catalogo de contextos funcionales donde se usa el visor PDF (por ejemplo `workflow`, `gestion_correspondencia`).

| Campo | Tipo | Nulo | Default | Llave | Descripcion |
|---|---|---|---|---|---|
| `id_impl` | `int(10) unsigned` | No | `AUTO_INCREMENT` | PK | Identificador interno de implementacion. |
| `codigo_impl` | `varchar(80)` | No | - | UK (`codigo_impl`,`id_empresa`) | Codigo tecnico de implementacion. |
| `nombre_impl` | `varchar(120)` | No | - | - | Nombre funcional visible. |
| `descripcion` | `varchar(255)` | Si | `NULL` | - | Detalle opcional de la implementacion. |
| `estado` | `int(11)` | No | `1` | IDX | Estado logico (1 activo, 0 inactivo). |
| `id_empresa` | `int(11)` | No | `0` | UK (`codigo_impl`,`id_empresa`) | Segmentacion por empresa/tenant. |
| `fecha_creacion` | `datetime` | No | `CURRENT_TIMESTAMP` | - | Fecha de alta del registro. |

## 2) Tabla: `ra_vis_per_permiso`
Catalogo central de permisos granulares del visor.

| Campo | Tipo | Nulo | Default | Llave | Descripcion |
|---|---|---|---|---|---|
| `id_perm` | `int(10) unsigned` | No | `AUTO_INCREMENT` | PK | Identificador interno del permiso. |
| `codigo_perm` | `varchar(120)` | No | - | UK | Codigo unico del permiso (ej: `pdf.view`). |
| `recurso` | `varchar(60)` | No | `pdf` | - | Recurso funcional al que aplica el permiso. |
| `accion` | `varchar(60)` | No | - | - | Accion especifica del recurso. |
| `descripcion` | `varchar(255)` | Si | `NULL` | - | Detalle funcional del permiso. |
| `estado` | `int(11)` | No | `1` | IDX | Estado logico del permiso. |

## 3) Tabla: `ra_vis_per_impl_perm_default`
Matriz default de permisos por implementacion.

| Campo | Tipo | Nulo | Default | Llave | Descripcion |
|---|---|---|---|---|---|
| `id_impl` | `int(10) unsigned` | No | - | PK, FK | Implementacion asociada. |
| `id_perm` | `int(10) unsigned` | No | - | PK, FK | Permiso asociado. |
| `permitido` | `int(11)` | No | `0` | - | Valor default (1 permite, 0 deniega). |

FK:
- `id_impl -> ra_vis_per_implementacion.id_impl`
- `id_perm -> ra_vis_per_permiso.id_perm`

## 4) Tabla: `ra_vis_per_perfil`
Perfiles de permisos por implementacion (ejemplo: `LECTOR`, `FIRMANTE`).

| Campo | Tipo | Nulo | Default | Llave | Descripcion |
|---|---|---|---|---|---|
| `id_perfil` | `int(10) unsigned` | No | `AUTO_INCREMENT` | PK | Identificador interno del perfil. |
| `id_impl` | `int(10) unsigned` | No | - | FK, UK (`id_impl`,`codigo_perfil`) | Implementacion a la que pertenece el perfil. |
| `codigo_perfil` | `varchar(80)` | No | - | UK (`id_impl`,`codigo_perfil`) | Codigo tecnico del perfil. |
| `nombre_perfil` | `varchar(120)` | No | - | - | Nombre funcional del perfil. |
| `descripcion` | `varchar(255)` | Si | `NULL` | - | Detalle opcional del perfil. |
| `estado` | `int(11)` | No | `1` | IDX | Estado logico del perfil. |

FK:
- `id_impl -> ra_vis_per_implementacion.id_impl`

## 5) Tabla: `ra_vis_per_perfil_permiso`
Matriz de permisos por perfil.

| Campo | Tipo | Nulo | Default | Llave | Descripcion |
|---|---|---|---|---|---|
| `id_perfil` | `int(10) unsigned` | No | - | PK, FK | Perfil asociado. |
| `id_perm` | `int(10) unsigned` | No | - | PK, FK | Permiso asociado. |
| `permitido` | `int(11)` | No | `0` | - | Valor del permiso para el perfil (1/0). |

FK:
- `id_perfil -> ra_vis_per_perfil.id_perfil`
- `id_perm -> ra_vis_per_permiso.id_perm`

## 6) Tabla: `ra_vis_per_usuario_perfil`
Asignacion de perfil a usuario por implementacion.

| Campo | Tipo | Nulo | Default | Llave | Descripcion |
|---|---|---|---|---|---|
| `id_usuario` | `int(10) unsigned` | No | - | PK, FK | Usuario interno (`remit_dest_interno.id_Remit_Dest_Int`). |
| `id_impl` | `int(10) unsigned` | No | - | PK, FK | Implementacion donde aplica el perfil. |
| `id_perfil` | `int(10) unsigned` | No | - | FK, IDX | Perfil asignado al usuario. |
| `estado` | `int(11)` | No | `1` | - | Estado logico de la asignacion. |
| `fecha_inicio` | `date` | Si | `NULL` | - | Fecha desde la cual aplica la asignacion. |
| `fecha_fin` | `date` | Si | `NULL` | - | Fecha hasta la cual aplica la asignacion. |

FK:
- `id_usuario -> remit_dest_interno.id_Remit_Dest_Int`
- `id_impl -> ra_vis_per_implementacion.id_impl`
- `id_perfil -> ra_vis_per_perfil.id_perfil`

## 7) Tabla: `ra_vis_per_usuario_override`
Excepciones por usuario que sobrescriben el perfil/default.

| Campo | Tipo | Nulo | Default | Llave | Descripcion |
|---|---|---|---|---|---|
| `id_usuario` | `int(10) unsigned` | No | - | PK, FK | Usuario interno. |
| `id_impl` | `int(10) unsigned` | No | - | PK, FK | Implementacion objetivo. |
| `id_perm` | `int(10) unsigned` | No | - | PK, FK | Permiso objetivo. |
| `permitido` | `int(11)` | No | `0` | - | Valor forzado para ese usuario (1/0). |
| `motivo` | `varchar(255)` | Si | `NULL` | - | Justificacion de la excepcion. |
| `estado` | `int(11)` | No | `1` | - | Estado logico del override. |

FK:
- `id_usuario -> remit_dest_interno.id_Remit_Dest_Int`
- `id_impl -> ra_vis_per_implementacion.id_impl`
- `id_perm -> ra_vis_per_permiso.id_perm`

## 8) Relacion Con Usuarios
- Usuario base: tabla `remit_dest_interno`.
- Campo de enlace: `remit_dest_interno.id_Remit_Dest_Int`.
- La matriz soporta perfiles distintos por implementacion para el mismo usuario.

## 9) Regla De Resolucion De Permisos (Efectivos)
Precedencia recomendada para backend:
1. `ra_vis_per_usuario_override`
2. `ra_vis_per_perfil_permiso` (segun `ra_vis_per_usuario_perfil`)
3. `ra_vis_per_impl_perm_default`
4. fallback denegar (`0`)

Consulta base disponible en:
- `Docs/Seguridad/PermisosVisorPdf/ra_vis_per_consulta_permisos_efectivos.sql`

