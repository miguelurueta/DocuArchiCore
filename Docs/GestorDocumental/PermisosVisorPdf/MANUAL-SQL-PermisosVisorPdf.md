# Manual SQL Permisos Visor PDF

## 1. Objetivo
Guía operativa SQL para administrar permisos del visor PDF en tablas `ra_vis_per_*`:
- Registrar implementación.
- Registrar perfil.
- Asignar usuario a perfil.
- Revocar permiso individual a usuario.
- Asignar permiso individual a usuario.

## 2. Tablas usadas
- `ra_vis_per_implementacion`
- `ra_vis_per_permiso`
- `ra_vis_per_perfil`
- `ra_vis_per_perfil_permiso`
- `ra_vis_per_usuario_perfil`
- `ra_vis_per_usuario_override`
- `remit_dest_interno` (fuente de `id_usuario`)

## 3. Convenciones del manual
- `id_usuario` = `remit_dest_interno.id_Remit_Dest_Int`.
- `permitido`: `1=permitir`, `0=denegar`.
- `estado`: `1=activo`, `0=inactivo`.
- Ejemplos sobre empresa `id_empresa=0`.

### 3.1 Parámetros base recomendados (copiar/pegar)
Usa estos parámetros al inicio de tu script para evitar hardcodear IDs:
```sql
SET @p_id_empresa = 0;
SET @p_codigo_impl = 'gestion_facturacion';
SET @p_codigo_perfil = 'SUPERVISOR';
SET @p_id_usuario = 141;
```

Resolver IDs técnicos:
```sql
SELECT id_impl INTO @p_id_impl
FROM ra_vis_per_implementacion
WHERE codigo_impl = @p_codigo_impl
  AND id_empresa = @p_id_empresa
  AND estado = 1
LIMIT 1;

SELECT id_perfil INTO @p_id_perfil
FROM ra_vis_per_perfil
WHERE id_impl = @p_id_impl
  AND codigo_perfil = @p_codigo_perfil
  AND estado = 1
LIMIT 1;
```

---

## 4. Tarea 1: Registro de una implementación

### Paso 1. Validar si ya existe
```sql
SELECT id_impl, codigo_impl, nombre_impl, estado, id_empresa
FROM ra_vis_per_implementacion
WHERE codigo_impl = 'gestion_facturacion'
  AND id_empresa = 0;
```

### Paso 2. Insertar implementación (o actualizar si existe)
```sql
INSERT INTO ra_vis_per_implementacion
(
  codigo_impl,
  nombre_impl,
  descripcion,
  estado,
  id_empresa
)
VALUES
(
  'gestion_facturacion',
  'Gestión Facturación',
  'Visor PDF para módulo de facturación',
  1,
  0
)
ON DUPLICATE KEY UPDATE
  nombre_impl = VALUES(nombre_impl),
  descripcion = VALUES(descripcion),
  estado = VALUES(estado);
```

### Paso 3. Obtener `id_impl` para siguientes tareas
```sql
SELECT id_impl
FROM ra_vis_per_implementacion
WHERE codigo_impl = 'gestion_facturacion'
  AND id_empresa = 0
  AND estado = 1;
```

### Paso 4. Inicializar defaults por implementación (recomendado)
Regla base: `pdf.view=1`, resto `0`.
```sql
INSERT INTO ra_vis_per_impl_perm_default (id_impl, id_perm, permitido)
SELECT i.id_impl,
       p.id_perm,
       CASE WHEN p.codigo_perm = 'pdf.view' THEN 1 ELSE 0 END
FROM ra_vis_per_implementacion i
JOIN ra_vis_per_permiso p ON p.estado = 1
WHERE i.codigo_impl = 'gestion_facturacion'
  AND i.id_empresa = 0
ON DUPLICATE KEY UPDATE
  permitido = VALUES(permitido);
```

---

## 5. Tarea 2: Registro de un nuevo perfil

### Paso 1. Obtener `id_impl`
```sql
SELECT id_impl
FROM ra_vis_per_implementacion
WHERE codigo_impl = 'gestion_facturacion'
  AND id_empresa = 0
  AND estado = 1;
```

