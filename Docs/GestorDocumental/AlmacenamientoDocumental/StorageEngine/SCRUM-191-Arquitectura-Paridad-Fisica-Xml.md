# SCRUM-191 - Arquitectura Paridad Fisica y XML

## Objetivo
Cerrar la paridad legacy VB de la fase fisica del Storage Engine en cuatro frentes: ruta final, naming, XML documental y XML de indice de expediente.

## Alcance Arquitectonico
- Componentes objetivo (`MiApp.Services`):
  - `StoragePhysicalPathService`
  - `StoragePlanBuilder`
  - `StorageFileWriter`
  - `StorageXmlWriter`
  - `ExpedienteIndiceXmlWriter`
  - `StorageCompensationManager`
- Referencia funcional legacy:
  - `funcion-almacena-consolidad.txt`
- No incluye:
  - cambios de tablas
  - cambios de contratos API
  - rediseño del pipeline transaccional

## Decisiones Arquitectonicas
1. La ruta final debe resolverse con configuracion real (`SYSTEM1RUT`) y no con rutas temporales.
2. El naming fisico debe mantener convencion legacy:
- principal `DIG########.ext`
- metadata XML `FXL########.xml`
- adjuntos `DIG########.0000`, `DIG########.0001`, ...
3. El XML documental debe ser compatible con el formato legacy (`Gabinetes/Gabinete` con atributos).
4. La actualizacion de XML de indice debe usar escritura segura (temp + replace atomico) para evitar corrupcion.
5. La compensacion debe eliminar solo artefactos generados por la operacion fallida y dejar trazabilidad por `RequestId`.

## Riesgos y Mitigaciones
- Riesgo: incompatibilidad con visor legacy por estructura XML distinta.
- Mitigacion: pruebas de regresion estructural sobre nodos y atributos obligatorios.

- Riesgo: colisiones o sobrescrituras por naming de adjuntos.
- Mitigacion: secuencia fija `.0000+` y validacion de existencia antes de mover.

- Riesgo: inconsistencias post-commit cuando falla FS/XML.
- Mitigacion: `StorageCompensationManager` con rollback fisico determinista.

## Criterio de Cierre Arquitectonico
- La fase fisica C# decide y materializa rutas/nombres/XML con las mismas reglas funcionales del legado VB para los escenarios soportados por SCRUM-191.
