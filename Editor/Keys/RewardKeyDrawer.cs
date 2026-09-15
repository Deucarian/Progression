using System;
using Deucarian.Editor;
using UnityEditor;

namespace Deucarian.Progression.Editor
{
    [CustomPropertyDrawer(typeof(RewardKey), true)]
    public sealed class RewardKeyDrawer : DeucarianKeyDrawer
    {
        public override Type KeyType => typeof(RewardKey);
        public override Type DefinitionSetAttribute => typeof(RewardKeySetAttribute);
        public override string SetupHint => "Select an existing RewardKey; declare reusable keys once in a [RewardKeySet] class.";
    }
}
