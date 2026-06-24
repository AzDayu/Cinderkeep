using Cinderkeep.Gameplay;
using System;
using UnityEngine;

// 5.00 direction: Routes combat hit and damage flow for player, enemy, building, and CinderHeart interactions.
// 5.01+ note: Keep damage delivery generic so weapons, towers, traps, and bosses can reuse the same contract.
// 공격자가 가진 데미지 값을 관리하는 컴포넌트입니다.
// 무기, 몬스터 공격 판정, 함정 오브젝트에 붙여 재사용할 수 있습니다.
public enum DamageSourceType
{
    Unknown,
    Player,
    Enemy,
    Tower,
    Trap,
    CinderHeart,
    Boss
}

public sealed class DamageDealer : MonoBehaviour
{
    public static event Action<DamageDealer, Damageable, float, DamageSourceType> DamageAppliedGlobal;

    [Tooltip("이 공격 판정이 대상에게 전달할 피해량입니다.")]
    [SerializeField] private float _damage = 40f;
    [Tooltip("Run Result and combat analytics use this to separate player, tower, trap, and boss damage.")]
    [SerializeField] private DamageSourceType _sourceType = DamageSourceType.Unknown;

    public float GetDamageValue()
    {
        return _damage;
    }

    public void SetDamageValue(float damage)
    {
        _damage = Mathf.Max(0f, damage);
    }

    public void SetSourceType(DamageSourceType sourceType)
    {
        _sourceType = sourceType;
    }

    public void SetDamageValue(WeaponData weaponData)
    {
        if (weaponData == null)
        {
            return;
        }

        SetDamageValue(weaponData.Damage);
    }

    public void ApplyDamage(Damageable damageable)
    {
        if (damageable == null)
        {
            return;
        }

        damageable.TakeDamage(_damage);
        NotifyDamageApplied(damageable);
    }

    private void NotifyDamageApplied(Damageable damageable)
    {
        if (DamageAppliedGlobal == null)
        {
            return;
        }

        DamageAppliedGlobal(this, damageable, _damage, _sourceType);
    }
}
