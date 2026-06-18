using Cinderkeep.Gameplay;
using System;
using UnityEngine;

// 적 관련 런타임 연결만 담당하는 컴포넌트입니다.
// CinderkeepGameLoopConnector가 모든 시스템을 직접 붙잡지 않도록 4.01부터 분리하기 시작한 구조입니다.
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

        EnemyStatus enemyStatus = enemyObject.GetComponent<EnemyStatus>();
        EnemyAttack enemyAttack = enemyObject.GetComponent<EnemyAttack>();
        EnemyDetector enemyDetector = enemyObject.GetComponent<EnemyDetector>();
        EnemyMovement enemyMovement = enemyObject.GetComponent<EnemyMovement>();
        EnemyHud enemyHud = enemyObject.GetComponentInChildren<EnemyHud>();

        InitializeEnemyComponents(enemyData, enemyStatus, enemyAttack, enemyDetector, enemyMovement, enemyHud);
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
            enemyRuntimeSet.EnemyHud);
    }

    private EnemyData GetEnemyData(string enemyDataId)
    {
        if (_gameDataManager == null)
        {
            return null;
        }

        _gameDataManager.Initialize();
        return _gameDataManager.GetEnemy(enemyDataId);
    }

    private void InitializeEnemyComponents(
        EnemyData enemyData,
        EnemyStatus enemyStatus,
        EnemyAttack enemyAttack,
        EnemyDetector enemyDetector,
        EnemyMovement enemyMovement,
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

        if (enemyHud != null)
        {
            enemyHud.SetTargetCamera(_gameCamera);
        }
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

    public EnemyHud EnemyHud
    {
        get { return _enemyHud; }
    }
}
