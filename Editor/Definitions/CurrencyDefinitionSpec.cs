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
    public sealed class CurrencyDefinitionSpec : DeucarianDefinitionSpec
    {
        [DefinitionField("maximumBalance")] public long MaximumBalance = 1000000L;
    }
}
