using System;
using System.Collections.Generic;

namespace Deucarian.Progression
{
    /// <summary>Mutable owner of progression runtime state. Instances are not internally synchronized.</summary>
    public sealed class ProgressionState
    {
        private readonly Dictionary<CurrencyId, long> _balances = new Dictionary<CurrencyId, long>();
        private readonly Dictionary<TrackId, long> _trackTotals = new Dictionary<TrackId, long>();
        private readonly HashSet<UnlockId> _unlocks = new HashSet<UnlockId>();
        private readonly Dictionary<ResearchNodeId, int> _researchRanks = new Dictionary<ResearchNodeId, int>();
        private readonly Dictionary<MetricId, long> _metrics = new Dictionary<MetricId, long>();
        private readonly HashSet<MilestoneId> _completedMilestones = new HashSet<MilestoneId>();
        private readonly HashSet<MilestoneId> _claimedMilestones = new HashSet<MilestoneId>();
        private readonly HashSet<ProgressionOperationId> _appliedOperations = new HashSet<ProgressionOperationId>();

        /// <summary>Gets a currency balance or zero when absent.</summary>
        public ProgressionAmount GetBalance(CurrencyId id) => new ProgressionAmount(_balances.TryGetValue(id, out long value) ? value : 0);

        /// <summary>Gets a cumulative track total or zero when absent.</summary>
        public ProgressionAmount GetTrackTotal(TrackId id) => new ProgressionAmount(_trackTotals.TryGetValue(id, out long value) ? value : 0);

        /// <summary>Gets a track level from the catalog definition.</summary>
        public int GetTrackLevel(ProgressionCatalog catalog, TrackId id)
        {
            if (catalog == null || !catalog.TryGetTrack(id, out ProgressionTrackDefinition definition))
            {
                return 0;
            }

            return definition.GetLevelForTotal(GetTrackTotal(id));
        }

        /// <summary>Gets whether an unlock has been granted.</summary>
        public bool IsUnlocked(UnlockId id) => _unlocks.Contains(id);

        /// <summary>Gets the purchased rank for a research node.</summary>
        public int GetResearchRank(ResearchNodeId id) => _researchRanks.TryGetValue(id, out int rank) ? rank : 0;

        /// <summary>Gets a milestone metric value.</summary>
        public ProgressionAmount GetMetric(MetricId id) => new ProgressionAmount(_metrics.TryGetValue(id, out long value) ? value : 0);

        /// <summary>Gets whether a milestone has reached its threshold.</summary>
        public bool IsMilestoneCompleted(MilestoneId id) => _completedMilestones.Contains(id);

        /// <summary>Gets whether a milestone reward has been claimed.</summary>
        public bool IsMilestoneClaimed(MilestoneId id) => _claimedMilestones.Contains(id);

        /// <summary>Applies an atomic currency transaction.</summary>
        public ProgressionResult ApplyTransaction(ProgressionCatalog catalog, ProgressionOperationId operationId, IReadOnlyList<CurrencyLine> lines)
        {
            if (WasApplied(operationId))
            {
                return ProgressionResult.Duplicate(operationId);
            }

            if (!StageCurrency(catalog, lines, out Dictionary<CurrencyId, long> staged, out ProgressionStatus status))
            {
                return ProgressionResult.Fail(status, operationId);
            }

            foreach (KeyValuePair<CurrencyId, long> pair in staged)
            {
                _balances[pair.Key] = pair.Value;
            }

            MarkApplied(operationId);
            return ProgressionResult.Success(operationId);
        }

        /// <summary>Applies a reward bundle atomically.</summary>
        public ProgressionResult ApplyReward(ProgressionCatalog catalog, ProgressionOperationId operationId, RewardBundle reward)
        {
            reward = reward ?? new RewardBundle();
            if (WasApplied(operationId))
            {
                return ProgressionResult.Duplicate(operationId);
            }

            if (catalog == null || !catalog.ValidateReward(reward))
            {
                return ProgressionResult.Fail(ProgressionStatus.UnknownDefinition, operationId);
            }

            if (!StageCurrency(catalog, reward.CurrencyLines, out Dictionary<CurrencyId, long> stagedBalances, out ProgressionStatus status))
            {
                return ProgressionResult.Fail(status, operationId);
            }

            Dictionary<TrackId, long> stagedTracks = new Dictionary<TrackId, long>(_trackTotals);
            for (int index = 0; index < reward.XpGrants.Count; index++)
            {
                XpGrant grant = reward.XpGrants[index];
                long current = stagedTracks.TryGetValue(grant.TrackId, out long value) ? value : 0;
                if (!ProgressionAmount.TryAdd(current, grant.Amount.Value, out long next))
                {
                    return ProgressionResult.Fail(ProgressionStatus.Overflow, operationId);
                }

                stagedTracks[grant.TrackId] = next;
            }

            foreach (KeyValuePair<CurrencyId, long> pair in stagedBalances)
            {
                _balances[pair.Key] = pair.Value;
            }

            foreach (KeyValuePair<TrackId, long> pair in stagedTracks)
            {
                _trackTotals[pair.Key] = pair.Value;
            }

            for (int index = 0; index < reward.Unlocks.Count; index++)
            {
                if (!reward.Unlocks[index].IsEmpty)
                {
                    _unlocks.Add(reward.Unlocks[index]);
                }
            }

            MarkApplied(operationId);
            return ProgressionResult.Success(operationId);
        }

