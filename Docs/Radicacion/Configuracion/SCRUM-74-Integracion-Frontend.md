# SCRUM-74 Integracion Backend

## Objetivo

Consultar la estructura de `ra_rad_config_plantilla_radicacion` por `idPlantilla` desde un repository dedicado, sin crear service ni API nuevos.

## Componente principal

- Repository: [SolicitaListaEstructuraConfiguracionPlantillaRadicacionRepository.cs](D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Repository/Repositorio/Configuracion/SolicitaListaEstructuraConfiguracionPlantillaRadicacionRepository.cs)
- Modelo reutilizado: [RaRadConfigPlantillaRadicacion.cs](D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Models/Models/Radicacion/Configuracion/RaRadConfigPlantillaRadicacion.cs)

## Firma implementada

```csharp
Task<AppResponses<List<RaRadConfigPlantillaRadicacion>?>> SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync(
    int idPlantilla,
    string defaultDbAlias)
```

## Consumo interno

```csharp
var result = await repository.SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync(67, "DA");
```

## Respuesta

- con datos: `success = true`, `message = "OK"`, `data = List<RaRadConfigPlantillaRadicacion>`
- sin datos: `success = true`, `message = "Sin resultados"`, `data = null`
- error: `success = false`, `errors` con detalle técnico
