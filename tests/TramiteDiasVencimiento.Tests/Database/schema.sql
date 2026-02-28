CREATE TABLE IF NOT EXISTS tipo_doc_entrante (
    id_Tipo_Doc_Entrante INT NOT NULL,
    system_plantilla_radicado_id_plantilla INT NOT NULL,
    numero_dias_vence INT NULL,
    PRIMARY KEY (id_Tipo_Doc_Entrante, system_plantilla_radicado_id_plantilla)
);
