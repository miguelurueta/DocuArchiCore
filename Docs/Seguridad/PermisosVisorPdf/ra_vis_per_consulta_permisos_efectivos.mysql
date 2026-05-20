-- ============================================================
-- Consulta permisos efectivos por usuario + implementacion
-- Precedencia:
--   1) override usuario
--   2) perfil asignado al usuario en la implementacion
--   3) default de implementacion
--   4) denegar (0)
-- ============================================================

-- Parametros esperados:
--   @p_id_usuario   -> remit_dest_interno.id_Remit_Dest_Int
--   @p_codigo_impl  -> codigo de implementacion (ej: workflow)

SELECT
  p.`codigo_perm`,
  CASE
    WHEN uo.`permitido` IS NOT NULL THEN uo.`permitido`
    WHEN pp.`permitido` IS NOT NULL THEN pp.`permitido`
    WHEN d.`permitido` IS NOT NULL THEN d.`permitido`
    ELSE 0
  END AS `permitido_efectivo`,
  CASE
    WHEN uo.`permitido` IS NOT NULL THEN 'usuario_override'
    WHEN pp.`permitido` IS NOT NULL THEN 'perfil'
    WHEN d.`permitido` IS NOT NULL THEN 'default_impl'
    ELSE 'fallback_deny'
  END AS `origen`
FROM `ra_vis_per_permiso` p
JOIN `ra_vis_per_implementacion` i
  ON i.`codigo_impl` = @p_codigo_impl
 AND i.`estado` = 1
LEFT JOIN `ra_vis_per_impl_perm_default` d
  ON d.`id_impl` = i.`id_impl`
 AND d.`id_perm` = p.`id_perm`
LEFT JOIN `ra_vis_per_usuario_perfil` up
  ON up.`id_impl` = i.`id_impl`
 AND up.`id_usuario` = @p_id_usuario
 AND up.`estado` = 1
 AND (up.`fecha_inicio` IS NULL OR up.`fecha_inicio` <= CURDATE())
 AND (up.`fecha_fin` IS NULL OR up.`fecha_fin` >= CURDATE())
LEFT JOIN `ra_vis_per_perfil_permiso` pp
  ON pp.`id_perfil` = up.`id_perfil`
 AND pp.`id_perm` = p.`id_perm`
LEFT JOIN `ra_vis_per_usuario_override` uo
  ON uo.`id_impl` = i.`id_impl`
 AND uo.`id_usuario` = @p_id_usuario
 AND uo.`id_perm` = p.`id_perm`
 AND uo.`estado` = 1
WHERE p.`estado` = 1
ORDER BY p.`codigo_perm` ASC;

