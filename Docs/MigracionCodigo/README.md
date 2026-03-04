# Migracion de Codigo Legacy a DocuArchiCore

## Objetivo

Esta guia define como migrar funcionalidades desde el sistema legacy VB.NET al entorno actual `DocuArchiCore`, usando tickets Jira como contrato tecnico y evitando replicar errores del legado.

## Contexto de migracion

### Repositorio fuente (legacy)
- Ruta: `D:\imagenesda\GestorDocumental\Desarrollo\Visual-2019\GestionDocumental-Docuarchi.net\GestionDocumental-Docuarchi.net`
- Funcion de referencia actual: `radicador/ClassRadicador.vb -> Registra_Radicacion_entrante`

### Repositorio destino
- Ruta: `D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore`

### Mapeo de conexiones legacy -> base de datos -> alias
- `conect.Dbase_Conction_Mysql` -> `workflowtconta` -> alias `WF`.
- `conect.Dbase_Conction_Mysql_RA` -> `docuarchi` -> alias `DA`.
- `conect.Dbase_Conction_Mysql_DA` -> `docuarchi` -> alias `DA`.

### Regla de acceso a datos
- Toda consulta nueva debe ser parametrizada.
- Usar `QueryOptions.DefaultAlias = defaultDbAlias`.
- Responder siempre con `AppResponses`.

### Regla de calidad
- No copiar defectos del legacy.
- Si se detecta error en origen, se documenta la correccion en el ticket.
- Aplicar Clean Architecture, SOLID, separacion de responsabilidades, codigo testeable y validacion de datos.

## Flujo recomendado

1. Ejecutar analisis en modo exploracion (`opsx:explore`).
2. Levantar alcance funcional de la funcion legacy y dependencias.
3. Detectar errores/deuda tecnica del legado.
4. Redactar ticket Jira de migracion con correcciones incluidas.
5. Dividir implementacion por etapas (tickets secuenciales):
   - base API/contratos
   - validaciones
   - autorizacion/workflow
   - persistencia/transacciones
   - integraciones auxiliares
   - pruebas de paridad
   - hardening/documentacion
6. Cargar ticket en Jira.
7. Ejecutar `opsxj:new -- <TICKET>` para iniciar implementacion operativa.
8. Implementar en destino por patron:
   - `Controller -> Service -> Repository -> DTO/Model/Mapping -> Tests/Docs`.
9. Validar con pruebas unitarias e integracion.
10. Adjuntar evidencia tecnica en ticket.

## Ejemplo practico (Registra_Radicacion_entrante)

### Objetivo de migracion
Migrar la logica de `Registra_Radicacion_entrante` a una arquitectura por capas en `DocuArchiCore`.

### Decisiones tecnicas
- Validaciones de entrada se ejecutan en Service.
- Controller solo orquesta y maneja errores controlados.
- Repository aplica alias segun origen legacy (`WF`/`DA`).
- SQL parametrizado para prevenir injection.

### Ejemplo de seudoflujo
1. Controller recibe request de radicacion.
2. Service valida datos obligatorios y reglas de negocio.
3. Service consulta autorizacion de horario con alias `WF`.
4. Service consulta usuario de radicacion y persistencia con alias `DA`.
5. Repository ejecuta query parametrizada con `DefaultAlias`.
6. Service retorna `AppResponses` con resultado final.

### Ejemplo de criterios de aceptacion
1. Si `defaultDbAlias` es vacio, retorna error controlado.
2. Si no hay usuario radicador relacionado, retorna validacion funcional.
3. Si la persistencia falla, retorna `success=false` con `errorMessage`.
4. En exito, retorna consecutivo de radicacion y metadata de aliases usados.

## Estructura sugerida de ticket Jira de migracion

1. Contexto origen/destino.
2. Alcance funcional.
3. Correcciones incluidas (no replicar errores legacy).
4. Reglas de datos/alias y tablas.
5. Requerimientos por capa (Controller/Service/Repository).
6. Criterios de aceptacion.
7. Plan de pruebas (unitarias/integracion/paridad).
8. Evidencia esperada (diff, pruebas, decisiones tecnicas).

## Referencia de contexto local

Para este tipo de solicitudes, usar:
- `openspec/context/CONTEXTO-SOLO-MIGRACION-LEGACY-A-TICKETS-JIRA.md`
