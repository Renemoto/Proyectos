# CU-01 — Registrar parte de tramo

## Scope
Implement only CU-01 from `kalma-arquitectura.md`: the `Parte` domain model and fixed care-period value object, plus the application use case `RegistrarParte` with idempotency and membership authorization.

## Acceptance criteria
- A part requires one of the four fixed periods, six required checklist answers, and optional note up to 500 characters.
- The part captures the period time as its own value and is immutable after creation.
- Corrections are new parts that reference the original part.
- Repeated IDs do not duplicate persistence; only a space member may register a part.
- Add tests following the architecture's strict TDD guidance.

## Tasks
1. Create .NET solution/project/test scaffolding needed for CU-01 and domain tests.
2. Implement and test domain invariants (`Parte`, `Tramo`, value types).
3. Implement and test application registration, membership check, and idempotency.
4. Run focused verification and record results.

## Constraints
- Do not implement CU-02 through CU-08, API endpoints, persistence adapters, or UI.
- Preserve Clean Architecture dependencies: Domain has no infrastructure dependencies; Application depends on Domain.
- No commit or delivery action without explicit request.

## Evidence
- Initial repository inspection found backend directories but no files, project manifests, or executable tests.
