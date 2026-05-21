# SCRUM-208 Implementacion Detallada

## Archivos
- `DocuArchi.Api/Controllers/GestorDocumental/Documentos/FirmaElectronicaDocumentoController.cs`
- `MiApp.Services/Service/GestorDocumental/Documentos/FirmaElectronica/IFirmaElectronicaDocumentoService.cs`
- `MiApp.Repository/Repositorio/GestorDocumental/Documentos/FirmaElectronica/IFirmaElectronicaDocumentoRepository.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/Documentos/FirmaElectronica/FirmaElectronicaDocumentoResponseDto.cs`

## Regla legacy aplicada
- Filtro por `id_archivo` + `nombre_gabinete`.
- Orden `id_registro_certificado_archivo DESC`.
- `Limit=1`.
- Si no hay registro: `FirmadoElectronico=false`, `IdCertificado=0`.

## DI
Registrado en `DocuArchi.Api/Program.cs`:
- `IFirmaElectronicaDocumentoService -> FirmaElectronicaDocumentoService`
- `IFirmaElectronicaDocumentoRepository -> FirmaElectronicaDocumentoRepository`
