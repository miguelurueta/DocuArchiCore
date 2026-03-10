# Diagrama de Casos de Uso

```mermaid
flowchart LR
    FE[Frontend] --> ORQ[ValidaCamposRadicacionService]
    ORQ --> OBL[ValidaCamposObligatorios]
    ORQ --> DIM[ValidaDimensionCampos]
    ORQ --> UNI[ValidaCamposDinamicosUnicos]
    OBL --> R1[Errores Required]
    DIM --> R2[Errores MaxLength]
    UNI --> R3[Errores Unique]
    R1 --> RESP[AppResponses List ValidationError]
    R2 --> RESP
    R3 --> RESP
```
