# Public API

## Identifiers

`CurrencyId`, `TrackId`, `UnlockId`, `ResearchNodeId`, `MilestoneId`, `MetricId`, and `ProgressionOperationId` are stable value types backed by Gameplay Foundation identifier rules.

## Amounts

`ProgressionAmount` wraps a non-negative `long`. Arithmetic is checked through internal staging helpers; public constructors reject negative values.

## Definitions

`CurrencyDefinition`, `ProgressionTrackDefinition`, `ResearchNodeDefinition`, and `MilestoneDefinition` are runtime-safe definitions. `ProgressionCatalog` validates duplicate definitions and research graph integrity.

## State

`ProgressionState` owns balances, track totals, unlocks, research ranks, metrics, milestone completion/claims, and applied operation IDs. Query paths are allocation-free after warm-up.

## Operations

`ApplyTransaction`, `ApplyReward`, `PurchaseResearch`, `SetMetric`, `IncrementMetric`, and `ClaimMilestone` return `ProgressionResult` with explicit `ProgressionStatus`.

## Snapshots

`CreateSnapshot` returns sorted immutable arrays through `ProgressionSnapshot`. Consumers map this snapshot to save DTOs without adding a Persistence dependency to this package.
