using System;
using UnityEngine;

namespace Deucarian.Progression.Unity
{
    /// <summary>Reusable defaults for code calls and serialized typed keys; runtime state remains in the core service.</summary>
    public sealed class ResearchDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private CurrencyDefinitionAsset currency = null;
        [SerializeField] private long[] rankCosts = new long[] { 10L, 20L, 30L };
        [SerializeField] private ResearchDefinitionAsset[] prerequisites = Array.Empty<ResearchDefinitionAsset>();
        public string Id => id;
        public string DisplayName => displayName;
        public ResearchKey Key => new AssetKey(id);
        public ResearchNodeDefinition ToRuntimeDefinition()
        {
            if (currency == null || rankCosts == null || rankCosts.Length == 0) throw new InvalidOperationException("Choose a currency and at least one rank cost for research '" + DisplayName + "'.");
            var costs = new CurrencyLine[rankCosts.Length];
            for (int i = 0; i < costs.Length; i++) costs[i] = new CurrencyLine(new CurrencyId(currency.Id), new ProgressionAmount(rankCosts[i]), false);
            var requirements = new ResearchPrerequisite[prerequisites.Length];
            for (int i = 0; i < requirements.Length; i++)
            {
                if (prerequisites[i] == null) throw new InvalidOperationException("Choose an existing research prerequisite for '" + DisplayName + "'.");
                requirements[i] = new ResearchPrerequisite(new ResearchNodeId(prerequisites[i].Id), 1);
            }
            return new ResearchNodeDefinition(new ResearchNodeId(Id), costs.Length, costs, requirements);
        }
        private sealed class AssetKey : ResearchKey { public AssetKey(string value) : base(value) { } }
    }
}
