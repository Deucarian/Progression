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
    public sealed class RewardDefinitionSpec : DeucarianDefinitionSpec
    {
        [DefinitionField("currency")] public CurrencyDefinitionAsset Currency = null;
        [DefinitionField("amount")] public long Amount = 10L;
    }
}
