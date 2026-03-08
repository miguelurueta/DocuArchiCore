# Diagrama de Casos de Uso

```mermaid
flowchart LR
    U[Usuario Radicador] --> CU1[Registrar Radicacion Entrante]
    U --> CU2[Validar Radicacion Entrante]
    U --> CU3[Consultar Flujo Inicial]

    CU1 --> R1[Validar request]
    CU1 --> R2[Resolver usuario y parametros]
    CU1 --> R3[Evaluar policy Q06/Q07/Q08/Q09]
    CU1 --> R4[Persistir Q01-Q05 en transaccion]
    CU1 --> R5[Q06-Q07 condicional por requiere_respuesta]
    CU1 --> R6[Q08 condicional por util_tipo_modulo_envio]
    CU1 --> R7[Q09 condicional por TipoRadicacion PQR]

    CU2 --> V1[Validar campos obligatorios]
    CU2 --> V2[Retornar alertas funcionales]

    CU3 --> F1[Resolver actividad inicial]
```
