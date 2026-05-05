# SCRUM-180 — Compatibilidad Visor Legacy

## Objetivo
Garantizar que la ruta física final de almacenamiento preserve la estructura consumida por el visor legacy.

## Estructura Compatibilizada
- Root desde `SYSTEM1RUT` (`ruta_gabi`).
- Segmento intermedio: `{gabinete}{disco}`.
- Carpeta de almacenamiento: formato legacy 5 dígitos (`00001`, `00015`, ...).

## Equivalencia Estructural
- VB: `_Ruta_Almacenamiento + _Nombre_Gabienete + disc + "\" + carpealma`
- C#: `StorageFolderLegacyPolicy.ResolveFolder(...)`

## Riesgos de Compatibilidad Controlados
- Eliminación de destino temporal no legacy (`Path.GetTempPath()`).
- Consistencia de naming/ruta para recuperación documental.
- Bloqueo de rutas fuera de root esperado.

## Resultado Esperado
Los documentos quedan en ubicación física equivalente a legacy, permitiendo navegación/recuperación por componentes dependientes de la estructura histórica.
