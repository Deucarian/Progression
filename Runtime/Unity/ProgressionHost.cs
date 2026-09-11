using System;
using UnityEngine;

namespace Deucarian.Progression.Unity
{
    /// <summary>Scene access to one explicitly supplied player progression profile.</summary>
    [DisallowMultipleComponent]
    public sealed class ProgressionHost : MonoBehaviour
    {
        private ProgressionProfile profile;
        private bool destroyed;
        public void Configure(ProgressionProfile value)
        {
            if (destroyed) throw new ObjectDisposedException(nameof(ProgressionHost));
            if (profile != null) throw new InvalidOperationException("The progression host is already configured.");
            profile = value ?? throw new ArgumentNullException(nameof(value));
        }
        public ProgressionResult GrantReward(RewardKey key, ProgressionOperationId operationId) => Profile.GrantReward(key, operationId);
        public ProgressionResult PurchaseResearch(ResearchKey key, ProgressionOperationId operationId) => Profile.PurchaseResearch(key, operationId);
        public ProgressionAmount GetBalance(CurrencyKey key) => Profile.GetBalance(key);
        public ProgressionSnapshot Snapshot => Profile.Snapshot;
        private ProgressionProfile Profile => profile ?? throw new InvalidOperationException("Configure the progression host first.");
        private void OnDestroy() { destroyed = true; profile = null; }
    }
}
