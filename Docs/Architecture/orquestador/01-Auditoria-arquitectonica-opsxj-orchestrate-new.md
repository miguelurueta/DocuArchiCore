# Auditoria Arquitectonica de `opsxj:orchestrate:new`

## Contexto

Este concepto se basa solo en el contenido funcional y el alcance implicito del ticket asociado a la auditoria del comando `opsxj:orchestrate:new`.

La evaluacion no mezcla el backlog `API-first DocuArchi.Api` ni decisiones externas al ticket. El objetivo es dictaminar si el comando debe seguir fortaleciendose en su forma actual, si debe extraerse, o si debe reemplazarse.

## Resumen Ejecutivo

`opsxj:orchestrate:new` no se comporta como un helper menor. Por su alcance aparente y por los sistemas que toca, opera como un orquestador accidental de flujo critico.

Dictamen:

```text
D. Activo mal ubicado
```

Tiene valor operativo y resuelve una necesidad real, pero no deberia seguir creciendo como un script embebido dentro del repositorio coordinador sin modularizacion formal, contratos claros y un plan de extraccion.

## Alcance Evaluado

Basado en el ticket, el comando participa o pretende participar en:

- Jira
- OpenSpec
- Git
- ramas
- commits
- pull requests
- validaciones tecnicas
- modo no interactivo
- coordinacion multi-repo
- trazabilidad entre artefactos
- enforcement parcial del flujo operativo

Esto lo ubica por encima de un utilitario local. Su radio de accion es propio de una pieza de plataforma interna.

## Responsabilidades Detectadas

Las responsabilidades que el ticket sugiere son demasiadas para una sola pieza procedural:

- leer e interpretar el ticket
- preparar contexto tecnico
- generar o actualizar artefactos OpenSpec
- orquestar Git y ramas
- coordinar PRs
- soportar trabajo asistido por IA/Codex
- mantener trazabilidad
- aplicar reglas operativas del flujo multi-repo

Esto indica una mezcla de responsabilidades que incrementa el costo de cambio y dificulta la mantenibilidad.

## Hallazgos Positivos

### 1. Resuelve una necesidad real

El flujo que intenta cubrir existe y es operativo: iniciar trabajo coordinado con trazabilidad entre ticket, cambio, rama y PR.

### 2. Tiene orientacion a trazabilidad

La intencion de conectar ticket, spec, rama, commits y PRs es correcta. Ese objetivo vale la pena conservarlo.

### 3. Intenta soportar no-interactividad

La presencia de `-NonInteractive` indica una orientacion valida hacia automatizacion y ejecucion repetible.

### 4. Apunta a multi-repo

Ese enfoque responde a una necesidad real del ecosistema actual y justifica tratar el comando como un activo mas serio.

## Hallazgos Criticos

### 1. Riesgo de orquestador accidental

El comando parece haber crecido hasta convertirse en un coordinador de varios sistemas externos sin haberse formalizado como plataforma.

### 2. Mezcla excesiva de responsabilidades

Si la misma pieza interpreta Jira, construye OpenSpec, coordina Git y decide impacto entre repos, la separacion interna probablemente es insuficiente.

### 3. No-interactividad probablemente parcial

Un modo `-NonInteractive` solo es fuerte si:

- las precondiciones estan modeladas
- las decisiones son deterministicas
- los errores son cerrados y auditables
- el rerun es seguro

Si depende de convenciones humanas o reparaciones manuales posteriores, no es no-interactivo real.

### 4. Alta fragilidad por contexto

Cuanto mas dependa de estructura de carpetas, naming local, convenciones tacitas y estados previos, mas costoso sera escalarlo o moverlo.

### 5. Riesgo de estados parciales

Este tipo de comando puede dejar el sistema en estados intermedios:

- ticket cambiado pero PR no creado
- artefactos escritos pero branch inconsistente
- repo coordinador correcto y satelites incompletos
- trazabilidad parcial si falla a mitad del flujo

## Acoplamiento

El nivel de acoplamiento esperado es alto.

Acoplamientos mas probables:

- al repositorio coordinador
- a la estructura local de carpetas
- a OpenSpec como convencion obligatoria
- a Jira como fuente de verdad
- a Git y GitHub como infraestructura base
- a naming conventions de branch y PR
- al proceso humano de merge manual

Clasificacion:

```text
Acoplamiento estructural: alto
Riesgo de acoplamiento critico: si
```

## Riesgos Operativos

Los riesgos operativos principales son:

- fallos parciales dificiles de reconstruir
- reruns ambiguos
- desalineacion entre Jira y GitHub
- baja claridad sobre que quedo hecho y que no
- dependencia del operador para cerrar huecos del proceso
- side effects distribuidos sin rollback claro

## Riesgos Arquitectonicos

### 1. Ubicacion incorrecta

Si el comando coordina multiples repos y sistemas externos, su ubicacion conceptual dentro de un repo de producto es debil.

### 2. Expansion accidental de alcance

Todo indica que puede seguir absorbiendo nuevas reglas si no se congela su expansion informal.

