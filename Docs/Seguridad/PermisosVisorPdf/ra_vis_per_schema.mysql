-- ============================================================
-- Esquema de permisos de visor PDF multi-implementacion
-- Prefijo obligatorio: ra_vis_per_
-- Motor objetivo: MySQL / InnoDB
-- ============================================================

-- 1) Implementaciones donde se usa el visor
CREATE TABLE IF NOT EXISTS `ra_vis_per_implementacion` (
  `id_impl` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `codigo_impl` varchar(80) NOT NULL,
  `nombre_impl` varchar(120) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  `estado` int(11) NOT NULL DEFAULT '1',
  `id_empresa` int(11) NOT NULL DEFAULT '0',
  `fecha_creacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_impl`),
  UNIQUE KEY `uk_rvpi_codigo_empresa` (`codigo_impl`, `id_empresa`),
  KEY `idx_rvpi_estado` (`estado`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- 2) Catalogo de permisos funcionales del visor
CREATE TABLE IF NOT EXISTS `ra_vis_per_permiso` (
  `id_perm` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `codigo_perm` varchar(120) NOT NULL,
  `recurso` varchar(60) NOT NULL DEFAULT 'pdf',
  `accion` varchar(60) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  `estado` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_perm`),
  UNIQUE KEY `uk_rvpp_codigo` (`codigo_perm`),
  KEY `idx_rvpp_estado` (`estado`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- 3) Default por implementacion
CREATE TABLE IF NOT EXISTS `ra_vis_per_impl_perm_default` (
  `id_impl` int(10) unsigned NOT NULL,
  `id_perm` int(10) unsigned NOT NULL,
  `permitido` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id_impl`, `id_perm`),
  CONSTRAINT `fk_rvpd_impl` FOREIGN KEY (`id_impl`) REFERENCES `ra_vis_per_implementacion` (`id_impl`),
  CONSTRAINT `fk_rvpd_perm` FOREIGN KEY (`id_perm`) REFERENCES `ra_vis_per_permiso` (`id_perm`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- 4) Perfiles por implementacion (ej: LECTOR, FIRMANTE, ADMIN_VISOR)
CREATE TABLE IF NOT EXISTS `ra_vis_per_perfil` (
  `id_perfil` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `id_impl` int(10) unsigned NOT NULL,
  `codigo_perfil` varchar(80) NOT NULL,
  `nombre_perfil` varchar(120) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  `estado` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_perfil`),
  UNIQUE KEY `uk_rvpf_impl_codigo` (`id_impl`, `codigo_perfil`),
  KEY `idx_rvpf_estado` (`estado`),
  CONSTRAINT `fk_rvpf_impl` FOREIGN KEY (`id_impl`) REFERENCES `ra_vis_per_implementacion` (`id_impl`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- 5) Matriz de permisos por perfil
CREATE TABLE IF NOT EXISTS `ra_vis_per_perfil_permiso` (
  `id_perfil` int(10) unsigned NOT NULL,
  `id_perm` int(10) unsigned NOT NULL,
  `permitido` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id_perfil`, `id_perm`),
  CONSTRAINT `fk_rvppf_perfil` FOREIGN KEY (`id_perfil`) REFERENCES `ra_vis_per_perfil` (`id_perfil`),
  CONSTRAINT `fk_rvppf_perm` FOREIGN KEY (`id_perm`) REFERENCES `ra_vis_per_permiso` (`id_perm`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- 6) Asignacion usuario -> perfil por implementacion
-- Usuario relacionado con remit_dest_interno.id_Remit_Dest_Int
CREATE TABLE IF NOT EXISTS `ra_vis_per_usuario_perfil` (
  `id_usuario` int(10) unsigned NOT NULL,
  `id_impl` int(10) unsigned NOT NULL,
  `id_perfil` int(10) unsigned NOT NULL,
  `estado` int(11) NOT NULL DEFAULT '1',
  `fecha_inicio` date DEFAULT NULL,
  `fecha_fin` date DEFAULT NULL,
  PRIMARY KEY (`id_usuario`, `id_impl`),
  KEY `idx_rvpup_perfil` (`id_perfil`),
  CONSTRAINT `fk_rvpup_usuario` FOREIGN KEY (`id_usuario`) REFERENCES `remit_dest_interno` (`id_Remit_Dest_Int`),
  CONSTRAINT `fk_rvpup_impl` FOREIGN KEY (`id_impl`) REFERENCES `ra_vis_per_implementacion` (`id_impl`),
  CONSTRAINT `fk_rvpup_perfil` FOREIGN KEY (`id_perfil`) REFERENCES `ra_vis_per_perfil` (`id_perfil`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- 7) Override por usuario (excepciones sobre el perfil)
CREATE TABLE IF NOT EXISTS `ra_vis_per_usuario_override` (
  `id_usuario` int(10) unsigned NOT NULL,
  `id_impl` int(10) unsigned NOT NULL,
  `id_perm` int(10) unsigned NOT NULL,
  `permitido` int(11) NOT NULL DEFAULT '0',
  `motivo` varchar(255) DEFAULT NULL,
  `estado` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_usuario`, `id_impl`, `id_perm`),
  CONSTRAINT `fk_rvpuo_usuario` FOREIGN KEY (`id_usuario`) REFERENCES `remit_dest_interno` (`id_Remit_Dest_Int`),
  CONSTRAINT `fk_rvpuo_impl` FOREIGN KEY (`id_impl`) REFERENCES `ra_vis_per_implementacion` (`id_impl`),
  CONSTRAINT `fk_rvpuo_perm` FOREIGN KEY (`id_perm`) REFERENCES `ra_vis_per_permiso` (`id_perm`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

