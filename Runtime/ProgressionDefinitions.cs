using System;
using System.Collections.Generic;

namespace Deucarian.Progression
{
    /// <summary>Authored configuration for a currency balance.</summary>
    public sealed class CurrencyDefinition
    {
        /// <summary>Creates a currency definition.</summary>
        public CurrencyDefinition(CurrencyId id, ProgressionAmount maxBalance)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("Currency id cannot be empty.", nameof(id));
            }

            Id = id;
            MaxBalance = maxBalance;
        }

        /// <summary>Gets the currency identifier.</summary>
        public CurrencyId Id { get; }

        /// <summary>Gets the largest supported balance for this currency.</summary>
        public ProgressionAmount MaxBalance { get; }
    }

    /// <summary>Authored configuration for a cumulative progression track.</summary>
    public sealed class ProgressionTrackDefinition
    {
        private readonly long[] _thresholds;

        /// <summary>Creates a track definition with strictly increasing cumulative thresholds.</summary>
        public ProgressionTrackDefinition(TrackId id, int startingLevel, IReadOnlyList<ProgressionAmount> cumulativeThresholds)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("Track id cannot be empty.", nameof(id));
            }

            if (startingLevel < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startingLevel), "Starting level cannot be negative.");
            }

            if (cumulativeThresholds == null)
            {
                throw new ArgumentNullException(nameof(cumulativeThresholds));
            }

            _thresholds = new long[cumulativeThresholds.Count];
            long previous = -1;
            for (int index = 0; index < cumulativeThresholds.Count; index++)
            {
                long threshold = cumulativeThresholds[index].Value;
                if (threshold <= previous)
                {
                    throw new ArgumentException("Track thresholds must be strictly increasing.", nameof(cumulativeThresholds));
                }

                _thresholds[index] = threshold;
                previous = threshold;
            }

            Id = id;
            StartingLevel = startingLevel;
        }

        /// <summary>Gets the track identifier.</summary>
        public TrackId Id { get; }

        /// <summary>Gets the level before any threshold is reached.</summary>
        public int StartingLevel { get; }

        /// <summary>Gets the maximum level implied by the thresholds.</summary>
        public int MaxLevel => StartingLevel + _thresholds.Length;

        /// <summary>Computes the level for a cumulative total.</summary>
        public int GetLevelForTotal(ProgressionAmount total)
        {
            int level = StartingLevel;
            for (int index = 0; index < _thresholds.Length; index++)
            {
                if (total.Value < _thresholds[index])
                {
                    break;
                }

                level++;
            }

            return level;
        }
    }

    /// <summary>One currency delta inside an atomic transaction.</summary>
    public readonly struct CurrencyLine
    {
        /// <summary>Creates a currency line.</summary>
        public CurrencyLine(CurrencyId currencyId, ProgressionAmount amount, bool isCredit)
        {
            if (currencyId.IsEmpty)
            {
                throw new ArgumentException("Currency id cannot be empty.", nameof(currencyId));
            }

            CurrencyId = currencyId;
            Amount = amount;
            IsCredit = isCredit;
        }

        /// <summary>Gets the currency to mutate.</summary>
        public CurrencyId CurrencyId { get; }

        /// <summary>Gets the non-negative amount.</summary>
        public ProgressionAmount Amount { get; }

        /// <summary>Gets whether this line credits instead of debits.</summary>
        public bool IsCredit { get; }
    }

    /// <summary>Reward bundle composed from generic progression operations.</summary>
    public sealed class RewardBundle
    {
        /// <summary>Creates a reward bundle.</summary>
        public RewardBundle(
            IReadOnlyList<CurrencyLine> currencyLines = null,
            IReadOnlyList<XpGrant> xpGrants = null,
            IReadOnlyList<UnlockId> unlocks = null)
        {
            CurrencyLines = currencyLines == null ? Array.Empty<CurrencyLine>() : Copy(currencyLines);
            XpGrants = xpGrants == null ? Array.Empty<XpGrant>() : Copy(xpGrants);
            Unlocks = unlocks == null ? Array.Empty<UnlockId>() : Copy(unlocks);
        }

        /// <summary>Gets currency mutations applied by this reward.</summary>
        public IReadOnlyList<CurrencyLine> CurrencyLines { get; }

        /// <summary>Gets XP grants applied by this reward.</summary>
        public IReadOnlyList<XpGrant> XpGrants { get; }

        /// <summary>Gets permanent unlocks applied by this reward.</summary>
        public IReadOnlyList<UnlockId> Unlocks { get; }

        private static T[] Copy<T>(IReadOnlyList<T> source)
        {
            T[] copy = new T[source.Count];
            for (int index = 0; index < source.Count; index++)
            {
                copy[index] = source[index];
            }

            return copy;
        }
    }

    /// <summary>XP grant for a progression track.</summary>
    public readonly struct XpGrant
    {
        /// <summary>Creates an XP grant.</summary>
        public XpGrant(TrackId trackId, ProgressionAmount amount)
        {
            if (trackId.IsEmpty)
            {
                throw new ArgumentException("Track id cannot be empty.", nameof(trackId));
            }

            TrackId = trackId;
            Amount = amount;
        }

        /// <summary>Gets the target track.</summary>
        public TrackId TrackId { get; }

        /// <summary>Gets the amount to add.</summary>
        public ProgressionAmount Amount { get; }
    }

    /// <summary>Requirement for a ranked research node.</summary>
    public readonly struct ResearchPrerequisite
    {
        /// <summary>Creates a ranked prerequisite.</summary>
        public ResearchPrerequisite(ResearchNodeId nodeId, int minimumRank)
        {
            if (nodeId.IsEmpty)
            {
                throw new ArgumentException("Node id cannot be empty.", nameof(nodeId));
            }

            if (minimumRank < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumRank), "Minimum rank must be at least one.");
            }

            NodeId = nodeId;
            MinimumRank = minimumRank;
        }

        /// <summary>Gets the required node.</summary>
        public ResearchNodeId NodeId { get; }

        /// <summary>Gets the required rank.</summary>
        public int MinimumRank { get; }
    }

    /// <summary>Authored ranked research or meta upgrade node.</summary>
    public sealed class ResearchNodeDefinition
    {
        /// <summary>Creates a research node definition.</summary>
        public ResearchNodeDefinition(
            ResearchNodeId id,
            int maxRank,
            IReadOnlyList<CurrencyLine> rankCosts,
            IReadOnlyList<ResearchPrerequisite> prerequisites = null,
            IReadOnlyList<UnlockId> requiredUnlocks = null)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("Node id cannot be empty.", nameof(id));
            }

            if (maxRank < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRank), "Max rank must be at least one.");
            }

            if (rankCosts == null)
            {
                throw new ArgumentNullException(nameof(rankCosts));
            }

            Id = id;
            MaxRank = maxRank;
            RankCosts = Copy(rankCosts);
            Prerequisites = prerequisites == null ? Array.Empty<ResearchPrerequisite>() : Copy(prerequisites);
            RequiredUnlocks = requiredUnlocks == null ? Array.Empty<UnlockId>() : Copy(requiredUnlocks);
        }

        /// <summary>Gets the research node identifier.</summary>
        public ResearchNodeId Id { get; }

        /// <summary>Gets the maximum purchasable rank.</summary>
        public int MaxRank { get; }

        /// <summary>Gets per-rank debit lines. One line per rank is supported in 0.1.0.</summary>
        public IReadOnlyList<CurrencyLine> RankCosts { get; }

        /// <summary>Gets ranked node prerequisites.</summary>
        public IReadOnlyList<ResearchPrerequisite> Prerequisites { get; }

        /// <summary>Gets unlock prerequisites.</summary>
        public IReadOnlyList<UnlockId> RequiredUnlocks { get; }

        internal CurrencyLine GetCostForNextRank(int currentRank)
        {
            int index = Math.Min(currentRank, RankCosts.Count - 1);
            return RankCosts[index];
        }

        private static T[] Copy<T>(IReadOnlyList<T> source)
        {
            T[] copy = new T[source.Count];
            for (int index = 0; index < source.Count; index++)
            {
                copy[index] = source[index];
            }

            return copy;
        }
    }

    /// <summary>Authored milestone rule and reward.</summary>
    public sealed class MilestoneDefinition
    {
        /// <summary>Creates a milestone definition.</summary>
        public MilestoneDefinition(MilestoneId id, MetricId metricId, ProgressionAmount threshold, RewardBundle reward)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("Milestone id cannot be empty.", nameof(id));
            }

            if (metricId.IsEmpty)
            {
                throw new ArgumentException("Metric id cannot be empty.", nameof(metricId));
            }

            Id = id;
            MetricId = metricId;
            Threshold = threshold;
            Reward = reward ?? new RewardBundle();
        }

        /// <summary>Gets the milestone identifier.</summary>
        public MilestoneId Id { get; }

        /// <summary>Gets the metric that completes the milestone.</summary>
        public MetricId MetricId { get; }

        /// <summary>Gets the completion threshold.</summary>
        public ProgressionAmount Threshold { get; }

        /// <summary>Gets the reward applied on claim.</summary>
        public RewardBundle Reward { get; }
    }
}
