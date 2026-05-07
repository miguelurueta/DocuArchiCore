# SCRUM-191 - Pruebas Paridad Fisica y XML

## Objetivo de Prueba
Verificar que la fase fisica y XML del Storage Engine mantiene paridad funcional con el legacy VB para rutas, naming, XML documental, XML indice y compensacion.

## Matriz Minima de Pruebas

| ID | Escenario | Esperado |
|---|---|---|
| P191-01 | Ruta final desde `SYSTEM1RUT` | Ruta calculada en carpeta legacy correcta |
| P191-02 | Documento principal | Nombre `DIG########.ext` |
| P191-03 | Adjuntos multiples | Nombres `.0000`, `.0001`, `.0002` sin colision |
| P191-04 | XML FXL | Estructura `Gabinetes/Gabinete` + atributos obligatorios |
| P191-05 | XML indice expediente | Inserta `DocumentoIndizado` con orden/paginas correctas |
| P191-06 | Falla en copia fisica | Compensacion elimina archivos creados |
| P191-07 | Falla en escritura XML | Compensacion ejecutada y error trazable |
| P191-08 | Path traversal | Rechazo con error de validacion |

## Evidencia Requerida
- Logs de ejecucion por `requestId`.
- Resultado `dotnet test` (unit/integration) en repos satelite.
- Muestra de rutas y nombres generados.
- Diff/inspeccion de XML generado.

## Estado de Ejecucion
- En `DocuArchiCore`: se actualiza especificacion y criterios.
- En satelites (`MiApp.Services`): ejecucion pendiente/segun PR tecnico asociado a SCRUM-191.
