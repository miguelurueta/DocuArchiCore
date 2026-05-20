# SCRUM-206 - Diagramas Permisos Visor PDF (UML / PlantUML)

## Alcance
Arquitectura de permisos del visor PDF por implementación/usuario, con precedencia efectiva y administración de overrides.

## 1) Diagrama de Casos de Uso
```plantuml
@startuml
left to right direction
actor "Frontend/Integrador" as FE
actor "Admin" as ADM
rectangle "Permisos Visor PDF" {
  usecase "Consultar mis permisos" as UC1
  usecase "Consultar permisos de usuario" as UC2
  usecase "Upsert override usuario" as UC3
  usecase "Eliminar override usuario" as UC4
}
FE --> UC1
ADM --> UC2
ADM --> UC3
ADM --> UC4
@enduml
```

## 2) Diagrama de Clases
```plantuml
@startuml
skinparam classAttributeIconSize 0

interface IPermisosVisorPdfService {
  +GetMyPermissionsAsync(codigoImpl,idUsuario,alias)
  +GetUserPermissionsAsync(codigoImpl,idUsuarioObjetivo,alias)
  +UpsertUserOverridesAsync(codigoImpl,idUsuarioObjetivo,request,alias)
  +DeleteUserOverrideAsync(codigoImpl,idUsuarioObjetivo,codigoPermiso,alias)
}

interface IPermisosVisorPdfRepository {
  +GetEffectivePermissionsAsync(codigoImpl,idUsuario,alias)
  +UpsertUserOverridesAsync(codigoImpl,idUsuario,overrides,alias)
  +DeleteUserOverrideAsync(codigoImpl,idUsuario,codigoPermiso,alias)
}

class PermisosVisorPdfController
class PermisosVisorPdfService
class PermisosVisorPdfRepository

IPermisosVisorPdfService <|.. PermisosVisorPdfService
IPermisosVisorPdfRepository <|.. PermisosVisorPdfRepository
PermisosVisorPdfController --> IPermisosVisorPdfService
PermisosVisorPdfService --> IPermisosVisorPdfRepository
@enduml
```

## 3) Diagrama de Secuencia
```plantuml
@startuml
autonumber
actor Front as Frontend
participant C as PermisosVisorPdfController
participant S as PermisosVisorPdfService
participant R as PermisosVisorPdfRepository
participant DB as DB(ra_vis_per_*)

Front -> C : GET /mis-permisos
C -> S : GetMyPermissionsAsync(codigoImpl,usuarioid,alias)
S -> R : GetEffectivePermissionsAsync(...)
R -> DB : leer catalogo/defaults/perfil/override
R --> S : permissions + sources
S --> C : AppResponses<VisorPdfPermissionsResponseDto>
C --> Front : 200

@enduml
```

## 4) Diagrama de Estados
```plantuml
@startuml
[*] --> RECEIVED
RECEIVED --> VALIDATED : claims + request ok
VALIDATED --> AUTHORIZED : endpoint admin autorizado
VALIDATED --> RESOLVED : endpoint mis-permisos
AUTHORIZED --> RESOLVED
RESOLVED --> CONSOLIDATED : precedence aplicada
CONSOLIDATED --> COMPLETED

VALIDATED --> FAILED : validacion
AUTHORIZED --> FAILED : no admin
RESOLVED --> FAILED : impl/permiso no existe
FAILED --> [*]
COMPLETED --> [*]
@enduml
```

## 5) Diagrama Visual De Tablas (ER)
```mermaid
erDiagram
    remit_dest_interno {
        int id_Remit_Dest_Int PK
        varchar Login_Usuario
        int Estado_Usuario
    }

    ra_vis_per_implementacion {
        int id_impl PK
        varchar codigo_impl
        int id_empresa
        int estado
    }

    ra_vis_per_permiso {
        int id_perm PK
        varchar codigo_perm
        int estado
    }

    ra_vis_per_impl_perm_default {
        int id_impl PK, FK
        int id_perm PK, FK
        int permitido
    }

    ra_vis_per_perfil {
        int id_perfil PK
        int id_impl FK
        varchar codigo_perfil
        int estado
    }

    ra_vis_per_perfil_permiso {
        int id_perfil PK, FK
        int id_perm PK, FK
        int permitido
    }

    ra_vis_per_usuario_perfil {
        int id_usuario PK, FK
        int id_impl PK, FK
        int id_perfil FK
        int estado
        date fecha_inicio
        date fecha_fin
    }

    ra_vis_per_usuario_override {
        int id_usuario PK, FK
        int id_impl PK, FK
        int id_perm PK, FK
        int permitido
        int estado
        varchar motivo
    }

    ra_vis_per_implementacion ||--o{ ra_vis_per_impl_perm_default : "defaults"
    ra_vis_per_permiso ||--o{ ra_vis_per_impl_perm_default : "permiso_default"

    ra_vis_per_implementacion ||--o{ ra_vis_per_perfil : "define_perfiles"
    ra_vis_per_perfil ||--o{ ra_vis_per_perfil_permiso : "matriz_permisos"
    ra_vis_per_permiso ||--o{ ra_vis_per_perfil_permiso : "permiso_perfil"

    remit_dest_interno ||--o{ ra_vis_per_usuario_perfil : "asignacion_perfil"
    ra_vis_per_implementacion ||--o{ ra_vis_per_usuario_perfil : "perfil_por_impl"
    ra_vis_per_perfil ||--o{ ra_vis_per_usuario_perfil : "perfil_asignado"

    remit_dest_interno ||--o{ ra_vis_per_usuario_override : "override_usuario"
    ra_vis_per_implementacion ||--o{ ra_vis_per_usuario_override : "override_impl"
    ra_vis_per_permiso ||--o{ ra_vis_per_usuario_override : "override_permiso"
```

### Lectura rápida del modelo
- `ra_vis_per_implementacion` define cada contexto (`workflow`, `gestion_correspondencia`, etc.).
- `ra_vis_per_permiso` es el catálogo global (`pdf.view`, `pdf.print`, ...).
- `ra_vis_per_impl_perm_default` guarda el baseline por implementación.
- `ra_vis_per_perfil` + `ra_vis_per_perfil_permiso` definen roles y su matriz.
- `ra_vis_per_usuario_perfil` asigna un perfil a un usuario por implementación y vigencia.
- `ra_vis_per_usuario_override` aplica excepciones por usuario.
