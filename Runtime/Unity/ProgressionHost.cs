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
        public ProgressionResult GrantReward(string rewardId, string operationId) => Profile.GrantReward(rewardId, operationId);
        public ProgressionResult PurchaseResearch(string researchId, string operationId) => Profile.PurchaseResearch(researchId, operationId);
        public ProgressionAmount GetBalance(string currencyId) => Profile.GetBalance(currencyId);
        public ProgressionSnapshot Snapshot => Profile.Snapshot;
        private ProgressionProfile Profile => profile ?? throw new InvalidOperationException("Configure the progression host first.");
        private void OnDestroy() { destroyed = true; profile = null; }
    }
}
