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
        public ProgressionResult GrantReward(string rewardId, string operationId)
        {
            if (!rewards.TryGetValue(rewardId, out var reward))
                throw new KeyNotFoundException("No reward is registered with ID '" + rewardId + "'.");
            return state.ApplyReward(catalog, new ProgressionOperationId(operationId), reward);
        }
        public ProgressionResult PurchaseResearch(string researchId, string operationId) =>
            state.PurchaseResearch(catalog, new ProgressionOperationId(operationId), new ResearchNodeId(researchId));
        public ProgressionAmount GetBalance(string currencyId) => state.GetBalance(new CurrencyId(currencyId));
        public ProgressionSnapshot Snapshot => state.CreateSnapshot();
    }
}
