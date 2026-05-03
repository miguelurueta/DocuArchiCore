# SCRUM-171 Pruebas E2E StorageEngine

## Estrategia
- Unitarias de controller para claims + feature flag + traducción HTTP.
- Integración/E2E completa sigue en backlog técnico porque la suite actual tiene fallas preexistentes.

## Cobertura implementada en este ticket
- Claim `defaulalias` faltante -> `400`.
- Claim `usuarioid` inválido -> `400` (`meta.status = validation`).
- Feature flag apagado -> `400` (`meta.status = feature_disabled`).
- UseCase retorna fallo -> `400`.
- UseCase retorna éxito -> `200`.

## Ejecución
- `dotnet build ..\\DocuArchi.Api\\DocuArchi.Api.csproj` OK.
- `dotnet test` del proyecto de pruebas: bloqueado por errores preexistentes en tests no relacionados.

## Riesgo residual
- Falta evidencia automatizada E2E end-to-end del endpoint nuevo por el bloqueo de suite.

