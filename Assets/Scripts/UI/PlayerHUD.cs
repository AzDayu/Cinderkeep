using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// 플레이어의 체력과 스태미나를 화면에 표시하는 HUD입니다.
// 실제 수치 계산은 PlayerStatus가 담당하고, 이 스크립트는 UI 표시만 담당합니다.
public sealed class PlayerHUD : MonoBehaviour
{
    [Header("연동할 대상 스크립트")]
    [FormerlySerializedAs("PlayerStatus_Target")]
    [SerializeField] private PlayerStatus _targetPlayerStatus;

    [Header("체력 UI")]
    [FormerlySerializedAs("Slider_Health")]
    [SerializeField] private Slider _healthSlider;
    [FormerlySerializedAs("Text_HealthCurrent")]
    [SerializeField] private TMP_Text _healthCurrentText;
    [FormerlySerializedAs("Text_HealthMax")]
    [SerializeField] private TMP_Text _healthMaxText;

    [Header("스태미나 UI")]
    [FormerlySerializedAs("Slider_Stamina")]
    [SerializeField] private Slider _staminaSlider;
    [FormerlySerializedAs("Text_StaminaCurrent")]
    [SerializeField] private TMP_Text _staminaCurrentText;
    [FormerlySerializedAs("Text_StaminaMax")]
    [SerializeField] private TMP_Text _staminaMaxText;

    private void Start()
    {
        InitializeHUD();
    }

    private void Update()
    {
        RefreshHUD();
    }

    public void SetPlayerStatus(PlayerStatus playerStatus)
    {
        _targetPlayerStatus = playerStatus;
        InitializeHUD();
    }

    private void InitializeHUD()
    {
        if (HasPlayerStatus() == false)
        {
            return;
        }

        InitializeSlider(_healthSlider, _targetPlayerStatus.GetMaxHealth(), _targetPlayerStatus.GetCurrentHealth());
        InitializeSlider(_staminaSlider, _targetPlayerStatus.GetMaxStamina(), _targetPlayerStatus.GetCurrentStamina());
        RefreshHUD();
    }

    private void RefreshHUD()
    {
        if (HasPlayerStatus() == false)
        {
            return;
        }

        RefreshSlider(_healthSlider, _targetPlayerStatus.GetCurrentHealth());
        RefreshSlider(_staminaSlider, _targetPlayerStatus.GetCurrentStamina());

        RefreshText(_healthCurrentText, _targetPlayerStatus.GetCurrentHealth());
        RefreshText(_healthMaxText, _targetPlayerStatus.GetMaxHealth());
        RefreshText(_staminaCurrentText, _targetPlayerStatus.GetCurrentStamina());
        RefreshText(_staminaMaxText, _targetPlayerStatus.GetMaxStamina());
    }

    private void InitializeSlider(Slider slider, float maxValue, float currentValue)
    {
        if (slider == null)
        {
            return;
        }

        slider.minValue = 0f;
        slider.maxValue = maxValue;
        slider.value = currentValue;
    }

    private void RefreshSlider(Slider slider, float currentValue)
    {
        if (slider == null)
        {
            return;
        }

        slider.value = currentValue;
    }

    private void RefreshText(TMP_Text text, float value)
    {
        if (text == null)
        {
            return;
        }

        text.text = value.ToString("F0");
    }

    private bool HasPlayerStatus()
    {
        return _targetPlayerStatus != null;
    }
}
