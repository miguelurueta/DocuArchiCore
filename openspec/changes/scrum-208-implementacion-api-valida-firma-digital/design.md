## Context

- Jira issue key: SCRUM-208
- Jira summary: IMPLEMENTACION-API-VALIDA-FIRMA-DIGITAL-DOCUMENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-208

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Se requiere una API backend para validar si un documento tiene firma electrónica, migrando de forma compatible la lógica legacy de `Solicita_registro_certificado_archivo_imagen_gabinete`.

Semántica legacy a preservar:

- Buscar por `id_archivo` y `nombre_gabinete` en `ra_cert_registro_certificado_archivo`.
- Ordenar por `id_registro_certificado_archivo DESC`.
- Tomar el primer registro.
- Si existe: `FirmadoElectronico=true` y `IdCertificado=ra_cert_certificado_id_certificado`.
- Si no existe: `FirmadoElectronico=false` y `IdCertificado=0`.

## Legacy Reference

- Function legacy: `Solicita_registro_certificado_archivo_imagen_gabinete`
- Tabla principal: `ra_cert_registro_certificado_archivo`

## Scope

### In Scope

- Endpoint:
  - `GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?nombreGabinete={nombreGabinete}`
- Controller, Service, Repository y DTO dedicados.
- Validaciones de entrada/claims.
- Implementación con `DapperCrudEngine + QueryOptions` (sin SQL manual).
- Respuesta con `AppResponses<FirmaElectronicaDocumentoResponseDto>`.
- Pruebas unitarias, de integración y de contrato.
- Documentación técnica SCRUM-208.

### Out of Scope

- Cambios de esquema en base de datos.
- Nuevos tipos de firma o validación criptográfica del certificado.
- Cambios en contratos de SCRUM-206 u otros endpoints ya publicados.

## Business Decisions

1. `defaulalias` es obligatorio y se toma solo desde claims.
2. `usuarioid` es opcional para trazabilidad/log.
3. `idArchivo` debe ser `> 0`.
4. `nombreGabinete` es requerido, con `Trim()`, y regex `^[A-Za-z0-9_]+$`.
5. El repository retorna datos puros (sin `AppResponses` ni semántica HTTP).
6. El service define fallback funcional (`no firmado`) cuando no hay fila.

## Target Architecture

- Patrón obligatorio: `Controller -> Service -> Repository`.
- `try/catch` obligatorio en las tres capas.
- `AppResponses<T>` como contrato de salida único.

Ubicación objetivo:

- Controller:
  - `DocuArchi.Api/Controllers/GestorDocumental/Documentos/FirmaElectronicaDocumentoController.cs`
- Service:
  - `MiApp.Services/Service/GestorDocumental/Documentos/FirmaElectronica/IFirmaElectronicaDocumentoService.cs`
  - `MiApp.Services/Service/GestorDocumental/Documentos/FirmaElectronica/FirmaElectronicaDocumentoService.cs`
- Repository:
  - `MiApp.Repository/Repositorio/GestorDocumental/Documentos/FirmaElectronica/IFirmaElectronicaDocumentoRepository.cs`
  - `MiApp.Repository/Repositorio/GestorDocumental/Documentos/FirmaElectronica/FirmaElectronicaDocumentoRepository.cs`
- DTO:
  - `MiApp.DTOs/DTOs/GestorDocumental/Documentos/FirmaElectronica/FirmaElectronicaDocumentoResponseDto.cs`

## API Contract

Request:

- `GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?nombreGabinete={nombreGabinete}`

Response DTO:

```csharp
public sealed class FirmaElectronicaDocumentoResponseDto
{
    public long IdArchivo { get; init; }
    public string NombreGabinete { get; init; } = string.Empty;
    public bool FirmadoElectronico { get; init; }
    public long IdCertificado { get; init; }
}
```

Semántica:

