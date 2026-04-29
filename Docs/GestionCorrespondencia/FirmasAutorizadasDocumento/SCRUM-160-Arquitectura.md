# SCRUM-160 Arquitectura

## objetivo arquitectónico
Exponer API de consulta de firmas autorizadas por usuario con contrato estable `AppResponses<List<ResponseDropdownDto>>`, validación de claims y límite duro de resultados.

## contexto funcional
Se requiere poblar selector de firmantes permitidos para un usuario autorizado en Gestión de Correspondencia.

## alcance y no alcance
- Alcance: endpoint GET, service de negocio, repository con `QueryOptions`, DI, tests unitarios, docs y artefactos OpenSpec.
- No alcance: cambios de esquema BD, UI frontend, permisos nuevos en catálogo.

## componentes involucrados
- Controller: `SolicitaListaFirmasAutorizadasDocumentoController`
- Service: `ServiceSolicitaListaFirmasAutorizadasDocumento`
- Repository: `SolicitaListaFirmasAutorizadasDocumentoRepository`
- DTO: `ResponseDropdownDto`, `AppResponses<T>`, `AppMeta`, `AppError`
- ClaimValidationService: `IClaimValidationService` para `defaulalias` y `usuarioid`
- QueryOptions: tabla, joins, filtros, columnas, order y `Limit=100`

## diagrama de clases
Controller -> Service -> Repository -> DapperCrudEngine

## diagrama de secuencia
Cliente -> Controller (claims + query) -> Service (autorización + dedupe) -> Repository (QueryOptions) -> DB -> Service -> Controller -> Cliente.

## diagrama de casos de uso
- Consultar firmantes autorizados
- Responder lista vacía
- Rechazar no autorizado

## diagrama de estados
- success: datos encontrados y mapeados
- empty: consulta válida sin resultados
- unauthorized: `usuarioId != idUsuarioAutorizado` o claim inválido
- error: excepción técnica

## flujo end-to-end
JWT con claims -> controller valida -> service aplica regla de autorización y deduplicación -> repository consulta -> response normalizado.

## reglas de seguridad
`[Authorize]` obligatorio y validación explícita de claims.

## reglas de autorización
Solo se permite consultar cuando `usuarioId` del claim coincide con `idUsuarioAutorizado` del query.

## regla de deduplicación
`GroupBy(Id)` y selección del primer registro.

## regla de cardinalidad
Límite máximo 100 filas en query y en salida.

## validación SOLID
Separación de responsabilidades: controller (entrada), service (reglas), repository (acceso datos).

## deuda técnica identificada
`Program.cs` contiene registros DI duplicados heredados fuera de este ticket.

## riesgos técnicos
Dependencia del nombre real de tabla/columnas y calidad de datos nulos en remitente/cargo.

## decisiones arquitectónicas
Patrón existente API-Service-Repository con `AppResponses`, sin introducir nuevos contratos.

## METADATA
- identificador del ticket: SCRUM-160
- usuario que creó el ticket: (según Jira)
- fecha de creación: (según Jira)
- módulo afectado: Gestión Correspondencia / Firmas
- objetivo resumido: API de firmas autorizadas por usuario
- relación con tickets previos: SCRUM-159 (firmas permitidas por solicitud)
- autor de implementación: Codex + equipo backend
- autor de documentación: Codex
- estado del ticket: En implementación
