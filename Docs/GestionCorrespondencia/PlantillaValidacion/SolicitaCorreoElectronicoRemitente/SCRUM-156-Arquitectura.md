# SCRUM-156 — Arquitectura

## Objetivo

Exponer la API `SolicitaCorreoElectronicoRemitente` para resolver el correo electrónico del remitente de forma dinámica a partir de:

- Script activo asociado a `idPlantillaRadicado`
- Plantilla de validación relacionada
- Campo marcado como correo electrónico
- Tabla/columna destino validada contra `INFORMATION_SCHEMA`

## Arquitectura

- `Controller` (validación de claim + parámetros + logging)
- `Service` (orquestación, normalización de `meta.status`)
- `Repository` (lectura de configuración + validaciones de seguridad + consulta final)

## Seguridad

- Claim obligatorio: `defaulalias`
- Consulta dinámica segura:
  - Validación regex de identificadores (`[A-Za-z0-9_]+`)
  - Validación de existencia en `INFORMATION_SCHEMA` (tabla y columnas)
  - Parametrización de `idDestinatarioExterno`

