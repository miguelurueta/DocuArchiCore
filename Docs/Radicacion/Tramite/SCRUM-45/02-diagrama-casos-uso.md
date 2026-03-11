# Diagrama de Casos de Uso

```mermaid
flowchart LR
    FE[Frontend] --> CU1[Validar dimension de campos]
    CU1 --> A1[Consultar longitudes de tabla plantilla]
    CU1 --> A2[Combinar longitudes de campos dinamicos tam_campo]
    CU1 --> A3[Validar longitud de campos fijos y dinamicos]
    CU1 --> A4[Retornar AppResponses<List<ValidationError>>]
```
