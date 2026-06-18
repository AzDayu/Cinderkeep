using UnityEngine;

// 몬스터가 어떤 대상을 공격할지 판단하는 컴포넌트입니다.
// 이동은 EnemyMovement, 감지는 EnemyDetector, 피해 적용은 EnemyAttack이 담당합니다.
public sealed class EnemyBrain : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string BuildTag = "Build";

    [SerializeField] private EnemyDetector _enemyDetector;
    [SerializeField] private EnemyAttack _enemyAttack;

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

    // 벽이나 구조물 공격이 필요할 때 외부에서 공격 대상을 지정하는 진입점입니다.
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
        if (_enemyDetector == null)
        {
            _enemyDetector = GetComponent<EnemyDetector>();
        }

        if (_enemyAttack == null)
        {
            _enemyAttack = GetComponent<EnemyAttack>();
        }
    }

    private void UpdatePlayerTargetFromDetector()
    {
        if (_enemyDetector == null)
        {
            return;
        }

        if (_enemyDetector.HasDetectedPlayer == false)
        {
            return;
        }

        Damageable detectedPlayerDamageable = GetDamageableFromTransform(_enemyDetector.DetectedPlayer);
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

        if (_enemyAttack == null)
        {
            return;
        }

        _enemyAttack.TryAttack(_currentAttackTarget);
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
