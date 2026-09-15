using System.Collections.Generic;
using NUnit.Framework;

namespace Deucarian.Progression.Tests
{
    public sealed class ProgressionProfileTests
    {
        private sealed class Reward : IRewardKey { public Reward(string id) { Id = id; } public string Id { get; } }
        private sealed class Research : IResearchKey { public Research(string id) { Id = id; } public string Id { get; } }
        private sealed class Currency : ICurrencyKey { public Currency(string id) { Id = id; } public string Id { get; } }

        [Test]
        public void RewardsAndResearchRetainIdempotencyAndPlayerIsolation()
        {
            var gold = new CurrencyId("gold");
            var catalog = new ProgressionCatalog(new[] { new CurrencyDefinition(gold, ProgressionAmount.Max) },
                research: new[] { new ResearchNodeDefinition(new ResearchNodeId("damage"), 1,
                    new[] { new CurrencyLine(gold, new ProgressionAmount(5), false) }) });
            var rewards = new Dictionary<string, RewardBundle>
            { ["battle.victory"] = new RewardBundle(currencyLines: new[] { new CurrencyLine(gold, new ProgressionAmount(10), true) }) };
            var a = new ProgressionProfile(new ProgressionState(), catalog, rewards);
            var b = new ProgressionProfile(new ProgressionState(), catalog, rewards);
            Assert.That(a.GrantReward(new Reward("battle.victory"), new ProgressionOperationId("win.1")).Succeeded, Is.True);
            Assert.That(a.GrantReward(new Reward("battle.victory"), new ProgressionOperationId("win.1")).Status, Is.EqualTo(ProgressionStatus.DuplicateOperation));
            Assert.That(a.PurchaseResearch(new Research("damage"), new ProgressionOperationId("purchase.1")).Succeeded, Is.True);
            Assert.That(a.PurchaseResearch(new Research("damage"), new ProgressionOperationId("purchase.1")).Status, Is.EqualTo(ProgressionStatus.DuplicateOperation));
            Assert.That(a.GetBalance(new Currency("gold")).Value, Is.EqualTo(5));
            Assert.That(b.GetBalance(new Currency("gold")).Value, Is.Zero);
        }
    }
}
