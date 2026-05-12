## 1. Discovery

- [x] 1.1 Confirmar con evidencia de logs que el error actual proviene de resolución por `TRD.IdTipoDocumento` en fase física.
- [x] 1.2 Documentar fuente de verdad legacy para clasificación técnica (`DA_EXTENSION` por `ESTENSION`).
- [x] 1.3 Confirmar lista de campos base obligatorios de `contabil` (`ID, DISC, PAG, DBT, IDEX, USER, DATE1, TIME1`).

## 2. Specs

- [x] 2.1 Definir requisito explícito: clasificación técnica por `DA_EXTENSION` y no por TRD.
- [x] 2.2 Definir requisito explícito: `DBT` deriva de `DA_EXTENSION.ESTADO_NORMAL`.
- [x] 2.3 Definir requisito de validación temprana sin commit cuando no hay mapeo de extensión.

## 3. Application

- [x] 3.1 Ajustar consulta de extensión técnica a `DA_EXTENSION` por `ESTENSION` normalizada.
- [x] 3.2 Propagar resultado técnico (`ESTADO_NORMAL`, `ESTADO_ADJUNTO`, `ESTADO_LINK`) al flujo de almacenamiento.
- [x] 3.3 Corregir origen de `DBT` en modelo/insert de gabinete para usar `ESTADO_NORMAL`.
- [x] 3.4 Incorporar fallo funcional temprano (pre-transacción) cuando no exista mapeo en `DA_EXTENSION`.
- [x] 3.5 Verificar que `TIME1` se puebla siempre en insert de gabinete.

## 4. Test

- [x] 4.1 Unit test: normalización de extensión (`pdf`, `.PDF`, etc.).
- [x] 4.2 Unit test: lookup exitoso por `ESTENSION` y error cuando no existe mapeo.
- [x] 4.3 Unit/integ test: `DBT` en gabinete usa `ESTADO_NORMAL` y no `TRD.IdTipoDocumento`.
- [x] 4.4 Unit/integ test: fallo temprano sin commit cuando falta `DA_EXTENSION`.
- [x] 4.5 Ejecutar pruebas de storage afectadas y registrar evidencia.

## 5. Documentation and OpenSpec

- [x] 5.1 Actualizar documentación StorageEngine con separación TRD vs clasificación técnica.
- [x] 5.2 Incluir tabla de origen de campos base de `contabil`, incluyendo `DBT` y `TIME1`.
- [x] 5.3 Ejecutar `openspec validate scrum-199-correcion-da-extension-almacenamiento`.
