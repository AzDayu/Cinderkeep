using UnityEngine;

// 나무, 돌처럼 맵에 배치되는 자원 오브젝트의 기본 컴포넌트입니다.
// 실제 인벤토리 지급과 오브젝트 제거는 후속 작업에서 GameManager/GameObjectManager 흐름과 연결합니다.
public sealed class ResourceNode : MonoBehaviour, IInteractable
{
    [Header("Resource Data")]
    [SerializeField] private string _resourceId = "stone";
    [SerializeField] private int _previewAmount = 1;
    [SerializeField] private bool _canInteract = true;

    public string ResourceId
    {
        get
        {
            return _resourceId;
        }
    }

    public int PreviewAmount
    {
        get
        {
            return _previewAmount;
        }
    }

    public bool CanInteract(GameObject gameObjectInteractor)
    {
        if (_canInteract == false)
        {
            return false;
        }

        return gameObjectInteractor != null;
    }

    public void Interact(GameObject gameObjectInteractor)
    {
        if (CanInteract(gameObjectInteractor) == false)
        {
            return;
        }

        RequestGather(gameObjectInteractor);
    }

    private void RequestGather(GameObject gameObjectInteractor)
    {
        // TODO: 인벤토리 구조가 들어오면 여기서 자원 지급을 연결합니다.
        // TODO: 자원 노드 제거 방식이 확정되면 GameObjectManager에 제거를 요청합니다.
        // 현재 단계에서는 상호작용 진입점만 유지합니다.
    }
}
