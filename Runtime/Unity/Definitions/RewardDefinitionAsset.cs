using System;
using UnityEngine;

namespace Deucarian.Progression.Unity
{
    /// <summary>Reusable defaults for code calls and serialized typed keys; runtime state remains in the core service.</summary>
    public sealed class RewardDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private CurrencyDefinitionAsset currency = null;
        [SerializeField] private long amount = 10L;
        public string Id => id;
        public string DisplayName => displayName;
        public RewardKey Key => new AssetKey(id);
        public RewardBundle ToRuntimeDefinition()
        {
            if (currency == null) throw new InvalidOperationException("Choose a currency for reward '" + DisplayName + "' in Definitions.");
            return new RewardBundle(new[] { new CurrencyLine(new CurrencyId(currency.Id), new ProgressionAmount(amount), true) });
        }
        private sealed class AssetKey : RewardKey { public AssetKey(string value) : base(value) { } }
    }
}
