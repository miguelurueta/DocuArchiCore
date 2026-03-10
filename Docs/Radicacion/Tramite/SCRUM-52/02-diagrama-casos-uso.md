# Diagrama de Casos de Uso - SCRUM-52

```mermaid
flowchart TD
    A[Frontend] --> B[Registrar Entrante API]
    B --> C[ValidaCamposRadicacionService]
    C --> D[Validaciones: Obligatorio/Dimension/Unico/Tipo]
    D --> E[AliasCamposRadicacionEntrante]
    E --> F[Mensaje funcional: Alias valor inválido]
    F --> A
```