### 3. Costo de cambio transversal

Cada nueva capacidad puede exigir tocar Jira, OpenSpec, Git, PRs y reglas de impacto al mismo tiempo.

### 4. Baja separabilidad

Sin modulos claros por responsabilidad, la evolucion del sistema sera costosa y riesgosa.

## Evaluacion del Modo `-NonInteractive`

Con el contenido del ticket, la postura correcta es conservadora:

```text
Debe considerarse sospechoso hasta demostrar lo contrario.
```

Para que sea realmente no interactivo debe cumplir:

- preflight completo
- decisiones deterministicas
- errores cerrados
- logs claros
- rerun seguro
- ausencia de dependencias manuales ocultas

Sin esas garantias, `-NonInteractive` es solo un modo con menos prompts, no una automatizacion robusta.

## Trazabilidad

La trazabilidad es uno de los objetivos correctos del comando, pero no hay base suficiente para asumir que este garantizada por diseno.

Clasificacion preliminar:

```text
Nivel de trazabilidad esperado: medio
Trazabilidad robusta por diseno: no demostrada
```

La pregunta clave no es si deja huellas, sino si permite reconstruir automaticamente el estado completo despues de una ejecucion parcial o fallida.

## Escalabilidad

El ticket ya lo tensiona con:

- multiples repos
- multiples usuarios
- PRs coordinados
- contratos compartidos
- orden de merge
- futuras reglas de gobernanza

Eso excede el perfil de un script operativo simple.

Clasificacion:

```text
Escalabilidad futura: soportable solo con rediseño o extraccion
```

## Deuda Tecnica y Mantenibilidad

Clasificacion estimada:

```text
Deuda tecnica: severa a estructural
Mantenibilidad actual: comprometida
```

La causa probable no es un bug puntual, sino la combinacion de:

- hardcodes
- convenciones implicitas
- side effects distribuidos
- baja separacion de capas
- crecimiento incremental sin formalizacion

## Piezas Rescatables

El valor del activo no debe descartarse. Las piezas potencialmente rescatables son:

- lectura de Jira
- generacion o actualizacion OpenSpec
- validacion de precondiciones
- logging y auditoria
- orquestacion de repos
- enforcement de reglas operativas

La recomendacion es rescatar estas capacidades como modulos, no necesariamente preservar intacta la implementacion actual.

## Piezas Problematicas

Las piezas que deben auditarse con mas dureza son:

- flujo procedural unico que mezcla todo
- decisiones de impacto embebidas
- reglas de negocio operativo escondidas
- no-interactividad no deterministica
- side effects sin rollback real
- dependencia fisica del repo coordinador

## Dictamen Final

La mejor clasificacion, con base solo en el ticket, es:

```text
D. Activo mal ubicado
```

No se recomienda:

- retirarlo de inmediato
- reemplazarlo a ciegas
- seguir parchandolo como si fuera un script menor

Si se confirma el valor operativo que hoy parece tener, la ruta correcta es tratarlo como un activo de plataforma y moverlo a un plan formal de extraccion y rediseño.

## Estrategia Recomendada

```text
No reemplazar de inmediato.
No seguir parchandolo en su forma actual.
Extraerlo y rediseñarlo como orquestador formal.
```

Esto implica:

1. congelar expansion informal de alcance
2. inventariar responsabilidades reales
3. separar contratos y side effects
4. endurecer el modo no interactivo
5. extraerlo de la identidad de repo de producto

## Plan de Transicion Sugerido

### Fase 1. Auditoria estructural real

- inventario de responsabilidades
- mapa de side effects
- mapa de dependencias externas
- matriz de fallos parciales y recuperacion

### Fase 2. Modularizacion interna

Separar al menos estos modulos:

- `JiraModule`
- `OpenSpecModule`
- `RepoImpactModule`
- `PRModule`
- `LoggingModule`
- `ValidationModule`

### Fase 3. Formalizacion del modo no interactivo

- preflight completo
- decisiones deterministicas
- errores cerrados
- rerun seguro

### Fase 4. Extraccion

Moverlo a una identidad propia de tooling interno, desacoplada del repo de producto.

### Fase 5. Endurecimiento

- pruebas por modulo
- pruebas de idempotencia
- pruebas de recovery
- trazabilidad auditable end-to-end

## Acciones Inmediatas Recomendadas

1. Tratarlo ya como activo de plataforma.
2. Congelar nuevas responsabilidades ad hoc.
3. Documentar contratos internos reales por modulo.
4. Medir explicitamente puntos de side effect y puntos sin rollback.
5. Definir si la organizacion invertira en su extraccion durante los proximos meses.

## Conclusión

La inversion mas rentable no parece ser seguir fortaleciendolo dentro de su ubicacion actual.

La recomendacion es:

```text
extraerlo y rediseñarlo como orquestador formal,
preservando sus capacidades utiles y corrigiendo su acoplamiento,
su mezcla de responsabilidades y su fragilidad operativa.
```
