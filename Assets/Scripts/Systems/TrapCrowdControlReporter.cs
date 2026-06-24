using System;
using UnityEngine;

// 5.03 direction: Gives trap prefabs a small reporting hook for Run Result crowd-control scoring.
// 5.04+ note: Keep real trap behavior in trap-specific components and report only the final score here.
public sealed class TrapCrowdControlReporter : MonoBehaviour
{
    public static event Action<float> TrapCrowdControlScoredGlobal;

    [Tooltip("Score multiplier applied when a trap reports slow, stun, knockback, or path delay value.")]
    [SerializeField] private float _scoreMultiplier = 1f;

    public void ReportScore(float score)
    {
        float finalScore = Mathf.Max(0f, score) * Mathf.Max(0f, _scoreMultiplier);
        if (finalScore <= 0f)
        {
            return;
        }

        NotifyTrapCrowdControlScored(finalScore);
    }

    public void ReportSlow(float normalTravelSeconds, float slowedTravelSeconds)
    {
        if (normalTravelSeconds <= 0f || slowedTravelSeconds <= normalTravelSeconds)
        {
            return;
        }

        float score = slowedTravelSeconds / normalTravelSeconds;
        ReportScore(score);
    }

    public void ReportStun(float stunSeconds)
    {
        ReportScore(stunSeconds);
    }

    private void NotifyTrapCrowdControlScored(float score)
    {
        if (TrapCrowdControlScoredGlobal == null)
        {
            return;
        }

        TrapCrowdControlScoredGlobal(score);
    }
}
