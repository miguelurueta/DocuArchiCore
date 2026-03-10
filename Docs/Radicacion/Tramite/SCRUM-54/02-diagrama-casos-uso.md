# Diagrama de Casos de Uso - SCRUM-54

```mermaid
flowchart TD
    A[Frontend] --> B[ValidaDimensionCamposService]
    B --> C[Detecta MaxLength]
    C --> D[ConstruirMensajeValidacion alias + MaxLength]
    D --> E[Campo Alias supera la longitud máxima permitida]
```
