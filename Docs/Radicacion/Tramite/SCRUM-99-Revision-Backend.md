# SCRUM-99 - Revision Backend

## Evaluacion General

- Estado: `REQUIERE AJUSTES`
- Componente revisado: `RegistroRadicadoTareaWorkflowRepository`
- Flujo asociado: `RegistrarRadicacionEntranteService`

## Hallazgos

### 1. Repository mezclando acceso a datos con decisiones funcionales

- Nivel: `Alto`
- Ubicacion:
  - `RegistroRadicadoTareaWorkflowRepository.cs:95`
  - `RegistroRadicadoTareaWorkflowRepository.cs:118`
- Problema:
  - El repository no solo persiste datos; tambien decide valores funcionales fijos como `Flag_sistema = 1`, `id_dat_ext = 0`, `ESTADO_PRIORIDAD = 0` y `ESTADO_TAREA = 0`.
  - Eso mete reglas de negocio y defaults operativos dentro de la capa de acceso a datos.
- Impacto:
  - Aumenta el acoplamiento con la regla actual del proceso.
  - Hace mas riesgoso reutilizar el repository desde otros flujos o extenderlo sin tocar codigo existente.
- Recomendacion:
  - Mover los defaults funcionales al service o encapsularlos en un request interno explicito.
  - Dejar el repository limitado a ejecutar la persistencia con parametros ya resueltos.

### 2. Consulta a `information_schema` en cada ejecucion del flujo transaccional

- Nivel: `Medio`
- Ubicacion:
  - `RegistroRadicadoTareaWorkflowRepository.cs:228`
  - `RegistroRadicadoTareaWorkflowRepository.cs:319`
- Problema:
  - Cada registro consulta metadata de columnas en `information_schema.columns` dentro del mismo flujo transaccional.
- Impacto:
  - Agrega latencia por llamada.
  - Hace que el throughput dependa de metadata de base de datos en tiempo de ejecucion.
  - En cargas altas o despliegues con catalogos grandes puede afectar performance de radicacion.
- Recomendacion:
  - Cachear metadata por `NombreRuta` con invalidacion controlada.
  - Si la estructura de tablas es estable, resolver columnas esperadas una vez por ruta en vez de consultar metadata en cada insercion.

### 3. El resultado interno del registro de tarea workflow se guarda pero no participa en el flujo posterior

- Nivel: `Medio`
- Ubicacion:
  - `RegistrarRadicacionEntranteService.cs:413`
  - `RegistrarRadicacionEntranteService.cs:451`
- Problema:
  - `localRegistroTareaWorkflowResultDto` queda declarado y asignado, pero despues no se consume.
  - El flujo valida exito y validez minima, pero el resultado interno no aporta trazabilidad ni alimenta pasos posteriores.
- Impacto:
  - Introduce estado muerto.
  - Hace mas dificil entender si el ticket realmente necesitaba reutilizacion posterior o solo persistencia con validacion.
- Recomendacion:
  - O bien eliminar la variable local si no aporta mas comportamiento.
  - O usarla explicitamente en el flujo posterior, logging o enriquecimiento tecnico del resultado interno.

### 4. Mensajeria de error acoplada al prompt y no al dominio tecnico

- Nivel: `Bajo`
- Ubicacion:
  - `RegistroRadicadoTareaWorkflowRepository.cs:343`
- Problema:
  - El mensaje `"CAMPO NO DEFINIDO EN PROMPT / no existe en tabla"` mezcla lenguaje de generacion/prompts con una validacion operacional de base de datos.
- Impacto:
  - Reduce claridad diagnostica en logs y soporte.
  - Expone terminologia ajena al dominio de produccion.
- Recomendacion:
  - Cambiar el mensaje por uno tecnico y estable, por ejemplo: `"La columna requerida no existe en la tabla dinamica configurada"`.

## Evaluacion Arquitectonica

- `Service -> Repository` se respeta formalmente.
- El repository mantiene buena atomicidad transaccional con `BeginTransaction`, `Commit` y `Rollback`.
- El uso de `DynamicParameters` evita concatenacion de valores y reduce riesgo de inyeccion sobre parametros.
- La construccion de nombre de tabla y columnas tiene una proteccion razonable con `SafeRouteRegex` y validacion previa contra metadata.

## Riesgos

- Seguridad: `Bajo`
  - La superficie de SQL dinamico esta parcialmente protegida por regex y metadata.
- Inconsistencia de datos: `Medio`
  - La atomicidad es correcta, pero el repository sigue concentrando demasiada decision funcional.
- Escalabilidad: `Medio`
  - La dependencia de `information_schema` por invocacion penaliza el flujo bajo carga.
- Mantenibilidad: `Medio`
  - El acoplamiento entre reglas del proceso y persistencia dificulta evolucionar el componente.

## Recomendaciones Prioritarias

1. Extraer defaults funcionales fuera del repository.
2. Introducir cache o resolucion previa de metadata por ruta workflow.
3. Decidir explicitamente si `localRegistroTareaWorkflowResultDto` debe usarse o eliminarse.
4. Normalizar mensajes de error del repository a lenguaje tecnico de dominio.

## Evidencia

- Revision estatica del componente:
  - `MiApp.Repository/Repositorio/Workflow/RutaTrabajo/RegistroRadicadoTareaWorkflowRepository.cs`
  - `MiApp.Services/Service/Radicacion/Tramite/RegistrarRadicacionEntranteService.cs`
  - `MiApp.DTOs/DTOs/Workflow/RutaTrabajo/RegistroTareaWorkflowResultDto.cs`
  - `MiApp.DTOs/DTOs/Radicacion/RelacionCamposRutaWorklflow/RelacionCamposRutaWorklflowDto.cs`
  - `MiApp.Models/Models/Workflow/RutaTrabajo/InicioTareasWorkflow.cs`
  - `MiApp.Models/Models/Workflow/RutaTrabajo/EstadosTareaWorkflow.cs`
- `dotnet test`: no requerido para esta entrega porque el ticket es de revision tecnica y no introduce cambios funcionales de runtime
