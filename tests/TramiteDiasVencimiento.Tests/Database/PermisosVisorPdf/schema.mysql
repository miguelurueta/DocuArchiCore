CREATE TABLE IF NOT EXISTS `remit_dest_interno` (
  `id_Remit_Dest_Int` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Login_Usuario` varchar(120) NOT NULL,
  PRIMARY KEY (`id_Remit_Dest_Int`),
  UNIQUE KEY `uk_login_usuario` (`Login_Usuario`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `ra_vis_per_implementacion` (
  `id_impl` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `codigo_impl` varchar(80) NOT NULL,
  `nombre_impl` varchar(120) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  `estado` int(11) NOT NULL DEFAULT '1',
  `id_empresa` int(11) NOT NULL DEFAULT '0',
  `fecha_creacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_impl`),
  UNIQUE KEY `uk_rvpi_codigo_empresa` (`codigo_impl`, `id_empresa`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `ra_vis_per_permiso` (
  `id_perm` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `codigo_perm` varchar(120) NOT NULL,
  `recurso` varchar(60) NOT NULL DEFAULT 'pdf',
  `accion` varchar(60) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  `estado` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_perm`),
  UNIQUE KEY `uk_rvpp_codigo` (`codigo_perm`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `ra_vis_per_impl_perm_default` (
  `id_impl` int(10) unsigned NOT NULL,
  `id_perm` int(10) unsigned NOT NULL,
  `permitido` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id_impl`, `id_perm`),
  CONSTRAINT `fk_rvpd_impl` FOREIGN KEY (`id_impl`) REFERENCES `ra_vis_per_implementacion` (`id_impl`),
  CONSTRAINT `fk_rvpd_perm` FOREIGN KEY (`id_perm`) REFERENCES `ra_vis_per_permiso` (`id_perm`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `ra_vis_per_perfil` (
  `id_perfil` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `id_impl` int(10) unsigned NOT NULL,
  `codigo_perfil` varchar(80) NOT NULL,
  `nombre_perfil` varchar(120) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  `estado` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_perfil`),
  UNIQUE KEY `uk_rvpf_impl_codigo` (`id_impl`, `codigo_perfil`),
  CONSTRAINT `fk_rvpf_impl` FOREIGN KEY (`id_impl`) REFERENCES `ra_vis_per_implementacion` (`id_impl`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `ra_vis_per_perfil_permiso` (
  `id_perfil` int(10) unsigned NOT NULL,
  `id_perm` int(10) unsigned NOT NULL,
  `permitido` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id_perfil`, `id_perm`),
  CONSTRAINT `fk_rvppf_perfil` FOREIGN KEY (`id_perfil`) REFERENCES `ra_vis_per_perfil` (`id_perfil`),
  CONSTRAINT `fk_rvppf_perm` FOREIGN KEY (`id_perm`) REFERENCES `ra_vis_per_permiso` (`id_perm`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `ra_vis_per_usuario_perfil` (
  `id_usuario` int(10) unsigned NOT NULL,
  `id_impl` int(10) unsigned NOT NULL,
  `id_perfil` int(10) unsigned NOT NULL,
  `estado` int(11) NOT NULL DEFAULT '1',
  `fecha_inicio` date DEFAULT NULL,
  `fecha_fin` date DEFAULT NULL,
  PRIMARY KEY (`id_usuario`, `id_impl`),
  CONSTRAINT `fk_rvpup_usuario` FOREIGN KEY (`id_usuario`) REFERENCES `remit_dest_interno` (`id_Remit_Dest_Int`),
  CONSTRAINT `fk_rvpup_impl` FOREIGN KEY (`id_impl`) REFERENCES `ra_vis_per_implementacion` (`id_impl`),
  CONSTRAINT `fk_rvpup_perfil` FOREIGN KEY (`id_perfil`) REFERENCES `ra_vis_per_perfil` (`id_perfil`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

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
