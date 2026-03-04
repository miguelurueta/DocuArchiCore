# JIRA Ticket - SCRUM-38

En el proyecto multirepo necesito migrar la lógica funcional de radicación entrante del sistema legacy (`Registra_Radicacion_entrante`) al entorno actual, lista para implementación por capas `Controller -> Service -> Repository`.

## Configuración de rutas (único lugar)

- Repositorio origen (legacy): `GestionDocumental-Docuarchi.net`
- Ruta archivo origen: `/radicador/ClassRadicador.vb`
- Función origen: `Registra_Radicacion_entrante`
- Repositorio destino: `DocuArchiCore`
- Ruta Controller destino: **POR DEFINIR**
- Ruta Service destino: **POR DEFINIR**
- Ruta Repository destino: **POR DEFINIR**
- Ruta Modelos destino: **POR DEFINIR**
- Ruta DTOs destino: **POR DEFINIR**
- Ruta Mapping destino: **POR DEFINIR**
- Ruta Docs técnicas destino: **POR DEFINIR**

## Objetivo

Desarrollar la especificación técnica y funcional para trasladar el comportamiento actual de radicación entrante al nuevo entorno, sin modificar el código legacy.

## Dependencias

1. Flujo actual de validaciones, permisos, plantilla y asignación contenido en `Registra_Radicacion_entrante`.
2. Componentes del entorno destino para `Controller`, `Service`, `Repository`, `Model`, `DTO` y `Mapping`.
3. Contrato de respuesta estándar `AppResponses`.

---

## Controller: API de Radicación Entrante

### Parámetros

- `defaultDbAlias`
- DTO de solicitud de radicación entrante

### Requerimientos funcionales del Controller

1. Exponer endpoint para registrar radicación entrante en el nuevo entorno.
2. Retornar estructura envuelta en `AppResponses`.
3. Implementar manejo controlado de errores con bloque `try/catch`.
4. Delegar toda la lógica de negocio al Service.

---

## Servicio: Migración de lógica de `Registra_Radicacion_entrante`

### Parámetros

- `defaultDbAlias`
- DTO de entrada de radicación

### Dependencias del Servicio

1. Repositorio de usuario/gestión para datos de radicador.
2. Repositorio de configuración/plantilla de radicación.
3. Repositorio de persistencia y consulta de estado/radicado.
4. Mapper para transformación entre entidades y DTOs.

### Requerimientos funcionales del Servicio

1. Replicar el comportamiento funcional de la función legacy en el nuevo entorno.
2. Separar validaciones por etapas:
   - controles requeridos
   - plantilla y flujo
   - campos obligatorios
   - validaciones de fecha
3. Estandarizar la salida con `AppResponses`.
4. Envolver el servicio en bloque `try/catch`.
5. Definir respuestas para éxito, validación fallida, sin resultados y excepción.

---

## Repositorio: Operaciones de Radicación Entrante

### Parámetros

- Identificadores requeridos por reglas del servicio (usuario, plantilla, flujo, estado).
- `defaultDbAlias`

### Dependencias

- `IDapperCrudEngine` o motor de acceso a datos existente.
- Configuración de conexión por alias.

### Requerimientos funcionales del Repositorio

1. Implementar consultas parametrizadas (evitar SQL Injection).
2. Retornar estructura envuelta en `AppResponses`.
3. Incluir `DefaultAlias = defaultDbAlias` en `QueryOptions`.
4. Crear interfaz junto a cada implementación para facilitar pruebas unitarias.
5. Registrar interfaz e implementación en `Program.cs`.
6. Si no hay registros: retornar `Success=true`, `Data=null`, `Message="Sin resultados"`.
7. En excepción: retornar `Success=false`, `ErrorMessage=ex.Message`, `Data` vacío.
8. Documentar funciones con comentarios XML.

---

## Modelo y DTO

### Requerimientos

1. Definir modelos y DTOs necesarios para entrada/salida del flujo migrado.
2. Usar DataAnnotations cuando aplique para mapeo y validación.
3. Definir perfil de AutoMapper para conversiones.
4. Documentar propiedades y contratos con comentarios XML.
5. Mantener separación de responsabilidades entre modelo persistencia y DTO de transporte.

---

## Requerimientos adicionales generales

1. Seguir patrón `Controller -> Service -> Repository`.
2. Cumplir SoC, SOLID, bajo acoplamiento y alta cohesión.
3. Incluir pruebas unitarias por capa.
4. Incluir pruebas de integración con MySQL usando Testcontainers/Docker.
5. Cubrir escenarios de éxito, validación fallida, sin resultados y excepción.
6. Documentar API para frontend: endpoint, payload, DTO respuesta y códigos esperados.
7. Incluir diagramas técnicos (casos de uso, clases, secuencia y estado).

## Criterio final del ticket

1. El ticket debe servir como base de implementación de la migración funcional al nuevo entorno.
2. Este ticket **no** contempla corrección directa del código legacy ni requerimientos OpenSpec.