### Paso 2. Crear perfil
Ejemplo: perfil `SUPERVISOR`.
```sql
INSERT INTO ra_vis_per_perfil
(
  id_impl,
  codigo_perfil,
  nombre_perfil,
  descripcion,
  estado
)
VALUES
(
  @p_id_impl,
  'SUPERVISOR',
  'Supervisor',
  'Puede visualizar, rotar, zoom, imprimir y descargar',
  1
)
ON DUPLICATE KEY UPDATE
  nombre_perfil = VALUES(nombre_perfil),
  descripcion = VALUES(descripcion),
  estado = VALUES(estado);
```

### Paso 3. Obtener `id_perfil`
```sql
SELECT id_perfil
FROM ra_vis_per_perfil
WHERE id_impl = @p_id_impl
  AND codigo_perfil = 'SUPERVISOR'
  AND estado = 1;
```

### Paso 4. Cargar matriz de permisos del perfil
Ejemplo: habilitar `view`, `zoom`, `rotate`, `print`, `download`; negar lo demás.
```sql
INSERT INTO ra_vis_per_perfil_permiso (id_perfil, id_perm, permitido)
SELECT pf.id_perfil,
       p.id_perm,
       CASE
         WHEN p.codigo_perm IN ('pdf.view','pdf.zoom','pdf.rotate','pdf.print','pdf.download') THEN 1
         ELSE 0
       END AS permitido
FROM ra_vis_per_perfil pf
JOIN ra_vis_per_permiso p ON p.estado = 1
WHERE pf.id_perfil = @p_id_perfil
ON DUPLICATE KEY UPDATE
  permitido = VALUES(permitido);
```

### Paso 5. Verificar matriz del perfil
```sql
SELECT p.codigo_perm, pp.permitido
FROM ra_vis_per_perfil_permiso pp
JOIN ra_vis_per_permiso p ON p.id_perm = pp.id_perm
WHERE pp.id_perfil = @p_id_perfil
ORDER BY p.codigo_perm;
```

---

## 6. Tarea 3: Registro de un usuario a un perfil

### Paso 1. Validar usuario interno
```sql
SELECT id_Remit_Dest_Int, Login_Usuario, Estado_Usuario
FROM remit_dest_interno
WHERE id_Remit_Dest_Int = 141;
```

### Paso 2. Asignar usuario al perfil de la implementación
```sql
INSERT INTO ra_vis_per_usuario_perfil
(
  id_usuario,
  id_impl,
  id_perfil,
  estado,
  fecha_inicio,
  fecha_fin
)
VALUES
(
  @p_id_usuario,
  @p_id_impl,
  @p_id_perfil,
  1,
  CURDATE(),
  NULL
)
ON DUPLICATE KEY UPDATE
  id_perfil = VALUES(id_perfil),
  estado = VALUES(estado),
  fecha_inicio = VALUES(fecha_inicio),
  fecha_fin = VALUES(fecha_fin);
```

### Paso 3. Verificar asignación vigente
```sql
SELECT id_usuario, id_impl, id_perfil, estado, fecha_inicio, fecha_fin
FROM ra_vis_per_usuario_perfil
WHERE id_usuario = @p_id_usuario
  AND id_impl = @p_id_impl;
```

---

## 7. Tarea 4: Revocar permiso a un usuario (override individual)

Revocar = forzar permiso a `0` en `ra_vis_per_usuario_override`.

### Paso 1. Obtener `id_perm` del permiso a revocar
```sql
SELECT id_perm, codigo_perm
FROM ra_vis_per_permiso
WHERE codigo_perm = 'pdf.download'
  AND estado = 1;
```

### Paso 2. Aplicar revocación individual
```sql
INSERT INTO ra_vis_per_usuario_override
(
  id_usuario,
  id_impl,
  id_perm,
  permitido,
  motivo,
  estado
)
VALUES
(
  @p_id_usuario,
  @p_id_impl,
  (SELECT id_perm FROM ra_vis_per_permiso WHERE codigo_perm = 'pdf.download' AND estado = 1 LIMIT 1),
  0,
  'Revocado por política de seguridad',
  1
)
ON DUPLICATE KEY UPDATE
  permitido = VALUES(permitido),
  motivo = VALUES(motivo),
  estado = VALUES(estado);
```

### Paso 3. Verificar override aplicado
```sql
SELECT uo.id_usuario, uo.id_impl, p.codigo_perm, uo.permitido, uo.motivo, uo.estado
FROM ra_vis_per_usuario_override uo
JOIN ra_vis_per_permiso p ON p.id_perm = uo.id_perm
WHERE uo.id_usuario = @p_id_usuario
  AND uo.id_impl = @p_id_impl
  AND p.codigo_perm = 'pdf.download';
```

