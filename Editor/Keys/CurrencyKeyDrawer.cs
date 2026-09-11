using System;
using Deucarian.Editor;
using UnityEditor;

namespace Deucarian.Progression.Editor
{
    [CustomPropertyDrawer(typeof(CurrencyKey), true)]
    public sealed class CurrencyKeyDrawer : DeucarianKeyDrawer
    {
        public override Type KeyType => typeof(CurrencyKey);
        public override Type DefinitionSetAttribute => typeof(CurrencyKeySetAttribute);
        public override string SetupHint => "Select an existing CurrencyKey; declare reusable keys once in a [CurrencyKeySet] class.";
    }
}
