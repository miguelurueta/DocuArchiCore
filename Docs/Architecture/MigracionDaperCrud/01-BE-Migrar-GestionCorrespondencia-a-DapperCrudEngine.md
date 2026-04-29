# PROMPT ARQUITECTONICO - Ticket BE-01

# Migrar GestionCorrespondencia a DapperCrudEngine

## OBJETIVO
Migrar consultas de datos a `DapperCrudEngine` en el modulo `GestionCorrespondencia` sin cambiar contratos publicos ni comportamiento funcional.

## REPOSITORIO Y RUTAS
- `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\GestionCorrespondencia\GestionRespuesta\SolicitaEstructuraRespuestaIdTareaRepository.cs`
- `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\GestionCorrespondencia\PlantillaValidacion\SolicitaCorreoElectronicoRemitente\SolicitaCorreoElectronicoRemitenteRepository.cs`

## FUNCIONES A INTERVENIR
- `SolicitaEstructuraRespuestaIdTareaAsync(long idTareaWf, string defaultDbAlias)`
- `SolicitaCorreoElectronicoRemitenteAsync(...)`

## REGLAS
1. Mantener firmas publicas y DTOs de salida.
2. Migrar acceso a datos a `IDapperCrudEngine` + `QueryOptions` donde aplique.
3. Si hay SQL dinamico sensible, encapsular y documentar decision tecnica.

## VALIDACION DEL LOTE
- Compilar: `MiApp.Repository`, `MiApp.Services`, `DocuArchi.Api`.
- Ejecutar tests focalizados de `GestionCorrespondencia`.
- Registrar riesgos/regresiones observadas.

## ENTREGABLE FINAL DEL TICKET
- listado de funciones migradas
- funciones pendientes
- decisiones tecnicas
- plan de cierre del modulo
