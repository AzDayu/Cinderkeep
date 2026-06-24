using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 5.00 direction: Displays or controls UI for the 5.00 playable loop without owning gameplay rules.
// 5.01+ note: Keep UI as a view/controller layer; read models and dispatch requests instead of duplicating game logic.
namespace Cinderkeep.Gameplay
{
    // 헬멧, 갑옷, 무기, 신발 장비 칸을 표시하는 UI 컴포넌트입니다.
    // 인벤토리 슬롯에서 드롭하면 InventoryUI에 장착 요청을 전달합니다.
    public sealed class EquipmentSlotView : MonoBehaviour, IDropHandler
    {
        [Header("Slot")]
        [SerializeField] private EquipmentSlotType _slotType;

        [Header("Text UI")]
        [SerializeField] private TMP_Text _slotText;

        [Header("Image UI")]
        [SerializeField] private Image _backgroundImage;

        private InventoryUI _ownerInventoryUI;

        public EquipmentSlotType SlotType
        {
            get
            {
                return _slotType;
            }
        }

        public void SetSlot(EquipmentSlotType slotType, string equippedItemId, InventoryUI ownerInventoryUI)
        {
            _slotType = slotType;
            _ownerInventoryUI = ownerInventoryUI;
            RefreshSlotText(equippedItemId);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (_ownerInventoryUI == null)
            {
                return;
            }

            _ownerInventoryUI.DropInventoryToEquipmentSlot(this);
        }

        private void RefreshSlotText(string equippedItemId)
        {
            if (_slotText == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(equippedItemId))
            {
                _slotText.text = GetSlotLabel() + "\nEmpty";
                RefreshBackground(false);
                return;
            }

            _slotText.text = GetSlotLabel() + "\n" + equippedItemId;
            RefreshBackground(true);
        }

        private string GetSlotLabel()
        {
            if (_slotType == EquipmentSlotType.Helmet)
            {
                return "Helmet";
            }

            if (_slotType == EquipmentSlotType.Armor)
            {
                return "Armor";
            }

            if (_slotType == EquipmentSlotType.Weapon)
            {
                return "Weapon";
            }

            if (_slotType == EquipmentSlotType.Boots)
            {
                return "Boots";
            }

            return "Slot";
        }

        private void RefreshBackground(bool hasItem)
        {
            if (_backgroundImage == null)
            {
                return;
            }

            if (hasItem)
            {
                _backgroundImage.color = new Color(0.28f, 0.24f, 0.15f, 0.95f);
            }
            else
            {
                _backgroundImage.color = new Color(0.11f, 0.11f, 0.13f, 0.86f);
            }
        }
    }
}
