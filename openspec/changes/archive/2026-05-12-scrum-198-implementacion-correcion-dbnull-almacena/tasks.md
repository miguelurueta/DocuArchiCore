## 1. Discovery

- [x] 1.1 Reproducir el error de inserción transaccional y confirmar stack trace (`SERIE_DOCUMENTO` + `System.DBNull`).
- [x] 1.2 Ubicar exactamente dónde se construye el payload de inserción en inventario (diccionario/reglas).
- [x] 1.3 Ejecutar búsqueda dirigida de `DBNull.Value`, `System.DBNull`, `Convert.DBNull` en Storage Engine y clasificar usos válidos/no válidos.

## 2. Implementation

- [x] 2.1 Reemplazar `DBNull.Value` por `null` en campos opcionales del insert transaccional de inventario.
- [x] 2.2 Verificar que `InsertBeginTrandAsync` preserve semántica de `null` sin requerir conversión manual.
- [x] 2.3 Ajustar mensaje de error transaccional para diagnóstico más claro (contexto transaccional/método).
- [x] 2.4 Mantener sin cambios contratos HTTP ni reglas de negocio del `ValidationPipeline`.

## 3. Tests

- [x] 3.1 Actualizar/agregar tests de repositorio para `SERIE_DOCUMENTO`, `SUBSERIE_DOCUMENTO`, `ID_SERIE_DOCUMENTO`, `ID_SUBSERIE_DOCUMENTO` en `null`.
- [x] 3.2 Ejecutar suite de pruebas del flujo de almacenamiento afectado y registrar resultados.
- [x] 3.3 Confirmar regresión cero para escenarios existentes de inserción válida.

## 4. Documentation and OpenSpec

- [x] 4.1 Actualizar documentación técnica del StorageEngine con política `null vs DBNull`.
- [x] 4.2 Completar evidencia de cambios y pruebas en artefactos del ticket.
- [x] 4.3 Ejecutar `openspec validate scrum-198-implementacion-correcion-dbnull-almacena` y dejar resultado en el flujo.
