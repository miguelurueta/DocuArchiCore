# SCRUM-201 - Integración Frontend (Contrato Completo)

## 1. Endpoints

## 1.1 Solicitar metadata temporal de firma
`GET /api/workflow/usuarios/firma-temporal`

## 1.2 Descargar binario de firma
`GET /api/workflow/usuarios/firma-temporal/download/{token}`

## 2. Seguridad requerida
- Header:
  - `Authorization: Bearer <JWT>`
- Claims obligatorios en JWT:
  - `defaulaliaswf`
  - `IdUsuarioWorkflow`

## 3. Contrato de respuesta (endpoint metadata)

## 3.1 Wrapper
| Campo | Tipo | Requerido | Descripción |
|---|---|---|---|
| `success` | `bool` | Sí | Estado funcional del proceso |
| `message` | `string` | Sí | Mensaje funcional (`YES` en éxito) |
| `data` | `FirmaTemporalUsuarioWorkflowDto \| null` | Sí | Payload del recurso temporal |
| `meta` | `AppMeta` | No | Metadatos del estado (`success`, `validation`, `error`) |
| `errors` | `array` | No | Lista de errores controlados |

## 3.2 `FirmaTemporalUsuarioWorkflowDto`
| Campo | Tipo | Requerido | Descripción |
|---|---|---|---|
| `IdUsuarioWorkflow` | `int` | Sí | Usuario workflow autenticado |
| `FileName` | `string` | Sí | Nombre de archivo temporal |
| `ContentType` | `string` | Sí | MIME de salida normalizado (`image/png`) |
| `RelativePath` | `string` | Sí | Ruta relativa interna del temporal |
| `UrlTemporal` | `string` | Sí | URL de descarga temporal |
| `ExpiresAt` | `datetime` | Sí | Fecha/hora de expiración del token |

## 4. Ejemplo completo (flujo real frontend)

## 4.1 Paso 1 - solicitar metadata
```bash
curl -X GET "https://localhost:7101/api/workflow/usuarios/firma-temporal" ^
  -H "Authorization: Bearer <JWT>"
```

Respuesta esperada:
```json
{
  "success": true,
  "message": "YES",
  "data": {
    "IdUsuarioWorkflow": 141,
    "FileName": "firma_workflow_141_20260514163501_7ab8....png",
    "ContentType": "image/png",
    "RelativePath": "signatures/firma_workflow_141_20260514163501_7ab8....png",
    "UrlTemporal": "/api/workflow/usuarios/firma-temporal/download/b36f0f9f....",
    "ExpiresAt": "2026-05-14T21:50:01Z"
  },
  "meta": {
    "Status": "success"
  },
  "errors": []
}
```

## 4.2 Paso 2 - descargar blob de firma
```bash
curl -X GET "https://localhost:7101/api/workflow/usuarios/firma-temporal/download/b36f0f9f...." ^
  -H "Authorization: Bearer <JWT>" ^
  --output firma.png
```

## 4.3 Paso 3 - uso en frontend React (ejemplo completo)
```ts
type FirmaTemporalDto = {
  IdUsuarioWorkflow: number;
  FileName: string;
  ContentType: string;
  RelativePath: string;
  UrlTemporal: string;
  ExpiresAt: string;
};

type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T | null;
  meta?: { Status?: string };
  errors?: Array<{ Type?: string; Field?: string; Message?: string }>;
};

export async function cargarFirmaTemporal(baseUrl: string, jwt: string): Promise<Blob> {
  const resMeta = await fetch(`${baseUrl}/api/workflow/usuarios/firma-temporal`, {
    method: "GET",
    headers: { Authorization: `Bearer ${jwt}` }
  });

  const metaJson = (await resMeta.json()) as ApiResponse<FirmaTemporalDto>;
  if (!resMeta.ok || !metaJson.success || !metaJson.data) {
    throw new Error(metaJson.message || "No fue posible obtener metadata de firma");
  }

  const downloadUrl = metaJson.data.UrlTemporal.startsWith("http")
    ? metaJson.data.UrlTemporal
    : `${baseUrl}${metaJson.data.UrlTemporal}`;

  const resFile = await fetch(downloadUrl, {
    method: "GET",
    headers: { Authorization: `Bearer ${jwt}` }
  });

  if (!resFile.ok) {
    throw new Error(`No fue posible descargar firma temporal. status=${resFile.status}`);
  }

  return await resFile.blob();
}
```

## 5. Errores y manejo recomendado
| HTTP | Caso | Acción frontend |
|---|---|---|
| `400` | Claims inválidos/faltantes | Renovar sesión o relogin |
| `400` | Firma no configurada | Mostrar mensaje de negocio al usuario |
| `404` | Token expirado/no encontrado | Re-solicitar metadata y reintentar descarga |
| `500` | Error inesperado | Mostrar fallback y registrar incidente |

## 6. Recomendaciones para plugin de firma PDF
- Solicitar metadata justo antes de firmar (evitar expiración de token).
- No cachear la URL temporal más allá de su `ExpiresAt`.
- Guardar solo `Blob`/`ObjectURL` en memoria de sesión frontend.
- El backend normaliza formatos soportados (`png/jpg/bmp`) a salida `PNG` con transparencia aplicada.
