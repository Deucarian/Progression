using System;
using UnityEngine;

namespace Deucarian.Progression.Unity
{
    [AddComponentMenu("Deucarian/Progression/Research Trigger")]
    public sealed class ResearchTrigger : MonoBehaviour
    {
        [SerializeField] private ProgressionHost host;
        [SerializeField] private ResearchKey research;
        public ProgressionResult PurchaseResearch()
        {
            if (host == null) throw new InvalidOperationException("Assign a ProgressionHost to ResearchTrigger '" + name + "'.");
            return host.PurchaseResearch(research, new ProgressionOperationId(Guid.NewGuid().ToString("N")));
        }
        public void Purchase() => PurchaseResearch();
    }
}
