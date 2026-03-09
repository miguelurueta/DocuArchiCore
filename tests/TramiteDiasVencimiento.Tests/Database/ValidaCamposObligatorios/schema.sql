DROP TABLE IF EXISTS detalle_plantilla_radicado;

CREATE TABLE detalle_plantilla_radicado (
  System_Plantilla_Radicado_id_Plantilla INT NOT NULL,
  Campo_Plantilla VARCHAR(45) NOT NULL,
  Tipo_Campo VARCHAR(100) NOT NULL,
  Comportamiento_Campo VARCHAR(45) NOT NULL,
  Alias_Campo VARCHAR(45) NOT NULL,
  Orden_Campo INT NOT NULL,
  Estado_Campo INT NULL,
  Descripcion_Campo VARCHAR(100) NOT NULL,
  Campo_Obligatorio INT NOT NULL,
  Campo_rad_interno INT NOT NULL,
  Campo_rad_externo INT NOT NULL,
  Campo_rad_simple INT NOT NULL,
  tam_campo INT NOT NULL,
  id_detalle_plantilla_radicado INT NOT NULL PRIMARY KEY,
  TagSesion VARCHAR(30) NOT NULL
);
