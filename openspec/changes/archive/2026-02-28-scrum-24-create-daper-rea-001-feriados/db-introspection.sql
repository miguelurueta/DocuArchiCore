-- SCRUM-24 - Introspeccion requerida para desbloquear modelo rea_001_feriados
-- Ejecutar en MySQL con acceso a la BD docuarchi

SHOW CREATE TABLE docuarchi.rea_001_feriados;

SELECT
  COLUMN_NAME,
  COLUMN_TYPE,
  IS_NULLABLE,
  COLUMN_DEFAULT,
  COLUMN_KEY,
  EXTRA
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'docuarchi'
  AND TABLE_NAME = 'rea_001_feriados'
ORDER BY ORDINAL_POSITION;
