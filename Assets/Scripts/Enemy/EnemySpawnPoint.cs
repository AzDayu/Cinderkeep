using Cinderkeep.Gameplay;
using UnityEngine;


public enum EnemySpawnStep
{
    Step1 = 1, // 스켈레톤
    Step2 = 2, // 얼음좀비
    Step3 = 3  // 얼음늑대
}

public class EnemySpawnPoint : MonoBehaviour
{

    [Header("Manager")]
    [SerializeField] private GameObjectManager GameObjectManager_Main;

    [Header("spawn Point")]
    [SerializeField] private int _spawnPointId = 0;
    [SerializeField] private bool _isActive = true;

    [Header("Spawn Step")]
    [SerializeField] private EnemySpawnStep _spawnStep = EnemySpawnStep.Step1;

    [Header("Enemy Prefab")]
    [SerializeField] private GameObject[] Prefab_Step1Enemies;
    [SerializeField] private GameObject[] Prefab_Step2Enemies;
    [SerializeField] private GameObject[] Prefab_Step3Enemies;

    [Header("Spawn Rule")]
    [SerializeField] private float _spawnInterval = 300f;
    [SerializeField] private float _spawnSpacing = 1.2f;
    [SerializeField] private int _enemyHp = 100;

    [Header("Spawn Position")]
    [SerializeField] private Transform Transform_Center;

    [Header("Gizmo")]
    [SerializeField] private bool _showGizmo = true;
    [SerializeField] private Color _gizmoColor = Color.red;

    private float _lastSpawnTime = 0f;

    public int SpawnPointId
    {
        get { return _spawnPointId; }
    }

    public EnemySpawnStep SpawnStep
    {
        get { return _spawnStep; }
    }

    private void Awake()
    {
        InitializeCenter();
        InitializeSpawnTime();
    }

    private void Update()
    {
        UpdateSpawn();
    }

    private void InitializeCenter()
    {
        if (Transform_Center == null)
        {
            Transform_Center = transform;
        }
    }

    // 게임 시작 후 5분 뒤 첫 스폰되게 설정
    private void InitializeSpawnTime()
    {
        _lastSpawnTime = Time.time;
    }

    // 스폰 가능하면 현재 단계의 몬스터들을 한번에 소환
    private void UpdateSpawn()
    {
        if (CanSpawn() == false)
        {
            return;
        }

        SpawnNormalEnemies();
    }


    private bool CanSpawn()
    {
        if (_isActive == false)
        {
            return false;
        }

        if (GameObjectManager_Main == null)
        {
            return false;
        }

        if (Time.time < _lastSpawnTime + _spawnInterval)
        {
            return false;
        }

        return true;
    }

    // 현재 단계에 해당하는 몬스터들을 모두 소환
    // 현재 단계에 맞는 프리팹 배열을 전부 생성
    // 현재 단계에 맞는 프리팹 배열을 전부 생성
    private void SpawnNormalEnemies()
    {
        GameObject[] enemyPrefabs = GetEnemyPrefabsByStep();

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            return;
        }

        _lastSpawnTime = Time.time;

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[i];

            if (enemyPrefab == null)
            {
                continue;
            }

            Vector3 spawnPosition = GetSpawnPosition(i, enemyPrefabs.Length);
            Quaternion spawnRotation = GetSpawnRotation();

            GameObject createdEnemy = GameObjectManager_Main.CreateGameObject(enemyPrefab, spawnPosition, spawnRotation);

            InitializeCreatedEnemy(createdEnemy);
        }
    }

    // 생성된 몬스터에 스폰 정보를 전달



    private GameObject[] GetEnemyPrefabsByStep()
    {
        switch (_spawnStep)
        {
            case EnemySpawnStep.Step1:
                return Prefab_Step1Enemies;

            case EnemySpawnStep.Step2:
                return Prefab_Step2Enemies;

            case EnemySpawnStep.Step3:
                return Prefab_Step3Enemies;
        }

        return Prefab_Step1Enemies;
    }

    // 여러 마리가 같은 위치에 겹치지 않도록 좌우로 배치
    private Vector3 GetSpawnPosition(int index, int totalCount)
    {
        Vector3 spawnPosition = Transform_Center.position;

        float centerOffset = (totalCount - 1) * 0.5f;
        float xOffset = (index - centerOffset) * _spawnSpacing;

        spawnPosition.x += xOffset;

        return spawnPosition;
    }

    private Quaternion GetSpawnRotation()
    {
        return Transform_Center.rotation;
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

        Transform center = Transform_Center;

        if (center == null)
        {
            center = transform;
        }

        Gizmos.color = _gizmoColor;

        DrawSpawnPositionGizmo(center.position, Prefab_Step1Enemies);
        DrawSpawnPositionGizmo(center.position, Prefab_Step2Enemies);
        DrawSpawnPositionGizmo(center.position, Prefab_Step3Enemies);
    }

    private void DrawSpawnPositionGizmo(Vector3 centerPosition, GameObject[] enemyPrefabs)
    {
        if (enemyPrefabs == null)
        {
            return;
        }

        int count = enemyPrefabs.Length;

        for (int i = 0; i < count; i++)
        {
            float centerOffset = (count - 1) * 0.5f;
            float xOffset = (i - centerOffset) * _spawnSpacing;

            Vector3 position = centerPosition;
            position.x += xOffset;

            Gizmos.DrawWireSphere(position, 0.25f);
        }
    }
}
