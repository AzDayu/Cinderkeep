using UnityEngine;

// 민석님 Enemy HP 작업을 기존 EnemyStatus 구조와 충돌하지 않게 연결하는 보조 컴포넌트입니다.
// 실제 체력 원본은 EnemyStatus가 담당하고, 이 스크립트는 테스트 호출과 HUD 갱신을 돕습니다.
public sealed class EnemyHp : MonoBehaviour
{
    [SerializeField] private EnemyStatus EnemyStatus_Target;
    [SerializeField] private EnemyHud EnemyHud_EnemyHud;

    private void Awake()
    {
        ConnectComponents();
        RefreshHud();
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            Debug.LogWarning("EnemyHp: 데미지는 1 이상이어야 합니다.");
            return;
        }

        if (EnemyStatus_Target == null)
        {
            Debug.LogWarning("EnemyHp: EnemyStatus가 연결되지 않았습니다.");
            return;
        }

        EnemyStatus_Target.TakeDamage(damage);
        RefreshHud();
    }

    public int GetCurHp()
    {
        if (EnemyStatus_Target == null)
        {
            return 0;
        }

        return Mathf.RoundToInt(EnemyStatus_Target.GetCurrentHealth());
    }

    public int GetMaxHp()
    {
        if (EnemyStatus_Target == null)
        {
            return 0;
        }

        return Mathf.RoundToInt(EnemyStatus_Target.GetMaxHealth());
    }

    public void PrintHp()
    {
        Debug.Log(gameObject.name + " 현재 체력 : " + GetCurHp() + " / 최대 체력 : " + GetMaxHp());
    }

    private void ConnectComponents()
    {
        if (EnemyStatus_Target == null)
        {
            EnemyStatus_Target = GetComponent<EnemyStatus>();
        }

        if (EnemyHud_EnemyHud == null)
        {
            EnemyHud_EnemyHud = GetComponentInChildren<EnemyHud>();
        }
    }

    private void RefreshHud()
    {
        if (EnemyHud_EnemyHud == null)
        {
            return;
        }

        EnemyHud_EnemyHud.RefreshHealth(GetCurHp(), GetMaxHp());
    }
}
