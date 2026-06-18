using UnityEngine;
using UnityEngine.UI;

// 몬스터 머리 위 체력 UI를 표시하는 컴포넌트입니다.
// 체력 계산은 EnemyStatus가 담당하고, 이 스크립트는 화면 표시만 담당합니다.
public sealed class EnemyHud : MonoBehaviour
{
    [SerializeField] private GameObject GameObject_HudRoot;
    [SerializeField] private Image Image_HpFill;
    [SerializeField] private Text Text_Hp;
    [SerializeField] private Camera Camera_Target;

    private void Awake()
    {
        InitializeHudRoot();
    }

    private void LateUpdate()
    {
        RotateToCamera();
    }

    public void SetTargetCamera(Camera targetCamera)
    {
        Camera_Target = targetCamera;
    }

    public void RefreshHealth(float currentHealth, float maxHealth)
    {
        RefreshHpBar(currentHealth, maxHealth);
        RefreshHpText(currentHealth, maxHealth);
    }

    private void InitializeHudRoot()
    {
        if (GameObject_HudRoot == null)
        {
            GameObject_HudRoot = gameObject;
        }
    }

    private void RefreshHpBar(float currentHealth, float maxHealth)
    {
        if (Image_HpFill == null)
        {
            return;
        }

        if (maxHealth <= 0f)
        {
            Image_HpFill.fillAmount = 0f;
            return;
        }

        Image_HpFill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }

    private void RefreshHpText(float currentHealth, float maxHealth)
    {
        if (Text_Hp == null)
        {
            return;
        }

        int currentHealthText = Mathf.RoundToInt(currentHealth);
        int maxHealthText = Mathf.RoundToInt(maxHealth);
        Text_Hp.text = currentHealthText + " / " + maxHealthText;
    }

    private void RotateToCamera()
    {
        if (Camera_Target == null)
        {
            return;
        }

        transform.rotation = Camera_Target.transform.rotation;
    }
}
