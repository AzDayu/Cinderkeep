using Cinderkeep.Gameplay;
using UnityEngine;

public enum EnemySpawnStep
{
    Step1 = 1, // 1단계: 기본 몬스터 후보
    Step2 = 2, // 2단계: 강화 몬스터 후보
    Step3 = 3  // 3단계: 특수 몬스터 후보
}

// 씬에 배치되는 적 스폰 지점입니다.
// 생성은 GameObjectManager를 통하고, 생성 직후 초기화는 EnemyLoopConnector에 맡깁니다.
// 이 클래스는 체력 계산이나 공격 로직을 직접 처리하지 않습니다.
public sealed class EnemySpawnPoint : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameObjectManager _gameObjectManager;
    [SerializeField] private EnemyLoopConnector _enemyLoopConnector;

    [Header("Spawn Point")]
    [SerializeField] private int _spawnPointId;
    [SerializeField] private bool _isActive = true;

    [Header("Spawn Step")]
    [SerializeField] private EnemySpawnStep _spawnStep = EnemySpawnStep.Step1;
    [SerializeField] private string _step1EnemyDataId = "ice_zombie";
    [SerializeField] private string _step2EnemyDataId = "ice_zombie";
    [SerializeField] private string _step3EnemyDataId = "ice_zombie";

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] _step1EnemyPrefabs;
    [SerializeField] private GameObject[] _step2EnemyPrefabs;
    [SerializeField] private GameObject[] _step3EnemyPrefabs;

    [Header("Spawn Rule")]
    [SerializeField] private float _spawnInterval = 300f;
    [SerializeField] private float _spawnSpacing = 1.2f;
    [SerializeField] private bool _spawnOnStart;

    [Header("Spawn Position")]
    [SerializeField] private Transform _centerTransform;

    [Header("Gizmo")]
    [SerializeField] private bool _showGizmo = true;
    [SerializeField] private Color _gizmoColor = Color.red;

    private float _lastSpawnTime;

    public int SpawnPointId
    {
        get { return _spawnPointId; }
    }

    public EnemySpawnStep SpawnStep
    {
        get { return _spawnStep; }
    }

    public void Initialize(GameObjectManager gameObjectManager, EnemyLoopConnector enemyLoopConnector)
    {
        _gameObjectManager = gameObjectManager;
        _enemyLoopConnector = enemyLoopConnector;
    }

    private void Start()
    {
        InitializeCenter();
        InitializeSpawnTime();
    }

    private void Update()
    {
        UpdateSpawn();
    }

    public void SetSpawnStep(EnemySpawnStep spawnStep)
    {
        _spawnStep = spawnStep;
    }

    public void SetSpawnPointActive(bool isActive)
    {
        _isActive = isActive;
    }

    public void ResetSpawnTime()
    {
        _lastSpawnTime = Time.time;
    }

    public void SpawnEnemiesOnce()
    {
        SpawnEnemiesByCurrentStep();
        ResetSpawnTime();
    }

    private void InitializeCenter()
    {
        if (_centerTransform == null)
        {
            _centerTransform = transform;
        }
    }

    private void InitializeSpawnTime()
    {
        if (_spawnOnStart)
        {
            _lastSpawnTime = Time.time - _spawnInterval;
            return;
        }

        _lastSpawnTime = Time.time;
    }

    private void UpdateSpawn()
    {
        if (CanSpawn() == false)
        {
            return;
        }

        SpawnEnemiesOnce();
    }

    private bool CanSpawn()
    {
        if (_isActive == false)
        {
            return false;
        }

        if (_gameObjectManager == null)
        {
            return false;
        }

        if (Time.time < _lastSpawnTime + _spawnInterval)
        {
            return false;
        }

        return true;
    }

    private void SpawnEnemiesByCurrentStep()
    {
        GameObject[] enemyPrefabs = GetEnemyPrefabsByStep();
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            return;
        }

        string enemyDataId = GetEnemyDataIdByStep();

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            SpawnEnemy(enemyPrefabs[i], i, enemyPrefabs.Length, enemyDataId);
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab, int index, int totalCount, string enemyDataId)
    {
        if (enemyPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = GetSpawnPosition(index, totalCount);
        Quaternion spawnRotation = GetSpawnRotation();
        GameObject createdEnemy = _gameObjectManager.CreateGameObject(enemyPrefab, spawnPosition, spawnRotation);
        InitializeCreatedEnemy(createdEnemy, enemyDataId);
    }

    private void InitializeCreatedEnemy(GameObject createdEnemy, string enemyDataId)
    {
        if (_enemyLoopConnector == null)
        {
            return;
        }

        _enemyLoopConnector.InitializeEnemyObject(createdEnemy, enemyDataId);
    }

    private GameObject[] GetEnemyPrefabsByStep()
    {
        switch (_spawnStep)
        {
            case EnemySpawnStep.Step1:
                return _step1EnemyPrefabs;
            case EnemySpawnStep.Step2:
                return _step2EnemyPrefabs;
            case EnemySpawnStep.Step3:
                return _step3EnemyPrefabs;
        }

        return _step1EnemyPrefabs;
    }

    private string GetEnemyDataIdByStep()
    {
        switch (_spawnStep)
        {
            case EnemySpawnStep.Step1:
                return _step1EnemyDataId;
            case EnemySpawnStep.Step2:
                return _step2EnemyDataId;
            case EnemySpawnStep.Step3:
                return _step3EnemyDataId;
        }

        return _step1EnemyDataId;
    }

    private Vector3 GetSpawnPosition(int index, int totalCount)
    {
        Vector3 spawnPosition = _centerTransform.position;
        float centerOffset = (totalCount - 1) * 0.5f;
        float xOffset = (index - centerOffset) * _spawnSpacing;
        spawnPosition.x += xOffset;
        return spawnPosition;
    }

    private Quaternion GetSpawnRotation()
    {
        return _centerTransform.rotation;
    }

    private void OnValidate()
    {
        if (_spawnInterval < 0f)
        {
            _spawnInterval = 0f;
        }

        if (_spawnSpacing < 0f)
        {
            _spawnSpacing = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_showGizmo == false)
        {
            return;
        }

        Transform center = _centerTransform;
        if (center == null)
        {
            center = transform;
        }

        Gizmos.color = _gizmoColor;
        DrawSpawnPositionGizmo(center.position, _step1EnemyPrefabs);
        DrawSpawnPositionGizmo(center.position, _step2EnemyPrefabs);
        DrawSpawnPositionGizmo(center.position, _step3EnemyPrefabs);
    }

    private void DrawSpawnPositionGizmo(Vector3 centerPosition, GameObject[] enemyPrefabs)
    {
        if (enemyPrefabs == null)
        {
            return;
        }

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            Vector3 position = GetGizmoPosition(centerPosition, i, enemyPrefabs.Length);
            Gizmos.DrawWireSphere(position, 0.25f);
        }
    }

    private Vector3 GetGizmoPosition(Vector3 centerPosition, int index, int totalCount)
    {
        float centerOffset = (totalCount - 1) * 0.5f;
        float xOffset = (index - centerOffset) * _spawnSpacing;
        Vector3 position = centerPosition;
        position.x += xOffset;
        return position;
    }
}
