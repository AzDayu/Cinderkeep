using Cinderkeep.Gameplay;
using UnityEngine;
// 게임 시작 시 팀원들이 만든 컴포넌트를 서로 연결하는 얇은 연결 컴포넌트입니다.
// 실제 기능은 각 전용 컴포넌트가 담당하고, 이 클래스는 시작 시점의 연결 순서만 관리합니다.

// 4.00 게임 루프 씬에서 팀원들이 만든 컴포넌트를 서로 연결하는 얇은 연결 컴포넌트입니다.
// 실제 기능은 각 컴포넌트가 담당하고, 이 클래스는 시작 시점의 연결 순서만 관리합니다.
public sealed class CinderkeepGameLoopConnector : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager GameManager_GameManager;
    [SerializeField] private GameDataManager GameDataManager_GameDataManager;

    [Header("Player")]
    [SerializeField] private PlayerStatus PlayerStatus_PlayerStatus;

    [Header("UI")]
    [SerializeField] private PlayerHUD PlayerHUD_PlayerHUD;
    [SerializeField] private ResourceUI ResourceUI_ResourceUI;

    [Header("World")]
    [SerializeField] private Transform Transform_CinderHeartTarget;
    [SerializeField] private Camera Camera_GameCamera;

    [Header("Enemies")]
    [SerializeField] private EnemyLoopConnector EnemyLoopConnector_EnemyLoopConnector;
    [SerializeField] private EnemyRuntimeSet[] _enemyRuntimeSets;

    private void Start()
    {
        InitializeGameFlow();
        ConnectPlayerHud();
        ConnectResourceHud();
        ConnectEnemies();
    }

    private void InitializeGameFlow()
    {
        if (GameManager_GameManager == null)
        {
            return;
        }

        GameManager_GameManager.StartNewGame();
    }

    private void ConnectPlayerHud()
    {
        if (PlayerHUD_PlayerHUD == null)
        {
            return;
        }

        PlayerHUD_PlayerHUD.SetPlayerStatus(PlayerStatus_PlayerStatus);
    }

    private void ConnectResourceHud()
    {
        if (ResourceUI_ResourceUI == null)
        {
            return;
        }

        if (GameManager_GameManager == null)
        {
            return;
        }

        ResourceUI_ResourceUI.SetPlayerModel(GameManager_GameManager.PlayerModel);
    }

    private void ConnectEnemies()
    {
        if (EnemyLoopConnector_EnemyLoopConnector == null)
        {
            EnemyLoopConnector_EnemyLoopConnector = GetComponent<EnemyLoopConnector>();
        }

        if (EnemyLoopConnector_EnemyLoopConnector == null)
        {
            EnemyLoopConnector_EnemyLoopConnector = gameObject.AddComponent<EnemyLoopConnector>();
        }

        EnemyLoopConnector_EnemyLoopConnector.Initialize(
            GameDataManager_GameDataManager,
            Transform_CinderHeartTarget,
            Camera_GameCamera,
            _enemyRuntimeSets);
        EnemyLoopConnector_EnemyLoopConnector.InitializeEnemies();
    }
}
