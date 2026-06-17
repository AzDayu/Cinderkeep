using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public sealed class PlayerHUD : MonoBehaviour
{
    [Header("연동할 대상 스크립트")]
    [SerializeField] private PlayerStatus _playerStatus;

    [Header("HUD 슬라이더 UI 컴포넌트")]
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private TMP_Text _hpSliderCurrent;
    [SerializeField] private TMP_Text _hpSliderMax;

    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private TMP_Text _staminaSliderCurrent;
    [SerializeField] private TMP_Text _staminaSliderMax;




    private void Start()
    {
        if (_playerStatus == null)
        {
            _playerStatus = FindFirstObjectByType<PlayerStatus>();
        }

        InitializeHUD();
    }

    private void Update()
    {
        UpdateHUD();
    }

    private void InitializeHUD()
    {
        if (_playerStatus == null)
        {
            return;
        }

        if (_hpSlider != null)
        {
            _hpSlider.minValue = 0f;
            _hpSlider.maxValue = _playerStatus.GetMaxHealth();
            _hpSlider.value = _playerStatus.GetCurrentHealth();
        }

        if (_staminaSlider != null)
        {
            _staminaSlider.minValue = 0f;
            _staminaSlider.maxValue = _playerStatus.GetMaxStamina();
            _staminaSlider.value = _playerStatus.GetCurrentStamina();
        }
    }

    private void UpdateHUD()
    {
        if (_playerStatus == null)
        {
            return;
        }

        if (_hpSlider != null)
        {
            _hpSlider.value = _playerStatus.GetCurrentHealth();
        }

        if (_staminaSlider != null)
        {
            _staminaSlider.value = _playerStatus.GetCurrentStamina();
        }

        if (_hpSliderCurrent != null)
        {
            string currentHp = _playerStatus.GetCurrentHealth().ToString("F0");
            _hpSliderCurrent.text = currentHp;
        }
        if (_hpSliderMax != null)
        {
            string MaxHp = _playerStatus.GetMaxHealth().ToString("F0");
            _hpSliderMax.text = MaxHp;
        }

        if (_staminaSliderCurrent != null)
        {
            string currentSt = _playerStatus.GetCurrentStamina().ToString("F0");
            _staminaSliderCurrent.text = currentSt;
        }

        if (_staminaSliderMax != null)
        {
            string MaxSt = _playerStatus.GetMaxStamina().ToString("F0");
            _staminaSliderMax.text = MaxSt;
        }

    }
}