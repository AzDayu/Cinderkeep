using UnityEngine;

// 5.00 direction: Defines narrow contracts used by 5.00 gameplay systems.
// 5.01+ note: Keep interfaces minimal so they stabilize collaboration without forcing broad abstractions.
// 플레이어가 E키로 상호작용할 수 있는 오브젝트의 공통 규칙입니다.
// 나무, 돌, 제작대, 문처럼 각 오브젝트가 자기 역할에 맞게 구현합니다.
public interface IInteractable
{
    bool CanInteract(GameObject gameObjectInteractor);

    void Interact(GameObject gameObjectInteractor);
}
