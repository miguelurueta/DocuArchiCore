# Diagrama de Casos de Uso - SCRUM-53

```mermaid
flowchart TD
    A[Frontend] --> B[ValidaCamposRadicacionService]
    B --> C[ValidaCamposObligatorios]
    B --> D[ValidaDimensionCampos]
    B --> E[ValidaCamposDinamicosUnicos]
    B --> F[ValidaTipoCampos]
    C --> G[Mensaje Required]
    D --> H[Mensaje MaxLength]
    E --> I[Mensaje Unique]
    F --> J[Mensaje InvalidType]
```