        /// <summary>Attempts to purchase the next rank of a research node atomically.</summary>
        public ProgressionResult PurchaseResearch(ProgressionCatalog catalog, ProgressionOperationId operationId, ResearchNodeId nodeId)
        {
            if (WasApplied(operationId))
            {
                return ProgressionResult.Duplicate(operationId);
            }

            if (catalog == null || !catalog.TryGetResearch(nodeId, out ResearchNodeDefinition definition))
            {
                return ProgressionResult.Fail(ProgressionStatus.UnknownDefinition, operationId);
            }

            int currentRank = GetResearchRank(nodeId);
            if (currentRank >= definition.MaxRank)
            {
                return ProgressionResult.Fail(ProgressionStatus.MaxRankReached, operationId);
            }

            for (int index = 0; index < definition.Prerequisites.Count; index++)
            {
                ResearchPrerequisite prerequisite = definition.Prerequisites[index];
                if (GetResearchRank(prerequisite.NodeId) < prerequisite.MinimumRank)
                {
                    return ProgressionResult.Fail(ProgressionStatus.MissingPrerequisite, operationId);
                }
            }

            for (int index = 0; index < definition.RequiredUnlocks.Count; index++)
            {
                if (!IsUnlocked(definition.RequiredUnlocks[index]))
                {
                    return ProgressionResult.Fail(ProgressionStatus.MissingPrerequisite, operationId);
                }
            }

            CurrencyLine cost = definition.GetCostForNextRank(currentRank);
            CurrencyLine[] lines = { cost };
            if (!StageCurrency(catalog, lines, out Dictionary<CurrencyId, long> staged, out ProgressionStatus status))
            {
                return ProgressionResult.Fail(status, operationId);
            }

            foreach (KeyValuePair<CurrencyId, long> pair in staged)
            {
                _balances[pair.Key] = pair.Value;
            }

            _researchRanks[nodeId] = currentRank + 1;
            MarkApplied(operationId);
            return ProgressionResult.Success(operationId);
        }

        /// <summary>Sets a metric to at least a value and completes matching milestones.</summary>
        public ProgressionResult SetMetric(ProgressionCatalog catalog, MetricId metricId, ProgressionAmount value)
        {
            long current = _metrics.TryGetValue(metricId, out long existing) ? existing : 0;
            if (value.Value > current)
            {
                _metrics[metricId] = value.Value;
            }

            CompleteMilestones(catalog, metricId);
            return ProgressionResult.Success(default);
        }

        /// <summary>Adds to a metric and completes matching milestones.</summary>
        public ProgressionResult IncrementMetric(ProgressionCatalog catalog, MetricId metricId, ProgressionAmount amount)
        {
            long current = _metrics.TryGetValue(metricId, out long existing) ? existing : 0;
            if (!ProgressionAmount.TryAdd(current, amount.Value, out long next))
            {
                return ProgressionResult.Fail(ProgressionStatus.Overflow, default);
            }

            _metrics[metricId] = next;
            CompleteMilestones(catalog, metricId);
            return ProgressionResult.Success(default);
        }

        /// <summary>Claims a completed milestone reward exactly once.</summary>
        public ProgressionResult ClaimMilestone(ProgressionCatalog catalog, ProgressionOperationId operationId, MilestoneId milestoneId)
        {
            if (WasApplied(operationId))
            {
                return ProgressionResult.Duplicate(operationId);
            }

            if (catalog == null || !catalog.TryGetMilestone(milestoneId, out MilestoneDefinition definition))
            {
                return ProgressionResult.Fail(ProgressionStatus.UnknownDefinition, operationId);
            }

            if (!_completedMilestones.Contains(milestoneId))
            {
                return ProgressionResult.Fail(ProgressionStatus.NotCompleted, operationId);
            }

            if (_claimedMilestones.Contains(milestoneId))
            {
                return ProgressionResult.Fail(ProgressionStatus.AlreadyClaimed, operationId);
            }

            ProgressionResult rewardResult = ApplyReward(catalog, operationId, definition.Reward);
            if (!rewardResult.Succeeded)
            {
                return rewardResult;
            }

            _claimedMilestones.Add(milestoneId);
            return rewardResult;
        }

