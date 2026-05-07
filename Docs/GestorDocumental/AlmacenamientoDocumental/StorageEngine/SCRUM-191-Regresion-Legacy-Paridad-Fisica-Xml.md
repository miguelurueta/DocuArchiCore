# SCRUM-191 - Regresion Legacy Paridad Fisica y XML

## Referencia Legacy
- Fuente: `funcion-almacena-consolidad.txt`
- Bloques funcionales:
  - construccion de ruta fisica de almacenamiento
  - naming `DIG` y `FXL`
  - generacion XML de metadata
  - actualizacion XML indice expediente
  - manejo de error y rollback fisico

## Tabla de Equivalencia VB vs C#

| Regla Legacy VB | Componente C# objetivo | Estado |
|---|---|---|
| Ruta final basada en configuracion gabinete/disco/carpeta | `StoragePhysicalPathService` + `StoragePlanBuilder` | En validacion SCRUM-191 |
| Nombre principal `DIG########.ext` | `StorageFileWriter`/naming service | En validacion SCRUM-191 |
| Adjuntos `.0000+` | `StorageFileWriter` | En validacion SCRUM-191 |
| XML `Gabinetes/Gabinete` con atributos | `StorageXmlWriter` | En validacion SCRUM-191 |
| Insercion `DocumentoIndizado` en XML expediente | `ExpedienteIndiceXmlWriter` | En validacion SCRUM-191 |
| Compensacion fisica ante fallo post-commit | `StorageCompensationManager` | En validacion SCRUM-191 |

## Criterio de Paridad
- `CUMPLE`: comportamiento equivalente al legacy para el escenario probado.
- `CUMPLE CON DESVIACION APROBADA`: desviacion documentada con aprobacion arquitectonica.
- `NO APLICA`: regla no usada por alcance del caso.

## Resultado de Regresion del Ticket
- Este documento define la base de comparacion.
- El cierre final depende de evidencia de pruebas unitarias/integracion en los repos de implementacion (`MiApp.Services`).
