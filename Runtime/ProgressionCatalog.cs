using System;
using System.Collections.Generic;

namespace Deucarian.Progression
{
    /// <summary>Validated collection of authored progression definitions.</summary>
    public sealed class ProgressionCatalog
    {
        private readonly Dictionary<CurrencyId, CurrencyDefinition> _currencies;
        private readonly Dictionary<TrackId, ProgressionTrackDefinition> _tracks;
        private readonly Dictionary<ResearchNodeId, ResearchNodeDefinition> _research;
        private readonly Dictionary<MilestoneId, MilestoneDefinition> _milestones;

        /// <summary>Creates and validates a progression catalog.</summary>
        public ProgressionCatalog(
            IReadOnlyList<CurrencyDefinition> currencies = null,
            IReadOnlyList<ProgressionTrackDefinition> tracks = null,
            IReadOnlyList<ResearchNodeDefinition> research = null,
            IReadOnlyList<MilestoneDefinition> milestones = null)
        {
            _currencies = BuildCurrencyMap(currencies);
            _tracks = BuildTrackMap(tracks);
            _research = BuildResearchMap(research);
            _milestones = BuildMilestoneMap(milestones);
            ValidateResearchGraph();
            ValidateMilestones();
        }

        /// <summary>Finds a currency definition.</summary>
        public bool TryGetCurrency(CurrencyId id, out CurrencyDefinition definition) => _currencies.TryGetValue(id, out definition);

        /// <summary>Finds a track definition.</summary>
        public bool TryGetTrack(TrackId id, out ProgressionTrackDefinition definition) => _tracks.TryGetValue(id, out definition);

        /// <summary>Finds a research definition.</summary>
        public bool TryGetResearch(ResearchNodeId id, out ResearchNodeDefinition definition) => _research.TryGetValue(id, out definition);

        /// <summary>Finds a milestone definition.</summary>
        public bool TryGetMilestone(MilestoneId id, out MilestoneDefinition definition) => _milestones.TryGetValue(id, out definition);

        /// <summary>Gets milestone definitions in deterministic order.</summary>
        public IReadOnlyList<MilestoneDefinition> GetMilestonesOrdered()
        {
            MilestoneDefinition[] values = new MilestoneDefinition[_milestones.Count];
            _milestones.Values.CopyTo(values, 0);
            Array.Sort(values, (left, right) => left.Id.CompareTo(right.Id));
            return values;
        }

        private static Dictionary<CurrencyId, CurrencyDefinition> BuildCurrencyMap(IReadOnlyList<CurrencyDefinition> definitions)
        {
            Dictionary<CurrencyId, CurrencyDefinition> map = new Dictionary<CurrencyId, CurrencyDefinition>();
            if (definitions == null)
            {
                return map;
            }

            for (int index = 0; index < definitions.Count; index++)
            {
                CurrencyDefinition definition = definitions[index] ?? throw new ArgumentException("Currency definitions cannot contain null.", nameof(definitions));
                if (map.ContainsKey(definition.Id))
                {
                    throw new ArgumentException("Duplicate currency definition: " + definition.Id, nameof(definitions));
                }

                map.Add(definition.Id, definition);
            }

            return map;
        }

        private static Dictionary<TrackId, ProgressionTrackDefinition> BuildTrackMap(IReadOnlyList<ProgressionTrackDefinition> definitions)
        {
            Dictionary<TrackId, ProgressionTrackDefinition> map = new Dictionary<TrackId, ProgressionTrackDefinition>();
            if (definitions == null)
            {
                return map;
            }

            for (int index = 0; index < definitions.Count; index++)
            {
                ProgressionTrackDefinition definition = definitions[index] ?? throw new ArgumentException("Track definitions cannot contain null.", nameof(definitions));
                if (map.ContainsKey(definition.Id))
                {
                    throw new ArgumentException("Duplicate track definition: " + definition.Id, nameof(definitions));
                }

                map.Add(definition.Id, definition);
            }

            return map;
        }

