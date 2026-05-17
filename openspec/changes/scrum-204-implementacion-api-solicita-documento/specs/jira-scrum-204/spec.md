## ADDED Requirements

### Requirement: Resolver documento a URL temporal única
El sistema MUST exponer un endpoint de resolución que entregue al frontend una única URL temporal para visualizar/descargar un único documento.

#### Scenario: Documento PDF original
- **GIVEN** un `nombreGabinete` válido y `idDocumento` existente con archivo final PDF único
- **WHEN** se consume el endpoint de resolución
- **THEN** la respuesta retorna `UrlTemporal` válida y `Origen = ORIGINAL`
- **AND** la descarga retorna `application/pdf`

### Requirement: Consolidar matriz TIF/multi-imagen a PDF temporal
El sistema MUST consolidar en PDF temporal cuando el documento esté compuesto por matriz TIF/múltiples imágenes y entregar URL temporal del consolidado.

#### Scenario: Documento TIF multi-página
- **GIVEN** un documento cuya matriz física corresponde a TIF/multi-imagen
- **WHEN** se consume el endpoint de resolución
- **THEN** se genera un PDF temporal consolidado
- **AND** la respuesta retorna `UrlTemporal` del consolidado con `Origen = TIF_CONSOLIDADO`

### Requirement: Reutilizar componentes existentes de ruta/naming/extensión
La implementación MUST reutilizar servicios y repositorios existentes de Storage Engine para evitar duplicidad de lógica legacy.

#### Scenario: Revisión de implementación
- **WHEN** se revisa la implementación del ticket
- **THEN** se evidencia reutilización de `StoragePhysicalPathService`, `StorageFolderLegacyPolicy`, `StoragePathResolver`, `StorageRouteRepository`, `IReemplazoPdfDocumentLocationRepository`, `IStorageExtensionResolver`, `IStorageExtensionRepository`, `IStorageNamingService` y `StorageUploadPathResolver`

### Requirement: Proteger acceso a archivo físico con token temporal
El sistema MUST exponer descarga mediante token temporal y no revelar rutas absolutas en el contrato público.

#### Scenario: Token expirado
- **GIVEN** un token temporal vencido
- **WHEN** se invoca endpoint de descarga
- **THEN** el sistema retorna error de validación/seguridad controlado

### Requirement: Cumplir reglas de validación y manejo de errores del backend
El sistema MUST validar claims y parámetros de entrada, y responder con `AppResponses<T>` y errores controlados.

#### Scenario: Claim o parámetros inválidos
- **GIVEN** claim `defaulalias` inválido o `idDocumento <= 0`
- **WHEN** se invoca endpoint de resolución
- **THEN** el sistema retorna `success=false` con estado de validación

### Requirement: Trazabilidad y documentación técnica
El sistema MUST dejar documentación técnica y contrato frontend del flujo de visualización.

#### Scenario: Entrega técnica
- **WHEN** se revisa la documentación del ticket
- **THEN** existe guía en `Docs/GestorDocumental/Documentos/VisualizacionDocumento/` con arquitectura, contrato, secuencia y errores
