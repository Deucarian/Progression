using System;
using System.Linq;
using UnityEngine;

namespace Deucarian.Progression.Unity
{
    public sealed class ProgressionDefinitionCatalog : ScriptableObject
    {
        public const string ResourcePath = "Deucarian/Definitions/ProgressionDefinitionCatalog";
        [SerializeField] private CurrencyDefinitionAsset[] currencies = Array.Empty<CurrencyDefinitionAsset>();
        [SerializeField] private RewardDefinitionAsset[] rewards = Array.Empty<RewardDefinitionAsset>();
        [SerializeField] private ResearchDefinitionAsset[] research = Array.Empty<ResearchDefinitionAsset>();
        public static ProgressionDefinitionCatalog LoadProject() => Resources.Load<ProgressionDefinitionCatalog>(ResourcePath) ?? throw new InvalidOperationException("Create progression definitions in Definitions before loading the project catalog.");
        public ProgressionProfile CreateProfile(ProgressionState state = null)
        {
            if (currencies.Any(x => x == null) || rewards.Any(x => x == null) || research.Any(x => x == null)) throw new InvalidOperationException("The progression catalog contains missing definitions. Synchronize it in Definitions.");
            var catalog = new ProgressionCatalog(currencies.Select(x => x.ToRuntimeDefinition()).ToArray(), research: research.Select(x => x.ToRuntimeDefinition()).ToArray());
            return new ProgressionProfile(state ?? new ProgressionState(), catalog, rewards.ToDictionary(x => x.Id, x => x.ToRuntimeDefinition(), StringComparer.Ordinal));
        }
    }
}
