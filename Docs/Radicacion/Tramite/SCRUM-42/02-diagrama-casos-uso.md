# Diagrama de Casos de Uso

```mermaid
flowchart LR
    U[Usuario Radicador] --> CU1[Registrar Radicacion Entrante]
    U --> CU2[Validar Radicacion Entrante]
    U --> CU3[Consultar Flujo Inicial]

    CU1 --> R1[Validar request y reglas]
    CU1 --> R2[Resolver usuario radicador]
    CU1 --> R3[Persistir Q01-Q08 en transaccion]
    CU1 --> R4[Ejecutar Q09 condicional]

    CU2 --> V1[Validar campos obligatorios]
    CU2 --> V2[Retornar alertas funcionales]

    CU3 --> F1[Resolver actividad inicial]
```
