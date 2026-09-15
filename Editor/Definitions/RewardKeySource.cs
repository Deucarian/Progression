using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Progression.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Progression.Editor.Definitions
{
    public sealed class RewardKeySource : DeucarianAssetKeySource<RewardDefinitionAsset>
    {
        public override Type KeyType => typeof(RewardKey);
        public override Type DefinitionSetAttribute => typeof(RewardKeySetAttribute);
        public override string GeneratedClassName => "ProjectRewards";
        protected override DeucarianKeyChoice ReadDefinition(RewardDefinitionAsset asset) => new DeucarianKeyChoice(asset.Id, asset.DisplayName);
    }
}
