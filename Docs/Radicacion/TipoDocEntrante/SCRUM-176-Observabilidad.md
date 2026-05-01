# SCRUM-176 - Observabilidad

## Campos recomendados en logging
- `requestId`
- `alias`
- `idTipoDocEntrante`
- `success`
- `duracionMs`

## Eventos clave
- Inicio de request del endpoint.
- Resultado de validación de claim.
- Resultado de consulta de estructura.
- Error de ejecución (si ocurre).

## Troubleshooting
- Error claim: revisar `defaulalias`.
- Error de consulta: revisar conectividad BD/alias.
- Error validación: revisar `idTipoDocEntrante` > 0.
