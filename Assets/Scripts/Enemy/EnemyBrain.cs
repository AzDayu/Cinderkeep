using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// 몬스터가 어떤 대상을 공격할지 판단하는 컴포넌트입니다.
// 이동은 EnemyMovement, 감지는 EnemyDetector, 피해 적용은 EnemyAttack이 담당합니다.
public sealed class EnemyBrain : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string BuildTag = "Build";
    private const string CinderHeartTag = "CinderHeart";
    private const float DecisionInterval = 0.2f;

    [Tooltip("플레이어 감지 결과를 가져오는 컴포넌트입니다.")]
    [SerializeField] private EnemyDetector _enemyDetector;
    [Tooltip("실제 피해 적용과 공격 쿨타임을 담당하는 컴포넌트입니다.")]
    [SerializeField] private EnemyAttack _enemyAttack;

    [Tooltip("실제 이동을 담당하는 컴포넌트입니다.")]
    [SerializeField] private EnemyMovement _enemyMovement;

    [Tooltip("플레이어를 감지하지 못했을 때 공격할 CinderHeart 피해 대상입니다.")]
    [SerializeField] private Damageable _cinderHeartDamageable;
    [Tooltip("플레이어와 구조물을 공격할 수 있는 거리입니다.")]
    [SerializeField] private float _attackDistance = 2.3f;
    [Tooltip("CinderHeart를 공격할 수 있는 거리입니다.")]
    [SerializeField] private float _cinderHeartAttackDistance = 3f;

    [Tooltip("CinderHeart 경로가 막혔을 때 앞쪽 건축물을 찾는 거리입니다.")]
    [SerializeField] private float _blockingBuildingDetectDistance = 5f;
    [Tooltip("CinderHeart 경로가 막혔을 때 앞쪽 건축물을 찾는 감지 반경입니다.")]
    [SerializeField] private float _blockingBuildingDetectRadius = 1f;

    private Coroutine _brainDecisionRoutine;
    private Damageable _currentAttackTarget;
    private BuildingHp _currentBuildingAttackTarget;

    private void Awake()
    {
        ConnectComponents();
    }

    private void OnEnable()
    {
        StartBrainRoutine();
    }

    private void OnDisable()
    {
        StopBrainRoutine();
    }

    public void SetCinderHeartTarget(Damageable cinderHeartDamageable)
    {
        _cinderHeartDamageable = cinderHeartDamageable;
    }

    // 플레이어, CinderHeart처럼 Damageable을 기준으로 공격할 대상을 지정합니다.
    public void SetAttackTarget(Damageable targetDamageable)
    {
        _currentAttackTarget = targetDamageable;
        _currentBuildingAttackTarget = null;
    }

    public void ClearAttackTarget(Damageable targetDamageable)
    {
        if (_currentAttackTarget != targetDamageable)
        {
            return;
        }

        _currentAttackTarget = null;
    }

    // 건축물이 이동 경로를 막고 있을 때 건축물 공격 대상으로 지정합니다.
    private void SetBuildingAttackTarget(BuildingHp buildingHp)
    {
        if (buildingHp == null)
        {
            return;
        }

        if (buildingHp.IsDestroyed)
        {
            return;
        }

        _currentBuildingAttackTarget = buildingHp;

        if (_currentAttackTarget == _cinderHeartDamageable)
        {
            _currentAttackTarget = null;
        }
    }

    public void ClearBuildingAttackTarget(BuildingHp buildingHp)
    {
        if (_currentBuildingAttackTarget != buildingHp)
        {
            return;
        }

        _currentBuildingAttackTarget = null;
    }

    private void ClearCurrentBuildingAttackTarget()
    {
        _currentBuildingAttackTarget = null;
    }

    private void MoveToCurrentBuildingAttackTarget()
    {
        if (_enemyMovement == null)
        {
            return;
        }

        if (_currentBuildingAttackTarget == null)
        {
            return;
        }
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

        if (_enemyMovement == null)
        {
            _enemyMovement = GetComponent<EnemyMovement>();
        }

    }

    private void StartBrainRoutine()
    {
        StopBrainRoutine();
        _brainDecisionRoutine = StartCoroutine(BrainDecisionRoutine());
    }

    private void StopBrainRoutine()
    {
        if (_brainDecisionRoutine == null)
        {
            return;
        }

        StopCoroutine(_brainDecisionRoutine);
        _brainDecisionRoutine = null;
    }

    private IEnumerator BrainDecisionRoutine()
    {
        // 적 판단은 매 프레임이 아니라 짧은 주기로 갱신해 과한 연산을 피합니다.
        // 타깃 우선순위는 플레이어 감지 -> 막고 있는 건축물 -> CinderHeart 순서로 유지합니다.
        WaitForSeconds waitInterval = new WaitForSeconds(DecisionInterval);

        while (true)
        {
            UpdatePlayerTargetFromDetector();
            UpdateBlockingBuildingTargetIfNeeded();
            UpdateCinderHeartTargetIfNeeded();
            TryAttackCurrentTarget();

            yield return waitInterval;
        }
    }

    // CinderHeart로 가는 길이 막혔으면 앞쪽 건축물을 공격 대상으로 잡습니다.
    private void UpdateBlockingBuildingTargetIfNeeded()
    {
        if (_enemyDetector != null && _enemyDetector.HasDetectedPlayer)
        {
            ClearCurrentBuildingAttackTarget();
            return;
        }

        if (_cinderHeartDamageable == null)
        {
            ClearCurrentBuildingAttackTarget();
            return;
        }

        if (IsCinderHeartPathBlocked() == false)
        {
            ClearCurrentBuildingAttackTarget();
            return;
        }

        if (_currentBuildingAttackTarget != null && _currentBuildingAttackTarget.IsDestroyed == false)
        {
            MoveToCurrentBuildingAttackTarget();
            return;
        }

        BuildingHp blockingBuildingHp = FindBlockingBuilding(_cinderHeartDamageable.transform.position);
        if (blockingBuildingHp == null)
        {
            ClearCurrentBuildingAttackTarget();
            return;
        }

        SetBuildingAttackTarget(blockingBuildingHp);
        MoveToCurrentBuildingAttackTarget();
    }

    private void UpdatePlayerTargetFromDetector()
    {
        if (_enemyDetector == null)
        {
            return;
        }

        if (_enemyDetector.HasDetectedPlayer == false)
        {
            ClearPlayerAttackTarget();
            return;
        }

        Damageable detectedPlayerDamageable = GetDamageableFromTransform(_enemyDetector.DetectedPlayer);
        if (detectedPlayerDamageable == null)
        {
            return;
        }

        _currentBuildingAttackTarget = null;
        _currentAttackTarget = detectedPlayerDamageable;
    }

    private void TryAttackCurrentTarget()
    {
        if (_currentBuildingAttackTarget != null)
        {
            TryAttackCurrentBuildingTarget();
            return;
        }

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

    private void TryAttackCurrentBuildingTarget()
    {
        if (_currentBuildingAttackTarget == null)
        {
            return;
        }

        if (_currentBuildingAttackTarget.IsDestroyed)
        {
            _currentBuildingAttackTarget = null;
            return;
        }

        if (CanAttackTarget(_currentBuildingAttackTarget.gameObject) == false)
        {
            return;
        }

        if (_enemyAttack == null)
        {
            return;
        }

        _enemyAttack.TryAttack(_currentBuildingAttackTarget);
    }

    private void UpdateCinderHeartTargetIfNeeded()
    {
        if (_enemyDetector != null && _enemyDetector.HasDetectedPlayer)
        {
            return;
        }

        if (_currentBuildingAttackTarget != null)
        {
            return;
        }

        if (_currentAttackTarget != null && _currentAttackTarget != _cinderHeartDamageable)
        {
            return;
        }

        if (_cinderHeartDamageable == null)
        {
            return;
        }

        _currentAttackTarget = _cinderHeartDamageable;
    }

    private bool CanAttackTarget(GameObject targetObject)
    {
        if (targetObject == null)
        {
            return false;
        }

        if (targetObject.CompareTag(PlayerTag))
        {
            return IsInAttackDistance(targetObject.transform, _attackDistance);
        }

        if (targetObject.CompareTag(BuildTag))
        {
            return IsInAttackDistance(targetObject.transform, _attackDistance);
        }

        if (targetObject.CompareTag(CinderHeartTag))
        {
            return IsInAttackDistance(targetObject.transform, _cinderHeartAttackDistance);
        }

        return false;
    }

    private bool IsInAttackDistance(Transform targetTransform, float attackDistance)
    {
        float distance = Vector3.Distance(transform.position, targetTransform.position);
        return distance <= attackDistance;
    }

    // CinderHeart까지의 NavMesh 경로가 막혔는지 확인합니다.
    private bool IsCinderHeartPathBlocked()
    {
        if (_cinderHeartDamageable == null)
        {
            return false;
        }

        NavMeshPath path = new NavMeshPath();

        bool hasPath = NavMesh.CalculatePath(
            transform.position,
            _cinderHeartDamageable.transform.position,
            NavMesh.AllAreas,
            path);

        if (hasPath == false)
        {
            return true;
        }

        return path.status == NavMeshPathStatus.PathPartial ||
               path.status == NavMeshPathStatus.PathInvalid;
    }

    private BuildingHp FindBlockingBuilding(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return null;
        }

        Vector3 origin = transform.position + Vector3.up * 0.8f;

        RaycastHit[] hits = Physics.SphereCastAll(
            origin,
            _blockingBuildingDetectRadius,
            direction.normalized,
            _blockingBuildingDetectDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hits.Length; i++)
        {
            BuildingHp buildingHp = GetBuildingHpFromCollider(hits[i].collider);
            if (buildingHp != null)
            {
                return buildingHp;
            }
        }

        return null;
    }

    private BuildingHp GetBuildingHpFromCollider(Collider hitCollider)
    {
        if (hitCollider == null)
        {
            return null;
        }

        BuildingHp buildingHp = hitCollider.GetComponentInParent<BuildingHp>();
        if (buildingHp == null)
        {
            return null;
        }

        if (buildingHp.IsDestroyed)
        {
            return null;
        }

        if (hitCollider.CompareTag(BuildTag))
        {
            return buildingHp;
        }

        if (buildingHp.CompareTag(BuildTag))
        {
            return buildingHp;
        }

        return null;
    }

    private void ClearPlayerAttackTarget()
    {
        if (_currentAttackTarget == null)
        {
            return;
        }

        if (_currentAttackTarget.CompareTag(PlayerTag))
        {
            _currentAttackTarget = null;
        }
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
