using Cinderkeep.Gameplay;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public sealed class EnemyMovement : MonoBehaviour
{
    private const string BuildTag = "Build";
    private const float PathUpdateInterval = 0.2f;

    [Tooltip("플레이어 감지 결과를 제공하는 컴포넌트입니다.")]
    [SerializeField] private EnemyDetector _enemyDetector;
    [Tooltip("플레이어를 못 봤을 때 몬스터가 향하는 CinderHeart Transform입니다.")]
    [SerializeField] private Transform _cinderHeartTarget;
    [Tooltip("몬스터 이동 속도입니다. EnemyData로 초기화됩니다.")]
    [SerializeField] private float _moveSpeed;
    [Tooltip("목표와 이 거리 안에 들어오면 이동을 멈춥니다. EnemyData로 초기화됩니다.")]
    [SerializeField] private float _stopDistance;
    [Tooltip("낮에도 CinderHeart를 향해 바로 이동할지 결정합니다. 밤에는 true로 바뀝니다.")]
    [SerializeField] private bool _canChaseCinderHeart = true;
    [Tooltip("낮에 CinderHeart를 향하지 않을 때 스폰 지점 주변을 배회하는 반경입니다.")]
    [SerializeField] private float _wanderRadius = 6f;
    [Tooltip("낮 배회 목표를 다시 정하는 간격입니다.")]
    [SerializeField] private float _wanderInterval = 2.5f;
    [Tooltip("몬스터의 공격 대상 판단을 담당하는 컴포넌트입니다.")]
    [SerializeField] private EnemyBrain _enemyBrain;
    [Tooltip("CinderHeart 경로가 막혔을 때 앞쪽 건축물을 찾는 거리입니다.")]
    [SerializeField] private float _blockingBuildingDetectDistance = 5f;
    [Tooltip("CinderHeart 경로가 막혔을 때 앞쪽 건축물을 찾는 감지 반경입니다.")]
    [SerializeField] private float _blockingBuildingDetectRadius = 1f;

    private NavMeshAgent _navMeshAgent;
    private Coroutine _movementRoutine;
    private Vector3 _spawnPosition;
    private Vector3 _wanderTargetPosition;
    private float _nextWanderTime;
    private bool _isInitialized;
    private BuildingHp _currentBlockingBuildingHp;

    private void OnEnable()
    {
        StartMovementRoutine();
    }

    private void OnDisable()
    {
        StopMovementRoutine();
    }

    public void Initialize(EnemyData enemyData)
    {
        Initialize(enemyData, _enemyDetector);
    }

    public void Initialize(EnemyData enemyData, EnemyDetector enemyDetector)
    {
        if (enemyData == null)
        {
            return;
        }

        ConnectComponents(enemyDetector);
        _moveSpeed = enemyData.MoveSpeed;
        _stopDistance = enemyData.StopDistance;
        _spawnPosition = transform.position;
        _wanderTargetPosition = _spawnPosition;
        ApplyNavMeshAgentSettings();

        _isInitialized = true;
        StartMovementRoutine();
    }

    public void SetCinderHeartTarget(Transform cinderHeartTarget)
    {
        _cinderHeartTarget = cinderHeartTarget;
    }

    public void SetCinderHeartChaseEnabled(bool isEnabled)
    {
        _canChaseCinderHeart = isEnabled;
        if (isEnabled == false)
        {
            StopMoving();
        }
    }

    public void MoveToTarget(Transform targetTransform)
    {
        if (targetTransform == null)
        {
            StopMoving();
            return;
        }

        if (CanStopAtTarget(targetTransform))
        {
            StopMoving();
            return;
        }

        MoveWithNavMeshOrTransform(targetTransform.position);
    }

    private void ConnectComponents(EnemyDetector enemyDetector)
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        if (_enemyBrain == null)
        {
            _enemyBrain = GetComponent<EnemyBrain>();
        }

        if (enemyDetector != null)
        {
            _enemyDetector = enemyDetector;
        }

        if (_enemyDetector == null)
        {
            _enemyDetector = GetComponent<EnemyDetector>();
        }
    }

    private void ApplyNavMeshAgentSettings()
    {
        if (_navMeshAgent == null)
        {
            return;
        }

        _navMeshAgent.speed = _moveSpeed;
        _navMeshAgent.stoppingDistance = _stopDistance;
    }

    private void StartMovementRoutine()
    {
        if (_isInitialized == false)
        {
            return;
        }

        StopMovementRoutine();
        _movementRoutine = StartCoroutine(MoveToPriorityTargetRoutine());
    }

    private void StopMovementRoutine()
    {
        if (_movementRoutine == null)
        {
            return;
        }

        StopCoroutine(_movementRoutine);
        _movementRoutine = null;
    }

    private IEnumerator MoveToPriorityTargetRoutine()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(PathUpdateInterval);

        while (true)
        {
            MoveToPriorityTarget();
            yield return waitInterval;
        }
    }

    private void MoveToPriorityTarget()
    {
        Transform targetTransform = GetPriorityTarget();
        if (targetTransform != null)
        {
            MoveToTarget(targetTransform);
            UpdateBlockingBuildingTargetIfNeeded(targetTransform);
            return;
        }

        ClearCurrentBlockingBuildingTarget();
        WanderAroundSpawnPoint();
    }

    private Transform GetPriorityTarget()
    {
        if (_enemyDetector != null && _enemyDetector.HasDetectedPlayer)
        {
            return _enemyDetector.DetectedPlayer;
        }

        if (_canChaseCinderHeart)
        {
            return _cinderHeartTarget;
        }

        return null;
    }

    private void WanderAroundSpawnPoint()
    {
        if (Time.time >= _nextWanderTime)
        {
            RefreshWanderTargetPosition();
        }

        if (Vector3.Distance(transform.position, _wanderTargetPosition) <= _stopDistance)
        {
            StopMoving();
            return;
        }

        MoveWithNavMeshOrTransform(_wanderTargetPosition);
    }

    private void RefreshWanderTargetPosition()
    {
        _nextWanderTime = Time.time + _wanderInterval;
        Vector2 randomCircle = Random.insideUnitCircle * _wanderRadius;
        _wanderTargetPosition = _spawnPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
        _wanderTargetPosition.y = transform.position.y;
    }

    private bool CanStopAtTarget(Transform targetTransform)
    {
        float distance = Vector3.Distance(transform.position, targetTransform.position);
        return distance <= _stopDistance;
    }

    private void UpdateBlockingBuildingTargetIfNeeded(Transform targetTransform)
    {
        if (IsCinderHeartTarget(targetTransform) == false)
        {
            ClearCurrentBlockingBuildingTarget();
            return;
        }

        UpdateBlockingBuildingTarget(targetTransform);
    }

    private bool IsCinderHeartTarget(Transform targetTransform)
    {
        return _cinderHeartTarget != null && targetTransform == _cinderHeartTarget;
    }

    private void UpdateBlockingBuildingTarget(Transform targetTransform)
    {
        if (_enemyBrain == null)
        {
            return;
        }

        if (IsNavMeshPathBlocked() == false)
        {
            ClearCurrentBlockingBuildingTarget();
            return;
        }

        BuildingHp blockingBuildingHp = FindBlockingBuilding(targetTransform.position);
        if (blockingBuildingHp == null)
        {
            ClearCurrentBlockingBuildingTarget();
            return;
        }

        _currentBlockingBuildingHp = blockingBuildingHp;
        _enemyBrain.SetBuildingAttackTarget(blockingBuildingHp);
        StopMoving();
    }

    private bool IsNavMeshPathBlocked()
    {
        if (_navMeshAgent == null)
        {
            return false;
        }

        if (_navMeshAgent.isOnNavMesh == false)
        {
            return false;
        }

        if (_navMeshAgent.pathPending)
        {
            return false;
        }

        return _navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial || _navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid;
    }

    // 몬스터 앞쪽에서 Build 태그를 가진 건축물을 찾습니다.
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

    private void ClearCurrentBlockingBuildingTarget()
    {
        if (_currentBlockingBuildingHp == null)
        {
            return;
        }

        if (_enemyBrain != null)
        {
            _enemyBrain.ClearBuildingAttackTarget(_currentBlockingBuildingHp);
        }

        _currentBlockingBuildingHp = null;
    }

    private void MoveWithNavMeshOrTransform(Vector3 targetPosition)
    {
        if (_navMeshAgent != null && _navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.isStopped = false;
            if (_navMeshAgent.SetDestination(targetPosition))
            {
                RotateToTarget(targetPosition);
                return;
            }
        }

        MoveDirectlyToTarget(targetPosition);
    }

    private void MoveDirectlyToTarget(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
        transform.position = nextPosition;
        RotateToTarget(targetPosition);
    }

    private void RotateToTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }

    private void StopMoving()
    {
        if (_navMeshAgent != null && _navMeshAgent.isOnNavMesh && _navMeshAgent.hasPath)
        {
            _navMeshAgent.ResetPath();
        }
    }
}
