namespace Deucarian.Progression.Samples
{
    /// <summary>Small code-only sample for the Progression package.</summary>
    public static class ProgressionSandboxSample
    {
        /// <summary>Runs a minimal progression scenario and returns the remaining shard balance.</summary>
        public static long Run()
        {
            CurrencyId shards = new CurrencyId("currency.blood-shards");
            TrackId legacy = new TrackId("track.legacy");
            UnlockId alchemist = new UnlockId("unlock.class.alchemist");
            ResearchNodeId damage = new ResearchNodeId("research.damage");
            ProgressionCatalog catalog = new ProgressionCatalog(
                new[] { new CurrencyDefinition(shards, ProgressionAmount.Max) },
                new[] { new ProgressionTrackDefinition(legacy, 1, new[] { new ProgressionAmount(10), new ProgressionAmount(25) }) },
                new[] { new ResearchNodeDefinition(damage, 1, new[] { new CurrencyLine(shards, new ProgressionAmount(5), false) }, requiredUnlocks: new[] { alchemist }) });

            ProgressionState state = new ProgressionState();
            RewardBundle reward = new RewardBundle(
                new[] { new CurrencyLine(shards, new ProgressionAmount(12), true) },
                new[] { new XpGrant(legacy, new ProgressionAmount(15)) },
                new[] { alchemist });

            state.ApplyReward(catalog, new ProgressionOperationId("sample.run"), reward);
            state.PurchaseResearch(catalog, new ProgressionOperationId("sample.research"), damage);
            return state.GetBalance(shards).Value;
        }
    }
}
