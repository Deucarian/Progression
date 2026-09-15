using System;
using UnityEngine;

namespace Deucarian.Progression.Unity
{
    /// <summary>One grant operation; repeat calls retain the operation ID so the core rejects duplicate grants.</summary>
    [AddComponentMenu("Deucarian/Progression/Reward Trigger")]
    public sealed class RewardTrigger : MonoBehaviour
    {
        [SerializeField] private ProgressionHost host;
        [SerializeField] private RewardKey reward;
        private ProgressionOperationId operation;
        public ProgressionResult GrantReward()
        {
            if (host == null) throw new InvalidOperationException("Assign a ProgressionHost to RewardTrigger '" + name + "'.");
            if (operation.IsEmpty) BeginNewGrant();
            return host.GrantReward(reward, operation);
        }
        public void Grant() => GrantReward();
        public void BeginNewGrant() => operation = new ProgressionOperationId(Guid.NewGuid().ToString("N"));
    }
}
