# First Care Report Implementation

## Goal
Create the first implementation in the new `kalma_free_premium` repository, following `kalma-arquitectura.md`: the first TDD cycle for registering a care-period report (`Parte`) in the domain.

## Constraints
- Keep the domain independent of frameworks and persistence.
- Follow the architecture's strict red-green-refactor order.
- First behavior: a valid report stores its period and supplied checklist values; required checklist fields cannot be omitted. Preserve immutable report and correction-by-new-entry semantics as scoped in the architecture.
- Do not implement application/API/persistence in this first work unit.
- Technical artifacts in English.
- TDD mode: enabled by the architecture; runner: `dotnet test` (verify availability and report actual command/result).

## Tasks
1. Create the new repository skeleton and a focused domain test project; demonstrate the first domain test fails for the missing behavior (RED).
2. Implement the minimum domain model to pass the focused tests; run the focused suite (GREEN), then refactor only if useful.
3. Read back the final files, report checks and repository state; preserve an uncommitted worktree (no commit unless separately authorized).

## Routing and evidence
- Route: delegated direct for multi-file implementation.
- Trigger: writer must touch multiple non-trivial files (domain model and tests); preparation for the write is delegated with it.
- Verification commands must be delegated/observed under the configured verification policy.
- Forecast authored changes: approximately 150 lines; delivery strategy: ask-on-risk.
- Commit: none authorized.
