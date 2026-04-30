CREATE TABLE IF NOT EXISTS tipo_doc_entrante (
    id_Tipo_Doc_Entrante INT NOT NULL,
    system_plantilla_radicado_id_plantilla INT NOT NULL,
    numero_dias_vence INT NULL,
    util_envio_correo_certificado INT NOT NULL DEFAULT 0,
    util_firma_digital_protocolo_respuesta INT NOT NULL DEFAULT 0,
    util_agrega_digital_protocolo_respuesta INT NOT NULL DEFAULT 0,
    PRIMARY KEY (id_Tipo_Doc_Entrante, system_plantilla_radicado_id_plantilla)
);
