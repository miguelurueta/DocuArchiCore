# SCRUM-207 - Integración Frontend (Contrato Completo)

## 1. Endpoints

## 1.1 Resolver gabinete por radicado
`GET /api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete`

## 1.2 Resolver gabinete por id tarea workflow
`GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`

## 2. Seguridad requerida
- Header:
  - `Authorization: Bearer <JWT>`
- Claims obligatorios:
  - `defaulaliaswf`
- Claim opcional:
  - `defaulalias` (solo compatibilidad; la resolución de `configuracion_gabinete` se ejecuta con alias workflow)

## 3. Contrato de respuesta

## 3.1 Wrapper
| Campo | Tipo | Requerido | Descripción |
|---|---|---|---|
| `success` | `bool` | Sí | Estado funcional del proceso |
| `message` | `string` | Sí | Mensaje funcional (`YES` en éxito) |
| `data` | `RadicadoGabineteWorkflowDto` | Sí | Payload de gabinete |
| `meta` | `AppMeta` | No | Metadatos del estado |
| `errors` | `array` | No | Lista de errores controlados |

## 3.2 `RadicadoGabineteWorkflowDto`
| Campo | Tipo | Requerido | Descripción |
|---|---|---|---|
| `Radicado` | `string` | Sí | Consecutivo del radicado |
| `IdTareaWorkflow` | `long` | Sí | Identificador de tarea workflow |
| `IdGabinete` | `long` | Sí | Identificador interno de gabinete |
| `NombreGabinete` | `string` | Sí | Nombre lógico del gabinete (`configuracion_gabinete.Nombre_Gabinete`) |
| `EstadoExistenciaRadicado` | `string` | Sí | `YES` si existe fila en ruta; `NO` si no existe |

## 4. Ejemplos completos de consumo

## 4.1 Por radicado (caso encontrado)
```bash
curl -X GET "https://localhost:7101/api/workflow/ruta-trabajo/radicados/2500466700035/gabinete" ^
  -H "Authorization: Bearer <JWT>"
```

Respuesta:
```json
{
  "success": true,
  "message": "YES",
  "data": {
    "Radicado": "2500466700035",
    "IdTareaWorkflow": 98765,
    "IdGabinete": 12,
    "NombreGabinete": "CORRESPO",
    "EstadoExistenciaRadicado": "YES"
  },
  "meta": {
    "Status": "success"
  },
  "errors": []
}
```

## 4.2 Por radicado (caso no encontrado)
```json
{
  "success": true,
  "message": "YES",
  "data": {
    "Radicado": "2500466700035",
    "IdTareaWorkflow": 0,
    "IdGabinete": 0,
    "NombreGabinete": "",
    "EstadoExistenciaRadicado": "NO"
  },
  "meta": {
    "Status": "success"
  },
  "errors": []
}
```

## 4.3 Por idTareaWorkflow (caso encontrado)
```bash
curl -X GET "https://localhost:7101/api/workflow/ruta-trabajo/tareas/98765/gabinete" ^
  -H "Authorization: Bearer <JWT>"
```

Respuesta:
```json
{
  "success": true,
  "message": "YES",
  "data": {
    "Radicado": "2500466700035",
    "IdTareaWorkflow": 98765,
    "IdGabinete": 12,
    "NombreGabinete": "CORRESPO",
    "EstadoExistenciaRadicado": "YES"
  },
  "meta": {
    "Status": "success"
  },
  "errors": []
}
```

## 4.4 Error por claim faltante
```json
{
  "success": false,
  "message": "No se encontró el claim 'defaulaliaswf'.",
  "data": "",
  "errors": [
    {
      "Type": "Claim",
      "Field": "defaulaliaswf",
      "Message": "El claim 'defaulaliaswf' es requerido para continuar."
    }
  ]
}
```

## 4.5 Error por `NombreGabinete` no resuelto
```json
{
  "success": false,
  "message": "NombreGabinete requerido",
  "data": {
    "Radicado": "2500466700035",
    "IdTareaWorkflow": 933,
    "IdGabinete": 15,
    "NombreGabinete": "",
    "EstadoExistenciaRadicado": "YES"
  },
  "errors": [
    {
      "Type": "Validation",
      "Field": "NombreGabinete",
      "Message": "No fue posible resolver el nombre del gabinete para el documento."
    }
  ]
}
```

## 4.6 Error por parámetro inválido (`idTareaWorkflow <= 0`)
```json
{
  "success": false,
  "message": "IdTareaWorkflow requerido",
  "data": {
    "Radicado": "",
    "IdTareaWorkflow": 0,
    "IdGabinete": 0,
    "NombreGabinete": "",
    "EstadoExistenciaRadicado": "NO"
  },
  "errors": [
    {
      "Type": "Validation",
      "Field": "idTareaWorkflow",
      "Message": "IdTareaWorkflow requerido"
    }
  ]
}
```

## 5. Reglas funcionales importantes para frontend
1. Frontend nunca envía `nombreRuta`; se resuelve internamente.
2. `success=true` con `EstadoExistenciaRadicado=NO` no es error HTTP, es "sin registro".
3. Si `EstadoExistenciaRadicado=YES`, `NombreGabinete` debe venir informado; de lo contrario el backend responde `success=false`.
4. Si `success=false`, tomar `message` como error de negocio/técnico controlado.

## 6. Ejemplo recomendado en React/TypeScript
```ts
type RadicadoGabineteWorkflowDto = {
  Radicado: string;
  IdTareaWorkflow: number;
  IdGabinete: number;
  NombreGabinete: string;
  EstadoExistenciaRadicado: "YES" | "NO";
};

type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T;
  errors?: Array<{ Type?: string; Field?: string; Message?: string }>;
};

export async function obtenerGabinetePorRadicado(baseUrl: string, jwt: string, radicado: string) {
  const url = `${baseUrl}/api/workflow/ruta-trabajo/radicados/${encodeURIComponent(radicado)}/gabinete`;
  const res = await fetch(url, { headers: { Authorization: `Bearer ${jwt}` } });
  const json = (await res.json()) as ApiResponse<RadicadoGabineteWorkflowDto>;

  if (!res.ok || !json.success) {
    throw new Error(json.message || "Error consultando gabinete workflow");
  }

  return json.data;
}
```
