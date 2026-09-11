using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Progression.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Progression.Editor.Definitions
{
    public sealed class ResearchKeySource : DeucarianAssetKeySource<ResearchDefinitionAsset>
    {
        public override Type KeyType => typeof(ResearchKey);
        public override Type DefinitionSetAttribute => typeof(ResearchKeySetAttribute);
        public override string GeneratedClassName => "ProjectResearch";
        protected override DeucarianKeyChoice ReadDefinition(ResearchDefinitionAsset asset) => new DeucarianKeyChoice(asset.Id, asset.DisplayName);
    }
}
