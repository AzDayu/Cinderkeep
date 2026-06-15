using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OODong.Muckhold
{
    public sealed class MuckholdQuickSlotDropTarget : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        [SerializeField] private MuckholdInventory _inventory;
        [SerializeField] private MuckholdHudView _hudView;
        [SerializeField] private Text _label;
        [SerializeField] private Image _frameImage;
        [SerializeField] private int _slotIndex;

        private bool _isSelected;

        public void OnDrop(PointerEventData eventData)
        {
            if (!MuckholdInventoryItemDragView.HasDraggedItem)
            {
                return;
            }

            MuckholdInventory sourceInventory = MuckholdInventoryItemDragView.DraggedInventory;
            MuckholdInventory targetInventory = _inventory != null ? _inventory : sourceInventory;
            targetInventory?.TryAssignQuickSlot(_slotIndex, MuckholdInventoryItemDragView.DraggedItemId);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _hudView?.SetSelectedQuickSlot(_slotIndex);
        }

        public void SetInventory(MuckholdInventory inventory)
        {
            _inventory = inventory;
        }

        public void SetHudView(MuckholdHudView hudView)
        {
            _hudView = hudView;
        }

        public void SetSlotIndex(int slotIndex)
        {
            _slotIndex = slotIndex;
        }

        public void SetViewReferences(Text label, Image frameImage)
        {
            _label = label;
            _frameImage = frameImage;
        }

        public void Refresh(MuckholdItemId itemId)
        {
            if (_label != null)
            {
                string itemName = MuckholdItemCatalog.GetDisplayName(itemId);
                _label.text = $"{_slotIndex + 1}\n{itemName}";
            }

            RefreshSelected(_isSelected);
        }

        public void RefreshSelected(bool isSelected)
        {
            _isSelected = isSelected;
            if (_frameImage != null)
            {
                _frameImage.color = isSelected
                    ? new Color(0.95f, 0.67f, 0.18f, 0.95f)
                    : new Color(0.08f, 0.12f, 0.14f, 0.9f);
            }
        }
    }
}
