# SCRUM-161 Arquitectura

## objetivo arquitectónico
Crear API orquestada para retornar firmas disponibles de documento respuesta en una sola llamada.

## contexto funcional
Frontend requiere lista consolidada de firmantes para respuesta: usuario principal + firmas autorizadas.

## alcance y no alcance
- Alcance: controller, service orquestador, DI, tests unitarios, docs.
- No alcance: cambios BD, nuevos repositorios, cambios UI.

## componentes involucrados
- Controller
- Service
- Repository (reutilizado vía servicios existentes)
- DTO
- ClaimValidationService
- QueryOptions (indirecto por servicio de firmas autorizadas)

## diagrama de clases
Controller -> ServiceOrquestado -> (ServiceUsuarioPrincipal + ServiceFirmasAutorizadas)

## diagrama de secuencia
Cliente -> Controller -> ServiceOrquestado -> Servicios dependientes -> respuesta consolidada.

## diagrama de casos de uso
- Obtener firmantes para documento respuesta.
- Retornar empty.
- Retornar no autorizado/error.

## diagrama de estados
- success
- empty
- unauthorized
- error

## flujo end-to-end
Claims -> validación controller -> orquestación service -> merge/dedup -> AppResponses.

## reglas de seguridad
`[Authorize]` y validación de claims.

## reglas de autorización
Se conserva regla de `ServiceSolicitaListaFirmasAutorizadasDocumento`.

## regla de deduplicación
El orquestador elimina duplicados por `Id` al consolidar principal + autorizadas.

## regla de cardinalidad
Salida final limitada a 100 elementos.

## validación SOLID
Responsabilidades separadas: controller validación de entrada, service composición de dominio.

## deuda técnica identificada
`Program.cs` mantiene ruido de registros duplicados heredados.

## riesgos técnicos
Dependencia del comportamiento de servicios base reutilizados.

## decisiones arquitectónicas
Composición de servicios existentes para minimizar duplicidad y riesgo.

## METADATA
- identificador del ticket: SCRUM-161
- usuario que creó el ticket: (según Jira)
- fecha de creación: (según Jira)
- módulo afectado: Gestión Correspondencia / Firmas
- objetivo resumido: API orquestada de firmas documento respuesta
- relación con tickets previos: SCRUM-159, SCRUM-160
- autor de implementación: Codex + equipo backend
- autor de documentación: Codex
- estado del ticket: En implementación
