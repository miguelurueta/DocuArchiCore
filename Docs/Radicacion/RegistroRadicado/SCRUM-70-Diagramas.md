# SCRUM-70 Diagramas

## Secuencia de integracion

```text
RegistrarRadicacionEntranteAsync
  -> valida request, usuario gestion, plantilla y configuracion
  -> evalua util_tipo_modulo_envio
     -> si != 2
        -> RegistrarRadicacionEntranteRepository
     -> si == 2
        -> ValidaPreRegistroWorkflowService
           -> claim defaulaliaswf
           -> SolicitaEstructuraRutaWorkflow(defaulaliaswf)
           -> nombreRuta = "dat_adic_tar" + Nombre_Ruta
           -> SolicitaCamposRelacionRutaPlantillaAsync
           -> AsignaDatosRadicacionAsync
           -> ValidaDatosRadicacionTareaWorkflowAsync
           -> retorna nombreRuta + relaciones asignadas/validadas
        -> RegistrarRadicacionEntranteRepository
```

## Diagrama de decision

```text
Inicio
  |
  v
Validaciones base radicacion
  |
  v
util_tipo_modulo_envio == 2 ?
  |-- No --> Registrar radicado
  |
  |-- Si --> Existe claim defaulaliaswf ?
               |-- No --> Retornar error y no registrar
               |
               |-- Si --> Existe ruta workflow activa ?
                            |-- No --> Retornar error y no registrar
                            |
                            |-- Si --> Existe relacion plantilla-ruta ?
                                         |-- No --> Retornar error y no registrar
                                         |
                                         |-- Si --> Asignacion OK ?
                                                      |-- No --> Retornar error y no registrar
                                                      |
                                                      |-- Si --> Validacion workflow OK ?
                                                                   |-- No --> Retornar error y no registrar
                                                                   |
                                                                   |-- Si --> Registrar radicado
```
