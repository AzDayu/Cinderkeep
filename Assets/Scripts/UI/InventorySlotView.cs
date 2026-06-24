using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 5.00 direction: Displays or controls UI for the 5.00 playable loop without owning gameplay rules.
// 5.01+ note: Keep UI as a view/controller layer; read models and dispatch requests instead of duplicating game logic.
namespace Cinderkeep.Gameplay
{
    // 인벤토리 한 칸을 표시하는 UI 컴포넌트입니다.
    // 실제 이동과 장착 판단은 InventoryUI가 맡고, 이 클래스는 슬롯 표시와 입력 전달만 담당합니다.
    public sealed class InventorySlotView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [Header("Text UI")]
        [SerializeField] private TMP_Text _itemText;

        [Header("Image UI")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _itemIconImage;

        private InventoryUI _ownerInventoryUI;
        private int _slotIndex;

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
            RefreshItemText(itemModel);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_ownerInventoryUI == null)
            {
                return;
            }

            _ownerInventoryUI.BeginDragInventorySlot(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_ownerInventoryUI == null)
            {
                return;
            }

            _ownerInventoryUI.EndDragInventorySlot();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_ownerInventoryUI == null || eventData == null)
            {
                return;
            }

            if (eventData.clickCount < 2)
            {
                return;
            }

            _ownerInventoryUI.DoubleClickInventorySlot(this);
        }

        private void RefreshItemText(InventoryItemModel itemModel)
        {
            if (_itemText == null)
            {
                return;
            }

            if (itemModel == null || itemModel.IsEmpty)
            {
                _itemText.text = "";
                RefreshBackground(false);
                RefreshItemIcon(null);
                return;
            }

            _itemText.text = UiItemDisplayFormatter.BuildItemStackText(itemModel, true);
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
                _backgroundImage.color = new Color(0.22f, 0.30f, 0.34f, 0.92f);
            }
            else
            {
                _backgroundImage.color = new Color(0.10f, 0.13f, 0.16f, 0.78f);
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
            if (itemType == InventoryItemType.Resource)
            {
                return new Color(0.68f, 0.78f, 0.86f, 0.95f);
            }

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

            return new Color(0.78f, 0.84f, 0.92f, 0.95f);
        }
    }
}
