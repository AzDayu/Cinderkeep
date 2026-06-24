using UnityEngine;

// 5.00 direction: Routes combat hit and damage flow for player, enemy, building, and CinderHeart interactions.
// 5.01+ note: Keep damage delivery generic so weapons, towers, traps, and bosses can reuse the same contract.
// 공격 판정용 Trigger Collider에 붙이는 컴포넌트입니다.
// 충돌 감지만 담당하고, 실제 데미지 값은 DamageDealer가 관리합니다.
public sealed class AttackHitbox : MonoBehaviour
{
    [SerializeField] private DamageDealer _damageDealer;

    private void Awake()
    {
        ConnectDamageDealerIfNeeded();
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (_damageDealer == null)
        {
            Debug.LogWarning(gameObject.name + ": DamageDealer가 연결되지 않았습니다.");
            return;
        }

        Damageable damageable = GetDamageableFromCollider(otherCollider);
        if (damageable == null)
        {
            return;
        }

        _damageDealer.ApplyDamage(damageable);
    }

    private void ConnectDamageDealerIfNeeded()
    {
        if (_damageDealer != null)
        {
            return;
        }

        _damageDealer = GetComponent<DamageDealer>();
    }

    private Damageable GetDamageableFromCollider(Collider otherCollider)
    {
        if (otherCollider == null)
        {
            return null;
        }

        Damageable damageable = otherCollider.GetComponent<Damageable>();
        if (damageable != null)
        {
            return damageable;
        }

        return otherCollider.GetComponentInParent<Damageable>();
    }
}
