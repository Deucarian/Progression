namespace Deucarian.Progression.Unity.Samples.SimpleUsage
{
    [RewardKeySet]
    public static class RewardKeys
    {
        public static RewardKey Victory => new Definition();
        private sealed class Definition : RewardKey
        {
            public Definition() : base("battle.victory") { }
        }
    }
}
