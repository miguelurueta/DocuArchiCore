# SCRUM-154 — Pruebas: Resolve Editor Document

## Unitarias (Controller)

Archivo: `tests/TramiteDiasVencimiento.Tests/ResolveEditorDocumentControllerTests.cs`

Casos cubiertos:

- Claim `defaulalias` inválido → `400 BadRequest`
- Parámetros inválidos (`contextCode` vacío, `entityId` inválido) → `400 BadRequest`
- `prefer` inválido → `400 BadRequest`
- Service retorna error tipo `Conflict` → `409 Conflict`
- Service retorna `success=true` → `200 OK`

## Unitarias (Service)

Pendiente de evidencia en suite actual:

- `prefer=existing` + doc existe → `Mode=existing`
- doc no existe → `Mode=initial` exige `idTareaWf`
- `prefer=initial` + doc existe + contexto no permite múltiples → `409 Conflict`
- múltiples documentos → `409 Conflict`
- `contextCode` inválido/inactivo → `400 BadRequest` (Validation)

## Integración / QT

Recomendado ejecutar (manual o automatizado):

1. Llamar `GET /api/gestor-documental/editor/document/resolve` con `prefer=existing` y validar:
   - si existe doc: `Mode=existing` + `DocumentId` informado.
2. Sin doc existente:
   - retorna `Mode=initial` con `Html` renderizable.
3. Forzar `prefer=initial`:
   - cuando no permite múltiples: `409 Conflict`.
4. Simular “múltiples documentos”:
   - retorna `409 Conflict` con `AppError.Type=Conflict`.

## Fecha de referencia

- Documentación generada: 20/04/2026.