        /// <summary>Creates an immutable snapshot with deterministic ordering.</summary>
        public ProgressionSnapshot CreateSnapshot()
        {
            return new ProgressionSnapshot(
                SnapshotSort(_balances),
                SnapshotSort(_trackTotals),
                SnapshotSort(_researchRanks),
                SnapshotSort(_metrics),
                SnapshotSort(_unlocks),
                SnapshotSort(_completedMilestones),
                SnapshotSort(_claimedMilestones),
                SnapshotSort(_appliedOperations));
        }

        private bool StageCurrency(ProgressionCatalog catalog, IReadOnlyList<CurrencyLine> lines, out Dictionary<CurrencyId, long> staged, out ProgressionStatus status)
        {
            staged = new Dictionary<CurrencyId, long>(_balances);
            status = ProgressionStatus.Success;
            if (catalog == null || lines == null)
            {
                status = ProgressionStatus.InvalidDefinition;
                return false;
            }

            HashSet<CurrencyId> seen = new HashSet<CurrencyId>();
            for (int index = 0; index < lines.Count; index++)
            {
                CurrencyLine line = lines[index];
                if (!seen.Add(line.CurrencyId))
                {
                    status = ProgressionStatus.DuplicateLine;
                    return false;
                }

                if (!catalog.TryGetCurrency(line.CurrencyId, out CurrencyDefinition definition))
                {
                    status = ProgressionStatus.UnknownDefinition;
                    return false;
                }

                long current = staged.TryGetValue(line.CurrencyId, out long value) ? value : 0;
                long next;
                if (line.IsCredit)
                {
                    if (!ProgressionAmount.TryAdd(current, line.Amount.Value, out next) || next > definition.MaxBalance.Value)
                    {
                        status = ProgressionStatus.Overflow;
                        return false;
                    }
                }
                else
                {
                    if (!ProgressionAmount.TrySubtract(current, line.Amount.Value, out next))
                    {
                        status = ProgressionStatus.InsufficientFunds;
                        return false;
                    }
                }

                staged[line.CurrencyId] = next;
            }

            return true;
        }

        private void CompleteMilestones(ProgressionCatalog catalog, MetricId metricId)
        {
            if (catalog == null)
            {
                return;
            }

            IReadOnlyList<MilestoneDefinition> milestones = catalog.GetMilestonesOrdered();
            long metricValue = _metrics.TryGetValue(metricId, out long value) ? value : 0;
            for (int index = 0; index < milestones.Count; index++)
            {
                MilestoneDefinition definition = milestones[index];
                if (definition.MetricId.Equals(metricId) && metricValue >= definition.Threshold.Value)
                {
                    _completedMilestones.Add(definition.Id);
                }
            }
        }

        private bool WasApplied(ProgressionOperationId operationId) => !operationId.IsEmpty && _appliedOperations.Contains(operationId);

        private void MarkApplied(ProgressionOperationId operationId)
        {
            if (!operationId.IsEmpty)
            {
                _appliedOperations.Add(operationId);
            }
        }

        private static SnapshotEntry<TId, long>[] SnapshotSort<TId>(Dictionary<TId, long> source)
            where TId : IComparable<TId>
        {
            SnapshotEntry<TId, long>[] values = new SnapshotEntry<TId, long>[source.Count];
            int index = 0;
            foreach (KeyValuePair<TId, long> pair in source)
            {
                values[index++] = new SnapshotEntry<TId, long>(pair.Key, pair.Value);
            }

            Array.Sort(values, (left, right) => left.Id.CompareTo(right.Id));
            return values;
        }

        private static SnapshotEntry<TId, int>[] SnapshotSort<TId>(Dictionary<TId, int> source)
            where TId : IComparable<TId>
        {
            SnapshotEntry<TId, int>[] values = new SnapshotEntry<TId, int>[source.Count];
            int index = 0;
            foreach (KeyValuePair<TId, int> pair in source)
            {
                values[index++] = new SnapshotEntry<TId, int>(pair.Key, pair.Value);
            }

            Array.Sort(values, (left, right) => left.Id.CompareTo(right.Id));
            return values;
        }

