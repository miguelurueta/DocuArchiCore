# SCRUMCORE-144 — Pruebas

## Unitarias Backend (mínimas)

- Con datos → `meta.status="success"`
- Sin datos definitivos → `meta.status="empty"`
- `TramiteDocumento` se mapea cuando exista

## Integración Backend

- Llamadas repetidas al mismo `idTareaWf` no alternan `empty`↔`success` sin explicación (alias/base/cambio real de datos).

## Frontend

- `empty` mantiene bloqueo sin flicker.
- `success` renderiza detalle directamente.
- `pending` muestra skeleton y no bloqueo definitivo.
- Fallback: ausencia de `meta.status` no rompe.

## QT

- Abrir `/dashboard/gestion-correspondencia/respuesta/934` y validar ausencia de parpadeo.

## Estado de ejecución local (CI/SDK)

En este entorno, `dotnet build` para `DocuArchi.Api`, `MiApp.Services` y `MiApp.Repository` está fallando antes de compilar por una resolución de SDK/workload:

- Error observado: `MSB4276` relacionado con `Microsoft.NET.SDK.WorkloadManifestTargetsLocator` (no encontrado en `C:\Program Files\dotnet\sdk\10.0.102\Sdks\...`).

Acciones sugeridas:
- Ejecutar en un agente/VM con workloads/SDK completos (o con el SDK alineado a lo esperado por el repo).
- Alternativamente, instalar/repair los workloads del SDK .NET que correspondan en la máquina de build.
