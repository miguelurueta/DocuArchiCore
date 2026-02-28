## ADDED Requirements

### Requirement: Generate proposal from Jira issue
El sistema SHALL generar una propuesta de OpenSpec a partir del `summary` y `description` del issue de Jira indicado por `issueKey`.

#### Scenario: Jira issue fetched and proposal generated
- **WHEN** el usuario ejecuta el generador con un `issueKey` valido
- **THEN** el sistema consulta Jira y usa el `summary` y `description` para construir el contenido de la propuesta

### Requirement: Persist proposal in change folder
El sistema SHALL guardar la propuesta generada en `openspec/changes/<issueKey>/proposal.md`.

#### Scenario: Proposal saved to OpenSpec change path
- **WHEN** la propuesta fue generada correctamente
- **THEN** el sistema crea (si no existe) la carpeta `openspec/changes/<issueKey>/` y guarda `proposal.md`

### Requirement: Clear failure messaging
El sistema SHALL reportar errores claros cuando falle la consulta a Jira o la generacion de la propuesta.

#### Scenario: Jira API request fails
- **WHEN** la consulta a Jira falla por credenciales, red o `issueKey` inexistente
- **THEN** el sistema retorna un error explicito y no escribe un `proposal.md` incompleto

### Requirement: Single command proposal generation
El sistema SHALL permitir ejecutar un solo comando que consulte Jira y genere `openspec/changes/<issueKey>/proposal.md`.

#### Scenario: Single command produces proposal file
- **WHEN** el usuario ejecuta `node scripts/generate-proposal-from-jira.js <issueKey>`
- **THEN** el sistema consulta Jira, genera la propuesta y guarda el archivo en el directorio del cambio

### Requirement: Automatic repository detection from requirement logic
El sistema SHALL detectar automaticamente el repositorio objetivo del cambio usando la logica del requerimiento y el contexto operativo de repositorios.

#### Scenario: Repository detected with confidence
- **WHEN** el contenido del ticket permite clasificar con claridad el destino
- **THEN** el sistema asigna automaticamente uno o mas repositorios candidatos
- **AND** registra la razon de clasificacion para trazabilidad

### Requirement: Manual fallback when repository is ambiguous
El sistema SHALL solicitar repo o plantilla de cambios cuando la IA no pueda determinar el repositorio con confianza suficiente.

#### Scenario: Ambiguous repository classification
- **WHEN** el ticket no contiene informacion suficiente para clasificar repositorio
- **THEN** el sistema detiene la automatizacion y solicita confirmacion del usuario
- **AND** muestra un mensaje claro: "No se pudo detectar el repositorio, especifique la plantilla de cambios."

### Requirement: Repository context for AI classification
El sistema SHALL usar el siguiente contexto minimo para clasificar repositorios:
- `DocuArchi.Api`: APIs y controllers
- `DocuArchiCore`: orquestador OpenSpec
- `MiApp.DTOs`: contratos DTOs
- `MiApp.Models`: clases Dapper
- `MiApp.Repository`: consultas/actualizaciones de BD
- `MiApp.Services`: servicios y automapeos

#### Scenario: Context applied during classification
- **WHEN** se ejecuta la deteccion de repositorio
- **THEN** el sistema considera el contexto definido para decidir destino del cambio

### Requirement: Ticket text is mandatory for archive
El sistema SHALL bloquear el archivado cuando el ticket no tenga texto asociado.

#### Scenario: Missing summary and description
- **WHEN** el ticket no contiene `summary` ni `description`
- **THEN** el sistema detiene el archivado
- **AND** muestra: "El ticket no tiene texto, no puede archivarse."

### Requirement: Multi-repo PR merge gate before archive
El sistema SHALL validar que todos los PR de los repos impactados esten mergeados antes de archivar.

#### Scenario: Pending PR blocks archive
- **WHEN** existe un PR pendiente de merge en cualquier repo impactado
- **THEN** el sistema no archiva el ticket
- **AND** reporta cuales PR faltan por merge

### Requirement: Archive orchestrator sequence
El sistema SHALL ejecutar la secuencia de cierre en orden: archive local, push en `DocuArchiCore`, cambio de estado en Jira.

#### Scenario: All PR merged
- **WHEN** todos los PR impactados estan mergeados
- **THEN** el sistema archiva en repositorio local
- **AND** hace push del archivado en `DocuArchiCore`
- **AND** cambia el estado del ticket en Jira

### Requirement: End-to-end traceability logs
El sistema SHALL registrar logs trazables en cada paso del flujo.

#### Scenario: Flow execution
- **WHEN** el comando se ejecuta de inicio a fin
- **THEN** se registran eventos de deteccion de cambios, creacion/validacion de PR, merge, archive, push y cambio de estado Jira

### Requirement: Clear operational error messages
El sistema SHALL devolver errores claros y accionables.

#### Scenario: Repository not detected
- **WHEN** la IA no puede detectar repositorio por ruta/logica
- **THEN** el sistema muestra: "No se pudo detectar el repositorio, especifique la plantilla de cambios."
