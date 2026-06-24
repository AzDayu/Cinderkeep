using System;
using Cinderkeep.Gameplay;
using UnityEngine;

// 바닥에 놓인 음식 픽업입니다. 현재는 생고기 수급과 Run Result 기록에 사용합니다.
public sealed class FoodPickup : MonoBehaviour, IInteractable
{
    public static event Action<string, int> FoodPickedUpGlobal;

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
        NotifyFoodPickedUp();
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

    private void NotifyFoodPickedUp()
    {
        if (FoodPickedUpGlobal == null)
        {
            return;
        }

        FoodPickedUpGlobal(_foodItemId, _amount);
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
