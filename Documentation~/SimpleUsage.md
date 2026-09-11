# Simple usage

Configure ProgressionHost once with a ProgressionProfile(playerState, catalog, rewardDictionary). Register battle.victory as a RewardBundle and damage as a research node in the existing catalog. Each player has a separate state/profile. GrantReward and PurchaseResearch return ProgressionResult. Reuse an operation ID when retrying the same action; use a new ID for a distinct action. Existing idempotency, validation, costs, and prerequisites remain authoritative. ProgressionProfile is pure C#; ProgressionHost lives in a separate Unity adapter assembly. Persistence remains an application composition concern.

Import the **Simple Usage** sample from Unity Package Manager. Its caller script is:

Definition fields now use named, domain-specific keys. Select an existing definition from the Inspector dropdown or pass the same named key in code. Declare each project key once in a marked key set; ordinary caller methods do not accept raw IDs. Generated keys for asset-authored definitions require no asset reference in the caller. Owner-issued selection and row handles represent runtime instances.

```csharp
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
```
