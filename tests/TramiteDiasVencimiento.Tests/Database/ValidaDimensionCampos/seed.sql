INSERT INTO system_plantilla_radicado (id_Plantilla, Nombre_Plantilla_Radicado)
VALUES (100, 'ra_plantilla_100');

INSERT INTO detalle_plantilla_radicado
(
  System_Plantilla_Radicado_id_Plantilla,
  Campo_Plantilla,
  Tipo_Campo,
  Comportamiento_Campo,
  Alias_Campo,
  Orden_Campo,
  Estado_Campo,
  Descripcion_Campo,
  Campo_Obligatorio,
  Campo_rad_interno,
  Campo_rad_externo,
  Campo_rad_simple,
  tam_campo,
  id_detalle_plantilla_radicado,
  TagSesion
)
VALUES
(100, 'CampoDinamico', 'VARCHAR', 'DIGITACION', 'CampoDinamico', 1, 1, 'Campo dinamico', 0, 1, 1, 1, 10, 1, 'TEST');
