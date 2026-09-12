using System;
using UnityEngine;
using Deucarian.Progression;
namespace Deucarian.Progression.Unity.Samples.DefinitionWorkflow
{
    /// <summary>Small caller example. The configured scene hosts own services and resource lifetimes.</summary>
    public sealed class ProgressionWorkflow : MonoBehaviour
    {
        [SerializeField] private ProgressionHost host;
        [SerializeField] private CurrencyKey currency;
        [SerializeField] private RewardTrigger reward;
        private string status = "Ready. Choose an action below.";
        public string Status => status;
        public void Grant() { var result = reward.GrantReward(); status = "Balance: " + host.GetBalance(currency) + " (" + result + ")"; }
        public void NewGrant() { reward.BeginNewGrant(); Grant(); }
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(24, 24, Math.Min(540, Screen.width - 48), Screen.height - 48), GUI.skin.box);
            GUILayout.Label("Progression — definition workflow");
            GUILayout.Label("A reward reuses its currency and amount from Definitions. Retrying the same operation cannot grant twice. A new operation represents a new reward.");
            GUILayout.Space(12);
            if (GUILayout.Button("Grant reward", GUILayout.Height(32))) { try { Grant(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Retry same grant", GUILayout.Height(32))) { try { Grant(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Begin another grant", GUILayout.Height(32))) { try { NewGrant(); } catch (Exception error) { status = error.Message; } }
            GUILayout.Space(12);
            GUILayout.Label(status);
            GUILayout.EndArea();
        }
    }
}
