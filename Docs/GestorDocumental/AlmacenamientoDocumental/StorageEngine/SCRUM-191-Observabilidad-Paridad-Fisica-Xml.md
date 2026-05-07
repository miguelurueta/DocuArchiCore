# SCRUM-191 - Observabilidad Paridad Fisica y XML

## Campos de Trazabilidad Obligatorios
- `requestId`
- `nombreGabinete`
- `idAlmacen`
- `disco`
- `carpeta`
- `rutaFinal`
- `nombrePrincipal`
- `nombreXml`
- `duracionMs`
- `estado` (OK/ERROR)

## Eventos Recomendados
1. `StoragePhysicalPathResolved`
2. `StorageFileCopyStarted`
3. `StorageFileCopyCompleted`
4. `StorageXmlWriteStarted`
5. `StorageXmlWriteCompleted`
6. `StorageIndiceXmlUpdateCompleted`
7. `StorageCompensationStarted`
8. `StorageCompensationCompleted`

## Politica de Datos Sensibles
- No registrar contenido documental.
- No registrar fulltext de indexacion.
- No registrar payloads completos de request.

## Uso Operativo
- Diagnostico de desviaciones de paridad VB/C#.
- Identificacion de fallas en fase fisica post-commit.
- Soporte a auditoria funcional por ticket (`SCRUM-191`).
