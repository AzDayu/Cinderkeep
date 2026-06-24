using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 5.00 direction: Displays or controls UI for the 5.00 playable loop without owning gameplay rules.
// 5.01+ note: Keep UI as a view/controller layer; read models and dispatch requests instead of duplicating game logic.
namespace Cinderkeep.Gameplay
{
    // 1~7번 퀵슬롯을 표시하는 UI 컴포넌트입니다.
    // 드래그 앤 드롭과 더블클릭 자동 등록 결과를 플레이어가 즉시 확인할 수 있게 보여줍니다.
    public sealed class QuickSlotView : MonoBehaviour, IDropHandler
    {
        [Header("Slot")]
        [SerializeField] private int _slotIndex;

        [Header("Text UI")]
        [SerializeField] private TMP_Text _slotText;

        [Header("Image UI")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _itemIconImage;

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
                _slotText.text = numberText;
                RefreshBackground(false);
                RefreshItemIcon(null);
                return;
            }

            _slotText.text = numberText + "\n" + UiItemDisplayFormatter.GetItemName(itemModel);
            RefreshBackground(true);
            RefreshItemIcon(itemModel);
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

        private void RefreshItemIcon(InventoryItemModel itemModel)
        {
            if (_itemIconImage == null)
            {
                return;
            }

            bool hasItem = itemModel != null && itemModel.IsEmpty == false;
            _itemIconImage.enabled = hasItem;
            if (hasItem == false)
            {
                return;
            }

            _itemIconImage.color = GetFallbackItemColor(itemModel.ItemType);
        }

        private Color GetFallbackItemColor(InventoryItemType itemType)
        {
            if (itemType == InventoryItemType.Weapon)
            {
                return new Color(0.95f, 0.35f, 0.32f, 0.95f);
            }

            if (itemType == InventoryItemType.Tool)
            {
                return new Color(0.82f, 0.73f, 0.50f, 0.95f);
            }

            if (itemType == InventoryItemType.Food)
            {
                return new Color(0.35f, 0.92f, 0.45f, 0.95f);
            }

            if (itemType == InventoryItemType.Building)
            {
                return new Color(0.50f, 0.70f, 0.95f, 0.95f);
            }

            return new Color(0.78f, 0.84f, 0.92f, 0.95f);
        }
    }
}
