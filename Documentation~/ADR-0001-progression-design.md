# ADR 0001: Progression Design

## Status

Accepted for Phase 1C.

## Decisions

- Runtime is pure C# and references only Gameplay Foundation.
- Progression identifiers compose `ContentId` instead of duplicating stable ID validation.
- Amounts are non-negative `long` values in `ProgressionAmount`; overflow, underflow, and currency cap violations return explicit operation statuses.
- Debt is not supported in 0.1.0. All debits require existing funds.
- Mutations are single-owner and not internally synchronized.
- Atomic operations stage all affected values before commit.
- Optional `ProgressionOperationId` values make transactions, rewards, research purchases, and milestone claims idempotent.
- Definitions are immutable runtime objects; authored ScriptableObjects or remote config should map into them at the game edge.
- Progression tracks use cumulative thresholds and compute levels deterministically from totals.
- Research graphs validate duplicate nodes, missing prerequisites, self-prerequisites, impossible ranks, and cycles.
- Milestones separate metric completion from reward claiming.
- Persistence is composition-only through `ProgressionSnapshot`; this package does not depend on Persistence.
- Result/status objects are intentionally small and deterministic. Rich UI copy belongs to consuming games.

## Future Work

- Very large idle numbers should be added through a separate numeric representation and adapter. `ProgressionAmount` intentionally keeps 0.1.0 safe for mobile AOT and ordinary progression values.
- Exclusive research branches, refund policy, rank reset, and time-gated research are excluded until a proven donor or product need appears.
- Delta payloads may be expanded once UI and analytics packages prove their needs.
