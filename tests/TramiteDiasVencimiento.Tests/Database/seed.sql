DELETE FROM tipo_doc_entrante;

INSERT INTO tipo_doc_entrante (
    id_Tipo_Doc_Entrante,
    system_plantilla_radicado_id_plantilla,
    numero_dias_vence,
    util_envio_correo_certificado,
    util_firma_digital_protocolo_respuesta,
    util_agrega_digital_protocolo_respuesta
) VALUES
    (200, 100, 12, 0, 0, 0),
    (201, 100, 3, 1, 0, 0),
    (200, 101, 7, 1, 1, 1);
