using UnityEngine;

// 5.00 direction: Supports base construction, defense objects, and building damage in the 5.00 loop.
// 5.01+ note: Keep placement, cost, health, tower attack, and upgrade rules split so 5.01+ defenses can expand.
public sealed class TowerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("타워가 적에게 주는 피해량입니다. buildings.json의 attackDamage로 덮어씁니다.")]
    [SerializeField] private float _attackDamage = 5f;
    [Tooltip("공격 후 다음 공격까지 기다리는 시간입니다. buildings.json의 attackInterval로 덮어씁니다.")]
    [SerializeField] private float _attackInterval = 1.4f;
    [Header("Connected Components")]
    [Tooltip("선택 사항입니다. 연결하면 DamageDealer를 통해 피해를 전달합니다. 없으면 TakeDamage를 직접 사용합니다.")]
    [SerializeField] private DamageDealer _damageDealer;

    private float _lastAttackTime;

    public float AttackDamage
    {
        get
        {
            return _attackDamage;
        }
    }

    private void Awake()
    {
        ConnectDamageDealer();
    }

    public void SetAttackDamage(float attackDamage)
    {
        _attackDamage = Mathf.Max(0f, attackDamage);

        if (_damageDealer != null)
        {
            _damageDealer.SetDamageValue(_attackDamage);
        }
    }

    public void SetAttackInterval(float attackInterval)
    {
        _attackInterval = Mathf.Max(0.1f, attackInterval);
    }

    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + _attackInterval;
    }

    public bool TryAttack(EnemyStatus targetEnemyStatus)
    {
        if (targetEnemyStatus == null)
        {
            return false;
        }

        if (targetEnemyStatus.IsDead)
        {
            return false;
        }

        if (CanAttack() == false)
        {
            return false;
        }

        ApplyDamageToEnemy(targetEnemyStatus);
        RecordAttackTime();
        return true;
    }

    private void ApplyDamageToEnemy(EnemyStatus targetEnemyStatus)
    {
        Damageable enemyDamageable = targetEnemyStatus.GetComponentInParent<Damageable>();
        if (_damageDealer != null && enemyDamageable != null)
        {
            _damageDealer.SetDamageValue(_attackDamage);
            _damageDealer.ApplyDamage(enemyDamageable);
            return;
        }

        targetEnemyStatus.TakeDamage(_attackDamage);
    }

    private void RecordAttackTime()
    {
        _lastAttackTime = Time.time;
    }

    private void ConnectDamageDealer()
    {
        if (_damageDealer != null)
        {
            return;
        }

        _damageDealer = GetComponent<DamageDealer>();
    }
}
