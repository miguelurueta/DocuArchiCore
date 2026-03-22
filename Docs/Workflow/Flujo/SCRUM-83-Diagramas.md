# SCRUM-83 Diagramas

## Flujo migrado

```text
idFlujoTrabajo + defaultDbAlias
              |
              v
SolicitaDatosActividadInicioFlujoRepository
              |
              v
wf_registro_actividaes_flujos_trabajo
  filtro: wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = idFlujoTrabajo
  filtro: ACTIVIDAD_INICIO = 1
              |
              v
AppResponses<SolicitaDatosActividadInicioFlujo>
```
