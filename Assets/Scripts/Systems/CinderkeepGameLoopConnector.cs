using Cinderkeep.Gameplay;
using UnityEngine;
// 게임 시작 시 팀원들이 만든 컴포넌트를 서로 연결하는 얇은 연결 컴포넌트입니다.
// 실제 기능은 각 전용 컴포넌트가 담당하고, 이 클래스는 시작 시점의 연결 순서만 관리합니다.

// 메인 게임 루프 씬에서 팀원들이 만든 컴포넌트를 서로 연결하는 얇은 연결 컴포넌트입니다.
// 실제 기능은 각 컴포넌트가 담당하고, 이 클래스는 시작 시점의 연결 순서만 관리합니다.
public sealed class CinderkeepGameLoopConnector : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameDataManager _gameDataManager;
    [SerializeField] private GameObjectManager _gameObjectManager;

    [Header("Player")]
    [SerializeField] private PlayerStatus _playerStatus;

    [Header("UI")]
    [SerializeField] private PlayerHUD _playerHud;
    [SerializeField] private ResourceUI _resourceUi;

    [Header("World")]
    [SerializeField] private Transform _cinderHeartTarget;
    [SerializeField] private Camera _gameCamera;

    [Header("Enemies")]
    [SerializeField] private EnemyLoopConnector _enemyLoopConnector;
    [SerializeField] private EnemyRuntimeSet[] _enemyRuntimeSets;

    private void Start()
    {
        ConnectPlayerHud();
        ConnectResourceHud();
        ConnectEnemies();
        ConnectEnemySpawnPoints();
        InitializeGameFlow();
    }

    private void InitializeGameFlow()
    {
        if (_gameManager == null)
        {
            return;
        }

        _gameManager.StartNewGame();
    }

    private void ConnectPlayerHud()
    {
        if (_playerHud == null)
        {
            return;
        }

        _playerHud.SetPlayerStatus(_playerStatus);
    }

    private void ConnectResourceHud()
    {
        if (_resourceUi == null)
        {
            return;
        }

        if (_gameManager == null)
        {
            return;
        }

        _resourceUi.SetPlayerModel(_gameManager.PlayerModel);
    }

    private void ConnectEnemies()
    {
        if (_enemyLoopConnector == null)
        {
            _enemyLoopConnector = GetComponent<EnemyLoopConnector>();
        }

        if (_enemyLoopConnector == null)
        {
            _enemyLoopConnector = gameObject.AddComponent<EnemyLoopConnector>();
        }

        _enemyLoopConnector.Initialize(
            _gameDataManager,
            _cinderHeartTarget,
            _gameCamera,
            _enemyRuntimeSets);
        _enemyLoopConnector.InitializeEnemies();
    }

    private void ConnectEnemySpawnPoints()
    {
        if (_gameManager == null)
        {
            return;
        }

        GameFlowController gameFlowController = _gameManager.GetGameFlowController();
        if (gameFlowController == null)
        {
            return;
        }

        GameObjectManager gameObjectManager = GetGameObjectManager();
        gameFlowController.InitializeEnemySpawnPoints(gameObjectManager, _enemyLoopConnector);
    }

    private GameObjectManager GetGameObjectManager()
    {
        if (_gameObjectManager != null)
        {
            return _gameObjectManager;
        }

        if (_gameManager == null)
        {
            return null;
        }

        _gameObjectManager = _gameManager.GetGameObjectManager();
        return _gameObjectManager;
    }
}
