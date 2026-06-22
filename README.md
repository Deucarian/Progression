# Deucarian Progression

`com.deucarian.progression` provides pure C# progression foundations for Deucarian games.

The package owns:

- stable progression identifiers composed from `Deucarian.GameplayFoundation.ContentId`
- non-negative integral progression amounts with explicit overflow checks
- capped currency balances and atomic currency transactions
- cumulative progression tracks with deterministic threshold evaluation
- idempotent reward bundles
- permanent unlocks
- ranked research or upgrade nodes with prerequisite graph validation
- milestone metrics, completion, claiming, and one-time rewards
- deterministic snapshots for persistence DTO mapping

The package deliberately does not own health, damage, weapons, progression UI, save storage, migrations, scenes, GameObjects, MonoBehaviours, networking, service locators, or global mutable state.

## Dependency Boundary

Runtime depends only on `com.deucarian.gameplay-foundation` and .NET collection/runtime APIs. Persistence integration is done by mapping `ProgressionSnapshot` to a save DTO in the consuming game or validation harness.

## Validation

See `Documentation~/PackageValidation.md` for the Phase 1C validation protocol and recorded results.
