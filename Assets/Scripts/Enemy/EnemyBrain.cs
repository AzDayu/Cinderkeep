using UnityEngine;

// 몬스터가 어떤 대상을 우선 공격할지 판단하는 행동 조정 컴포넌트입니다.
// 이동은 EnemyMovement, 감지는 EnemyDetector, 피해 적용은 EnemyAttack이 담당합니다.
public sealed class EnemyBrain : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string BuildTag = "Build";

    [SerializeField] private EnemyDetector EnemyDetector_EnemyDetector;
    [SerializeField] private EnemyAttack EnemyAttack_EnemyAttack;

    private Damageable _currentAttackTarget;

    private void Awake()
    {
        ConnectComponents();
    }

    private void Update()
    {
        UpdatePlayerTargetFromDetector();
        TryAttackCurrentTarget();
    }

    // 구조물, 플레이어처럼 외부에서 지정한 대상도 공격 대상으로 받을 수 있게 둡니다.
    // 예: CinderHeart로 가는 길이 벽에 막혔을 때 벽의 Damageable을 넘겨서 공격합니다.
    public void SetAttackTarget(Damageable targetDamageable)
    {
        _currentAttackTarget = targetDamageable;
    }

    public void ClearAttackTarget(Damageable targetDamageable)
    {
        if (_currentAttackTarget != targetDamageable)
        {
            return;
        }

        _currentAttackTarget = null;
    }

    private void ConnectComponents()
    {
        if (EnemyDetector_EnemyDetector == null)
        {
            EnemyDetector_EnemyDetector = GetComponent<EnemyDetector>();
        }

        if (EnemyAttack_EnemyAttack == null)
        {
            EnemyAttack_EnemyAttack = GetComponent<EnemyAttack>();
        }
    }

    private void UpdatePlayerTargetFromDetector()
    {
        if (EnemyDetector_EnemyDetector == null)
        {
            return;
        }

        if (EnemyDetector_EnemyDetector.HasDetectedPlayer == false)
        {
            return;
        }

        Damageable detectedPlayerDamageable = GetDamageableFromTransform(EnemyDetector_EnemyDetector.DetectedPlayer);
        if (detectedPlayerDamageable == null)
        {
            return;
        }

        _currentAttackTarget = detectedPlayerDamageable;
    }

    private void TryAttackCurrentTarget()
    {
        if (_currentAttackTarget == null)
        {
            return;
        }

        if (CanAttackTarget(_currentAttackTarget.gameObject) == false)
        {
            return;
        }

        if (EnemyAttack_EnemyAttack == null)
        {
            return;
        }

        EnemyAttack_EnemyAttack.TryAttack(_currentAttackTarget);
    }

    private bool CanAttackTarget(GameObject targetObject)
    {
        if (targetObject == null)
        {
            return false;
        }

        if (targetObject.CompareTag(PlayerTag))
        {
            return true;
        }

        return targetObject.CompareTag(BuildTag);
    }

    private Damageable GetDamageableFromTransform(Transform targetTransform)
    {
        if (targetTransform == null)
        {
            return null;
        }

        Damageable damageable = targetTransform.GetComponent<Damageable>();
        if (damageable != null)
        {
            return damageable;
        }

        return targetTransform.GetComponentInParent<Damageable>();
    }
}
