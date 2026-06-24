using System.Collections;
using Cinderkeep.Gameplay;
using UnityEngine;

// 5.00 direction: Supports base construction, defense objects, and building damage in the 5.00 loop.
// 5.01+ note: Keep placement, cost, health, tower attack, and upgrade rules split so 5.01+ defenses can expand.
// 타워의 자동 공격 흐름을 조율하는 컴포넌트입니다.
// 적 탐지는 TowerTargeting, 피해 적용은 TowerAttack이 담당합니다.
public sealed class BuildingTower : MonoBehaviour
{
    private const float AttackLoopDelay = 0.2f;

    [Header("Connected Components")]
    [SerializeField] private TowerTargeting _towerTargeting;
    [SerializeField] private TowerAttack _towerAttack;
    [Header("Optional View")]
    [Tooltip("선택 사항입니다. 연결하면 공격 대상 방향으로 회전합니다.")]
    [SerializeField] private Transform _turretHead;

    private Coroutine _attackLoopRoutine;

    private void Awake()
    {
        ConnectTowerComponents();
    }

    private void OnEnable()
    {
        StartAttackLoopRoutine();
    }

    private void OnDisable()
    {
        StopAttackLoopRoutine();
    }

    public void Initialize(BuildingData buildingData)
    {
        if (buildingData == null)
        {
            Debug.LogWarning(gameObject.name + " BuildingData가 없어 타워 수치를 초기화하지 못했습니다.");
            return;
        }

        ConnectTowerComponents();

        if (_towerTargeting != null)
        {
            _towerTargeting.SetAttackRange(buildingData.AttackRange);
        }

        if (_towerAttack != null)
        {
            _towerAttack.SetAttackDamage(buildingData.AttackDamage);
            _towerAttack.SetAttackInterval(buildingData.AttackInterval);
        }
    }

    private void ConnectTowerComponents()
    {
        if (_towerTargeting == null)
        {
            _towerTargeting = GetComponent<TowerTargeting>();
        }

        if (_towerAttack == null)
        {
            _towerAttack = GetComponent<TowerAttack>();
        }
    }

    private void StartAttackLoopRoutine()
    {
        StopAttackLoopRoutine();
        _attackLoopRoutine = StartCoroutine(AttackLoopRoutine());
    }

    private void StopAttackLoopRoutine()
    {
        if (_attackLoopRoutine == null)
        {
            return;
        }

        StopCoroutine(_attackLoopRoutine);
        _attackLoopRoutine = null;
    }

    private IEnumerator AttackLoopRoutine()
    {
        WaitForSeconds waitDelay = new WaitForSeconds(AttackLoopDelay);

        while (true)
        {
            ProcessAttack();
            yield return waitDelay;
        }
    }

    private void ProcessAttack()
    {
        if (_towerTargeting == null || _towerAttack == null)
        {
            return;
        }

        if (_towerTargeting.HasTarget == false)
        {
            return;
        }

        EnemyStatus currentTarget = _towerTargeting.CurrentTarget;
        if (currentTarget == null)
        {
            return;
        }

        RotateTurretHeadTowardTarget(currentTarget.transform.position);
        _towerAttack.TryAttack(currentTarget);
    }

    private void RotateTurretHeadTowardTarget(Vector3 targetPosition)
    {
        if (_turretHead == null)
        {
            return;
        }

        Vector3 direction = targetPosition - _turretHead.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        _turretHead.rotation = lookRotation;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        ConnectTowerComponents();
    }
#endif
}
