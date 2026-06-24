using Cinderkeep.Gameplay;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private Dictionary<string, float> _patternCooldowns = new Dictionary<string, float>();
    private GameDataManager _gameDataManager;

    private float _lastBasicAttackTime;

    // 보스 공격 시 이펙트 재생 등을 위해 필요할 수 있습니다.
    //[SerializeField] private Transform _attackOrigin;

    public void Initialize(GameDataManager gameDataManager)
    {
        _gameDataManager = gameDataManager;
        _patternCooldowns.Clear();
    }

    public bool TryBasicAttack(string bossId, Damageable target)
    {
        if (target == null || _gameDataManager == null)
        {
            return false;
        }

        var bossData = _gameDataManager.GetBoss(bossId);
        if (bossData == null)
        {
            return false;
        }

        if (Time.time < _lastBasicAttackTime + bossData.AttackInterval)
        {
            return false;
        }

        target.TakeDamage(bossData.AttackDamage);
        _lastBasicAttackTime = Time.time;

        return true;
    }

    public bool TryPatternAttack(string patternId, Damageable target)
    {
        if (target == null || _gameDataManager == null)
        {
            return false;
        }

        var bossPatternData = _gameDataManager.GetBossPattern(patternId);
        if (bossPatternData == null)
        {
            return false;
        }

        if (IsPatternOnCooldown(patternId, bossPatternData.Cooldown))
        {
            return false;
        }

        // 1. 공격 수행
        target.TakeDamage(bossPatternData.Damage);

        // 2. 이펙트/연출 (데이터의 EffectKey 활용)
        PlayPatternEffect(bossPatternData.EffectKey);

        // 3. 쿨타임 기록
        _patternCooldowns[bossPatternData.Id] = Time.time;

        return true;
    }

    private bool IsPatternOnCooldown(string patternId, float cooldown)
    {
        if (_patternCooldowns.TryGetValue(patternId, out float lastTime))
        {
            return Time.time < lastTime + cooldown;
        }
        return false;
    }

    private void PlayPatternEffect(string effectKey)
    {
        if (string.IsNullOrEmpty(effectKey)) return;
        // 추후 스킬 이펙트 호출
    }
}