- Con registro: `success=true`, `message="OK"`, `FirmadoElectronico=true`, `IdCertificado>0`.
- Sin registro: `success=true`, `message="OK"`, `FirmadoElectronico=false`, `IdCertificado=0`.
- Error de validación: `success=false`, `meta.status="validation"`, `errors[]` controlado.

## Data Access Strategy

Reglas obligatorias:

1. Solo `DapperCrudEngine` y `QueryOptions`.
2. Cero SQL manual (`QueryAsync`/`ExecuteAsync` directos prohibidos).
3. Filtros parametrizados por `id_archivo` y `nombre_gabinete`.
4. Orden descendente por `id_registro_certificado_archivo` y `limit=1`.

Ejemplo objetivo de `QueryOptions`:

```csharp
var options = new QueryOptions
{
    TableName = "ra_cert_registro_certificado_archivo",
    DefaultAlias = "rc",
    Columns =
    [
        "rc.id_registro_certificado_archivo",
        "rc.ra_cert_certificado_id_certificado"
    ],
    Filters = new Dictionary<string, object?>
    {
        ["id_archivo"] = idArchivo,
        ["nombre_gabinete"] = nombreGabinete
    },
    OrderByFields = [ new QueryOrderBy("id_registro_certificado_archivo", "DESC") ],
    Limit = 1
};
```

## Execution Flow

1. Controller valida claim `defaulalias`.
2. Controller valida `idArchivo` y `nombreGabinete` (entrada mínima HTTP).
3. Service aplica reglas de negocio y sanitización final.
4. Repository consulta tabla de certificados con `QueryOptions`.
5. Service mapea primer registro a `FirmadoElectronico=true` o aplica fallback no firmado.
6. Controller retorna `Ok(...)` o `BadRequest(...)` con `AppResponses`.

## Security and Validation

- Regex de `nombreGabinete`: `^[A-Za-z0-9_]+$`.
- Prohibido aceptar alias por query/body.
- No exponer stacktrace ni SQL en errores.
- Mensajes de error controlados con `AppError { Type, Field, Message }`.

## Observability

Logs mínimos:

- `Information`: inicio consulta, resultado firmado/no firmado, `duracionMs`.
- `Warning`: claim faltante, `idArchivo` inválido, `nombreGabinete` inválido.
- `Error`: excepción de repository/service/controller.

Campos sugeridos:

- `requestId`, `alias`, `usuarioid`, `idArchivo`, `nombreGabinete`, `firmadoElectronico`, `idCertificado`, `duracionMs`.

## Testing Strategy

- Controller:
  - claim `defaulalias` faltante.
  - `idArchivo <= 0`.
  - `nombreGabinete` inválido.
  - éxito (`200`) contrato correcto.
- Service:
  - registro existe -> firmado.
  - registro no existe -> fallback no firmado.
  - error de repository -> `AppResponses` controlado.
- Repository:
  - `QueryOptions` correcto (tabla, filtros, order, limit).
  - consulta parametrizada sin SQL manual.
  - selección efectiva del último registro.
- Integración:
  - seed/schema para caso firmado y no firmado.
  - claim válido/inválido.
- Regresión:
  - no romper contratos de SCRUM-206 ni claims actuales.

## Documentation Deliverables

- `Docs/GestorDocumental/Documentos/FirmaElectronica/SCRUM-208-Arquitectura.md`
- `Docs/GestorDocumental/Documentos/FirmaElectronica/SCRUM-208-Implementacion-Detallada.md`
- `Docs/GestorDocumental/Documentos/FirmaElectronica/SCRUM-208-Integracion-Frontend.md`
- `Docs/GestorDocumental/Documentos/FirmaElectronica/SCRUM-208-Pruebas.md`
- `Docs/GestorDocumental/Documentos/FirmaElectronica/SCRUM-208-Observabilidad.md`
- `Docs/GestorDocumental/Documentos/FirmaElectronica/SCRUM-208-Seguridad.md`
- `Docs/GestorDocumental/Documentos/FirmaElectronica/SCRUM-208-Metadata.md`

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-208-implementacion-api-valida-firma-digital.
