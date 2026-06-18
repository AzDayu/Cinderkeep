using Cinderkeep.Gameplay;
using UnityEngine;

// 3일 게임 루프의 시간표를 지휘하는 컨트롤러입니다.
// 실제 전투, UI 표시, 적 행동은 각 전용 컴포넌트가 맡고, 이 클래스는 진행 순서만 관리합니다.
public sealed class GameFlowController : MonoBehaviour, IGameInitializable
{
    [Header("Day Loop Time")]
    [SerializeField] private float _dayDuration = 180f;
    [SerializeField] private float _nightDuration = 120f;
    [SerializeField] private float _morningRewardDuration = 15f;

    [Header("Boss Flow Time")]
    [SerializeField] private float _bossApproachDuration = 180f;

    [Header("Enemy Spawn")]
    [SerializeField] private EnemySpawnPoint[] _enemySpawnPoints;

    private GameManager _gameManager;
    private GameRunModel _gameRunModel;
    private bool _isInitialized;
    private bool _isFlowRunning;

    public bool IsInitialized
    {
        get
        {
            return _isInitialized;
        }
    }

    public void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        if (_gameManager == null)
        {
            _gameRunModel = null;
            return;
        }

        _gameRunModel = _gameManager.GameRunModel;
    }

    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        StopEnemySpawnPoints();
        _isFlowRunning = false;
        _isInitialized = true;
    }

    private void Update()
    {
        UpdateFlowTime();
    }

    public void StartFlow()
    {
        if (_gameRunModel == null)
        {
            Debug.LogWarning("GameFlowController: GameRunModel reference is empty.");
            return;
        }

        _gameRunModel.StartRun();
        _isFlowRunning = true;
        StartDay(GameRunModel.FirstDay);
    }

    public void StopFlowAsGameOver()
    {
        _isFlowRunning = false;
        StopEnemySpawnPoints();
    }

    public void ClearFlow()
    {
        if (_gameRunModel == null)
        {
            return;
        }

        _isFlowRunning = false;
        StopEnemySpawnPoints();
        _gameRunModel.ClearRun();
    }

    private void UpdateFlowTime()
    {
        if (CanUpdateFlow() == false)
        {
            return;
        }

        float remainingTime = _gameRunModel.RemainingTime - Time.deltaTime;
        _gameRunModel.SetRemainingTime(remainingTime);

        if (_gameRunModel.RemainingTime > 0f)
        {
            return;
        }

        AdvancePhase();
    }

    private bool CanUpdateFlow()
    {
        if (_isFlowRunning == false)
        {
            return false;
        }

        if (_gameRunModel == null)
        {
            return false;
        }

        if (_gameRunModel.IsPlaying == false)
        {
            return false;
        }

        if (_gameRunModel.Phase == GameRunPhase.BossFight)
        {
            return false;
        }

        return true;
    }

    private void AdvancePhase()
    {
        switch (_gameRunModel.Phase)
        {
            case GameRunPhase.Day:
                StartNight();
                return;
            case GameRunPhase.Night:
                FinishNight();
                return;
            case GameRunPhase.MorningReward:
                StartNextDay();
                return;
            case GameRunPhase.BossApproach:
                StartBossFight();
                return;
        }
    }

    private void StartDay(int day)
    {
        _gameRunModel.SetDay(day);
        _gameRunModel.SetPhase(GameRunPhase.Day);
        _gameRunModel.SetRemainingTime(_dayDuration);
        StopEnemySpawnPoints();
    }

    private void StartNight()
    {
        _gameRunModel.SetPhase(GameRunPhase.Night);
        _gameRunModel.SetRemainingTime(_nightDuration);
        StartEnemySpawnPointsByDay();
    }

    private void FinishNight()
    {
        StopEnemySpawnPoints();

        if (_gameRunModel.IsFinalDay())
        {
            StartBossApproach();
            return;
        }

        StartMorningReward();
    }

    private void StartMorningReward()
    {
        _gameRunModel.SetPhase(GameRunPhase.MorningReward);
        _gameRunModel.SetRemainingTime(_morningRewardDuration);
    }

    private void StartNextDay()
    {
        _gameRunModel.AdvanceDay();
        StartDay(_gameRunModel.Day);
    }

    private void StartBossApproach()
    {
        _gameRunModel.SetPhase(GameRunPhase.BossApproach);
        _gameRunModel.SetRemainingTime(_bossApproachDuration);
        SetEnemySpawnStep(EnemySpawnStep.Step3);
        StartEnemySpawnPoints();
    }

    private void StartBossFight()
    {
        _gameRunModel.SetPhase(GameRunPhase.BossFight);
        _gameRunModel.SetRemainingTime(0f);
        SetEnemySpawnStep(EnemySpawnStep.Step3);
        StartEnemySpawnPoints();
    }

    private void StartEnemySpawnPointsByDay()
    {
        SetEnemySpawnStepByDay();
        StartEnemySpawnPoints();
    }

    private void SetEnemySpawnStepByDay()
    {
        if (_gameRunModel.Day <= 1)
        {
            SetEnemySpawnStep(EnemySpawnStep.Step1);
            return;
        }

        if (_gameRunModel.Day == 2)
        {
            SetEnemySpawnStep(EnemySpawnStep.Step2);
            return;
        }

        SetEnemySpawnStep(EnemySpawnStep.Step3);
    }

    private void SetEnemySpawnStep(EnemySpawnStep spawnStep)
    {
        if (_enemySpawnPoints == null)
        {
            return;
        }

        for (int i = 0; i < _enemySpawnPoints.Length; i++)
        {
            EnemySpawnPoint spawnPoint = _enemySpawnPoints[i];
            if (spawnPoint == null)
            {
                continue;
            }

            spawnPoint.SetSpawnStep(spawnStep);
        }
    }

    private void StartEnemySpawnPoints()
    {
        if (_enemySpawnPoints == null)
        {
            return;
        }

        for (int i = 0; i < _enemySpawnPoints.Length; i++)
        {
            EnemySpawnPoint spawnPoint = _enemySpawnPoints[i];
            if (spawnPoint == null)
            {
                continue;
            }

            spawnPoint.SetSpawnPointActive(true);
            spawnPoint.ResetSpawnTime();
            spawnPoint.SpawnEnemiesOnce();
        }
    }

    private void StopEnemySpawnPoints()
    {
        if (_enemySpawnPoints == null)
        {
            return;
        }

        for (int i = 0; i < _enemySpawnPoints.Length; i++)
        {
            EnemySpawnPoint spawnPoint = _enemySpawnPoints[i];
            if (spawnPoint == null)
            {
                continue;
            }

            spawnPoint.SetSpawnPointActive(false);
        }
    }

    private void OnValidate()
    {
        ClampDuration();
    }

    private void ClampDuration()
    {
        if (_dayDuration < 1f)
        {
            _dayDuration = 1f;
        }

        if (_nightDuration < 1f)
        {
            _nightDuration = 1f;
        }

        if (_morningRewardDuration < 1f)
        {
            _morningRewardDuration = 1f;
        }

        if (_bossApproachDuration < 1f)
        {
            _bossApproachDuration = 1f;
        }
    }
}
