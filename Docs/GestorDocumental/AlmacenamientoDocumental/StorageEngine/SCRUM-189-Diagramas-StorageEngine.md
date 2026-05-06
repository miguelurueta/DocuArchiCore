# SCRUM-189 — Diagramas StorageEngine

## 1. Diagrama de Componentes
```mermaid
flowchart LR
    A[API Controller] --> B[UseCase]
    B --> C[DocumentStorageOrchestrator]
    C --> D[ValidationPipeline]
    C --> E[MetadataAnalyzer]
    C --> F[NamingService]
    C --> G[PhysicalPathService]
    C --> H[StorageTransactionCoordinator]
    H --> I[IdentityAllocator]
    H --> J[Repositories]
    C --> K[FileSystem Writer]
    C --> L[XML Writer]
    C --> M[CompensationManager]
```

## 2. Diagrama de Secuencia (flujo nominal)
```mermaid
sequenceDiagram
    participant U as Usuario/API
    participant C as Controller
    participant UC as UseCase
    participant O as Orchestrator
    participant V as ValidationPipeline
    participant T as TransactionCoordinator
    participant R as Repositories
    participant P as PhysicalPhase

    U->>C: POST almacenar
    C->>UC: ExecuteAsync(request,alias,usuario,ip)
    UC->>O: ExecuteAsync(context)
    O->>V: ValidateAsync(context)
    V-->>O: ValidationResult
    O->>T: ExecuteAsync(context)
    T->>R: Lock/Update/Insert (Serializable)
    R-->>T: resultados DB
    T-->>O: StorageTransactionResult
    O->>P: ExecuteAsync(context, txResult)
    P-->>O: PhysicalStatus
    O-->>UC: AlmacenarDocumentoResult
    UC-->>C: AppResponses OK/Error
```

## 3. Diagrama de Clases (núcleo)
```mermaid
classDiagram
    class AlmacenamientoDocumentalController {
      +AlmacenarDocumento(request)
    }
    class AlmacenarDocumentoUseCase {
      +ExecuteAsync(request, alias, usuario, usuarioId, ip)
    }
    class DocumentStorageOrchestrator {
      +ExecuteAsync(context)
    }
    class StorageValidationPipeline {
      +ValidateAsync(context)
    }
    class StorageTransactionCoordinator {
      +ExecuteAsync(context)
    }
    class StoragePhysicalPhaseExecutor {
      +ExecuteAsync(context, txResult)
    }

    AlmacenamientoDocumentalController --> AlmacenarDocumentoUseCase
    AlmacenarDocumentoUseCase --> DocumentStorageOrchestrator
    DocumentStorageOrchestrator --> StorageValidationPipeline
    DocumentStorageOrchestrator --> StorageTransactionCoordinator
    DocumentStorageOrchestrator --> StoragePhysicalPhaseExecutor
```

## 4. Casos de Uso
```mermaid
flowchart TB
    actor[(Usuario Integración)]
    actor --> uc1[Almacenar documento simple]
    actor --> uc2[Almacenar batch con preindex]
    actor --> uc3[Almacenar con inventario]
    actor --> uc4[Almacenar con expediente/unidad]
    actor --> uc5[Almacenar con workflow log]
```

## 5. Estados del Documento
```mermaid
stateDiagram-v2
    [*] --> Pending
    Pending --> Reserved: Tx DB OK
    Reserved --> PhysicalCompleted: FS/XML OK
    Reserved --> PhysicalFailed: FS/XML fail
    PhysicalFailed --> Compensated: compensación ejecutada
    PhysicalCompleted --> [*]
    Compensated --> [*]
```

## 6. Flujo transaccional
```mermaid
flowchart TD
    A[Begin Serializable] --> B[Lock system1]
    B --> C[Calcular reserva proxid/carpeta]
    C --> D[Update system1]
    D --> E[Lock disco_detalle]
    E --> F[Update disco_detalle]
    F --> G[Insert inventario opcional]
    G --> H[Update expediente/unidad opcional]
    H --> I[Insert workflow log opcional]
    I --> J[Commit]
    J --> K[Fase física]
```

## 7. Flujo de compensación
```mermaid
flowchart TD
    A[Fase física inicia] --> B{Error en copy/xml?}
    B -- No --> C[Estado PhysicalCompleted]
    B -- Sí --> D[Construir plan compensación]
    D --> E[Eliminar archivos creados]
    E --> F[Log de compensación]
    F --> G[Estado PhysicalFailed/Compensated]
```

## 8. Despliegue lógico
```mermaid
flowchart LR
    Client[Cliente/Front] --> API[DocuArchi.Api]
    API --> Services[MiApp.Services]
    Services --> Repo[MiApp.Repository]
    Repo --> MySQL[(MySQL)]
    Services --> FS[(FileSystem Storage)]
    Services --> XML[(XML FXL / Índice)]
```
