using System;
using System.Collections.Generic;
using Deucarian.GameplayFoundation;
using NUnit.Framework;

namespace Deucarian.Progression.Tests
{
    public sealed class ProgressionPackageTests
    {
        private static readonly CurrencyId Shards = new CurrencyId("currency.blood-shards");
        private static readonly CurrencyId Gold = new CurrencyId("currency.gold");
        private static readonly TrackId Legacy = new TrackId("track.legacy");
        private static readonly UnlockId ClassUnlock = new UnlockId("unlock.class.alchemist");
        private static readonly MetricId Waves = new MetricId("metric.waves");

        [Test]
        public void StableIdentifiers_UseGameplayFoundationRules()
        {
            Assert.AreEqual("currency.blood-shards", Shards.Value);
            Assert.Throws<ArgumentException>(() => new CurrencyId("Blood Shards"));
            Assert.Throws<ArgumentException>(() => new TrackId(" track.legacy"));
            Assert.Throws<ArgumentException>(() => new ProgressionOperationId(string.Empty));
        }

        [Test]
        public void Amounts_RejectNegativeValues_AndDetectOverflow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProgressionAmount(-1));
            Assert.IsFalse(ProgressionAmount.TryAdd(long.MaxValue, 1, out _));
            Assert.IsFalse(ProgressionAmount.TrySubtract(1, 2, out _));
        }

        [Test]
        public void CurrencyTransactions_CreditDebitAndZeroModifierBehavior()
        {
            ProgressionCatalog catalog = Catalog();
            ProgressionState state = new ProgressionState();

            ProgressionResult empty = state.ApplyTransaction(catalog, new ProgressionOperationId("op.empty"), Array.Empty<CurrencyLine>());
            Assert.IsTrue(empty.Succeeded);
            Assert.AreEqual(0, state.GetBalance(Shards).Value);

            ProgressionResult credit = state.ApplyTransaction(catalog, new ProgressionOperationId("op.credit"), Lines(Credit(Shards, 30)));
            Assert.IsTrue(credit.Succeeded);
            Assert.AreEqual(30, state.GetBalance(Shards).Value);

            ProgressionResult debit = state.ApplyTransaction(catalog, new ProgressionOperationId("op.debit"), Lines(Debit(Shards, 12)));
            Assert.IsTrue(debit.Succeeded);
            Assert.AreEqual(18, state.GetBalance(Shards).Value);
        }

        [Test]
        public void CurrencyTransactions_AreAtomic_AndRejectDuplicateLines()
        {
            ProgressionCatalog catalog = Catalog();
            ProgressionState state = Seeded(Shards, 10);

            ProgressionResult result = state.ApplyTransaction(
                catalog,
                new ProgressionOperationId("op.atomic"),
                Lines(Credit(Gold, 5), Debit(Shards, 100)));

            Assert.AreEqual(ProgressionStatus.InsufficientFunds, result.Status);
            Assert.AreEqual(0, state.GetBalance(Gold).Value);
            Assert.AreEqual(10, state.GetBalance(Shards).Value);

            ProgressionResult duplicate = state.ApplyTransaction(
                catalog,
                new ProgressionOperationId("op.duplicate-line"),
                Lines(Credit(Shards, 1), Credit(Shards, 1)));
            Assert.AreEqual(ProgressionStatus.DuplicateLine, duplicate.Status);
        }

        [Test]
        public void Operations_AreIdempotent_AndFailuresDoNotConsumeOperationId()
        {
            ProgressionCatalog catalog = Catalog();
            ProgressionState state = new ProgressionState();
            ProgressionOperationId id = new ProgressionOperationId("op.reward.1");

            Assert.AreEqual(ProgressionStatus.InsufficientFunds, state.ApplyTransaction(catalog, id, Lines(Debit(Shards, 1))).Status);
            Assert.AreEqual(ProgressionStatus.Success, state.ApplyTransaction(catalog, id, Lines(Credit(Shards, 7))).Status);
            Assert.AreEqual(ProgressionStatus.DuplicateOperation, state.ApplyTransaction(catalog, id, Lines(Credit(Shards, 7))).Status);
            Assert.AreEqual(7, state.GetBalance(Shards).Value);
        }

        [Test]
        public void CurrencyTransactions_RespectConfiguredCaps()
        {
            ProgressionCatalog catalog = new ProgressionCatalog(new[] { new CurrencyDefinition(Shards, new ProgressionAmount(10)) });
            ProgressionState state = new ProgressionState();

            Assert.AreEqual(ProgressionStatus.Overflow, state.ApplyTransaction(catalog, new ProgressionOperationId("op.cap"), Lines(Credit(Shards, 11))).Status);
            Assert.AreEqual(0, state.GetBalance(Shards).Value);
        }

        [Test]
        public void Tracks_AdvanceResetAndClampAtMaxLevel()
        {
            ProgressionCatalog catalog = Catalog();
            ProgressionState state = new ProgressionState();
            RewardBundle reward = new RewardBundle(xpGrants: new[] { new XpGrant(Legacy, new ProgressionAmount(25)) });

            Assert.AreEqual(1, state.GetTrackLevel(catalog, Legacy));
            Assert.IsTrue(state.ApplyReward(catalog, new ProgressionOperationId("op.xp"), reward).Succeeded);
            Assert.AreEqual(25, state.GetTrackTotal(Legacy).Value);
            Assert.AreEqual(3, state.GetTrackLevel(catalog, Legacy));

            ProgressionState reset = new ProgressionState();
            Assert.AreEqual(0, reset.GetTrackTotal(Legacy).Value);
            Assert.AreEqual(1, reset.GetTrackLevel(catalog, Legacy));
        }

        [Test]
        public void TrackDefinitions_RejectInvalidThresholdOrdering()
        {
            Assert.Throws<ArgumentException>(() => new ProgressionTrackDefinition(Legacy, 1, new[] { new ProgressionAmount(10), new ProgressionAmount(10) }));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProgressionTrackDefinition(Legacy, -1, Array.Empty<ProgressionAmount>()));
        }

        [Test]
        public void Rewards_CombineCurrencyXpAndUnlocks_Atomically()
        {
            ProgressionCatalog catalog = Catalog();
            ProgressionState state = new ProgressionState();
            RewardBundle reward = new RewardBundle(
                Lines(Credit(Shards, 20)),
                new[] { new XpGrant(Legacy, new ProgressionAmount(10)) },
                new[] { ClassUnlock });

            Assert.IsTrue(state.ApplyReward(catalog, new ProgressionOperationId("op.combined"), reward).Succeeded);
            Assert.AreEqual(20, state.GetBalance(Shards).Value);
            Assert.AreEqual(2, state.GetTrackLevel(catalog, Legacy));
            Assert.IsTrue(state.IsUnlocked(ClassUnlock));

            RewardBundle invalidReward = new RewardBundle(currencyLines: Lines(Credit(new CurrencyId("currency.unknown"), 1)));
            Assert.AreEqual(ProgressionStatus.UnknownDefinition, state.ApplyReward(catalog, new ProgressionOperationId("op.invalid-reward"), invalidReward).Status);
        }

        [Test]
        public void ResearchGraph_ValidatesDuplicatesMissingPrerequisitesAndCycles()
        {
            ResearchNodeDefinition root = Research("research.root", 1, Shards, 1);
            Assert.Throws<ArgumentException>(() => new ProgressionCatalog(research: new[] { root, root }));

            ResearchNodeDefinition missing = new ResearchNodeDefinition(
                new ResearchNodeId("research.child"),
                1,
                Lines(Debit(Shards, 1)),
                new[] { new ResearchPrerequisite(new ResearchNodeId("research.missing"), 1) });
            Assert.Throws<ArgumentException>(() => new ProgressionCatalog(research: new[] { missing }));

            ResearchNodeDefinition a = new ResearchNodeDefinition(
                new ResearchNodeId("research.a"),
                1,
                Lines(Debit(Shards, 1)),
                new[] { new ResearchPrerequisite(new ResearchNodeId("research.b"), 1) });
            ResearchNodeDefinition b = new ResearchNodeDefinition(
                new ResearchNodeId("research.b"),
                1,
                Lines(Debit(Shards, 1)),
                new[] { new ResearchPrerequisite(new ResearchNodeId("research.a"), 1) });
            Assert.Throws<ArgumentException>(() => new ProgressionCatalog(research: new[] { a, b }));
        }

        [Test]
        public void ResearchPurchase_UsesPrerequisitesCostsUnlocksAndMaxRank()
        {
            ResearchNodeId rootId = new ResearchNodeId("research.root");
            ResearchNodeId childId = new ResearchNodeId("research.child");
            ResearchNodeDefinition root = new ResearchNodeDefinition(rootId, 1, Lines(Debit(Shards, 5)));
            ResearchNodeDefinition child = new ResearchNodeDefinition(
                childId,
                1,
                Lines(Debit(Shards, 7)),
                new[] { new ResearchPrerequisite(rootId, 1) },
                new[] { ClassUnlock });
            ProgressionCatalog catalog = new ProgressionCatalog(new[] { new CurrencyDefinition(Shards, ProgressionAmount.Max) }, research: new[] { root, child });
            ProgressionState state = Seeded(Shards, 20);

            Assert.AreEqual(ProgressionStatus.MissingPrerequisite, state.PurchaseResearch(catalog, new ProgressionOperationId("op.child.fail"), childId).Status);
            Assert.IsTrue(state.PurchaseResearch(catalog, new ProgressionOperationId("op.root"), rootId).Succeeded);
            Assert.AreEqual(ProgressionStatus.MissingPrerequisite, state.PurchaseResearch(catalog, new ProgressionOperationId("op.child.unlock-fail"), childId).Status);
            Assert.IsTrue(state.ApplyReward(catalog, new ProgressionOperationId("op.unlock"), new RewardBundle(unlocks: new[] { ClassUnlock })).Succeeded);
            Assert.IsTrue(state.PurchaseResearch(catalog, new ProgressionOperationId("op.child"), childId).Succeeded);
            Assert.AreEqual(1, state.GetResearchRank(childId));
            Assert.AreEqual(ProgressionStatus.MaxRankReached, state.PurchaseResearch(catalog, new ProgressionOperationId("op.child.max"), childId).Status);
        }

        [Test]
        public void Milestones_CompleteClaimOnceAndDoNotRepeatRewards()
        {
            MilestoneId id = new MilestoneId("milestone.wave-10");
            ProgressionCatalog catalog = Catalog(milestones: new[] { new MilestoneDefinition(id, Waves, new ProgressionAmount(10), new RewardBundle(Lines(Credit(Shards, 3)))) });
            ProgressionState state = new ProgressionState();

            Assert.IsTrue(state.IncrementMetric(catalog, Waves, new ProgressionAmount(9)).Succeeded);
            Assert.IsFalse(state.IsMilestoneCompleted(id));
            Assert.AreEqual(ProgressionStatus.NotCompleted, state.ClaimMilestone(catalog, new ProgressionOperationId("op.claim.fail"), id).Status);

            Assert.IsTrue(state.IncrementMetric(catalog, Waves, new ProgressionAmount(1)).Succeeded);
            Assert.IsTrue(state.IsMilestoneCompleted(id));
            Assert.IsTrue(state.ClaimMilestone(catalog, new ProgressionOperationId("op.claim"), id).Succeeded);
            Assert.AreEqual(3, state.GetBalance(Shards).Value);
            Assert.AreEqual(ProgressionStatus.AlreadyClaimed, state.ClaimMilestone(catalog, new ProgressionOperationId("op.claim.2"), id).Status);
            Assert.AreEqual(3, state.GetBalance(Shards).Value);
        }

        [Test]
        public void Snapshots_AreConsistentAndDeterministicallyOrdered()
        {
            ProgressionCatalog catalog = Catalog();
            ProgressionState state = new ProgressionState();
            state.ApplyReward(
                catalog,
                new ProgressionOperationId("op.snapshot"),
                new RewardBundle(
                    Lines(Credit(Gold, 1), Credit(Shards, 2)),
                    new[] { new XpGrant(Legacy, new ProgressionAmount(10)) },
                    new[] { ClassUnlock }));

            ProgressionSnapshot snapshot = state.CreateSnapshot();
            Assert.AreEqual(Shards, snapshot.Balances[0].Id);
            Assert.AreEqual(Gold, snapshot.Balances[1].Id);
            Assert.AreEqual(Legacy, snapshot.Tracks[0].Id);
            Assert.AreEqual(ClassUnlock, snapshot.Unlocks[0]);
            Assert.AreEqual(new ProgressionOperationId("op.snapshot"), snapshot.AppliedOperations[0]);
        }

        [Test]
        public void DonorWorkflow_MapsBloodShardsLegacyXpAndMetaUpgrade()
        {
            CurrencyId bloodShards = new CurrencyId("currency.blood-shards");
            TrackId legacyXp = new TrackId("track.legacy-experience");
            ResearchNodeId metaDamage = new ResearchNodeId("meta.damage");
            ProgressionCatalog catalog = new ProgressionCatalog(
                new[] { new CurrencyDefinition(bloodShards, ProgressionAmount.Max) },
                new[] { new ProgressionTrackDefinition(legacyXp, 1, new[] { new ProgressionAmount(50), new ProgressionAmount(125) }) },
                new[] { new ResearchNodeDefinition(metaDamage, 2, Lines(Debit(bloodShards, 5), Debit(bloodShards, 8))) });
            ProgressionState state = new ProgressionState();
            RewardBundle runReward = new RewardBundle(
                Lines(Credit(bloodShards, 13)),
                new[] { new XpGrant(legacyXp, new ProgressionAmount(80)) });

            Assert.IsTrue(state.ApplyReward(catalog, new ProgressionOperationId("run.0001"), runReward).Succeeded);
            Assert.IsTrue(state.PurchaseResearch(catalog, new ProgressionOperationId("meta.damage.rank1"), metaDamage).Succeeded);
            Assert.AreEqual(8, state.GetBalance(bloodShards).Value);
            Assert.AreEqual(2, state.GetTrackLevel(catalog, legacyXp));

            StatModifier modifier = new StatModifier(
                new StatModifierHandle("meta.damage.rank1"),
                new ModifierSourceHandle("meta.damage"),
                new StatId("stat.damage"),
                StatModifierOperation.Multiplicative,
                1.1,
                0);
            StatBlock stats = new StatBlock();
            stats.SetBaseValue(new StatId("stat.damage"), 10);
            stats.AddModifier(modifier);
            Assert.AreEqual(11, stats.GetValue(new StatId("stat.damage")), 0.0001);
        }

        [Test]
        public void CrossGenreProof_SupportsIdleAutoDefenseAndClassicTowerDefense()
        {
            CurrencyId scrap = new CurrencyId("currency.scrap");
            CurrencyId gems = new CurrencyId("currency.gems");
            ResearchNodeId turretSpeed = new ResearchNodeId("research.turret-speed");
            TrackId account = new TrackId("track.account");
            ProgressionCatalog catalog = new ProgressionCatalog(
                new[] { new CurrencyDefinition(scrap, ProgressionAmount.Max), new CurrencyDefinition(gems, ProgressionAmount.Max) },
                new[] { new ProgressionTrackDefinition(account, 0, new[] { new ProgressionAmount(1000) }) },
                new[] { new ResearchNodeDefinition(turretSpeed, 1, Lines(Debit(scrap, 100))) });
            ProgressionState state = new ProgressionState();

            Assert.IsTrue(state.ApplyReward(catalog, new ProgressionOperationId("idle.offline.1"), new RewardBundle(Lines(Credit(scrap, 120)), new[] { new XpGrant(account, new ProgressionAmount(1500)) })).Succeeded);
            Assert.IsTrue(state.PurchaseResearch(catalog, new ProgressionOperationId("tower.speed.1"), turretSpeed).Succeeded);
            Assert.IsTrue(state.ApplyTransaction(catalog, new ProgressionOperationId("td.wave-gem"), Lines(Credit(gems, 3))).Succeeded);
            Assert.AreEqual(20, state.GetBalance(scrap).Value);
            Assert.AreEqual(3, state.GetBalance(gems).Value);
            Assert.AreEqual(1, state.GetTrackLevel(catalog, account));
        }

        [Test]
        public void HotPathQueries_DoNotAllocateAfterWarmup()
        {
            ProgressionCatalog catalog = Catalog();
            ProgressionState state = new ProgressionState();
            state.ApplyReward(catalog, new ProgressionOperationId("op.warm"), new RewardBundle(Lines(Credit(Shards, 10)), new[] { new XpGrant(Legacy, new ProgressionAmount(10)) }, new[] { ClassUnlock }));

            for (int index = 0; index < 100; index++)
            {
                state.GetBalance(Shards);
                state.GetTrackLevel(catalog, Legacy);
                state.IsUnlocked(ClassUnlock);
                state.GetResearchRank(new ResearchNodeId("research.none"));
            }

            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int index = 0; index < 1000; index++)
            {
                state.GetBalance(Shards);
                state.GetTrackLevel(catalog, Legacy);
                state.IsUnlocked(ClassUnlock);
                state.GetResearchRank(new ResearchNodeId("research.none"));
            }

            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            Assert.AreEqual(0, allocated);
        }

        private static ProgressionCatalog Catalog(IReadOnlyList<MilestoneDefinition> milestones = null)
        {
            return new ProgressionCatalog(
                new[] { new CurrencyDefinition(Shards, ProgressionAmount.Max), new CurrencyDefinition(Gold, ProgressionAmount.Max) },
                new[] { new ProgressionTrackDefinition(Legacy, 1, new[] { new ProgressionAmount(10), new ProgressionAmount(25), new ProgressionAmount(50) }) },
                milestones: milestones);
        }

        private static ProgressionState Seeded(CurrencyId currencyId, long amount)
        {
            ProgressionState state = new ProgressionState();
            ProgressionCatalog catalog = new ProgressionCatalog(new[] { new CurrencyDefinition(currencyId, ProgressionAmount.Max), new CurrencyDefinition(Gold, ProgressionAmount.Max) });
            state.ApplyTransaction(catalog, new ProgressionOperationId("op.seed." + currencyId.Value), Lines(Credit(currencyId, amount)));
            return state;
        }

        private static ResearchNodeDefinition Research(string id, int maxRank, CurrencyId currencyId, long cost)
        {
            List<CurrencyLine> lines = new List<CurrencyLine>();
            for (int index = 0; index < maxRank; index++)
            {
                lines.Add(Debit(currencyId, cost));
            }

            return new ResearchNodeDefinition(new ResearchNodeId(id), maxRank, lines);
        }

        private static CurrencyLine Credit(CurrencyId id, long amount) => new CurrencyLine(id, new ProgressionAmount(amount), true);

        private static CurrencyLine Debit(CurrencyId id, long amount) => new CurrencyLine(id, new ProgressionAmount(amount), false);

        private static CurrencyLine[] Lines(params CurrencyLine[] lines) => lines;
    }
}
