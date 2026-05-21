# SCRUM-208 Arquitectura

## Objetivo
Exponer `GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica` para validar si un documento tiene registro de firma electrónica.

## Capas
- Controller: valida claim `defaulalias`, delega al service y retorna `AppResponses`.
- Service: validaciones de negocio, fallback firmado/no firmado y observabilidad.
- Repository: consulta parametrizada con `DapperCrudEngine + QueryOptions`.

## Flujo
1. Controller valida claim `defaulalias`.
2. Service valida `idArchivo` y `nombreGabinete`.
3. Repository consulta `ra_cert_registro_certificado_archivo` con filtros + order desc + limit 1.
4. Service responde firmado/no firmado.

## SOLID
- SRP: responsabilidades separadas por capa.
- DIP: dependencias por interfaces `IFirmaElectronicaDocumentoService` y `IFirmaElectronicaDocumentoRepository`.
