using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OODong.Muckhold
{
    public sealed class MuckholdInventoryItemDragView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        private static MuckholdItemId _draggedItemId = MuckholdItemId.None;
        private static MuckholdInventory _draggedInventory;

        [SerializeField] private MuckholdInventory _inventory;
        [SerializeField] private Text _label;
        [SerializeField] private Image _frameImage;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private int _defaultQuickSlotIndex;
        [SerializeField] private MuckholdItemId _itemId;

        public static bool HasDraggedItem => _draggedItemId != MuckholdItemId.None;
        public static MuckholdItemId DraggedItemId => _draggedItemId;
        public static MuckholdInventory DraggedInventory => _draggedInventory;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_inventory == null || !_inventory.HasItem(_itemId))
            {
                return;
            }

            _draggedItemId = _itemId;
            _draggedInventory = _inventory;

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0.55f;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _draggedItemId = MuckholdItemId.None;
            _draggedInventory = null;

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount < 2 || _inventory == null)
            {
                return;
            }

            _inventory.TryAssignQuickSlot(_defaultQuickSlotIndex, _itemId);
        }

        public void SetInventory(MuckholdInventory inventory)
        {
            _inventory = inventory;
        }

        public void SetItem(MuckholdItemId itemId)
        {
            _itemId = itemId;
        }

        public void SetDefaultQuickSlotIndex(int defaultQuickSlotIndex)
        {
            _defaultQuickSlotIndex = defaultQuickSlotIndex;
        }

        public void SetViewReferences(Text label, Image frameImage, CanvasGroup canvasGroup)
        {
            _label = label;
            _frameImage = frameImage;
            _canvasGroup = canvasGroup;
        }

        public void Refresh(int count)
        {
            if (_label != null)
            {
                _label.text = $"{MuckholdItemCatalog.GetDisplayName(_itemId)} x{count}";
            }

            if (_frameImage != null)
            {
                _frameImage.color = count > 0
                    ? new Color(0.12f, 0.18f, 0.16f, 0.92f)
                    : new Color(0.08f, 0.08f, 0.08f, 0.75f);
            }
        }
    }
}
