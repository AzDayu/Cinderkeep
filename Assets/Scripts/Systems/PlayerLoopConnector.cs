using UnityEngine;

// 5.00 direction: Runs one concrete gameplay system in the 5.00 closed loop.
// 5.01+ note: Keep the class focused on one responsibility and expose simple events or methods for cross-system links.
// 플레이어 상태와 플레이어 HUD를 연결하는 컴포넌트입니다.
// 이동, 공격, 체력 계산은 각 전용 컴포넌트가 담당하고 여기서는 HUD 연결만 담당합니다.
public sealed class PlayerLoopConnector : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerStatus _playerStatus;

    [Header("UI")]
    [SerializeField] private PlayerHUD _playerHud;

    public void Initialize(PlayerStatus playerStatus, PlayerHUD playerHud)
    {
        _playerStatus = playerStatus;
        _playerHud = playerHud;
    }

    public void ConnectPlayerHud()
    {
        if (_playerHud == null)
        {
            return;
        }

        _playerHud.SetPlayerStatus(_playerStatus);
    }
}
