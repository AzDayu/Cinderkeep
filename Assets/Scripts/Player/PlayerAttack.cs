using UnityEngine;

// 플레이어의 기본 근접 공격을 담당하는 컴포넌트입니다.
// 도끼/곡괭이 채집은 PlayerToolUse가 담당하고, 이 클래스는 적과 피해 대상 공격만 담당합니다.
public sealed class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("플레이어가 좌클릭 공격으로 주는 기본 피해량입니다.")]
    [SerializeField] private float _attackDamage = 10f;
    [Tooltip("플레이어 공격이 닿는 최대 거리입니다.")]
    [SerializeField] private float _attackDistance = 2.5f;
    [Tooltip("공격 판정의 두께입니다. 값이 클수록 조금 빗나가도 맞습니다.")]
    [SerializeField] private float _attackRadius = 0.35f;
    [Tooltip("공격 후 다음 공격까지 기다리는 시간입니다.")]
    [SerializeField] private float _attackInterval = 0.5f;
    [Tooltip("공격 판정에 사용할 레이어입니다.")]
    [SerializeField] private LayerMask _attackLayerMask = ~0;

    [Header("Connected Objects")]
    [Tooltip("공격 Ray가 시작되는 위치입니다. 비어 있으면 자식 카메라를 찾아 사용합니다.")]
    [SerializeField] private Transform _attackOrigin;
    [Tooltip("좌클릭 공격 시 1인칭 도구 휘두르기 연출을 담당합니다.")]
    [SerializeField] private FirstPersonToolView _firstPersonToolView;

    private float _lastAttackTime;

    private void Start()
    {
        ConnectComponents();
    }

    private void Update()
    {
        ReadAttackInput();
    }

    public void TryAttack()
    {
        if (CanAttack() == false)
        {
            return;
        }

        Collider targetCollider = GetAttackTargetCollider();
        if (targetCollider == null)
        {
            return;
        }

        if (CanDamageTarget(targetCollider) == false)
        {
            return;
        }

        _lastAttackTime = Time.time;
        PlayAttackView();
        ApplyDamageToHitTarget(targetCollider);
    }

    private void ConnectComponents()
    {
        if (_attackOrigin == null)
        {
            Camera camera = GetComponentInChildren<Camera>();
            if (camera != null)
            {
                _attackOrigin = camera.transform;
            }
        }

        if (_firstPersonToolView == null)
        {
            _firstPersonToolView = GetComponentInChildren<FirstPersonToolView>();
        }
    }

    private void ReadAttackInput()
    {
        if (CinderkeepInput.WasLeftMousePressedThisFrame())
        {
            TryAttack();
        }
    }

    private bool CanAttack()
    {
        return Time.time >= _lastAttackTime + _attackInterval;
    }

    private Collider GetAttackTargetCollider()
    {
        if (_attackOrigin == null)
        {
            return null;
        }

        Ray attackRay = new Ray(_attackOrigin.position, _attackOrigin.forward);
        RaycastHit hitInfo;

        if (Physics.SphereCast(attackRay, _attackRadius, out hitInfo, _attackDistance, _attackLayerMask) == false)
        {
            return null;
        }

        return hitInfo.collider;
    }

    private bool CanDamageTarget(Collider targetCollider)
    {
        if (targetCollider == null)
        {
            return false;
        }

        if (targetCollider.GetComponentInParent<ResourceNode>() != null)
        {
            return false;
        }

        if (targetCollider.GetComponentInParent<EnemyStatus>() != null)
        {
            return true;
        }

        return targetCollider.GetComponentInParent<Damageable>() != null;
    }

    private void PlayAttackView()
    {
        if (_firstPersonToolView == null)
        {
            return;
        }

        _firstPersonToolView.PlaySwing();
    }

    private void ApplyDamageToHitTarget(Collider targetCollider)
    {
        if (targetCollider == null)
        {
            return;
        }

        EnemyStatus enemyStatus = targetCollider.GetComponentInParent<EnemyStatus>();
        if (enemyStatus != null)
        {
            enemyStatus.TakeDamage(_attackDamage);
            return;
        }

        Damageable damageable = targetCollider.GetComponentInParent<Damageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_attackDamage);
        }
    }
}