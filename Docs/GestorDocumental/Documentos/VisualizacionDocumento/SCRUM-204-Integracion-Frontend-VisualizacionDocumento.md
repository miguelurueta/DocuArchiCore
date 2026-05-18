# SCRUM-204 Integración Frontend Visualizacion Documento

## 1. Objetivo Frontend
Consumir un flujo de 2 llamadas para visualizar documento:
1) resolver URL temporal
2) descargar binario

## 2. Seguridad
- Header obligatorio: `Authorization: Bearer {jwt}`
- Claims mínimos en JWT:
  - `defaulalias`
  - `usuarioid`

## 3. APIs del módulo
- `POST /api/gestor-documental/documentos/visualizacion/resolve`
- `GET /api/gestor-documental/documentos/visualizacion/download/{token}`

## 4. Contratos por API
### 4.1 Resolve
Request:
```json
{
  "NombreGabinete": "contabil",
  "IdDocumento": 15416
}
```

Response OK:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "IdDocumento": 15416,
    "NombreGabinete": "contabil",
    "FileName": "DIG00015416_tif2pdf_4f9a....pdf",
    "ContentType": "application/pdf",
    "Origen": "TIF_TO_PDF",
    "UrlTemporal": "/api/gestor-documental/documentos/visualizacion/download/2b9c...",
    "UrlTemporalAbsoluta": "https://localhost:7101/api/gestor-documental/documentos/visualizacion/download/2b9c...",
    "ExpiresAt": "2026-05-17T20:40:00Z"
  },
  "meta": { "Status": "success" },
  "errors": []
}
```

### 4.2 Download
- Método: `GET`
- Ruta: `/api/gestor-documental/documentos/visualizacion/download/{token}`
- Respuesta: binario (`application/pdf` en TIF convertido, o content-type original).

## 5. Paso a paso de consumo frontend
1. Invocar `resolve` con JWT.
2. Leer `data.UrlTemporal` o `data.UrlTemporalAbsoluta` (recomendado para visor embebido).
3. Invocar `download/{token}` con el mismo JWT.
4. Renderizar blob en visor PDF del frontend.
5. Si falla por expiración (`404`), repetir `resolve`.

## 6. Errores esperados
- `400 Validation`: claims inválidos, documento no existe, ruta física inconsistente.
- `404`: token vencido/inexistente/no asociado al usuario.
- `500`: error técnico no controlado.

## 7. Checklist de depuración frontend
- JWT vigente con `defaulalias` y `usuarioid`.
- `NombreGabinete` exacto.
- `IdDocumento` válido.
- Ejecutar `download` antes de `ExpiresAt`.
- Ante `404`, renovar con nuevo `resolve`.
