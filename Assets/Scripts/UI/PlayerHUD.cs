using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 플레이어의 체력과 스태미나를 화면에 표시하는 HUD입니다.
// 실제 수치 계산은 PlayerStatus가 담당하고, 이 스크립트는 UI 표시만 담당합니다.
public sealed class PlayerHUD : MonoBehaviour
{
    [Header("연동할 대상 스크립트")]
    [SerializeField] private PlayerStatus PlayerStatus_Target;

    [Header("체력 UI")]
    [SerializeField] private Slider Slider_Health;
    [SerializeField] private TMP_Text Text_HealthCurrent;
    [SerializeField] private TMP_Text Text_HealthMax;

    [Header("스태미나 UI")]
    [SerializeField] private Slider Slider_Stamina;
    [SerializeField] private TMP_Text Text_StaminaCurrent;
    [SerializeField] private TMP_Text Text_StaminaMax;

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
        PlayerStatus_Target = playerStatus;
        InitializeHUD();
    }

    private void InitializeHUD()
    {
        if (HasPlayerStatus() == false)
        {
            return;
        }

        InitializeSlider(Slider_Health, PlayerStatus_Target.GetMaxHealth(), PlayerStatus_Target.GetCurrentHealth());
        InitializeSlider(Slider_Stamina, PlayerStatus_Target.GetMaxStamina(), PlayerStatus_Target.GetCurrentStamina());
        RefreshHUD();
    }

    private void RefreshHUD()
    {
        if (HasPlayerStatus() == false)
        {
            return;
        }

        RefreshSlider(Slider_Health, PlayerStatus_Target.GetCurrentHealth());
        RefreshSlider(Slider_Stamina, PlayerStatus_Target.GetCurrentStamina());

        RefreshText(Text_HealthCurrent, PlayerStatus_Target.GetCurrentHealth());
        RefreshText(Text_HealthMax, PlayerStatus_Target.GetMaxHealth());
        RefreshText(Text_StaminaCurrent, PlayerStatus_Target.GetCurrentStamina());
        RefreshText(Text_StaminaMax, PlayerStatus_Target.GetMaxStamina());
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
        return PlayerStatus_Target != null;
    }
}
