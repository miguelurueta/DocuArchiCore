# Diagrama de Clases

```mermaid
classDiagram
    class RadicacionController {
      +RegistrarEntrante(request)
      +ValidarEntrante(request)
      +FlujoInicial(idTipoTramite)
    }

    class IRegistrarRadicacionEntranteService {
      +RegistrarRadicacionEntranteAsync(request,idUsuarioGestion,defaultDbAlias)
    }

    class IValidarRadicacionEntranteService {
      +ValidarRadicacionEntranteAsync(request)
    }

    class IFlujoInicialRadicacionService {
      +ObtenerFlujoInicialAsync(idTipoTramite,defaultDbAlias)
    }

    class IRegistrarRadicacionEntranteRepository {
      +RegistrarRadicacionEntranteAsync(request,idUsuarioRadicador,idUsuarioGestion,defaultDbAlias,ip,modulo,estructuraPlantilla,camposDinamicos,parametros,citaTipoDocEntrante)
    }

    class IRegistroRadicacionPolicyService {
      +Evaluate(request,citaTipoDocEntrante)
    }

    class RegistroRadicacionDecision {
      +EjecutaQ06Q07: bool
      +EjecutaQ08: bool
      +EjecutaQ09: bool
    }

    class RegistrarRadicacionEntranteRequestDto
    class RegistrarRadicacionEntranteResponseDto
    class ValidarRadicacionEntranteRequestDto
    class ValidarRadicacionEntranteResponseDto
    class FlujoInicialDto

    RadicacionController --> IRegistrarRadicacionEntranteService
    RadicacionController --> IValidarRadicacionEntranteService
    RadicacionController --> IFlujoInicialRadicacionService
    IRegistrarRadicacionEntranteService --> IRegistrarRadicacionEntranteRepository
    IRegistrarRadicacionEntranteRepository --> IRegistroRadicacionPolicyService
    IRegistroRadicacionPolicyService --> RegistroRadicacionDecision
    IRegistrarRadicacionEntranteService --> RegistrarRadicacionEntranteRequestDto
    IRegistrarRadicacionEntranteService --> RegistrarRadicacionEntranteResponseDto
    IValidarRadicacionEntranteService --> ValidarRadicacionEntranteRequestDto
    IValidarRadicacionEntranteService --> ValidarRadicacionEntranteResponseDto
    IFlujoInicialRadicacionService --> FlujoInicialDto
```
