# Pruebas De Concurrencia - Registrar Entrante

## Objetivo

Validar el comportamiento de `POST /api/radicacion/registrar-entrante` bajo carga concurrente para detectar colisiones de consecutivo y errores transaccionales.

## Cobertura Implementada

- Prueba automatizada: `RegistrarEntranteConcurrencyTests.RegistrarEntrante_CincuentaSolicitudesConcurrentes_GeneraConsecutivosUnicos`
- Escenario: 50 solicitudes concurrentes.
- Validaciones:
  - todas las respuestas retornan `success=true`
  - se generan 50 consecutivos
  - no hay duplicados de consecutivo

## Script Base k6 (Referencia)

```javascript
import http from "k6/http";
import { check } from "k6";

export const options = {
  vus: 50,
  iterations: 50
};

const payload = JSON.stringify({
  IdPlantilla: 67,
  ASUNTO: "Prueba registro",
  Remitente: { Nombre: "EMSA", id_Dest_Ext: 7 },
  Destinatario: { Destinatario: "Carolina Cruz Villareal Nuñes", id_Remit_Dest_Int: 136 },
  Tipo_tramite: { Descripcion: "DERECHOS DE PETECION", tipo_doc_entrante: 1 },
  TipoRadicado: { TipoRadicacion: "EXTERNA", IdTipoRadicado: 1 },
  TipoPlantillaRadicado: { TipoPlantillaRadicado: "", IdTipoPlantillaRdicado: 1 },
  ANEXOS_COR: "NA",
  FECHALIMITERESPUESTA: "2026-03-08",
  numeroFolios: 10,
  Campos: []
});

export default function () {
  const res = http.post("https://localhost:5001/api/radicacion/registrar-entrante", payload, {
    headers: { "Content-Type": "application/json" }
  });

  check(res, {
    "status 200 or 400": (r) => r.status === 200 || r.status === 400
  });
}
```
