using UnityEngine;

// 5.00 direction: Runs one concrete gameplay system in the 5.00 closed loop.
// 5.01+ note: Keep the class focused on one responsibility and expose simple events or methods for cross-system links.
public sealed class CinderHeartPlayerRecoveryAura : MonoBehaviour
{
    [SerializeField] private float _healRadius = 7f;
    [SerializeField] private float _healPerSecond = 1.5f;

    private CinderHeart _cinderHeart;
    private PlayerStatus _playerStatus;

    public static void EnsureSceneAura()
    {
        CinderHeart cinderHeart = Object.FindFirstObjectByType<CinderHeart>();
        if (cinderHeart == null)
        {
            return;
        }

        CinderHeartPlayerRecoveryAura aura = cinderHeart.GetComponent<CinderHeartPlayerRecoveryAura>();
        if (aura == null)
        {
            aura = cinderHeart.gameObject.AddComponent<CinderHeartPlayerRecoveryAura>();
        }

        aura.ConnectTargets();
    }

    private void Awake()
    {
        ConnectTargets();
    }

    private void Update()
    {
        ConnectTargets();
        TryHealPlayer();
    }

    private void ConnectTargets()
    {
        if (_cinderHeart == null)
        {
            _cinderHeart = GetComponent<CinderHeart>();
        }

        if (_playerStatus == null)
        {
            _playerStatus = Object.FindFirstObjectByType<PlayerStatus>();
        }
    }

    private void TryHealPlayer()
    {
        if (_cinderHeart == null || _playerStatus == null)
        {
            return;
        }

        if (_cinderHeart.IsDestroyed || _playerStatus.IsDead())
        {
            return;
        }

        float distanceSqr = (_playerStatus.transform.position - _cinderHeart.transform.position).sqrMagnitude;
        if (distanceSqr > _healRadius * _healRadius)
        {
            return;
        }

        _playerStatus.Heal(_healPerSecond * Time.deltaTime);
    }
}
