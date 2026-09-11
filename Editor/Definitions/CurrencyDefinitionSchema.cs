using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Progression.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Progression.Editor.Definitions
{
    public sealed class CurrencyDefinitionSchema : DeucarianSerializedDefinitionSchema<CurrencyDefinitionAsset, CurrencyDefinitionSpec>
    {
        public override string Id => "currencies";
        public override string DisplayName => "Currencies";
        public override void ValidateAssetReady(ScriptableObject asset)
        {
            base.ValidateAssetReady(asset);
            ((CurrencyDefinitionAsset)asset).ToRuntimeDefinition();
        }
        public override void RefreshCatalog(bool validateOnly = false) { ProjectDefinitionCatalogAuthoring.Refresh(validateOnly); }
        [MenuItem("Assets/Create/Deucarian/Progression/Currency Definition")]
        private static void CreateDefinition() { Selection.activeObject = DeucarianDefinitionSync.Create(new CurrencyDefinitionSchema(), "NewCurrency"); }
    }
}
