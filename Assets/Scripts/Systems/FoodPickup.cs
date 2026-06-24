using Cinderkeep.Gameplay;
using UnityEngine;

// 바닥 음식 픽업입니다. 5.11에서는 생고기만 사용하고, 이후 순록 드롭으로 대체합니다.
public sealed class FoodPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string _foodItemId = FoodItemIds.RawMeat;
    [SerializeField] private int _amount = 1;
    [SerializeField] private bool _disableAfterPickup = true;
    [SerializeField] private bool _canInteract = true;

    public bool CanInteract(GameObject gameObjectInteractor)
    {
        return _canInteract && gameObjectInteractor != null;
    }

    public void Interact(GameObject gameObjectInteractor)
    {
        if (CanInteract(gameObjectInteractor) == false)
        {
            return;
        }

        if (TryGiveFood() == false)
        {
            return;
        }

        PlayPickupSfx();
        ProcessPickedUp();
    }

    public void ResetPickup()
    {
        _canInteract = true;
        gameObject.SetActive(true);
    }

    private bool TryGiveFood()
    {
        if (GameManager.Inst == null)
        {
            return false;
        }

        PlayerInventoryModel inventoryModel = GameManager.Inst.PlayerInventoryModel;
        if (inventoryModel == null)
        {
            return false;
        }

        return inventoryModel.TryAddItem(_foodItemId, InventoryItemType.Food, _amount);
    }

    private void PlayPickupSfx()
    {
        if (GameManager.Inst == null || GameManager.Inst.GetSoundManager() == null)
        {
            return;
        }

        GameManager.Inst.GetSoundManager().PlayResourcePickup();
    }

    private void ProcessPickedUp()
    {
        _canInteract = false;
        if (_disableAfterPickup)
        {
            gameObject.SetActive(false);
        }
    }
}
