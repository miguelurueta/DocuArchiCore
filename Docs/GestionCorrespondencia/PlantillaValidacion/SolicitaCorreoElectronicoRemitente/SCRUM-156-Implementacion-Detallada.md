# SCRUM-156 — Implementación detallada

## Archivos

- Controller: `..\DocuArchi.Api\Controllers\GestionCorrespondencia\PlantillaValidacion\SolicitaCorreoElectronicoRemitenteController.cs`
- Service: `..\MiApp.Services\Service\GestionCorrespondencia\PlantillaValidacion\SolicitaCorreoElectronicoRemitente\ServiceSolicitaCorreoElectronicoRemitente.cs`
- Repository: `..\MiApp.Repository\Repositorio\GestionCorrespondencia\PlantillaValidacion\SolicitaCorreoElectronicoRemitente\SolicitaCorreoElectronicoRemitenteRepository.cs`
- DI: `..\DocuArchi.Api\Program.cs`

## Flujo end-to-end

1. Controller valida claim `defaulalias` y query params.
2. Service valida inputs, llama a repository y normaliza `meta.status`.
3. Repository:
   - Obtiene `Id_Script` en `ra_script_actividades` (por `system_plantilla_radicado_id_Plantilla`).
   - Obtiene `Id_Plantilla_Validacion` por `relacion_script_plantilla`.
   - Obtiene `Nombre_Campo` (correo) en `campos_plantilla_validacion`.
   - Obtiene `Nombre_Plantilla` (tabla destino) en `plantilla_validacion`.
   - Obtiene PK por `campos_plantilla_validacion`.
   - Valida tabla/columnas contra `INFORMATION_SCHEMA`.
   - Ejecuta query final parametrizada por `idDestinatarioExterno`.

