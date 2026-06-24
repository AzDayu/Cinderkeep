using System;
using Cinderkeep.Gameplay;
using UnityEngine;

// 5.00 direction: Runs one concrete gameplay system in the 5.00 closed loop.
// 5.01+ note: Keep the class focused on one responsibility and expose simple events or methods for cross-system links.
public sealed class CinderHeartSkillApplier : MonoBehaviour
{
    private const string EffectTypeAttackDamageAdd = "CinderHeartAttackDamageAdd";
    private const string EffectTypeMaxHealthAdd = "CinderHeartMaxHealthAdd";
    private const string EffectTypeCinderHeartHealFlat = "CinderHeartHealFlat";
    private const string EffectTypeCinderHeartHealRate = "CinderHeartHealRate";
    private const string EffectTypePlayerHealRate = "PlayerHealRate";
    private const string EffectTypePlayerReviveRate = "PlayerReviveRate";
    private const string EffectTypePlayerMaxHealthAdd = "PlayerMaxHealthAdd";
    private const string EffectTypePlayerMaxStaminaAdd = "PlayerMaxStaminaAdd";
    private const string EffectTypePlayerStaminaRecoveryAdd = "PlayerStaminaRecoveryAdd";
    private const string EffectTypePlayerMaxSatietyAdd = "PlayerMaxSatietyAdd";
    private const string EffectTypePlayerAttackDamageAdd = "PlayerAttackDamageAdd";

    [SerializeField] private CinderHeart _cinderHeart;
    [SerializeField] private PlayerStatus _playerStatus;
    [SerializeField] private PlayerAttack _playerAttack;

    public void SetTargets(CinderHeart cinderHeart, PlayerStatus playerStatus)
    {
        _cinderHeart = cinderHeart;
        _playerStatus = playerStatus;
    }

    public void ApplySkill(CinderHeartSkillData skillData)
    {
        if (skillData == null)
        {
            return;
        }

        ConnectTargetsIfNeeded();

        if (IsEffectType(skillData, EffectTypeAttackDamageAdd))
        {
            ApplyCinderHeartAttackDamageAdd(skillData.Value);
            return;
        }

        if (IsEffectType(skillData, EffectTypeMaxHealthAdd))
        {
            ApplyCinderHeartMaxHealthAdd(skillData.Value);
            return;
        }

        if (IsEffectType(skillData, EffectTypeCinderHeartHealFlat))
        {
            ApplyCinderHeartHealFlat(skillData.Value);
            return;
        }

        if (IsEffectType(skillData, EffectTypeCinderHeartHealRate))
        {
            ApplyCinderHeartHealRate(skillData.Value);
            return;
        }

        if (IsEffectType(skillData, EffectTypePlayerHealRate))
        {
            ApplyPlayerHealRate(skillData.Value);
            return;
        }

        if (IsEffectType(skillData, EffectTypePlayerReviveRate))
        {
            ApplyPlayerReviveRate(skillData.Value);
            return;
        }

        if (IsEffectType(skillData, EffectTypePlayerMaxHealthAdd))
        {
            ApplyPlayerMaxHealthAdd(skillData.Value);
            return;
        }

        if (IsEffectType(skillData, EffectTypePlayerMaxStaminaAdd))
        {
            ApplyPlayerMaxStaminaAdd(skillData.Value);
            return;
        }

        if (IsEffectType(skillData, EffectTypePlayerStaminaRecoveryAdd))
        {
            ApplyPlayerStaminaRecoveryAdd(skillData.Value);
            return;
        }

        if (IsEffectType(skillData, EffectTypePlayerMaxSatietyAdd))
        {
            ApplyPlayerMaxSatietyAdd(skillData.Value);
            return;
        }

        if (IsEffectType(skillData, EffectTypePlayerAttackDamageAdd))
        {
            ApplyPlayerAttackDamageAdd(skillData.Value);
            return;
        }

        Debug.LogWarning("[CinderHeartSkillApplier] Unsupported reward effect: " + skillData.EffectType);
    }

    private bool IsEffectType(CinderHeartSkillData skillData, string effectType)
    {
        return string.Equals(skillData.EffectType, effectType, StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyCinderHeartAttackDamageAdd(float amount)
    {
        if (_cinderHeart == null)
        {
            return;
        }

        _cinderHeart.AddAttackDamage(amount);
    }

    private void ApplyCinderHeartMaxHealthAdd(float amount)
    {
        if (_cinderHeart == null)
        {
            return;
        }

        _cinderHeart.AddMaxHealth(amount);
    }

    private void ApplyCinderHeartHealFlat(float amount)
    {
        if (_cinderHeart == null)
        {
            return;
        }

        _cinderHeart.Heal(amount);
        PlayHealSfx();
    }

    private void ApplyCinderHeartHealRate(float rate)
    {
        if (_cinderHeart == null || rate <= 0f)
        {
            return;
        }

        _cinderHeart.HealByRate(rate);
        PlayHealSfx();
    }

    private void ApplyPlayerHealRate(float rate)
    {
        if (_playerStatus == null || rate <= 0f)
        {
            return;
        }

        float healAmount = _playerStatus.GetMaxHealth() * rate;
        _playerStatus.Heal(healAmount);
        PlayHealSfx();
    }

    private void ApplyPlayerReviveRate(float rate)
    {
        if (_playerStatus == null)
        {
            return;
        }

        _playerStatus.Revive(rate);
        PlayHealSfx();
    }

    private void ApplyPlayerMaxHealthAdd(float amount)
    {
        if (_playerStatus == null)
        {
            return;
        }

        _playerStatus.AddMaxHealth(amount);
        PlayHealSfx();
    }

    private void ApplyPlayerMaxStaminaAdd(float amount)
    {
        if (_playerStatus == null)
        {
            return;
        }

        _playerStatus.AddMaxStamina(amount);
    }

    private void ApplyPlayerStaminaRecoveryAdd(float amount)
    {
        if (_playerStatus == null)
        {
            return;
        }

        _playerStatus.AddStaminaRecoveryRate(amount);
    }

    private void ApplyPlayerMaxSatietyAdd(float amount)
    {
        if (_playerStatus == null)
        {
            return;
        }

        _playerStatus.AddMaxSatiety(amount);
    }

    private void ApplyPlayerAttackDamageAdd(float amount)
    {
        if (_playerAttack == null)
        {
            return;
        }

        _playerAttack.AddBonusAttackDamage(amount);
    }

    private void ConnectTargetsIfNeeded()
    {
        if (_cinderHeart == null)
        {
            _cinderHeart = UnityEngine.Object.FindFirstObjectByType<CinderHeart>();
        }

        if (_playerStatus == null)
        {
            _playerStatus = UnityEngine.Object.FindFirstObjectByType<PlayerStatus>();
        }

        if (_playerAttack == null)
        {
            _playerAttack = UnityEngine.Object.FindFirstObjectByType<PlayerAttack>();
        }
    }

    private void PlayHealSfx()
    {
        if (GameManager.Inst == null || GameManager.Inst.GetSoundManager() == null)
        {
            return;
        }

        GameManager.Inst.GetSoundManager().PlayHeal();
    }
}
