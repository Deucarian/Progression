// <deucarian-definition schema="rewards" />
// Editable declaration. Use the package definition editor or edit the values below.
namespace Deucarian.ProjectDefinitions.Definition_rewards
{
    public static class Definition_ProgressionSampleReward
    {
        public static global::Deucarian.Progression.Editor.Definitions.RewardDefinitionSpec Value =>
        // definition-value
        new global::Deucarian.Progression.Editor.Definitions.RewardDefinitionSpec
        {
            Amount = 10L,
            Currency = global::Deucarian.Editor.Definitions.DeucarianDefinitionAssets.Load<global::Deucarian.Progression.Unity.CurrencyDefinitionAsset>("202e19d1c54174e4393c4e7ac3814bba", 11400000L),
            Id = "f48fd3f7a8f04c419e8d17e8d7d47f1f",
            Name = "ProgressionSampleReward",
        };
        // end-definition-value
    }
}
