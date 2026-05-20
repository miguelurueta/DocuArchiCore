-- ============================================================
-- Seed base para permisos del visor PDF
-- ============================================================

-- Implementaciones ejemplo
INSERT INTO `ra_vis_per_implementacion`
(`codigo_impl`, `nombre_impl`, `descripcion`, `estado`, `id_empresa`)
VALUES
('workflow', 'Workflow', 'Visor PDF en modulo workflow', 1, 0),
('gestion_correspondencia', 'Gestion Correspondencia', 'Visor PDF en gestion correspondencia', 1, 0)
ON DUPLICATE KEY UPDATE
`nombre_impl` = VALUES(`nombre_impl`),
`descripcion` = VALUES(`descripcion`),
`estado` = VALUES(`estado`);

-- Catalogo de permisos
INSERT INTO `ra_vis_per_permiso` (`codigo_perm`, `recurso`, `accion`, `descripcion`, `estado`) VALUES
('pdf.view', 'pdf', 'view', 'Abrir visor', 1),
('pdf.print', 'pdf', 'print', 'Imprimir documento', 1),
('pdf.download', 'pdf', 'download', 'Descargar/exportar documento', 1),
('pdf.annotate.open_signature_modal', 'pdf', 'annotate_open_signature_modal', 'Abrir modal de firma', 1),
('pdf.annotate.signature.draw', 'pdf', 'annotate_signature_draw', 'Tab dibujar firma', 1),
('pdf.annotate.signature.upload', 'pdf', 'annotate_signature_upload', 'Tab subir firma', 1),
('pdf.annotate.signature.personal', 'pdf', 'annotate_signature_personal', 'Tab firma personal', 1),
('pdf.annotate.signature.place', 'pdf', 'annotate_signature_place', 'Ubicar firma en PDF', 1),
('pdf.annotate.signature.delete', 'pdf', 'annotate_signature_delete', 'Eliminar firma', 1),
('pdf.annotate.signature.lock', 'pdf', 'annotate_signature_lock', 'Bloquear firma', 1),
('pdf.annotate.signature.unlock', 'pdf', 'annotate_signature_unlock', 'Desbloquear firma', 1),
('pdf.rotate', 'pdf', 'rotate', 'Rotar paginas', 1),
('pdf.zoom', 'pdf', 'zoom', 'Control de zoom', 1)
ON DUPLICATE KEY UPDATE
`descripcion` = VALUES(`descripcion`),
`estado` = VALUES(`estado`);

-- Perfiles ejemplo por implementacion
INSERT INTO `ra_vis_per_perfil`
(`id_impl`, `codigo_perfil`, `nombre_perfil`, `descripcion`, `estado`)
SELECT i.`id_impl`, 'LECTOR', 'Lector', 'Solo lectura y navegacion basica', 1
FROM `ra_vis_per_implementacion` i
WHERE i.`codigo_impl` IN ('workflow', 'gestion_correspondencia')
ON DUPLICATE KEY UPDATE
`nombre_perfil` = VALUES(`nombre_perfil`),
`descripcion` = VALUES(`descripcion`),
`estado` = VALUES(`estado`);

INSERT INTO `ra_vis_per_perfil`
(`id_impl`, `codigo_perfil`, `nombre_perfil`, `descripcion`, `estado`)
SELECT i.`id_impl`, 'FIRMANTE', 'Firmante', 'Lectura + acciones de firma', 1
FROM `ra_vis_per_implementacion` i
WHERE i.`codigo_impl` IN ('workflow', 'gestion_correspondencia')
ON DUPLICATE KEY UPDATE
`nombre_perfil` = VALUES(`nombre_perfil`),
`descripcion` = VALUES(`descripcion`),
`estado` = VALUES(`estado`);

-- Defaults por implementacion:
-- Regla base: todo denegado excepto pdf.view
INSERT INTO `ra_vis_per_impl_perm_default` (`id_impl`, `id_perm`, `permitido`)
SELECT i.`id_impl`, p.`id_perm`,
       CASE WHEN p.`codigo_perm` = 'pdf.view' THEN 1 ELSE 0 END AS permitido
FROM `ra_vis_per_implementacion` i
JOIN `ra_vis_per_permiso` p ON p.`estado` = 1
WHERE i.`codigo_impl` IN ('workflow', 'gestion_correspondencia')
ON DUPLICATE KEY UPDATE
`permitido` = VALUES(`permitido`);

-- Matriz para perfil LECTOR:
-- view/zoom habilitado, resto denegado
INSERT INTO `ra_vis_per_perfil_permiso` (`id_perfil`, `id_perm`, `permitido`)
SELECT pf.`id_perfil`, p.`id_perm`,
       CASE WHEN p.`codigo_perm` IN ('pdf.view', 'pdf.zoom') THEN 1 ELSE 0 END
FROM `ra_vis_per_perfil` pf
JOIN `ra_vis_per_permiso` p ON p.`estado` = 1
WHERE pf.`codigo_perfil` = 'LECTOR'
ON DUPLICATE KEY UPDATE
`permitido` = VALUES(`permitido`);

-- Matriz para perfil FIRMANTE:
-- habilita lectura, navegacion y firma; no descarga/impresion por defecto
INSERT INTO `ra_vis_per_perfil_permiso` (`id_perfil`, `id_perm`, `permitido`)
SELECT pf.`id_perfil`, p.`id_perm`,
       CASE
         WHEN p.`codigo_perm` IN (
           'pdf.view',
           'pdf.zoom',
           'pdf.rotate',
           'pdf.annotate.open_signature_modal',
           'pdf.annotate.signature.draw',
           'pdf.annotate.signature.upload',
           'pdf.annotate.signature.personal',
           'pdf.annotate.signature.place',
           'pdf.annotate.signature.delete',
           'pdf.annotate.signature.lock',
           'pdf.annotate.signature.unlock'
         ) THEN 1
         ELSE 0
       END
FROM `ra_vis_per_perfil` pf
JOIN `ra_vis_per_permiso` p ON p.`estado` = 1
WHERE pf.`codigo_perfil` = 'FIRMANTE'
ON DUPLICATE KEY UPDATE
`permitido` = VALUES(`permitido`);