        private static TId[] SnapshotSort<TId>(HashSet<TId> source)
            where TId : IComparable<TId>
        {
            TId[] values = new TId[source.Count];
            source.CopyTo(values);
            Array.Sort(values);
            return values;
        }
    }

    /// <summary>Operation result with a stable status.</summary>
    public readonly struct ProgressionResult
    {
        private ProgressionResult(ProgressionStatus status, ProgressionOperationId operationId)
        {
            Status = status;
            OperationId = operationId;
        }

        /// <summary>Gets the operation status.</summary>
        public ProgressionStatus Status { get; }

        /// <summary>Gets the operation identifier.</summary>
        public ProgressionOperationId OperationId { get; }

        /// <summary>Gets whether the operation succeeded and changed state.</summary>
        public bool Succeeded => Status == ProgressionStatus.Success;

        /// <summary>Creates a success result.</summary>
        public static ProgressionResult Success(ProgressionOperationId operationId) => new ProgressionResult(ProgressionStatus.Success, operationId);

        /// <summary>Creates an idempotent duplicate result.</summary>
        public static ProgressionResult Duplicate(ProgressionOperationId operationId) => new ProgressionResult(ProgressionStatus.DuplicateOperation, operationId);

        /// <summary>Creates a failure result.</summary>
        public static ProgressionResult Fail(ProgressionStatus status, ProgressionOperationId operationId) => new ProgressionResult(status, operationId);
    }

    /// <summary>Immutable key/value snapshot entry.</summary>
    public readonly struct SnapshotEntry<TId, TValue>
    {
        /// <summary>Creates a snapshot entry.</summary>
        public SnapshotEntry(TId id, TValue value)
        {
            Id = id;
            Value = value;
        }

        /// <summary>Gets the identifier.</summary>
        public TId Id { get; }

        /// <summary>Gets the value.</summary>
        public TValue Value { get; }
    }

    /// <summary>Immutable progression state snapshot suitable for persistence DTO mapping.</summary>
    public sealed class ProgressionSnapshot
    {
        /// <summary>Creates a progression snapshot.</summary>
        public ProgressionSnapshot(
            IReadOnlyList<SnapshotEntry<CurrencyId, long>> balances,
            IReadOnlyList<SnapshotEntry<TrackId, long>> tracks,
            IReadOnlyList<SnapshotEntry<ResearchNodeId, int>> research,
            IReadOnlyList<SnapshotEntry<MetricId, long>> metrics,
            IReadOnlyList<UnlockId> unlocks,
            IReadOnlyList<MilestoneId> completedMilestones,
            IReadOnlyList<MilestoneId> claimedMilestones,
            IReadOnlyList<ProgressionOperationId> appliedOperations)
        {
            Balances = balances ?? Array.Empty<SnapshotEntry<CurrencyId, long>>();
            Tracks = tracks ?? Array.Empty<SnapshotEntry<TrackId, long>>();
            Research = research ?? Array.Empty<SnapshotEntry<ResearchNodeId, int>>();
            Metrics = metrics ?? Array.Empty<SnapshotEntry<MetricId, long>>();
            Unlocks = unlocks ?? Array.Empty<UnlockId>();
            CompletedMilestones = completedMilestones ?? Array.Empty<MilestoneId>();
            ClaimedMilestones = claimedMilestones ?? Array.Empty<MilestoneId>();
            AppliedOperations = appliedOperations ?? Array.Empty<ProgressionOperationId>();
        }

        /// <summary>Gets sorted currency balances.</summary>
        public IReadOnlyList<SnapshotEntry<CurrencyId, long>> Balances { get; }

        /// <summary>Gets sorted track totals.</summary>
        public IReadOnlyList<SnapshotEntry<TrackId, long>> Tracks { get; }

        /// <summary>Gets sorted research ranks.</summary>
        public IReadOnlyList<SnapshotEntry<ResearchNodeId, int>> Research { get; }

        /// <summary>Gets sorted metric values.</summary>
        public IReadOnlyList<SnapshotEntry<MetricId, long>> Metrics { get; }

        /// <summary>Gets sorted unlock identifiers.</summary>
        public IReadOnlyList<UnlockId> Unlocks { get; }

        /// <summary>Gets sorted completed milestone identifiers.</summary>
        public IReadOnlyList<MilestoneId> CompletedMilestones { get; }

        /// <summary>Gets sorted claimed milestone identifiers.</summary>
        public IReadOnlyList<MilestoneId> ClaimedMilestones { get; }

        /// <summary>Gets sorted applied operation identifiers.</summary>
        public IReadOnlyList<ProgressionOperationId> AppliedOperations { get; }
    }
}
