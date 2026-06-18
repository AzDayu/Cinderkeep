using UnityEngine;

public enum EnemySpawnMode
{
    Day,
    Night,
    Boss
}

// 낮, 밤, 보스 접근 단계별 스폰 숫자를 담는 설정 클래스입니다.
// 팀원은 이 값만 조절해서 스폰 주기와 한 번에 나오는 수를 쉽게 바꿀 수 있습니다.
[System.Serializable]
public sealed class EnemySpawnRule
{
    [SerializeField] private float _spawnInterval = 45f;
    [SerializeField] private int _spawnCountPerWave = 1;
    [SerializeField] private int _maxAliveEnemyCount = 3;
    [SerializeField] private bool _spawnOnModeStart = true;

    public EnemySpawnRule()
    {
    }

    public EnemySpawnRule(float spawnInterval, int spawnCountPerWave, int maxAliveEnemyCount, bool spawnOnModeStart)
    {
        _spawnInterval = spawnInterval;
        _spawnCountPerWave = spawnCountPerWave;
        _maxAliveEnemyCount = maxAliveEnemyCount;
        _spawnOnModeStart = spawnOnModeStart;
        ClampValues();
    }

    public float SpawnInterval
    {
        get
        {
            return _spawnInterval;
        }
    }

    public int SpawnCountPerWave
    {
        get
        {
            return _spawnCountPerWave;
        }
    }

    public int MaxAliveEnemyCount
    {
        get
        {
            return _maxAliveEnemyCount;
        }
    }

    public bool SpawnOnModeStart
    {
        get
        {
            return _spawnOnModeStart;
        }
    }

    public void ClampValues()
    {
        if (_spawnInterval < 1f)
        {
            _spawnInterval = 1f;
        }

        if (_spawnCountPerWave < 0)
        {
            _spawnCountPerWave = 0;
        }

        if (_maxAliveEnemyCount < 0)
        {
            _maxAliveEnemyCount = 0;
        }
    }
}
