# SCRUM-182 - Observabilidad Tamano y Conteo Real de Paginas

## Señales Operativas
Logs esperados en componentes de metadata:
- requestId
- gabinete
- archivo principal
- paginas resueltas
- fuente de paginas (`fisico` o `fallback`)

## Alertas Operativas
- parser retorna `null` para tipo no soportado y el comando no trae paginas.
- diferencia significativa entre paginas declaradas y detectadas.
- fallo al leer archivo temporal previo a persistencia.

## Politica de Datos Sensibles
No loguear:
- contenido documental,
- fulltext,
- bytes de archivo en bruto.

Solo loguear:
- extension,
- tamano agregado,
- conteos,
- codigos de error.
