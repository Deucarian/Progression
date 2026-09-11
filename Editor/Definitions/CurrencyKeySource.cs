using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Progression.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Progression.Editor.Definitions
{
    public sealed class CurrencyKeySource : DeucarianAssetKeySource<CurrencyDefinitionAsset>
    {
        public override Type KeyType => typeof(CurrencyKey);
        public override Type DefinitionSetAttribute => typeof(CurrencyKeySetAttribute);
        public override string GeneratedClassName => "ProjectCurrencies";
        protected override DeucarianKeyChoice ReadDefinition(CurrencyDefinitionAsset asset) => new DeucarianKeyChoice(asset.Id, asset.DisplayName);
    }
}
