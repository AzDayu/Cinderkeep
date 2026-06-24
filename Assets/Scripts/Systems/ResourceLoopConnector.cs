using Cinderkeep.Gameplay;
using UnityEngine;

// 5.00 direction: Runs one concrete gameplay system in the 5.00 closed loop.
// 5.01+ note: Keep the class focused on one responsibility and expose simple events or methods for cross-system links.
// 플레이어 모델과 자원 UI를 연결하는 컴포넌트입니다.
// 자원 수량 계산은 PlayerModel이 담당하고 여기서는 화면 표시 연결만 담당합니다.
public sealed class ResourceLoopConnector : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager _gameManager;

    [Header("UI")]
    [SerializeField] private ResourceUI _resourceUi;

    public void Initialize(GameManager gameManager, ResourceUI resourceUi)
    {
        _gameManager = gameManager;
        _resourceUi = resourceUi;
    }

    public void ConnectResourceHud()
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
}
