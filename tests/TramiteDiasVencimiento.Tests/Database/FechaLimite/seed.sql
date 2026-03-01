INSERT INTO system_plantilla_radicado (
    id_Plantilla,
    Nombre_Plantilla_Radicado,
    Tipo_Plantilla,
    util_default_radicado
) VALUES
    (100, 'Plantilla Default', 'GEN', 1);

INSERT INTO tipo_doc_entrante (
    id_Tipo_Doc_Entrante,
    system_plantilla_radicado_id_plantilla,
    numero_dias_vence
) VALUES
    (200, 100, 5);
