# SCRUM-43 - Migracion SolicitaIdAreaNombreAreaDestinatario

## Origen
- Funcion VB: `Solicita_id_area_nombre_area_destinatario`
- Archivo:
  `D:/imagenesda/GestorDocumental/Desarrollo/Visual-2019/GestionDocumental-Docuarchi.net/GestionDocumental-Docuarchi.net/radicador/Class_remit_dest_interno.vb`

## Destino
- Repositorio: `MiApp.Repository`
- Clase: `RemitDestInternoR`
- Metodo nuevo:
  `SolicitaIdAreaNombreAreaDestinatario(int idUsuarioDestinatario, string defaultDbAlias)`
- Retorno:
  `AppResponses<NombreAreaRemitdestDto>`

## DTO
- Proyecto: `MiApp.DTOs`
- Clase: `NombreAreaRemitdestDto`
- Propiedades:
  - `IdArea`
  - `NombreArea`

## Comportamiento Migrado
- Consulta:
  - Tabla base: `remit_dest_interno AS rdi`
  - Join: `areas_depart_radicacion AS adr ON adr.Codigo_Area = rdi.Areas_Dep_Radicacion_id_Areas_Dep`
  - Filtro: `id_Remit_Dest_Int = idUsuarioDestinatario`
- Si hay datos: `success=true`, `message=YES`, DTO con area.
- Si no hay datos: error de negocio equivalente al origen.
- Si hay excepcion: error tecnico en `AppResponses.errors`.

## Pruebas
- Archivo:
  `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/RemitDestInternoRepositoryTests.cs`
- Casos:
  - Retorno exitoso con datos.
  - Retorno de error cuando no hay filas.
  - Verificacion de armado de `QueryOptions`.
