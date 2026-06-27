using UnityEngine;

// 적이 최근 자신을 공격한 플레이어와 타워를 짧게 기억합니다.
// EnemyBrain은 이 메모리에서 우선 반격 후보만 받아 타깃 판단 흐름을 단순하게 유지합니다.
public sealed class EnemyAggroMemory
{
    private const string PlayerTag = "Player";

    private Damageable _recentPlayerAttacker;
    private Damageable _recentTowerAttacker;
    private float _lastPlayerAttackedTime;
    private float _lastTowerAttackedTime;

    public void ReportAttacker(Damageable attackerDamageable)
    {
        if (attackerDamageable == null)
        {
            return;
        }

        if (attackerDamageable.CompareTag(PlayerTag))
        {
            _recentPlayerAttacker = attackerDamageable;
            _lastPlayerAttackedTime = Time.time;
            return;
        }

        if (attackerDamageable.GetComponentInParent<BuildingTower>() != null)
        {
            _recentTowerAttacker = attackerDamageable;
            _lastTowerAttackedTime = Time.time;
        }
    }

    public Damageable SelectNearestRecentAttacker(Transform ownerTransform, float memoryDuration)
    {
        return EnemyTargetSelector.SelectNearestRecentAttacker(
            ownerTransform,
            _recentPlayerAttacker,
            _lastPlayerAttackedTime,
            _recentTowerAttacker,
            _lastTowerAttackedTime,
            memoryDuration);
    }
}
