# SCRUM-198 - Metadata

- Ticket Jira: `SCRUM-198`
- Cambio OpenSpec: `openspec/changes/scrum-198-implementacion-correcion-dbnull-almacena`
- Objetivo: corregir uso de `DBNull.Value` en inserts transaccionales de Storage Engine
- Tipo de cambio: corrección quirúrgica sin cambio funcional de negocio
- Repos impactados:
  - `MiApp.Repository`
  - `DocuArchiCore` (tests y documentación)

## Componentes tocados

1. `InventarioDocumentalRepository` (payload de inserción)
2. `DapperCrudEngine.InsertBeginTrandAsync` (mensaje de error transaccional)
3. Helpers `AddColumn` en repositorios de Storage (gabinete, índice electrónico, workflow, compensación)
4. `InventarioDocumentalRepositoryTests` (assertions de null/ausencia de DBNull)

## Política técnica establecida

- En Storage Engine, usar `null` de C# para ausencia de valor en parámetros.
- No usar `DBNull.Value` dentro de builders/repos/services del módulo.

## Inventario de revisión global

Patrones revisados:
- `DBNull.Value`
- `System.DBNull`
- `Convert.DBNull`

Cobertura de revisión:
1. `MiApp.Repository` (`*.cs`)
2. `MiApp.Services` (`*.cs`)
3. `MiApp.Models` (`*.cs`)

Hallazgos:
1. Storage Engine (`Repositorio/GestorDocumental/AlmacenamientoDocumental`): sin usos de `DBNull` luego del ajuste.
2. `MiApp.Models`: sin usos de `DBNull`.
3. `MiApp.Services`: 4 usos detectados en `Workflow/BandejaCorrespondencia` (fuera de alcance del ticket).
