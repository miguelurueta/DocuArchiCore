# JIRA Ticket Draft - SCRUM-38

## Tipo
Historia Tecnica

## Resumen
Migrar el contexto funcional de `Registra_Radicacion_entrante` (VB.NET legacy) al flujo OpenSpec de DocuArchiCore para generar especificaciones y tareas de implementacion en este entorno.

## Objetivo
Crear un cambio OpenSpec que capture el comportamiento del flujo de radicacion entrante del sistema monolitico legacy y lo traduzca a artefactos implementables (`proposal.md`, `design.md`, `tasks.md`, `spec.md`) en el repositorio destino, sin modificar el codigo legacy original.

## Contexto Origen (Legacy)
- Repo origen: `D:\imagenesda\GestorDocumental\Desarrollo\Visual-2019\GestionDocumental-Docuarchi.net\GestionDocumental-Docuarchi.net`
- Archivo clave: `radicador\ClassRadicador.vb`
- Funcion fuente: `Registra_Radicacion_entrante`

## Contexto Destino (OpenSpec)
- Repo destino: `D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore`
- OpenSpec disponible en: `openspec\`
- Carpeta objetivo del cambio: `openspec\changes\<change-name>\`
- Especificacion funcional objetivo: `openspec\specs\jira-scrum-38\spec.md`

## Alcance Funcional del Ticket
1. Levantar el comportamiento actual de `Registra_Radicacion_entrante` como flujo funcional (entradas, validaciones, salidas, dependencias).
2. Definir el mapa de equivalencia funcional hacia componentes del entorno actual (controller/service/repository/DTO).
3. Generar cambio OpenSpec con artefactos completos para implementacion futura.
4. Incluir en el alcance las incidencias detectadas del flujo legacy como reglas de migracion y criterios de no regresion, sin ejecutar correcciones en legacy.

## Entregables Esperados
1. `openspec/changes/<change-name>/proposal.md`
2. `openspec/changes/<change-name>/design.md`
3. `openspec/changes/<change-name>/tasks.md`
4. `openspec/specs/jira-scrum-38/spec.md`
5. Matriz de trazabilidad: regla legacy -> requisito OpenSpec -> tarea tecnica

## Criterios de Aceptacion
1. Existe un change OpenSpec para SCRUM-38 con `proposal`, `design` y `tasks` completos.
2. `spec.md` describe el flujo de radicacion entrante en lenguaje funcional verificable.
3. Se documentan precondiciones, postcondiciones, errores y eventos del flujo.
4. Se incluyen riesgos de migracion y estrategia de validacion (pruebas de paridad funcional).
5. El ticket queda listo para pasar a `/opsx:apply` sin ambiguedad de alcance.

## No Incluye
- Modificacion directa del codigo VB.NET legacy.
- Refactor o correccion en produccion durante esta fase.
- Ejecucion de despliegues.

## Definicion de Terminado (DoD)
1. Jira contiene descripcion completa del cambio y artefactos OpenSpec referenciados.
2. Las tareas tecnicas estan desglosadas por capas (API, dominio, infraestructura, pruebas).
3. El equipo puede iniciar implementacion con `/opsx:apply` sin requerir redefinir alcance.

## Prompt IA para crear el cambio OpenSpec
"Crear un cambio OpenSpec para SCRUM-38 que migre funcionalmente la logica de `Registra_Radicacion_entrante` desde `radicador/ClassRadicador.vb` (sistema VB.NET legacy) al entorno DocuArchiCore. Generar `proposal.md`, `design.md`, `tasks.md` y `spec.md` con: flujo funcional, contratos de entrada/salida, reglas de validacion, manejo de errores, dependencias, matriz de trazabilidad y pruebas de paridad funcional. No modificar codigo legacy; enfocar en especificacion implementable para el nuevo entorno." 
