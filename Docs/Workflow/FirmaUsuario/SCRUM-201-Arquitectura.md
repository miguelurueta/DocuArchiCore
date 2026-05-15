# SCRUM-201 - Arquitectura API Firma Temporal Usuario Workflow

## 1. Objetivo
Exponer una API segura para que el frontend obtenga la firma del usuario workflow desde `usuario_workflow.Firma_Usuario` como recurso temporal descargable.

## 2. Alcance
- Endpoint de generación de recurso temporal.
- Endpoint de descarga binaria por token temporal.
- Seguridad por claims workflow (`defaulaliaswf`, `IdUsuarioWorkflow`).
- Sin exponer blob ni path físico absoluto.

## 3. Componentes y responsabilidades

## 3.1 Controller
`DocuArchi.Api/Controllers/Workflow/UsuarioWorkflow/FirmaTemporalUsuarioWorkflowController.cs`
- Validar claims.
- Orquestar service.
- Retornar `AppResponses<T>` en endpoint de metadata.
- Retornar `File(...)` en endpoint de descarga.

## 3.2 Service
`MiApp.Services/Service/Workflow/Usuario/ServiceFirmaTemporalUsuarioWorkflow.cs`
- Validar reglas de negocio.
- Consultar firma en repositorio.
- Detectar formato de imagen.
- Escribir temporal en disco.
- Generar token temporal y TTL en cache memoria.
- Resolver token para descarga.

## 3.3 Repository
`MiApp.Repository/Repositorio/Workflow/usuario/UsuarioWorkflowR.cs`
- Consultar `Firma_Usuario` por `idU_suario` con `defaultDbAlias`.
- Usar `QueryOptions` + `DapperCrudEngine`.

## 3.4 DTO
`MiApp.DTOs/DTOs/Workflow/Usuario/FirmaTemporalUsuarioWorkflowDto.cs`
- Contrato de salida para frontend.

## 4. Diagrama de clases
```mermaid
classDiagram
    class FirmaTemporalUsuarioWorkflowController {
      +Get()
      +Download(token)
    }

    class IServiceFirmaTemporalUsuarioWorkflow {
      +SolicitaFirmaTemporalAsync(idUsuarioWorkflow, alias)
      +TryResolveFirmaTemporal(token, idUsuarioWorkflow, out path, out contentType, out fileName)
    }

    class ServiceFirmaTemporalUsuarioWorkflow {
      -IUsuarioWorkflowR usuarioWorkflowRepository
      -IStorageUploadPathResolver pathResolver
      -IMemoryCache memoryCache
    }

    class IUsuarioWorkflowR {
      +SolicitaFirmaUsuarioWorkflowAsync(idUsuarioWorkflow, alias)
    }

    class FirmaTemporalUsuarioWorkflowDto {
      +IdUsuarioWorkflow
      +FileName
      +ContentType
      +RelativePath
      +UrlTemporal
      +ExpiresAt
    }

    FirmaTemporalUsuarioWorkflowController --> IServiceFirmaTemporalUsuarioWorkflow
    ServiceFirmaTemporalUsuarioWorkflow ..|> IServiceFirmaTemporalUsuarioWorkflow
    ServiceFirmaTemporalUsuarioWorkflow --> IUsuarioWorkflowR
    ServiceFirmaTemporalUsuarioWorkflow --> FirmaTemporalUsuarioWorkflowDto
```

## 5. Diagrama de secuencia (metadata + descarga)
```mermaid
sequenceDiagram
    autonumber
    participant FE as Frontend
    participant API as FirmaTemporalUsuarioWorkflowController
    participant SRV as ServiceFirmaTemporalUsuarioWorkflow
    participant REP as UsuarioWorkflowR
    participant DB as usuario_workflow
    participant FS as Temp FS
    participant MC as MemoryCache

    FE->>API: GET /api/workflow/usuarios/firma-temporal
    API->>API: Validar claims defaulaliaswf + IdUsuarioWorkflow
    API->>SRV: SolicitaFirmaTemporalAsync(idUsuarioWorkflow, alias)
    SRV->>REP: SolicitaFirmaUsuarioWorkflowAsync(idUsuarioWorkflow, alias)
    REP->>DB: SELECT Firma_Usuario FROM usuario_workflow WHERE idU_suario = @id
    DB-->>REP: blob firma
    REP-->>SRV: firma bytes
    SRV->>SRV: Validar blob + detectar formato
    SRV->>FS: Escribir archivo temporal signatures/*
    SRV->>MC: Guardar token con TTL
    SRV-->>API: AppResponses<FirmaTemporalUsuarioWorkflowDto>
    API-->>FE: 200 + metadata (UrlTemporal)

    FE->>API: GET /api/workflow/usuarios/firma-temporal/download/{token}
    API->>API: Validar claim IdUsuarioWorkflow
    API->>SRV: TryResolveFirmaTemporal(token, idUsuarioWorkflow)
    SRV->>MC: Consultar token
    SRV->>FS: Validar existencia archivo
    SRV-->>API: path + contentType + fileName
    API-->>FE: 200 File(binary)
```

## 6. Diagrama de estados del token temporal
```mermaid
stateDiagram-v2
    [*] --> Created: Token generado
    Created --> Active: Guardado en cache
    Active --> Consumed: Descarga exitosa
    Active --> Expired: TTL vencido
    Active --> Invalid: Claim no coincide o token no existe
    Consumed --> [*]
    Expired --> [*]
    Invalid --> [*]
```

## 7. Contratos y reglas de seguridad
- Claim `defaulaliaswf`: obligatorio para seleccionar alias de conexión workflow.
- Claim `IdUsuarioWorkflow`: obligatorio para consulta y autorización de descarga.
- No se permite obtener firma de otro usuario alterando parámetros.
- El token temporal no es persistente en DB; vive en memoria con TTL.

## 8. Depuración operativa
- Si falla metadata:
  - validar claims en JWT (`defaulaliaswf`, `IdUsuarioWorkflow`)
  - validar firma existente en `usuario_workflow.Firma_Usuario`
- Si falla descarga:
  - validar `token` vigente (TTL)
  - validar que el archivo temporal exista
  - validar que `IdUsuarioWorkflow` del claim coincida con el registro cacheado

## 9. Tablas/almacenamientos relacionados

## 9.1 Modelo de datos persistente
```mermaid
erDiagram
  USUARIO_WORKFLOW {
    int idU_suario PK
    longblob Firma_Usuario
    varchar login_Usuario
    int ESTADO_USUARIO
  }
```

## 9.2 Almacenamiento temporal (no persistente)
- Root temporal: `StorageUploadPathResolver.GetTempRoot()`
- Carpeta funcional: `signatures`
- Cache en memoria:
  - key: `workflow-signature:{token}`
  - value: path + contentType + fileName + idUsuarioWorkflow + expiresAt
