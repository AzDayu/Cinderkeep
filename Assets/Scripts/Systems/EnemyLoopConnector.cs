using Cinderkeep.Gameplay;
using System;
using UnityEngine;

// 적 관련 런타임 연결만 담당하는 컴포넌트입니다.
// CinderkeepGameLoopConnector가 모든 시스템을 직접 붙잡지 않도록 4.01부터 분리하기 시작한 구조입니다.
public sealed class EnemyLoopConnector : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameDataManager GameDataManager_GameDataManager;

    [Header("World")]
    [SerializeField] private Transform Transform_CinderHeartTarget;
    [SerializeField] private Camera Camera_GameCamera;

    [Header("Enemies")]
    [SerializeField] private EnemyRuntimeSet[] _enemyRuntimeSets;

    public void Initialize(
        GameDataManager gameDataManager,
        Transform cinderHeartTarget,
        Camera gameCamera,
        EnemyRuntimeSet[] enemyRuntimeSets)
    {
        GameDataManager_GameDataManager = gameDataManager;
        Transform_CinderHeartTarget = cinderHeartTarget;
        Camera_GameCamera = gameCamera;
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
        if (GameDataManager_GameDataManager == null)
        {
            return null;
        }

        GameDataManager_GameDataManager.Initialize();
        return GameDataManager_GameDataManager.GetEnemy(enemyDataId);
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
            enemyMovement.SetCinderHeartTarget(Transform_CinderHeartTarget);
            enemyMovement.Initialize(enemyData, enemyDetector);
        }

        if (enemyHud != null)
        {
            enemyHud.SetTargetCamera(Camera_GameCamera);
        }
    }
}

[Serializable]
public sealed class EnemyRuntimeSet
{
    [SerializeField] private string _enemyDataId = "ice_zombie";
    [SerializeField] private EnemyStatus EnemyStatus_EnemyStatus;
    [SerializeField] private EnemyAttack EnemyAttack_EnemyAttack;
    [SerializeField] private EnemyDetector EnemyDetector_EnemyDetector;
    [SerializeField] private EnemyMovement EnemyMovement_EnemyMovement;
    [SerializeField] private EnemyHud EnemyHud_EnemyHud;

    public string EnemyDataId
    {
        get { return _enemyDataId; }
    }

    public EnemyStatus EnemyStatus
    {
        get { return EnemyStatus_EnemyStatus; }
    }

    public EnemyAttack EnemyAttack
    {
        get { return EnemyAttack_EnemyAttack; }
    }

    public EnemyDetector EnemyDetector
    {
        get { return EnemyDetector_EnemyDetector; }
    }

    public EnemyMovement EnemyMovement
    {
        get { return EnemyMovement_EnemyMovement; }
    }

    public EnemyHud EnemyHud
    {
        get { return EnemyHud_EnemyHud; }
    }
}
