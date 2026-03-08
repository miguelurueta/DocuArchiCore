# SCRUM-43 - SolicitaEstructuraTipoDoEntrante

## Objetivo
Agregar en `TipoDocEntranteR` la función `SolicitaEstructuraTipoDoEntrante` para consultar la estructura completa de `tipo_doc_entrante` por identificador.

## Destino
- Repositorio:
  - `MiApp.Repository/Repositorio/Radicador/Tramite/TipoDocEntranteR.cs`
- Metodo:
  - `SolicitaEstructuraTipoDoEntrante(int idTipoDocEntrante, string defaultDbAlias)`
- Retorno:
  - `AppResponses<TipoDocEntrante>`

## Comportamiento
- Consulta tabla `tipo_doc_entrante`.
- Filtro: `id_Tipo_Doc_Entrante = @idTipoDocEntrante`.
- `Limit = 1`.
- Si existe registro:
  - `success = true`
  - `message = "YES"`
  - `data` con objeto `TipoDocEntrante`
- Si no existe:
  - `success = false`
  - `message = "Imposible encontrar estructura tipo documento entrante (...)"`
  - `errors` con `Field = "idTipoDocEntrante"`
- Excepción:
  - `success = false`
  - `message = "Inconsistencia funcion SolicitaEstructuraTipoDoEntrante ..."`

## Pruebas
- Archivo: `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/TipoDocEntranteRepositoryTests.cs`
- Casos incluidos:
  - Retorna `YES` cuando hay datos.
  - Retorna error cuando no hay filas.
  - Verifica `QueryOptions` (tabla, filtro, alias y limit).
