using System;
using Deucarian.Editor;
using UnityEditor;

namespace Deucarian.Progression.Editor
{
    [CustomPropertyDrawer(typeof(ResearchKey), true)]
    public sealed class ResearchKeyDrawer : DeucarianKeyDrawer
    {
        public override Type KeyType => typeof(ResearchKey);
        public override Type DefinitionSetAttribute => typeof(ResearchKeySetAttribute);
        public override string SetupHint => "Select an existing ResearchKey; declare reusable keys once in a [ResearchKeySet] class.";
    }
}
