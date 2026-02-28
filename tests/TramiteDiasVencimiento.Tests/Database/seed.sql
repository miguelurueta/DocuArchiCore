DELETE FROM tipo_doc_entrante;

INSERT INTO tipo_doc_entrante (
    id_Tipo_Doc_Entrante,
    system_plantilla_radicado_id_plantilla,
    numero_dias_vence
) VALUES
    (200, 100, 12),
    (201, 100, 3),
    (200, 101, 7);