        private static Dictionary<ResearchNodeId, ResearchNodeDefinition> BuildResearchMap(IReadOnlyList<ResearchNodeDefinition> definitions)
        {
            Dictionary<ResearchNodeId, ResearchNodeDefinition> map = new Dictionary<ResearchNodeId, ResearchNodeDefinition>();
            if (definitions == null)
            {
                return map;
            }

            for (int index = 0; index < definitions.Count; index++)
            {
                ResearchNodeDefinition definition = definitions[index] ?? throw new ArgumentException("Research definitions cannot contain null.", nameof(definitions));
                if (definition.RankCosts.Count != definition.MaxRank)
                {
                    throw new ArgumentException("Research rank cost count must match max rank for " + definition.Id, nameof(definitions));
                }

                for (int costIndex = 0; costIndex < definition.RankCosts.Count; costIndex++)
                {
                    if (definition.RankCosts[costIndex].IsCredit)
                    {
                        throw new ArgumentException("Research costs must be debit lines.", nameof(definitions));
                    }
                }

                if (map.ContainsKey(definition.Id))
                {
                    throw new ArgumentException("Duplicate research definition: " + definition.Id, nameof(definitions));
                }

                map.Add(definition.Id, definition);
            }

            return map;
        }

        private static Dictionary<MilestoneId, MilestoneDefinition> BuildMilestoneMap(IReadOnlyList<MilestoneDefinition> definitions)
        {
            Dictionary<MilestoneId, MilestoneDefinition> map = new Dictionary<MilestoneId, MilestoneDefinition>();
            if (definitions == null)
            {
                return map;
            }

            for (int index = 0; index < definitions.Count; index++)
            {
                MilestoneDefinition definition = definitions[index] ?? throw new ArgumentException("Milestone definitions cannot contain null.", nameof(definitions));
                if (map.ContainsKey(definition.Id))
                {
                    throw new ArgumentException("Duplicate milestone definition: " + definition.Id, nameof(definitions));
                }

                map.Add(definition.Id, definition);
            }

            return map;
        }

        private void ValidateResearchGraph()
        {
            foreach (ResearchNodeDefinition definition in _research.Values)
            {
                for (int index = 0; index < definition.Prerequisites.Count; index++)
                {
                    ResearchPrerequisite prerequisite = definition.Prerequisites[index];
                    if (prerequisite.NodeId.Equals(definition.Id))
                    {
                        throw new ArgumentException("Research node cannot require itself: " + definition.Id);
                    }

                    if (!_research.TryGetValue(prerequisite.NodeId, out ResearchNodeDefinition required))
                    {
                        throw new ArgumentException("Research node references missing prerequisite: " + prerequisite.NodeId);
                    }

                    if (prerequisite.MinimumRank > required.MaxRank)
                    {
                        throw new ArgumentException("Research prerequisite rank exceeds max rank: " + prerequisite.NodeId);
                    }
                }
            }

            Dictionary<ResearchNodeId, int> visit = new Dictionary<ResearchNodeId, int>();
            foreach (ResearchNodeDefinition definition in _research.Values)
            {
                Visit(definition.Id, visit);
            }
        }

        private void Visit(ResearchNodeId id, Dictionary<ResearchNodeId, int> visit)
        {
            if (visit.TryGetValue(id, out int state))
            {
                if (state == 1)
                {
                    throw new ArgumentException("Research graph contains a cycle at " + id);
                }

                return;
            }

            visit[id] = 1;
            ResearchNodeDefinition definition = _research[id];
            for (int index = 0; index < definition.Prerequisites.Count; index++)
            {
                Visit(definition.Prerequisites[index].NodeId, visit);
            }

            visit[id] = 2;
        }

        private void ValidateMilestones()
        {
            foreach (MilestoneDefinition definition in _milestones.Values)
            {
                ValidateReward(definition.Reward);
            }
        }

        internal bool ValidateReward(RewardBundle reward)
        {
            for (int index = 0; index < reward.CurrencyLines.Count; index++)
            {
                if (!_currencies.ContainsKey(reward.CurrencyLines[index].CurrencyId))
                {
                    return false;
                }
            }

            for (int index = 0; index < reward.XpGrants.Count; index++)
            {
                if (!_tracks.ContainsKey(reward.XpGrants[index].TrackId))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
