# SCRUM-169 Seguridad Hash

## Algoritmo
- SHA256 en `IndiceElectronicoBuilder`.

## Razon
- Hash deterministico y reproducible para trazabilidad de indice electronico.
- Evita algoritmos obsoletos como MD5.

## Consideraciones
- La huella actual se calcula sobre metadatos lógicos del documento/indice.
- Futuro recomendado: incorporar hash físico del archivo persistido cuando la ruta definitiva esté disponible en el flujo.
