INSERT INTO ra_rad_config_plantilla_radicacion
(
    system_plantilla_radicado_id_Plantilla,
    Tipo_radicacion_plantilla,
    Descripcion_tipo_radicacion,
    util_notificacion_remitente,
    util_notificacion_destinatario,
    util_valida_restriccion_radicacion
)
VALUES
(67, 1, 'Externa', 1, 0, 1),
(67, 2, 'Interna', 0, 1, 0),
(70, 1, 'Externa', 1, 1, 1);