---

## 8. Tarea 5: Asignar permiso a un usuario (override individual)

Asignar = forzar permiso a `1` en `ra_vis_per_usuario_override`.

### Paso 1. Obtener `id_perm` del permiso a asignar
```sql
SELECT id_perm, codigo_perm
FROM ra_vis_per_permiso
WHERE codigo_perm = 'pdf.print'
  AND estado = 1;
```

### Paso 2. Aplicar asignación individual
```sql
INSERT INTO ra_vis_per_usuario_override
(
  id_usuario,
  id_impl,
  id_perm,
  permitido,
  motivo,
  estado
)
VALUES
(
  @p_id_usuario,
  @p_id_impl,
  (SELECT id_perm FROM ra_vis_per_permiso WHERE codigo_perm = 'pdf.print' AND estado = 1 LIMIT 1),
  1,
  'Habilitado por excepción operativa',
  1
)
ON DUPLICATE KEY UPDATE
  permitido = VALUES(permitido),
  motivo = VALUES(motivo),
  estado = VALUES(estado);
```

### Paso 3. Verificar override aplicado
```sql
SELECT uo.id_usuario, uo.id_impl, p.codigo_perm, uo.permitido, uo.motivo, uo.estado
FROM ra_vis_per_usuario_override uo
JOIN ra_vis_per_permiso p ON p.id_perm = uo.id_perm
WHERE uo.id_usuario = @p_id_usuario
  AND uo.id_impl = @p_id_impl
  AND p.codigo_perm = 'pdf.print';
```

---

## 9. Validación final de permisos efectivos (diagnóstico)
Consulta única para ver resultado final por permiso y origen:
```sql
SELECT
  p.codigo_perm,
  CASE
    WHEN uo.permitido IS NOT NULL THEN uo.permitido
    WHEN pp.permitido IS NOT NULL THEN pp.permitido
    WHEN d.permitido IS NOT NULL THEN d.permitido
    ELSE 0
  END AS permitido_efectivo,
  CASE
    WHEN uo.permitido IS NOT NULL THEN 'usuario_override'
    WHEN pp.permitido IS NOT NULL THEN 'perfil'
    WHEN d.permitido IS NOT NULL THEN 'default_impl'
    ELSE 'fallback_deny'
  END AS origen
FROM ra_vis_per_permiso p
JOIN ra_vis_per_implementacion i
  ON i.codigo_impl = @p_codigo_impl
 AND i.estado = 1
LEFT JOIN ra_vis_per_impl_perm_default d
  ON d.id_impl = i.id_impl
 AND d.id_perm = p.id_perm
LEFT JOIN ra_vis_per_usuario_perfil up
  ON up.id_impl = i.id_impl
 AND up.id_usuario = @p_id_usuario
 AND up.estado = 1
 AND (up.fecha_inicio IS NULL OR up.fecha_inicio <= CURDATE())
 AND (up.fecha_fin IS NULL OR up.fecha_fin >= CURDATE())
LEFT JOIN ra_vis_per_perfil_permiso pp
  ON pp.id_perfil = up.id_perfil
 AND pp.id_perm = p.id_perm
LEFT JOIN ra_vis_per_usuario_override uo
  ON uo.id_impl = i.id_impl
 AND uo.id_usuario = @p_id_usuario
 AND uo.id_perm = p.id_perm
 AND uo.estado = 1
WHERE p.estado = 1
ORDER BY p.codigo_perm;
```

## 10. Recomendaciones operativas
- Ejecutar cambios en ambiente controlado y con respaldo.
- Evitar hardcodear `id_impl`, `id_perfil`, `id_perm`; resolverlos por `codigo_*`.
- Mantener `motivo` en overrides para auditoría.
- Si se quiere “quitar excepción”, borrar override o poner `estado=0`.

---

## 11. Ejemplos adicionales necesarios (operación diaria)

### 11.1 Cambiar un usuario de perfil (misma implementación)
```sql
SET @p_nuevo_codigo_perfil = 'LECTOR';

SELECT id_perfil INTO @p_nuevo_id_perfil
FROM ra_vis_per_perfil
WHERE id_impl = @p_id_impl
  AND codigo_perfil = @p_nuevo_codigo_perfil
  AND estado = 1
LIMIT 1;

UPDATE ra_vis_per_usuario_perfil
SET id_perfil = @p_nuevo_id_perfil,
    estado = 1,
    fecha_inicio = CURDATE(),
    fecha_fin = NULL
WHERE id_usuario = @p_id_usuario
  AND id_impl = @p_id_impl;
```

