using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Progression.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Progression.Editor.Definitions
{
    public sealed class ResearchDefinitionSchema : DeucarianSerializedDefinitionSchema<ResearchDefinitionAsset, ResearchDefinitionSpec>
    {
        public override string Id => "research";
        public override string DisplayName => "Research";
        public override void ValidateAssetReady(ScriptableObject asset)
        {
            base.ValidateAssetReady(asset);
            ((ResearchDefinitionAsset)asset).ToRuntimeDefinition();
        }
        public override void RefreshCatalog(bool validateOnly = false) { ProjectDefinitionCatalogAuthoring.Refresh(validateOnly); }
        [MenuItem("Assets/Create/Deucarian/Progression/Research Definition")]
        private static void CreateDefinition() { Selection.activeObject = DeucarianDefinitionSync.Create(new ResearchDefinitionSchema(), "NewResearch"); }
    }
}
