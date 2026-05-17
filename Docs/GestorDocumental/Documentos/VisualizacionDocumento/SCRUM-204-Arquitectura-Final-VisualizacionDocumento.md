# SCRUM-204 Arquitectura Final Visualizacion Documento

## 1. Objetivo
Definir la arquitectura final del caso de uso de visualización documental con URL temporal única para frontend, reutilizando la localización física legacy y usando conversión TIF->PDF solo como servicio técnico interno.

## 2. Alcance
- Resolver documento físico de gabinete (`DIG########.*`) y devolver URL temporal de descarga.
- Mantener contrato único para frontend (un archivo por solicitud).
- Cuando origen es TIF/TIFF, consolidar a PDF temporal mediante servicio interno.
- Proteger descarga con token temporal ligado a `usuarioid`.

Fuera de alcance:
- Reemplazo físico de archivo (lo cubre SCRUM-202).
- Versionamiento documental legacy.
- Reescritura de índice XML de expediente.

## 3. Contexto Arquitectónico
`Controller -> Service (UseCase) -> Repositorios de ubicación/ruta -> FileSystem -> Conversor interno TIF->PDF`

### 3.1 Separación por módulos
- API: `DocuArchi.Api/Controllers/GestorDocumental/Documentos/VisualizacionDocumentoController.cs`
- Servicio caso de uso: `MiApp.Services/Service/GestorDocumental/Documentos/VisualizacionDocumento/IVisualizacionDocumentoService.cs`
- Servicio técnico interno: `MiApp.Services/Service/GestorDocumental/Documentos/PdfTools/ITifToPdfConverterService.cs`
- Repositorio ubicación: `MiApp.Repository/Repositorio/GestorDocumental/Documentos/ReemplazoPdf/IReemplazoPdfDocumentLocationRepository.cs`
- Repositorio ruta gabinete: `MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/StorageRoute/IStorageRouteRepository.cs`

## 4. Contrato API
- Resolve: `POST /api/gestor-documental/documentos/visualizacion/resolve`
- Download: `GET /api/gestor-documental/documentos/visualizacion/download/{token}`
- Seguridad: JWT Bearer
- Claims requeridos:
  - `defaulalias`
  - `usuarioid`

### 4.1 Contrato frontend completo por endpoint
El contrato con ejemplos y paso a paso operativo se documenta en:
- `Docs/GestorDocumental/Documentos/VisualizacionDocumento/SCRUM-204-Integracion-Frontend-VisualizacionDocumento.md`

## 5. Flujo End-to-End
1. Controller valida claims y `request`.
2. Service valida `NombreGabinete` e `IdDocumento`.
3. Service consulta ubicación física (`DISC`, `IDEX`, `PAG`, `DBT`) del registro.
4. Service resuelve ruta final con política legacy (`ruta_gabi + gabinete + disco + carpeta`).
5. Service localiza archivo `DIG{id:D8}.*` y derivados.
6. Si origen es TIF/TIFF, invoca `ITifToPdfConverterService` para generar PDF temporal.
7. Service emite token temporal (TTL 15 min) en memoria, ligado a `usuarioid`.
8. Front descarga con endpoint `download/{token}` usando mismo JWT.

## 6. Validaciones
### 6.1 Seguridad
- Claim `defaulalias` obligatorio.
- Claim `usuarioid` obligatorio, numérico y > 0.

### 6.2 Funcionales
- `NombreGabinete` requerido.
- `IdDocumento` > 0.
- Registro en gabinete debe existir.
- Ruta física final debe existir.
- Archivo físico `DIG########.*` debe existir.
- Token temporal debe existir, pertenecer al usuario y no estar vencido.

### 6.3 Integridad de rutas
- Resolución de rutas con `IStoragePathResolver` (anti traversal).
- Conversión temporal controlada bajo `StorageUploadPathResolver.GetTempRoot()`.

## 7. Manejo de Errores
- `400 Validation`: claims/entrada inválida, documento no encontrado, ruta inexistente.
- `404`: token inexistente o expirado en download.
- `500`: fallo no controlado.
- Todas las funciones nuevas quedaron con `try/catch`.

## 8. Observabilidad
Logs mínimos:
- Inicio/fin de resolución por `requestId`.
- Decisión de origen (`ORIGINAL` vs `TIF_TO_PDF`).
- Error de conversión TIF->PDF.
- Error de token/descarga.

## 9. Riesgos y Mitigaciones
- Riesgo: exposición de rutas físicas.
  - Mitigación: API retorna solo URL tokenizada.
- Riesgo: consumo alto en TIF multipágina.
  - Mitigación: conversión temporal bajo demanda, no persistente.
- Riesgo: token compartido entre usuarios.
  - Mitigación: token ligado a `usuarioid` autenticado.

## 10. Depuración Guiada
1. Verificar JWT (`defaulalias`, `usuarioid`).
2. Validar `resolve` con gabinete/id real.
3. Revisar ruta física legacy resuelta.
4. Confirmar detección de origen (TIF/PDF).
5. Si TIF: validar creación PDF temporal en root temporal.
6. Probar `download/{token}` antes y después de expiración.