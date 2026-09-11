using UnityEngine;

namespace Deucarian.Progression.Unity.Samples.SimpleUsage
{
    public sealed class SimpleUsageExample : MonoBehaviour
    {
        [SerializeField] private ProgressionHost progression;
        public void Reward(string operationId) => progression.GrantReward("battle.victory", operationId);
        public void Upgrade(string operationId) => progression.PurchaseResearch("damage", operationId);
    }
}
