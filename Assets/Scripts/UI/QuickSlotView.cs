using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 5.00 direction: Displays or controls UI for the 5.00 playable loop without owning gameplay rules.
// 5.01+ note: Keep UI as a view/controller layer; read models and dispatch requests instead of duplicating game logic.
namespace Cinderkeep.Gameplay
{
    // 1~7번 퀵슬롯을 표시하는 UI 컴포넌트입니다.
    // 인벤토리 슬롯에서 드롭하면 해당 퀵슬롯에 아이템 바로가기를 등록합니다.
    public sealed class QuickSlotView : MonoBehaviour, IDropHandler
    {
        [Header("Slot")]
        [SerializeField] private int _slotIndex;

        [Header("Text UI")]
        [SerializeField] private TMP_Text _slotText;

        [Header("Image UI")]
        [SerializeField] private Image _backgroundImage;

        private InventoryUI _ownerInventoryUI;

        public int SlotIndex
        {
            get
            {
                return _slotIndex;
            }
        }

        public void SetSlot(int slotIndex, InventoryItemModel itemModel, InventoryUI ownerInventoryUI)
        {
            _slotIndex = slotIndex;
            _ownerInventoryUI = ownerInventoryUI;
            RefreshSlotText(itemModel);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (_ownerInventoryUI == null)
            {
                return;
            }

            _ownerInventoryUI.DropInventoryToQuickSlot(this);
        }

        private void RefreshSlotText(InventoryItemModel itemModel)
        {
            if (_slotText == null)
            {
                return;
            }

            string numberText = (_slotIndex + 1).ToString();
            if (itemModel == null || itemModel.IsEmpty)
            {
                _slotText.text = numberText + "\nEmpty";
                RefreshBackground(false);
                return;
            }

            _slotText.text = numberText + "\n" + itemModel.ItemId;
            RefreshBackground(true);
        }

        private void RefreshBackground(bool hasItem)
        {
            if (_backgroundImage == null)
            {
                return;
            }

            if (hasItem)
            {
                _backgroundImage.color = new Color(0.18f, 0.31f, 0.24f, 0.95f);
            }
            else
            {
                _backgroundImage.color = new Color(0.08f, 0.10f, 0.12f, 0.86f);
            }
        }
    }
}
