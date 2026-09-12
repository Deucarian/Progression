using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Progression.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Progression.Editor.Definitions
{
    [Serializable]
    public sealed class ResearchDefinitionSpec : DeucarianDefinitionSpec
    {
        [DefinitionField("currency")] public CurrencyDefinitionAsset Currency = null;
        [DefinitionField("rankCosts")] public long[] RankCosts = new long[] { 10L, 20L, 30L };
        [DefinitionField("prerequisites")] public ResearchDefinitionAsset[] Prerequisites = Array.Empty<ResearchDefinitionAsset>();
    }
}
