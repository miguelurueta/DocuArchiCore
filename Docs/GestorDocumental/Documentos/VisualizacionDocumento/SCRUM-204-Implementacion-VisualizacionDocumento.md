# SCRUM-204 Implementación Técnica Visualizacion Documento

## 1. Resumen de Implementación
Se implementó una API dedicada de visualización documental con URL temporal única y descarga tokenizada. La conversión TIF->PDF quedó desacoplada como servicio técnico interno (`PdfTools`).

## 2. Endpoints
- `POST /api/gestor-documental/documentos/visualizacion/resolve`
- `GET /api/gestor-documental/documentos/visualizacion/download/{token}`

## 3. Artefactos Implementados
### 3.1 API
- `DocuArchi.Api/Controllers/GestorDocumental/Documentos/VisualizacionDocumentoController.cs`

### 3.2 Services
- `MiApp.Services/Service/GestorDocumental/Documentos/VisualizacionDocumento/IVisualizacionDocumentoService.cs`
- `MiApp.Services/Service/GestorDocumental/Documentos/PdfTools/ITifToPdfConverterService.cs`

### 3.3 DTOs
- `MiApp.DTOs/DTOs/GestorDocumental/Documentos/VisualizacionDocumento/VisualizacionDocumentoRequestDto.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/Documentos/VisualizacionDocumento/VisualizacionDocumentoResponseDto.cs`

### 3.4 Reutilización
- `IReemplazoPdfDocumentLocationRepository`
- `IStorageRouteRepository`
- `IStorageFolderLegacyPolicy`
- `IStoragePathResolver`
- `IStorageUploadPathResolver`

### 3.5 DI
- Registro de `IVisualizacionDocumentoService` y `ITifToPdfConverterService` en `DocuArchi.Api/Program.cs`.

## 4. Reglas funcionales implementadas
- Contrato de visualización siempre retorna un solo recurso final.
- Si origen es PDF/imagen simple: retorna archivo original tokenizado.
- Si origen es TIF/TIFF: genera PDF temporal y retorna ese PDF tokenizado.
- Token temporal con TTL 15 minutos, ligado a `usuarioid`.

## 5. Manejo de Excepciones
- `try/catch` en controller y servicios nuevos.
- `AppResponses` uniforme para errores de validación y error técnico.
- `download` retorna `404` cuando token no existe/expiró/no corresponde al usuario.

## 6. Casos de error cubiertos
- Claim `defaulalias` ausente/invalid.
- Claim `usuarioid` ausente/invalid.
- `NombreGabinete` vacío.
- `IdDocumento <= 0`.
- Documento no existe en gabinete.
- Carpeta/ruta física no existe.
- Conversión TIF->PDF falla.
- Token inválido o vencido.

## 7. Resultado esperado
- Frontend consume `resolve` y luego `download` con el mismo JWT.
- No se exponen rutas físicas absolutas en contrato.
- Servicio `PdfTools` queda reutilizable para futuras capacidades (split/merge/PDF->TIF) sin cambiar contrato público.