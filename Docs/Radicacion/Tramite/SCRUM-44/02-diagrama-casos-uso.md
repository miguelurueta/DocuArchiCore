# Diagrama de Casos de Uso

```mermaid
flowchart LR
    FE[Frontend] --> CU1[Validar campos obligatorios]
    CU1 --> A1[Validar campos fijos requeridos]
    CU1 --> A2[Validar campos dinámicos requeridos]
    CU1 --> A3[Retornar AppResponses<List<ValidationError>>]
```
