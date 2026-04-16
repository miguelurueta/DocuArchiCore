# SCRUM-141 — Arquitectura — Guardar imagen (Tiptap)

## Flujo

Controller → Service → Repository → `IDapperCrudEngine`

## Decisiones

- Validación de claim `defaulalias` con `IClaimValidationService`.
- Validación de tipo: `image/png|jpeg|jpg|webp`
- Límite de tamaño: 5MB (constante en service; ajustable luego).
- Storage: `storage_type_code = "db"` y persistencia del binario en `image_bytes`.

## Deuda técnica

- Falta endpoint para servir la imagen (public_url) si se requiere consumo directo por el editor.
- Falta antivirus / scanning (fuera de alcance).

