using System.Collections.Generic;
using UnityEngine;

// CinderHeart 주변 방어 함정입니다. 적이 밟으면 일정 시간 느려지고 Run Result에 이동방해 점수를 보고합니다.
public sealed class TrapSlowZone : MonoBehaviour
{
    [SerializeField] private float _speedMultiplier = 0.5f;
    [SerializeField] private float _slowDurationSeconds = 2.5f;
    [SerializeField] private float _reapplyCooldownSeconds = 1.2f;
    [SerializeField] private TrapCrowdControlReporter _crowdControlReporter;

    private readonly Dictionary<EnemyMovement, float> _lastAppliedTimes = new Dictionary<EnemyMovement, float>();

    private void Awake()
    {
        ConnectReporter();
    }

    private void OnTriggerStay(Collider other)
    {
        EnemyMovement enemyMovement = other == null ? null : other.GetComponentInParent<EnemyMovement>();
        if (enemyMovement == null)
        {
            return;
        }

        if (CanApplySlow(enemyMovement) == false)
        {
            return;
        }

        enemyMovement.ApplyMoveSpeedMultiplier(_speedMultiplier, _slowDurationSeconds);
        _lastAppliedTimes[enemyMovement] = Time.time;
        ReportCrowdControlScore();
    }

    private bool CanApplySlow(EnemyMovement enemyMovement)
    {
        if (enemyMovement == null)
        {
            return false;
        }

        float lastAppliedTime;
        if (_lastAppliedTimes.TryGetValue(enemyMovement, out lastAppliedTime) == false)
        {
            return true;
        }

        return Time.time >= lastAppliedTime + _reapplyCooldownSeconds;
    }

    private void ReportCrowdControlScore()
    {
        ConnectReporter();
        if (_crowdControlReporter == null)
        {
            return;
        }

        float slowScore = 1f + Mathf.Clamp01(1f - _speedMultiplier);
        _crowdControlReporter.ReportScore(slowScore);
    }

    private void ConnectReporter()
    {
        if (_crowdControlReporter != null)
        {
            return;
        }

        _crowdControlReporter = GetComponent<TrapCrowdControlReporter>();
        if (_crowdControlReporter == null)
        {
            _crowdControlReporter = gameObject.AddComponent<TrapCrowdControlReporter>();
        }
    }
}
