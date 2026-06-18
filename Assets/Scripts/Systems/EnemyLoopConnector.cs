using Cinderkeep.Gameplay;
using System;
using UnityEngine;
using UnityEngine.AI;

// 적 관련 런타임 연결만 담당하는 컴포넌트입니다.
// 스폰된 오브젝트가 실제 적처럼 움직이고 공격하도록 필요한 컴포넌트를 연결합니다.
public sealed class EnemyLoopConnector : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameDataManager _gameDataManager;

    [Header("World")]
    [SerializeField] private Transform _cinderHeartTarget;
    [SerializeField] private Camera _gameCamera;

    [Header("Enemies")]
    [SerializeField] private EnemyRuntimeSet[] _enemyRuntimeSets;

    public void Initialize(
        GameDataManager gameDataManager,
        Transform cinderHeartTarget,
        Camera gameCamera,
        EnemyRuntimeSet[] enemyRuntimeSets)
    {
        _gameDataManager = gameDataManager;
        _cinderHeartTarget = cinderHeartTarget;
        _gameCamera = gameCamera;
        _enemyRuntimeSets = enemyRuntimeSets;
        ConnectGameDataManagerIfNeeded();
    }

    public void InitializeEnemies()
    {
        if (_enemyRuntimeSets == null)
        {
            return;
        }

        for (int i = 0; i < _enemyRuntimeSets.Length; i++)
        {
            InitializeEnemyRuntimeSet(_enemyRuntimeSets[i]);
        }
    }

    public void InitializeEnemyObject(GameObject enemyObject, string enemyDataId)
    {
        if (enemyObject == null)
        {
            return;
        }

        EnemyData enemyData = GetEnemyData(enemyDataId);
        if (enemyData == null)
        {
            Debug.LogWarning("EnemyLoopConnector: EnemyData가 없습니다. id=" + enemyDataId);
            return;
        }

        PrepareEnemyPhysics(enemyObject);

        Damageable damageable = GetOrAddComponent<Damageable>(enemyObject);
        EnemyStatus enemyStatus = GetOrAddComponent<EnemyStatus>(enemyObject);
        EnemyAttack enemyAttack = GetOrAddComponent<EnemyAttack>(enemyObject);
        EnemyDetector enemyDetector = GetOrAddComponent<EnemyDetector>(enemyObject);
        EnemyMovement enemyMovement = GetOrAddComponent<EnemyMovement>(enemyObject);
        EnemyBrain enemyBrain = GetOrAddComponent<EnemyBrain>(enemyObject);
        EnemyHud enemyHud = enemyObject.GetComponentInChildren<EnemyHud>();

        if (damageable == null)
        {
            Debug.LogWarning("EnemyLoopConnector: Damageable 연결에 실패했습니다. object=" + enemyObject.name);
        }

        InitializeEnemyComponents(enemyData, enemyStatus, enemyAttack, enemyDetector, enemyMovement, enemyBrain, enemyHud);
    }

    private void InitializeEnemyRuntimeSet(EnemyRuntimeSet enemyRuntimeSet)
    {
        if (enemyRuntimeSet == null)
        {
            return;
        }

        EnemyData enemyData = GetEnemyData(enemyRuntimeSet.EnemyDataId);
        if (enemyData == null)
        {
            Debug.LogWarning("EnemyLoopConnector: EnemyData가 없습니다. id=" + enemyRuntimeSet.EnemyDataId);
            return;
        }

        InitializeEnemyComponents(
            enemyData,
            enemyRuntimeSet.EnemyStatus,
            enemyRuntimeSet.EnemyAttack,
            enemyRuntimeSet.EnemyDetector,
            enemyRuntimeSet.EnemyMovement,
            enemyRuntimeSet.EnemyBrain,
            enemyRuntimeSet.EnemyHud);
    }

    private EnemyData GetEnemyData(string enemyDataId)
    {
        ConnectGameDataManagerIfNeeded();

        if (_gameDataManager == null)
        {
            return null;
        }

        _gameDataManager.Initialize();
        return _gameDataManager.GetEnemy(enemyDataId);
    }

    private void ConnectGameDataManagerIfNeeded()
    {
        if (_gameDataManager != null)
        {
            return;
        }

        if (GameManager.Inst == null)
        {
            return;
        }

        _gameDataManager = GameManager.Inst.GetGameDataManager();
    }

    private void PrepareEnemyPhysics(GameObject enemyObject)
    {
        Rigidbody rigidbody = enemyObject.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        NavMeshAgent navMeshAgent = enemyObject.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            return;
        }

        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
    }

    private void InitializeEnemyComponents(
        EnemyData enemyData,
        EnemyStatus enemyStatus,
        EnemyAttack enemyAttack,
        EnemyDetector enemyDetector,
        EnemyMovement enemyMovement,
        EnemyBrain enemyBrain,
        EnemyHud enemyHud)
    {
        if (enemyStatus != null)
        {
            enemyStatus.Initialize(enemyData);
        }

        if (enemyAttack != null)
        {
            enemyAttack.Initialize(enemyData);
        }

        if (enemyDetector != null)
        {
            enemyDetector.Initialize(enemyData);
        }

        if (enemyMovement != null)
        {
            enemyMovement.SetCinderHeartTarget(_cinderHeartTarget);
            enemyMovement.Initialize(enemyData, enemyDetector);
        }

        if (enemyBrain != null)
        {
            enemyBrain.SetCinderHeartTarget(GetCinderHeartDamageable());
        }

        if (enemyHud != null)
        {
            enemyHud.SetTargetCamera(_gameCamera);
        }
    }

    private TComponent GetOrAddComponent<TComponent>(GameObject targetObject)
        where TComponent : Component
    {
        TComponent component = targetObject.GetComponent<TComponent>();
        if (component != null)
        {
            return component;
        }

        return targetObject.AddComponent<TComponent>();
    }

    private Damageable GetCinderHeartDamageable()
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

        return _cinderHeartTarget.GetComponentInParent<Damageable>();
    }
}

[Serializable]
public sealed class EnemyRuntimeSet
{
    [SerializeField] private string _enemyDataId = "ice_zombie";
    [SerializeField] private EnemyStatus _enemyStatus;
    [SerializeField] private EnemyAttack _enemyAttack;
    [SerializeField] private EnemyDetector _enemyDetector;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private EnemyBrain _enemyBrain;
    [SerializeField] private EnemyHud _enemyHud;

    public string EnemyDataId
    {
        get { return _enemyDataId; }
    }

    public EnemyStatus EnemyStatus
    {
        get { return _enemyStatus; }
    }

    public EnemyAttack EnemyAttack
    {
        get { return _enemyAttack; }
    }

    public EnemyDetector EnemyDetector
    {
        get { return _enemyDetector; }
    }

    public EnemyMovement EnemyMovement
    {
        get { return _enemyMovement; }
    }

    public EnemyBrain EnemyBrain
    {
        get { return _enemyBrain; }
    }

    public EnemyHud EnemyHud
    {
        get { return _enemyHud; }
    }
}
