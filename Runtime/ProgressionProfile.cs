using System;
using System.Collections.Generic;

namespace Deucarian.Progression
{
    /// <summary>One player's state, catalog, and explicitly registered reward recipes.</summary>
    public sealed class ProgressionProfile
    {
        private readonly ProgressionState state;
        private readonly ProgressionCatalog catalog;
        private readonly Dictionary<string, RewardBundle> rewards;
        public ProgressionProfile(ProgressionState state, ProgressionCatalog catalog,
            IReadOnlyDictionary<string, RewardBundle> rewards)
        {
            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            if (rewards == null) throw new ArgumentNullException(nameof(rewards));
            this.rewards = new Dictionary<string, RewardBundle>(StringComparer.Ordinal);
            foreach (var reward in rewards)
            {
                if (string.IsNullOrWhiteSpace(reward.Key) || reward.Value == null)
                    throw new ArgumentException("Every reward needs an ID and bundle.", nameof(rewards));
                this.rewards.Add(reward.Key, reward.Value);
            }
        }
        public ProgressionResult GrantReward(IRewardKey key, ProgressionOperationId operationId)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "Select a RewardKey or reuse a named reward definition.");
            string rewardId = key.Id;
            if (!rewards.TryGetValue(rewardId, out var reward))
                throw new KeyNotFoundException("ProgressionProfile has no reward '" + rewardId + "'. Register this key's reward bundle in this player's profile.");
            return state.ApplyReward(catalog, operationId, reward);
        }
        public ProgressionResult PurchaseResearch(IResearchKey key, ProgressionOperationId operationId)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "Select a ResearchKey or reuse a named research definition.");
            var id = new ResearchNodeId(key.Id);
            if (!catalog.TryGetResearch(id, out _))
                throw new KeyNotFoundException("ProgressionProfile cannot find research '" + key.Id + "'. Add it to this player's progression catalog.");
            return state.PurchaseResearch(catalog, operationId, id);
        }
        public ProgressionAmount GetBalance(ICurrencyKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "Select a CurrencyKey or reuse a named currency definition.");
            var id = new CurrencyId(key.Id);
            if (!catalog.TryGetCurrency(id, out _))
                throw new KeyNotFoundException("ProgressionProfile cannot find currency '" + key.Id + "'. Add it to this player's progression catalog.");
            return state.GetBalance(id);
        }
        public ProgressionSnapshot Snapshot => state.CreateSnapshot();
    }
}
