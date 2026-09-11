using UnityEngine;

namespace Deucarian.Progression.Unity.Samples.SimpleUsage
{
    public sealed class SimpleUsageExample : MonoBehaviour
    {
        [SerializeField] private ResearchKey damage = ResearchKeys.Damage;

        [SerializeField] private RewardKey victory = RewardKeys.Victory;

        [SerializeField] private ProgressionHost progression;
        public void Reward(ProgressionOperationId operationId) => progression.GrantReward(victory, operationId);
        public void Upgrade(ProgressionOperationId operationId) => progression.PurchaseResearch(damage, operationId);
    }
}
