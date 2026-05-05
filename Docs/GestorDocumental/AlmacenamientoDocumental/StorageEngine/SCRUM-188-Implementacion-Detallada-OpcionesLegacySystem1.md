# SCRUM-188 - Implementacion Detallada Opciones Legacy System1

## Resumen de Cambios
- SCRUM-188 consolida evidencia tecnica y trazabilidad de la paridad legacy para opciones `system1`.
- El delta funcional runtime de estas reglas se materializo en satelites (`MiApp.DTOs` y `MiApp.Services`) via PRs de SCRUM-188.
- En `DocuArchiCore` se deja documentacion enterprise y OpenSpec refinado para auditoria/merge seguro.

## Contexto de Paridad Legacy
Funciones legacy objetivo:
- `VerificaOpcionAplicarInventarioDocumental(...)`
- `VerificaOpcionAplicarTablaRetencion(...)`
- `Verfica_opcion_seleccion_unidad(...)`

Opciones de `system1` que gobiernan el flujo:
- `INVENTARIO_DOCUMENTAL`
- `APLICA_TRD`
- `ASIGNA_UNIDAD`

## Cambios por Capa y Repositorio

### MiApp.DTOs (PR #67)
- `DTOs/GestorDocumental/AlmacenamientoDocumental/ExpedienteStorageDto.cs`
  - Se agrega `IdClaseDocumento` (`int?`).
  - Motivo: cerrar contrato requerido por validaciones de expediente/unidad cuando `ASIGNA_UNIDAD` esta activa.

### MiApp.Services (PR #123)
- `Service/GestorDocumental/AlmacenamientoDocumental/Physical/IStoragePathResolver.cs`
  - Se exponen:
    - `GetStorageRoot(string nombreGabinete)`
    - `GetFinalFolder(string nombreGabinete, int disco, int carpeta)`
- `Service/GestorDocumental/AlmacenamientoDocumental/Physical/StoragePathResolver.cs`
  - Implementacion de ambos metodos con validaciones de seguridad.
  - `GetStorageRoot`: normaliza root por gabinete.
  - `GetFinalFolder`: construye destino final (`d{disco}/c{carpeta}`) y valida integridad de ruta.

### DocuArchiCore (PR #242)
- Refinamiento OpenSpec de SCRUM-188 (`proposal`, `design`, `tasks`, `spec`).
- Paquete tecnico en `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine` para:
  - arquitectura,
  - implementacion,
  - pruebas,
  - observabilidad,
  - regresion legacy,
  - metadata.

## Contratos Funcionales Cubiertos
- Contrato de request para expediente/unidad:
  - `ExpedienteStorageDto.IdClaseDocumento` queda disponible para reglas de negocio.
- Contrato de resolución de path físico:
  - `IStoragePathResolver` recupera operaciones necesarias para construir ruta final en escenarios legacy.

## Flujo de Ejecucion Cubierto
1. Pipeline de validacion evalua reglas de inventario/TRD/unidad segun opciones resueltas por gabinete.
2. Si aplica unidad/expediente, el request debe aportar `IdClaseDocumento`.
3. En fase fisica, el resolved path builder dispone de `GetStorageRoot` y `GetFinalFolder`.
4. TransactionCoordinator opera con opciones resueltas y no ejecuta ramas no aplicables.

## Reglas Funcionales Consolidadas (VB -> C#)
- Inventario activo:
  - exige bloque `Inventario`,
  - exige `IdUsuarioGestion`,
  - exige `IdEmpresa`.
- TRD activa:
  - exige bloque TRD y claves minimas segun regla vigente del dominio.
- Unidad activa:
  - exige contexto de expediente/unidad y clase documental cuando corresponde.

## Validaciones y Errores Esperados
- Si faltan datos requeridos por opcion activa, la validacion debe fallar antes de persistencia.
- Si una opcion esta desactivada, no se deben exigir datos de esa rama.
- Si carpeta/disco son invalidos para ruta final, el resolver fisico debe rechazar la operacion.

## DI y Acoplamiento
- Se mantiene separacion por responsabilidad:
  - DTOs: contrato de entrada.
  - Services: resolucion de rutas y reglas de acceso.
  - Core: orquestacion, OpenSpec y evidencia tecnica.
- Sin introducir hardcode de opciones; las decisiones deben permanecer gobernadas por configuracion de gabinete.

## Pruebas y Evidencia
- Evidencia runtime principal:
  - `MiApp.DTOs` PR #67
  - `MiApp.Services` PR #123
- Evidencia de trazabilidad y cobertura documental:
  - `DocuArchiCore` PR #242

## Riesgos Atendidos
- Brecha de contrato entre validadores y DTO (`IdClaseDocumento` inexistente).
- Regresion por metodos de ruta faltantes en `IStoragePathResolver`.
- Conflictos de merge por duplicidad de implementacion entre tickets.

## Relacion con SCRUM-180
Este documento adopta el mismo nivel de detalle operacional del modelo SCRUM-180:
- resumen de cambios,
- desglose por capa,
- contratos,
- flujo,
- riesgos,
- trazabilidad por PR.
