# SCRUM-191 - Implementacion Detallada Paridad Fisica y XML

## Resumen Tecnico
Este ticket implementa (o deja especificado para implementacion en repos satelite) el cierre de brechas de paridad fisica/XML detectadas contra el flujo legacy VB.

## Componentes y Cambios Esperados

## 1) Resolucion de ruta final
- Componente: `StoragePlanBuilder` + `IStoragePhysicalPathService`.
- Regla: usar ruta final legacy (`SYSTEM1RUT + gabinete + disco + carpeta`) y no `%TEMP%`.
- Validaciones:
  - `nombreGabinete` requerido
  - disco/carpeta validos
  - path traversal bloqueado

## 2) Naming fisico
- Componente: `StorageFileWriter`.
- Regla principal:
  - Documento principal: `DIG########.<ext>`
  - Adjuntos: `DIG########.0000`, `DIG########.0001`, ...
- Regla de seguridad:
  - no sobrescribir silenciosamente
  - reportar conflicto con error tipado

## 3) XML documental (FXL)
- Componente: `StorageXmlWriter`.
- Estructura objetivo legacy:
  - nodo raiz: `Gabinetes`
  - nodo documento: `Gabinete` con atributos
- Campos minimos:
  - `ID`, `DISC`, `PAG`, `IDEX`, `USER`, `DATE1`, `TIME1`
  - atributos dinamicos de indexacion

## 4) XML indice de expediente
- Componente: `ExpedienteIndiceXmlWriter`.
- Fuente de ruta legacy (diferente a almacenamiento documental):
  - `ra_ruta_expediente.RUTA`
  - `ra_ruta_expediente.DISCO`
  - `id_expediente` con zero-fill a 9
- Nota: esta ruta es independiente de `SYSTEM1RUT`.
- Regla:
  - agregar `DocumentoIndizado` en `tipodocumentoFoliado`
  - escritura segura con archivo temporal y reemplazo atomico
- Compatibilidad:
  - mantener semantica de orden/paginacion legacy

## 5) Compensacion
- Componente: `StorageCompensationManager`.
- Regla:
  - ante fallo fisico/XML post-commit, remover solo artefactos creados en la transaccion
  - registrar eventos con `requestId`, ruta y resultado

## Dependencias de Integracion
- `Program.cs` debe registrar las implementaciones reales (sin stubs/placeholders).
- `StorageTransactionCoordinator` y fase fisica deben consumir el plan final ya resuelto.

## Estado en este repositorio
- Este repositorio (`DocuArchiCore`) contiene OpenSpec y documentacion.
- La implementacion de codigo reside en repos satelite (`MiApp.Services`, y si aplica `MiApp.Repository`/`MiApp.Models`).
