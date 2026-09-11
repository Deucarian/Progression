namespace Deucarian.Progression.Unity.Samples.SimpleUsage
{
    [ResearchKeySet]
    public static class ResearchKeys
    {
        public static ResearchKey Damage => new Definition();
        private sealed class Definition : ResearchKey
        {
            public Definition() : base("damage") { }
        }
    }
}
