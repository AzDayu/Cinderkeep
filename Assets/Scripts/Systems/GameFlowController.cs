using Cinderkeep.Gameplay;
using UnityEngine;

// 3일 게임 루프의 페이즈 전환을 지휘하는 컨트롤러입니다.
// 실제 전투, UI 표시, 적 행동, 스폰 지점 제어는 각 전용 컴포넌트가 담당합니다.
// 이 클래스는 현재 페이즈에서 다음 페이즈로 넘어가는 순서만 관리합니다.
public sealed class GameFlowController : MonoBehaviour, IGameInitializable
{
    [Header("Flow Settings")]
    [Tooltip("낮, 밤, 아침 보상, 보스 접근 시간표입니다.")]
    [SerializeField] private GameFlowSettings _gameFlowSettings = new GameFlowSettings();

    [Header("Enemy Spawn Director")]
    [Tooltip("현재 낮, 밤, 보스 페이즈에 맞춰 적 스폰 지점들을 켜고 끄는 컴포넌트입니다.")]
    [SerializeField] private GameFlowEnemySpawnDirector _enemySpawnDirector;

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

        StopEnemySpawn();
        _isFlowRunning = false;
        _isInitialized = true;
    }

    public void InitializeEnemySpawnPoints(GameObjectManager gameObjectManager, EnemyLoopConnector enemyLoopConnector)
    {
        if (_enemySpawnDirector == null)
        {
            return;
        }

        _enemySpawnDirector.Initialize(gameObjectManager, enemyLoopConnector);
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
        StopEnemySpawn();
    }

    public void ClearFlow()
    {
        if (_gameRunModel == null)
        {
            return;
        }

        _isFlowRunning = false;
        StopEnemySpawn();
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
        _gameRunModel.SetRemainingTime(_gameFlowSettings.DayDuration);
        StartEnemySpawn(EnemySpawnMode.Day);
    }

    private void StartNight()
    {
        _gameRunModel.SetPhase(GameRunPhase.Night);
        _gameRunModel.SetRemainingTime(_gameFlowSettings.NightDuration);
        StartEnemySpawn(EnemySpawnMode.Night);
    }

    private void FinishNight()
    {
        StopEnemySpawn();

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
        _gameRunModel.SetRemainingTime(_gameFlowSettings.MorningRewardDuration);
    }

    private void StartNextDay()
    {
        _gameRunModel.AdvanceDay();
        StartDay(_gameRunModel.Day);
    }

    private void StartBossApproach()
    {
        _gameRunModel.SetPhase(GameRunPhase.BossApproach);
        _gameRunModel.SetRemainingTime(_gameFlowSettings.BossApproachDuration);
        StartEnemySpawn(EnemySpawnMode.Boss);
    }

    private void StartBossFight()
    {
        _gameRunModel.SetPhase(GameRunPhase.BossFight);
        _gameRunModel.SetRemainingTime(0f);
        StartEnemySpawn(EnemySpawnMode.Boss);
    }

    private void StartEnemySpawn(EnemySpawnMode spawnMode)
    {
        if (_enemySpawnDirector == null)
        {
            return;
        }

        _enemySpawnDirector.StartSpawn(spawnMode, _gameRunModel.Day);
    }

    private void StopEnemySpawn()
    {
        if (_enemySpawnDirector == null)
        {
            return;
        }

        _enemySpawnDirector.StopSpawn();
    }

    private void OnValidate()
    {
        if (_gameFlowSettings == null)
        {
            _gameFlowSettings = new GameFlowSettings();
        }

        _gameFlowSettings.ClampValues();
    }
}
