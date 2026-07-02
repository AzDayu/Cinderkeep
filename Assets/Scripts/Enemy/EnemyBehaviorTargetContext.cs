using UnityEngine;

public sealed class EnemyBehaviorTargetContext : MonoBehaviour
{
    [Header("Runtime Targets")]
    [SerializeField] private Transform _cinderHeartTarget;

    public Transform CinderHeartTarget
    {
        get
        {
            return _cinderHeartTarget;
        }
    }

    public GameObject CinderHeartObject
    {
        get
        {
            return _cinderHeartTarget == null ? null : _cinderHeartTarget.gameObject;
        }
    }

    public Damageable CinderHeartDamageable
    {
        get
        {
            if (_cinderHeartTarget == null)
            {
                return null;
            }

            Damageable damageable = _cinderHeartTarget.GetComponent<Damageable>();
            if (damageable != null)
            {
                return damageable;
            }

            damageable = _cinderHeartTarget.GetComponentInParent<Damageable>();
            if (damageable != null)
            {
                return damageable;
            }

            return _cinderHeartTarget.GetComponentInChildren<Damageable>(true);
        }
    }

    public void SetCinderHeartTarget(Transform cinderHeartTarget)
    {
        _cinderHeartTarget = cinderHeartTarget;
    }
}