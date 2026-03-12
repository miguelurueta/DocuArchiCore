DROP TABLE IF EXISTS ra_rad_config_plantilla_radicacion;

CREATE TABLE ra_rad_config_plantilla_radicacion (
    id_rad_config_plantilla_radicacion INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    system_plantilla_radicado_id_Plantilla INT NOT NULL,
    Tipo_radicacion_plantilla INT NOT NULL,
    Descripcion_tipo_radicacion VARCHAR(40) NULL,
    util_notificacion_remitente INT NOT NULL DEFAULT 0,
    util_notificacion_destinatario INT NOT NULL DEFAULT 0,
    util_valida_restriccion_radicacion INT NOT NULL
);
