using UnityEngine;

// 5.00 direction: Runs one concrete gameplay system in the 5.00 closed loop.
// 5.01+ note: Keep the class focused on one responsibility and expose simple events or methods for cross-system links.
// 메인 게임 씬에서 전용 LoopConnector들의 실행 순서만 관리하는 컴포넌트입니다.
// 4.35 기준으로 이 클래스는 "무엇을 먼저 연결할지"만 지휘합니다.
// Player, Resource, Enemy, GameFlow의 실제 연결 작업은 각 전용 Connector가 담당합니다.
// 새 기능을 추가할 때 이 클래스에 참조를 계속 늘리지 말고, 전용 Connector를 먼저 검토합니다.
public sealed class CinderkeepGameLoopConnector : MonoBehaviour
{
    [Header("Loop Connectors")]
    [SerializeField] private PlayerLoopConnector _playerLoopConnector;
    [SerializeField] private ResourceLoopConnector _resourceLoopConnector;
    [SerializeField] private EnemyLoopConnector _enemyLoopConnector;
    [SerializeField] private GameFlowLoopConnector _gameFlowLoopConnector;

    private void Start()
    {
        ConnectGameLoop();
    }

    public void ConnectGameLoop()
    {
        ConnectPlayerLoop();
        ConnectResourceLoop();
        ConnectEnemyLoop();
        ConnectGameFlowLoop();
    }

    private void ConnectPlayerLoop()
    {
        if (_playerLoopConnector == null)
        {
            return;
        }

        _playerLoopConnector.ConnectPlayerHud();
    }

    private void ConnectResourceLoop()
    {
        if (_resourceLoopConnector == null)
        {
            return;
        }

        _resourceLoopConnector.ConnectResourceHud();
    }

    private void ConnectEnemyLoop()
    {
        if (_enemyLoopConnector == null)
        {
            return;
        }

        _enemyLoopConnector.InitializeEnemies();
    }

    private void ConnectGameFlowLoop()
    {
        if (_gameFlowLoopConnector == null)
        {
            return;
        }

        _gameFlowLoopConnector.ConnectGameFlow();
    }
}
