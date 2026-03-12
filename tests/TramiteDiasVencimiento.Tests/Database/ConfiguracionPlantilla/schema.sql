DROP TABLE IF EXISTS ra_rad_config_plantilla_radicacion;

CREATE TABLE ra_rad_config_plantilla_radicacion (
    id_config_plantilla_radicacion INT NOT NULL PRIMARY KEY,
    system_plantilla_radicado_id_Plantilla INT NOT NULL,
    Tipo_radicacion_plantilla INT NOT NULL,
    requiere_respuesta INT NULL,
    util_tipo_modulo_envio INT NULL,
    estado INT NULL
);
