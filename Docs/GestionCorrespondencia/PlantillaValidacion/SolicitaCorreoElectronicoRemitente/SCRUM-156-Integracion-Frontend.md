# SCRUM-156 — Integración Frontend

## Endpoint

- Método: `GET`
- Ruta: `/api/GestionCorrespondencia/PlantillaValidacion/solicita-correo-electronico-remitente`
- Query params:
  - `idPlantillaRadicado` (long, > 0)
  - `idDestinatarioExterno` (long, > 0)
- Claim requerido: `defaulalias`

## Response

Tipo: `AppResponses<string>`

- `success=true` + `data != ""` → correo encontrado (`meta.status="success"`)
- `success=true` + `data == ""` → sin configuración o sin valor (`meta.status="empty"`)
- `success=false` → error controlado (`meta.status="error"`)

## Recomendación UX

- Si `meta.status == "empty"`: mostrar estado vacío sin mensaje técnico.
- Si `success == false`: mostrar mensaje general (sin detalles SQL).

