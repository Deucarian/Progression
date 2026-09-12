using System;
using System.Linq;
using Deucarian.Editor.Definitions;
using Deucarian.Progression.Unity;
using UnityEditor;

namespace Deucarian.Progression.Editor.Definitions
{
    internal static class ProjectDefinitionCatalogAuthoring
    {
        internal static void Refresh(bool validateOnly = false)
        {
            var currencies = AssetDatabase.FindAssets("t:CurrencyDefinitionAsset", new[] { "Assets" })
                .Select(x => AssetDatabase.LoadAssetAtPath<CurrencyDefinitionAsset>(AssetDatabase.GUIDToAssetPath(x)))
                .Where(x => x != null).OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            if (currencies.GroupBy(x => x.Id).Any(x => x.Count() > 1)) throw new InvalidOperationException("Currency definition IDs must be unique.");
            DeucarianDefinitionCatalog.Update<ProgressionDefinitionCatalog>("Assets/DeucarianDefinitions/Resources/Deucarian/Definitions/ProgressionDefinitionCatalog.asset", "currencies", currencies, validateOnly);
            var rewards = AssetDatabase.FindAssets("t:RewardDefinitionAsset", new[] { "Assets" })
                .Select(x => AssetDatabase.LoadAssetAtPath<RewardDefinitionAsset>(AssetDatabase.GUIDToAssetPath(x)))
                .Where(x => x != null).OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            if (rewards.GroupBy(x => x.Id).Any(x => x.Count() > 1)) throw new InvalidOperationException("Reward definition IDs must be unique.");
            DeucarianDefinitionCatalog.Update<ProgressionDefinitionCatalog>("Assets/DeucarianDefinitions/Resources/Deucarian/Definitions/ProgressionDefinitionCatalog.asset", "rewards", rewards, validateOnly);
            var research = AssetDatabase.FindAssets("t:ResearchDefinitionAsset", new[] { "Assets" })
                .Select(x => AssetDatabase.LoadAssetAtPath<ResearchDefinitionAsset>(AssetDatabase.GUIDToAssetPath(x)))
                .Where(x => x != null).OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            if (research.GroupBy(x => x.Id).Any(x => x.Count() > 1)) throw new InvalidOperationException("Research definition IDs must be unique.");
            DeucarianDefinitionCatalog.Update<ProgressionDefinitionCatalog>("Assets/DeucarianDefinitions/Resources/Deucarian/Definitions/ProgressionDefinitionCatalog.asset", "research", research, validateOnly);
        }
    }
}
