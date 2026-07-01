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

## Install

Stable:

```json
"com.deucarian.progression": "https://github.com/Deucarian/Progression.git#main"
```

Development:

```json
"com.deucarian.progression": "https://github.com/Deucarian/Progression.git#develop"
```

Use `#main` for stable package consumption and `#develop` when testing active package work.

## When To Use This

Use this package when you need Pure C# progression foundations for currencies, tracks, rewards, unlocks, research graphs, milestones, snapshots, and deterministic result deltas.

Do not use this package to take ownership of capabilities outside its `AGENTS.md` boundary. Reusable behavior should stay with the package that owns that capability in the Package Registry governance docs.

## Quick Start

1. Install the package through Deucarian Package Installer or Unity Package Manager using the URL above.
2. Let Unity finish resolving packages and compiling assemblies.
3. Import the `Progression Sandbox` sample if you want a working reference scene or setup.
4. Start from the package README sections above and the public runtime/editor APIs in this repository.

## Integrations

Direct Deucarian package dependencies:

- `com.deucarian.gameplay-foundation`

Install optional companion packages only when their owned capability is needed by production code, samples, or tests.

## Troubleshooting

- Package does not resolve: confirm the stable or development Git URL matches the Package Registry entry and that required Deucarian dependencies are installed.
- Unity compile errors after install: let Package Manager finish resolving dependencies, then check asmdef references against `package.json` dependencies.
- Behavior appears to belong in another package: consult `AGENTS.md` and the Package Registry governance docs before moving or duplicating code.

## License

MIT. See `LICENSE.md`.
