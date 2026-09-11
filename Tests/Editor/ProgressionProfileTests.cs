using System.Collections.Generic;
using NUnit.Framework;

namespace Deucarian.Progression.Tests
{
    public sealed class ProgressionProfileTests
    {
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
            Assert.That(a.GrantReward("battle.victory", "win.1").Succeeded, Is.True);
            Assert.That(a.GrantReward("battle.victory", "win.1").Status, Is.EqualTo(ProgressionStatus.DuplicateOperation));
            Assert.That(a.PurchaseResearch("damage", "purchase.1").Succeeded, Is.True);
            Assert.That(a.PurchaseResearch("damage", "purchase.1").Status, Is.EqualTo(ProgressionStatus.DuplicateOperation));
            Assert.That(a.GetBalance("gold").Value, Is.EqualTo(5));
            Assert.That(b.GetBalance("gold").Value, Is.Zero);
        }
    }
}
