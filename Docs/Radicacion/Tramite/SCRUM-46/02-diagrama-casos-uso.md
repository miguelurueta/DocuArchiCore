# Diagrama de Casos de Uso

```mermaid
flowchart LR
    FE[Frontend] --> CU1[Validar campos dinamicos unicos]
    CU1 --> A1[Identificar campos UNICO en detalle plantilla]
    CU1 --> A2[Consultar coincidencias en tabla de plantilla]
    CU1 --> A3[Retornar ValidationError si hay duplicados]
    CU1 --> A4[Retornar Sin resultados si no hay coincidencias]
```
