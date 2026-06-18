using Cinderkeep.Gameplay;
using UnityEngine;

public sealed class EnemyStatus : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 1f;
    [SerializeField] private EnemyHud EnemyHud_EnemyHud;

    private EnemyDetector _enemyDetector;
    private float _currentHealth;

    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
    }

    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
    }

    public bool IsDead
    {
        get
        {
            return _currentHealth <= 0f;
        }
    }

    private void Awake()
    {
        ConnectComponents();
        InitializeHealth(_maxHealth);
    }

    public void Initialize(EnemyData enemyData)
    {
        if (enemyData == null)
        {
            return;
        }

        InitializeHealth(enemyData.Health);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
        {
            return;
        }

        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        RefreshHud();

        if (_enemyDetector != null)
        {
            _enemyDetector.EnableAlertMode();
        }
    }

    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

    public float GetMaxHealth()
    {
        return _maxHealth;
    }

    private void ConnectComponents()
    {
        _enemyDetector = GetComponent<EnemyDetector>();

        if (EnemyHud_EnemyHud == null)
        {
            EnemyHud_EnemyHud = GetComponentInChildren<EnemyHud>();
        }
    }

    private void InitializeHealth(float maxHealth)
    {
        _maxHealth = Mathf.Max(1f, maxHealth);
        _currentHealth = _maxHealth;
        RefreshHud();
    }

    private void RefreshHud()
    {
        if (EnemyHud_EnemyHud == null)
        {
            return;
        }

        EnemyHud_EnemyHud.RefreshHealth(_currentHealth, _maxHealth);
    }
}
