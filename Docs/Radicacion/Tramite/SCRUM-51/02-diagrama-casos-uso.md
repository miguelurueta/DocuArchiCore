# Diagrama de Casos de Uso - SCRUM-51

```mermaid
flowchart TD
    A[Frontend Radicacion] --> B[Registrar Entrante API]
    B --> C[ValidaCamposRadicacionService]
    C --> D[ValidaTipoCamposService]
    D --> E[ValidaTipoCamposRepository]
    E --> F[(information_schema.columns)]
    D --> G[ValidationHelper]
    G --> H[AppResponses con ValidationError]
    H --> A
```
