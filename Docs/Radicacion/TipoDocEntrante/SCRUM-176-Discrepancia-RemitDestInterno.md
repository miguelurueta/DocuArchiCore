# SCRUM-176 - Discrepancia RemitDestInterno

## Contexto
En el levantamiento inicial se mencionó `RemitDestInterno.cs` como posible modelo objetivo.

## Resolución
- `RemitDestInterno` no corresponde al alcance de esta API.
- El origen correcto es `tipo_doc_entrante`.
- El modelo correcto es:
  - `MiApp.Models/Models/Radicacion/TipoTramite/TipoDocEntrante.cs`

## Impacto
La implementación final de SCRUM-176 se construyó sobre el modelo correcto y expone la estructura completa de `TipoDocEntrante`.
