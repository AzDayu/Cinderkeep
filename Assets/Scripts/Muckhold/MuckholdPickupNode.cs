using UnityEngine;

namespace OODong.Muckhold
{
    public sealed class MuckholdPickupNode : MonoBehaviour, IMuckholdInteractable
    {
        [SerializeField] private MuckholdItemId _itemId = MuckholdItemId.Stone;
        [SerializeField] private int _amount = 1;
        [SerializeField] private string _displayName = "Stone";

        public string GetPrompt()
        {
            return $"[E] Pick up {_displayName}";
        }

        public bool CanInteract(MuckholdFirstPersonPlayer player)
        {
            return player != null && _itemId != MuckholdItemId.None && _amount > 0;
        }

        public void Interact(MuckholdFirstPersonPlayer player)
        {
            if (!CanInteract(player))
            {
                return;
            }

            player.AddItem(_itemId, _amount);
            player.ShowStatus($"+{_amount} {MuckholdItemCatalog.GetDisplayName(_itemId)}");
            gameObject.SetActive(false);
        }

        public void SetPickup(MuckholdItemId itemId, int amount, string displayName)
        {
            _itemId = itemId;
            _amount = amount;
            _displayName = displayName;
        }
    }
}
