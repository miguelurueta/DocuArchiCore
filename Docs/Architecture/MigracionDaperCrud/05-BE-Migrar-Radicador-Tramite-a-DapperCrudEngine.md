# PROMPT ARQUITECTONICO - Ticket BE-03

# Migrar Radicador/Tramite a DapperCrudEngine

## OBJETIVO
Migrar funciones de acceso a datos del modulo `Radicador/Tramite` a `DapperCrudEngine` sin cambiar contratos publicos ni logica funcional.

## REPOSITORIO Y RUTAS/FUNCIONES
- `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Radicador\Tramite\RegistrarRadicacionEntranteRepository.cs`
  - `RegistrarRadicacionEntranteAsync(...)`
  - `ExecuteQ03InsertPlantillaAsync(...)`
  - `ExecuteQ05InsertRegistroGeneralAsync(...)`
  - `ExecuteQ06InsertSolicitudRespuestaAsync(...)`
  - `ExecuteQ07InsertLogRespuestaAsync(...)`
  - `ExecuteQ08InsertEstadoInicialAsync(...)`
- `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Radicador\Tramite\ValidaCamposDinamicosUnicosRadicacionRepository.cs`
  - `SolicitaCoincidenciasCamposUnicosAsync(...)`
- `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Radicador\Tramite\ValidaDimensionCamposRepository.cs`
  - `SolicitaLongitudesCamposAsync(...)`
- `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Radicador\Tramite\ValidaTipoCamposRepository.cs`
  - `SolicitaTiposCamposAsync(...)`

## REGLAS
- No alterar semantica transaccional.
- Mantener estructura de errores y mensajes actuales.
- Migrar por sublotes (validaciones primero, registro complejo despues).

## VALIDACION
- Build de repos impactados.
- Tests focalizados de radicacion.
- Analisis de riesgo por transacciones y rollback.

## ENTREGABLE
- funciones migradas
- funciones pendientes
- decisiones tecnicas
- plan de cierre
