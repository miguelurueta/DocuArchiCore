# SCRUM-43 - Migracion RetornaIdNombreSedeEmpresa

## Origen (VB.NET)
- Archivo: `D:\imagenesda\GestorDocumental\Desarrollo\Visual-2019\GestionDocumental-Docuarchi.net\GestionDocumental-Docuarchi.net\radicador\Class_usuario_radicador.vb`
- Funcion: `Retorna_Id_Nombre_sede_Empresa`
- Comportamiento original:
  - Consulta `usuario_radicador` + `sedes_empresa`.
  - Filtra por `id_usuario`.
  - Retorna `ID_SEDES_EMPRESA`, `NOMBRE_SEDE`.
  - Respuesta textual: `YES` o mensaje de error.

## Destino (.NET Core)
- Repositorio:
  - `MiApp.Repository/Repositorio/GestorDocumental/Sede/SedeEmpresaR.cs`
  - Interfaz: `ISedeEmpresaR`
  - Metodo: `RetornaIdNombreSedeEmpresa(int idUsuarioRadicador, string defaultDbAlias)`
- DTO:
  - `MiApp.DTOs/DTOs/GestorDocumental/Sede/IdSedeNombreDto.cs`
  - Campos:
    - `IdSede`
    - `NombreSede`
- Inyeccion DI:
  - `DocuArchi.Api/Program.cs`
  - Registro: `AddScoped<ISedeEmpresaR, SedeEmpresaR>()`

## Query Migrada
```sql
SELECT
  se.ID_SEDES_EMPRESA AS IdSede,
  se.NOMBRE_SEDE AS NombreSede
FROM usuario_radicador ur
INNER JOIN sedes_empresa se
  ON se.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA = ur.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA
 AND se.ID_SEDES_EMPRESA = ur.SEDES_EMPRESA_ID_SEDES_EMPRESA
WHERE ur.id_usuario = @idUsuarioRadicador
LIMIT 1;
```

## Contrato de Respuesta
- Tipo: `AppResponses<IdSedeNombreDto>`
- Exito:
  - `success = true`
  - `message = "YES"`
  - `data = { IdSede, NombreSede }`
- Sin datos:
  - `success = false`
  - `message = "Imposible encontrar id sede nombre sede del id usuario radicador (...)"`
  - `errors` con `Field = "idUsuarioRadicador"`
- Excepcion:
  - `success = false`
  - `message = "Inconsistencia funcion RetornaIdNombreSedeEmpresa ..."`

## Pruebas
- Archivo: `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/SedeEmpresaRepositoryTests.cs`
- Casos:
  - Retorna `YES` cuando hay datos.
  - Retorna error cuando no hay filas.
  - Construye `QueryOptions` esperada (tabla, join, filtros, alias, limit).
