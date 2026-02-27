## Context

Actualmente las propuestas de OpenSpec se redactan manualmente, lo que introduce demora y variabilidad. Se quiere un flujo estandarizado que, dado un `issueKey` de Jira, consulte el ticket, construya una propuesta con IA y la guarde en `openspec/changes/<issueKey>/proposal.md`.

## Goals / Non-Goals

**Goals:**
- Definir un flujo reproducible para generar propuestas OpenSpec desde Jira.
- Establecer como se obtienen los datos del ticket (summary/description) y como se persistira la propuesta.
- Mantener el flujo compatible con la estructura de OpenSpec existente.

**Non-Goals:**
- No implementar un UI para ejecutar el generador.
- No cambiar el esquema de OpenSpec ni la estructura de carpetas base.
- No cubrir autenticacion/rotacion de credenciales de Jira mas alla de lo necesario para el script.

## Decisions

- **Usar un script Node para consulta de Jira**: permite reutilizar la infraestructura existente en `scripts/` y mantiene el flujo automatizable en CI o local.
  - Alternativas: llamada directa desde una accion de OpenSpec o un servicio externo. Se descartan por mayor acoplamiento o complejidad.
- **Generacion con un paso de IA**: el contenido de la propuesta se construye a partir del summary y description del ticket.
  - Alternativas: plantilla estatica sin IA. Se descarta por menor calidad y mas edicion manual.
- **Salida en `openspec/changes/<issueKey>/proposal.md`**: alinea la propuesta con el flujo de cambios de OpenSpec.
  - Alternativas: guardar en una carpeta temporal o en un repositorio externo. Se descarta por dificultar el seguimiento del cambio.

## Risks / Trade-offs

- [Dependencia de Jira/API] → Mitigacion: manejar errores y mensajes claros cuando la consulta falle.
- [Calidad variable de la propuesta generada] → Mitigacion: permitir edicion manual posterior antes de continuar el flujo.
- [Requisitos de credenciales/entorno] → Mitigacion: documentar variables necesarias y fallar de forma explicita.
