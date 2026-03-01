DROP TABLE IF EXISTS tipo_doc_entrante;
DROP TABLE IF EXISTS rea_001_feriados;
DROP TABLE IF EXISTS system_plantilla_radicado;

CREATE TABLE system_plantilla_radicado (
    id_Plantilla INT NOT NULL PRIMARY KEY,
    Nombre_Plantilla_Radicado VARCHAR(45) NOT NULL,
    Tipo_Plantilla VARCHAR(45) NOT NULL,
    util_default_radicado INT NOT NULL
);

CREATE TABLE tipo_doc_entrante (
    id_Tipo_Doc_Entrante INT NOT NULL,
    system_plantilla_radicado_id_plantilla INT NOT NULL,
    numero_dias_vence INT NULL,
    PRIMARY KEY (id_Tipo_Doc_Entrante, system_plantilla_radicado_id_plantilla)
);

CREATE TABLE rea_001_feriados (
    ID_FERIADO INT AUTO_INCREMENT PRIMARY KEY,
    FECHA_FERIADO DATE NOT NULL,
    ESTADO_DIA INT NOT NULL
);
