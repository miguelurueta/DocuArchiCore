# Cross-Repo OpenSpec Checklist

Use this checklist before validating a change that impacts 2+ repositories.

0. Define impacted repos first:
   - Copy `openspec/context/REPO_IMPACT_TEMPLATE.md` into your change folder (for example `sync.md`) and fill `Impacta?` per repo.

1. Regenerate context:
   - `powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Generate-OpenSpecMultiRepoContext.ps1`
   - This also refreshes: `Docs/OpenSpec_Contexto_Operativo.md`
2. Ensure `proposal.md` references:
   - `openspec/context/multi-repo-context.md`
3. Ensure `design.md` contains:
   - `## Context Reference`
   - Reference to `openspec/context/multi-repo-context.md`
4. Run context gate:
   - `powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Validate-OpenSpecContextReference.ps1 -ChangeName <change-name>`
5. Run OpenSpec validation:
   - `openspec.cmd validate <change-name>`
