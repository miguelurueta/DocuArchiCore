DROP TABLE IF EXISTS ra_plantilla_100;
DROP TABLE IF EXISTS system_plantilla_radicado;

CREATE TABLE system_plantilla_radicado (
  id_Plantilla INT NOT NULL PRIMARY KEY,
  Nombre_Plantilla_Radicado VARCHAR(100) NOT NULL
);

CREATE TABLE ra_plantilla_100 (
  id_radicado INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  CampoIdentificador VARCHAR(50) NOT NULL,
  Asunto VARCHAR(240) NOT NULL
);
