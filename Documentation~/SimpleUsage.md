# Simple usage

Configure ProgressionHost once with a ProgressionProfile(playerState, catalog, rewardDictionary). Register battle.victory as a RewardBundle and damage as a research node in the existing catalog. Each player has a separate state/profile. GrantReward and PurchaseResearch return ProgressionResult. Reuse an operation ID when retrying the same action; use a new ID for a distinct action. Existing idempotency, validation, costs, and prerequisites remain authoritative. ProgressionProfile is pure C#; ProgressionHost lives in a separate Unity adapter assembly. Persistence remains an application composition concern.

Import the **Simple Usage** sample from Unity Package Manager. Its caller script is:

```csharp
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
```
