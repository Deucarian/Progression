# Donor Integration Proof

The donor project uses:

- `PersistentProgressionService` for save-backed meta progress and settings
- blood shard lifetime and unspent balances
- legacy experience totals
- ranked `MetaUpgradeDefinition` purchases with class availability
- `SkillTreeRuntimeState` for ranked node purchases, prerequisites, point costs, starting nodes, and exclusions
- ScriptableObject/Odin-authored definitions

## Clean Mappings

- Blood shards map to `CurrencyId` plus capped currency balances.
- Legacy experience maps to a `TrackId` with cumulative thresholds.
- Meta upgrade ranks map to `ResearchNodeDefinition` and `PurchaseResearch`.
- Run rewards map to `RewardBundle`.
- Unlocked content IDs map to `UnlockId`.

## Adapters Required

- Donor ScriptableObjects need edge adapters into pure runtime definitions.
- Class availability should remain a game-level prerequisite adapter or unlock prerequisite.
- Stat effects belong in Gameplay Foundation `StatModifier` application outside this package.
- Save storage and migrations belong to Persistence composition.

## Discarded Assumptions

- Direct save mutation during every progression operation.
- Unity `Mathf`, ScriptableObject, Odin, and display-label coupling in core logic.
- `int` economy totals without explicit overflow policy.
- UI failure strings in the domain layer.

## Genre Check

The same API supports survivor meta progression, Idle Auto Defense offline rewards and research, and classic Tower Defense wave rewards and tower upgrades without survivor-specific naming.