### 11.2 Desasignar perfil de un usuario (sin borrado físico)
```sql
UPDATE ra_vis_per_usuario_perfil
SET estado = 0,
    fecha_fin = CURDATE()
WHERE id_usuario = @p_id_usuario
  AND id_impl = @p_id_impl;
```

### 11.3 Quitar un override puntual (volver a perfil/default)
```sql
DELETE uo
FROM ra_vis_per_usuario_override uo
JOIN ra_vis_per_permiso p ON p.id_perm = uo.id_perm
WHERE uo.id_usuario = @p_id_usuario
  AND uo.id_impl = @p_id_impl
  AND p.codigo_perm = 'pdf.print';
```

Alternativa sin borrar (inactivar):
```sql
UPDATE ra_vis_per_usuario_override uo
JOIN ra_vis_per_permiso p ON p.id_perm = uo.id_perm
SET uo.estado = 0
WHERE uo.id_usuario = @p_id_usuario
  AND uo.id_impl = @p_id_impl
  AND p.codigo_perm = 'pdf.print';
```

### 11.4 Cargar overrides masivos para un usuario
```sql
INSERT INTO ra_vis_per_usuario_override
(id_usuario, id_impl, id_perm, permitido, motivo, estado)
SELECT @p_id_usuario, @p_id_impl, p.id_perm,
       CASE
         WHEN p.codigo_perm IN ('pdf.print','pdf.download') THEN 1
         ELSE 0
       END AS permitido,
       'Carga masiva por política de operación',
       1
FROM ra_vis_per_permiso p
WHERE p.estado = 1
ON DUPLICATE KEY UPDATE
  permitido = VALUES(permitido),
  motivo = VALUES(motivo),
  estado = VALUES(estado);
```

### 11.5 Duplicar configuración de perfil entre implementaciones
Caso: copiar matriz `SUPERVISOR` de `workflow` a `gestion_facturacion`.
```sql
SET @p_impl_origen = 'workflow';
SET @p_impl_destino = 'gestion_facturacion';
SET @p_perfil_codigo = 'SUPERVISOR';

SELECT pf.id_perfil INTO @p_id_perfil_origen
FROM ra_vis_per_perfil pf
JOIN ra_vis_per_implementacion i ON i.id_impl = pf.id_impl
WHERE i.codigo_impl = @p_impl_origen
  AND i.id_empresa = @p_id_empresa
  AND pf.codigo_perfil = @p_perfil_codigo
  AND pf.estado = 1
LIMIT 1;

SELECT pf.id_perfil INTO @p_id_perfil_destino
FROM ra_vis_per_perfil pf
JOIN ra_vis_per_implementacion i ON i.id_impl = pf.id_impl
WHERE i.codigo_impl = @p_impl_destino
  AND i.id_empresa = @p_id_empresa
  AND pf.codigo_perfil = @p_perfil_codigo
  AND pf.estado = 1
LIMIT 1;

INSERT INTO ra_vis_per_perfil_permiso (id_perfil, id_perm, permitido)
SELECT @p_id_perfil_destino, id_perm, permitido
FROM ra_vis_per_perfil_permiso
WHERE id_perfil = @p_id_perfil_origen
ON DUPLICATE KEY UPDATE
  permitido = VALUES(permitido);
```

### 11.6 Detectar usuarios con perfil vencido
```sql
SELECT up.id_usuario, up.id_impl, up.id_perfil, up.fecha_inicio, up.fecha_fin
FROM ra_vis_per_usuario_perfil up
WHERE up.estado = 1
  AND up.fecha_fin IS NOT NULL
  AND up.fecha_fin < CURDATE();
```

### 11.7 Auditoría rápida de overrides activos por implementación
```sql
SELECT uo.id_usuario, i.codigo_impl, p.codigo_perm, uo.permitido, uo.motivo
FROM ra_vis_per_usuario_override uo
JOIN ra_vis_per_implementacion i ON i.id_impl = uo.id_impl
JOIN ra_vis_per_permiso p ON p.id_perm = uo.id_perm
WHERE uo.estado = 1
  AND i.codigo_impl = @p_codigo_impl
ORDER BY uo.id_usuario, p.codigo_perm;
```
