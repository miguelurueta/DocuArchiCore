DROP TABLE IF EXISTS ra_test_radicado;
DROP TABLE IF EXISTS system_plantilla_radicado;

CREATE TABLE system_plantilla_radicado (
    id_Plantilla INT NOT NULL PRIMARY KEY,
    Nombre_Plantilla_Radicado VARCHAR(128) NOT NULL,
    util_default_radicado TINYINT NOT NULL
);

CREATE TABLE ra_test_radicado (
    id_registro INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    consecutivo_rad VARCHAR(64) NOT NULL
);
