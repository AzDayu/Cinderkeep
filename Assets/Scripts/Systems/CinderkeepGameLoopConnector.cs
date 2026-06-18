using Cinderkeep.Gameplay;
using UnityEngine;
// 메인 게임 루프 씬의 연결 순서를 관리하는 임시 루트 컴포넌트입니다.
// 실제 연결 책임은 PlayerLoopConnector, ResourceLoopConnector, EnemyLoopConnector, GameFlowLoopConnector로 분리합니다.
public sealed class CinderkeepGameLoopConnector : MonoBehaviour
{
    [Header("Loop Connectors")]
    [SerializeField] private PlayerLoopConnector _playerLoopConnector;
    [SerializeField] private ResourceLoopConnector _resourceLoopConnector;
    [SerializeField] private GameFlowLoopConnector _gameFlowLoopConnector;

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
        ConnectPlayerLoop();
        ConnectResourceLoop();
        ConnectEnemies();
        ConnectGameFlowLoop();
    }

    private void ConnectPlayerLoop()
    {
        PlayerLoopConnector playerLoopConnector = GetPlayerLoopConnector();
        if (playerLoopConnector == null)
        {
            return;
        }

        playerLoopConnector.Initialize(_playerStatus, _playerHud);
        playerLoopConnector.ConnectPlayerHud();
    }

    private void ConnectResourceLoop()
    {
        ResourceLoopConnector resourceLoopConnector = GetResourceLoopConnector();
        if (resourceLoopConnector == null)
        {
            return;
        }

        resourceLoopConnector.Initialize(_gameManager, _resourceUi);
        resourceLoopConnector.ConnectResourceHud();
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

    private void ConnectGameFlowLoop()
    {
        GameFlowLoopConnector gameFlowLoopConnector = GetGameFlowLoopConnector();
        if (gameFlowLoopConnector == null)
        {
            return;
        }

        gameFlowLoopConnector.Initialize(_gameManager, GetGameObjectManager(), _enemyLoopConnector);
        gameFlowLoopConnector.ConnectGameFlow();
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

    private PlayerLoopConnector GetPlayerLoopConnector()
    {
        if (_playerLoopConnector != null)
        {
            return _playerLoopConnector;
        }

        _playerLoopConnector = GetComponent<PlayerLoopConnector>();
        if (_playerLoopConnector != null)
        {
            return _playerLoopConnector;
        }

        _playerLoopConnector = gameObject.AddComponent<PlayerLoopConnector>();
        return _playerLoopConnector;
    }

    private ResourceLoopConnector GetResourceLoopConnector()
    {
        if (_resourceLoopConnector != null)
        {
            return _resourceLoopConnector;
        }

        _resourceLoopConnector = GetComponent<ResourceLoopConnector>();
        if (_resourceLoopConnector != null)
        {
            return _resourceLoopConnector;
        }

        _resourceLoopConnector = gameObject.AddComponent<ResourceLoopConnector>();
        return _resourceLoopConnector;
    }

    private GameFlowLoopConnector GetGameFlowLoopConnector()
    {
        if (_gameFlowLoopConnector != null)
        {
            return _gameFlowLoopConnector;
        }

        _gameFlowLoopConnector = GetComponent<GameFlowLoopConnector>();
        if (_gameFlowLoopConnector != null)
        {
            return _gameFlowLoopConnector;
        }

        _gameFlowLoopConnector = gameObject.AddComponent<GameFlowLoopConnector>();
        return _gameFlowLoopConnector;
    }
}
