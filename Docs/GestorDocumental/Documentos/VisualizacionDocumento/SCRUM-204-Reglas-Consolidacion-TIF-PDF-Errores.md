# SCRUM-204 Reglas de Consolidación TIF->PDF Temporal y Errores

## 1. Regla funcional principal
- El frontend siempre recibe una sola URL temporal para descarga/visualización.
- Si el origen físico es PDF o imagen única soportada, se publica el original.
- Si el origen físico es TIF/TIFF (incluyendo multiimagen), se consolida a PDF temporal antes de exponer URL.

## 2. Detección de origen
- `ORIGINAL`: archivo final disponible sin consolidación.
- `TIF_CONSOLIDADO`: archivo final generado temporalmente desde TIF/TIFF.

## 3. Contrato de salida esperado (resolve)
- `ContentType`: `application/pdf` para consolidado TIF->PDF.
- `FileName`: nombre final del recurso expuesto al frontend.
- `UrlTemporal`: URL tokenizada de descarga.
- `ExpiresAt`: expiración del token temporal.
- `Origen`: `ORIGINAL` o `TIF_CONSOLIDADO`.

## 4. Política de seguridad y acceso
- La URL temporal nunca expone rutas físicas absolutas.
- El token temporal debe estar ligado a `usuarioid`.
- La descarga debe validar token existente, vigente y asociado al usuario autenticado.
- Token inválido, vencido o no asociado al usuario debe devolver `404` en `download`.

## 5. Estrategia de temporales y cleanup
- Los PDF consolidados se generan bajo root temporal controlado.
- La vigencia operativa se alinea al TTL del token temporal (15 minutos).
- El archivo temporal debe considerarse efímero: no hace parte de persistencia funcional del documento.
- La limpieza se realiza por política de expiración/rotación de temporales del entorno de ejecución.

## 6. Errores y comportamiento esperado
- `400 Validation`:
  - claims faltantes/inválidos (`defaulalias`, `usuarioid`)
  - request inválido (`nombreGabinete`, `idDocumento`)
  - documento/ruta física no resoluble
- `404`:
  - token inexistente, vencido o no autorizado para el usuario actual
- `500`:
  - falla no controlada en resolución o en proceso de consolidación

## 7. Observabilidad mínima recomendada
- Registrar inicio/fin de `resolve` con `requestId`.
- Registrar decisión de origen (`ORIGINAL` o `TIF_CONSOLIDADO`).
- Registrar fallos de consolidación TIF->PDF.
- Registrar fallos de validación/descarga por token temporal.
