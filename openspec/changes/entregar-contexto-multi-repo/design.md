## Overview

Este cambio formaliza un punto unico de verdad para contexto de arquitectura distribuida en varios repositorios, orientado a mejorar la calidad de propuestas OpenSpec que tocan mas de un repo.

## Data Flow

1. Detectar repositorios Git bajo la raiz de trabajo.
2. Recolectar metadata minima por repositorio (branch, commit, remote, artefactos tecnicos).
3. Consolidar en `openspec/context/multi-repo-context.md`.
4. Referenciar el archivo en `proposal.md` y `design.md` de cada cambio cross-repo.

## Decisions

- El archivo de contexto vive en `openspec/context/` para mantenerlo junto a artefactos OpenSpec.
- El contexto es descriptivo y no normativo: no bloquea cambios, pero reduce ambiguedad.
- Se privilegia informacion operativa minima y verificable.

## Risks

- Obsolescencia del contexto si no se regenera periodicamente.
- Duplicidad parcial con documentacion existente en cada repo.

## Mitigations

- Regenerar contexto antes de iniciar cambios cross-repo.
- Referenciar fecha/hora de generacion en el documento consolidado.

## Context Reference

- `openspec/context/multi-repo-context.md`
- Dependencias observadas entre proyectos .NET mediante `ProjectReference` en los `.csproj`.
- Impacto transversal en repositorios compartidos: `DocuArchiCore`, `DocuArchi.Api`, `DocuArchiCore.Abstractions`, `MiApp.DTOs`, `MiApp.Models`, `MiApp.Repository`, `MiApp.Services`.
