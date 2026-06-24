using Cinderkeep.Gameplay;
using UnityEngine;

public sealed class HandStonePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string _toolDataId = PlayerToolController.HandStoneToolDataId;
    [SerializeField] private int _quickSlotIndex = 0;
    [SerializeField] private bool _equipOnPickup = true;
    [SerializeField] private bool _disableAfterPickup = true;
    [SerializeField] private bool _canInteract = true;

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

        if (TryGiveHandStone(gameObjectInteractor) == false)
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

    private bool TryGiveHandStone(GameObject gameObjectInteractor)
    {
        if (GameManager.Inst == null)
        {
            Debug.LogWarning("HandStonePickup: GameManager is missing.");
            return false;
        }

        PlayerInventoryModel inventoryModel = GameManager.Inst.PlayerInventoryModel;
        if (inventoryModel == null)
        {
            return false;
        }

        bool isSet = inventoryModel.TrySetQuickSlotItem(_quickSlotIndex, _toolDataId, InventoryItemType.Tool, 1);
        if (isSet == false)
        {
            return false;
        }

        if (_equipOnPickup)
        {
            PlayerToolController toolController = gameObjectInteractor.GetComponent<PlayerToolController>();
            if (toolController != null)
            {
                toolController.EquipToolData(_toolDataId);
            }
        }

        return true;
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

        if (_disableAfterPickup == false)
        {
            return;
        }

        gameObject.SetActive(false);
    }
}
