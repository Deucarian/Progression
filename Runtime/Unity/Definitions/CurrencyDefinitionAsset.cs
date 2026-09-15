using System;
using UnityEngine;

namespace Deucarian.Progression.Unity
{
    /// <summary>Reusable defaults for code calls and serialized typed keys; runtime state remains in the core service.</summary>
    public sealed class CurrencyDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private long maximumBalance = 1000000L;
        public string Id => id;
        public string DisplayName => displayName;
        public CurrencyKey Key => new AssetKey(id);
        public CurrencyDefinition ToRuntimeDefinition() => new CurrencyDefinition(new CurrencyId(Id), new ProgressionAmount(maximumBalance));
        private sealed class AssetKey : CurrencyKey { public AssetKey(string value) : base(value) { } }
    }
}
