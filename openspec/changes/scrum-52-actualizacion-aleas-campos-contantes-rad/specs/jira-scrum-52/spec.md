## ADDED Requirements

### Requirement: Alias actualizados para campos constantes
La validación de radicación entrante MUST usar alias funcionales para campos constantes definidos en SCRUM-52.

#### Scenario: Alias de tipo de radicado
- **WHEN** se valida `TipoRadicado` o `IdtipoRadicado`
- **THEN** el alias mostrado es `Tipo de Radicado`

#### Scenario: Alias de solicitante y responsable
- **WHEN** se valida `Remitente_Cor` o `Remit_Dest_Interno_id_Remit_Dest_Int`
- **THEN** el alias mostrado es `Solicitante`
- **WHEN** se valida `Destinatario_Cor` o `Destinatario_Externo_id_Dest_Ext`
- **THEN** el alias mostrado es `Responsable del trámite`

### Requirement: Formato de mensaje de validación
Los errores de validación MUST evitar formato técnico y retornar mensaje funcional.

#### Scenario: Mensaje funcional sin texto técnico
- **WHEN** ocurre un error de validación en un campo
- **THEN** el mensaje sigue formato `<Alias>: valor inválido.`
- **AND** no incluye prefijo técnico `Error en campo`

### Requirement: Validación integrada en servicios de radicación
El ajuste de alias y mensaje MUST aplicarse en validaciones de obligatorio, dimensión, unicidad y tipo de campo.

#### Scenario: Orquestación consolidada
- **WHEN** `ValidaCamposRadicacionService` consolida validaciones
- **THEN** los `ValidationError.Message` respetan alias y formato funcional
