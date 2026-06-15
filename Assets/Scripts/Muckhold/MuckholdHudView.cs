using UnityEngine;
using UnityEngine.UI;

namespace OODong.Muckhold
{
    public sealed class MuckholdHudView : MonoBehaviour
    {
        [SerializeField] private MuckholdInventory _inventory;
        [SerializeField] private Text _promptText;
        [SerializeField] private Text _statusText;
        [SerializeField] private Text _miningText;
        [SerializeField] private Image _miningFillImage;
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private MuckholdInventoryItemDragView[] _inventoryItemViews;
        [SerializeField] private MuckholdQuickSlotDropTarget[] _quickSlotViews;

        private readonly MuckholdItemId[] _inventoryOrder =
        {
            MuckholdItemId.Arrow,
            MuckholdItemId.Pickaxe,
            MuckholdItemId.Stone,
            MuckholdItemId.Ore,
            MuckholdItemId.Apple
        };

        private int _selectedQuickSlotIndex;
        private bool _isInventoryOpen;

        public bool IsInventoryOpen => _isInventoryOpen;

        private void Awake()
        {
            if (_inventory == null)
            {
                _inventory = FindFirstObjectByType<MuckholdInventory>();
            }

            SetInventory(_inventory);
            SetMiningProgress(0f);
            SetInventoryOpen(false);
        }

        private void OnDestroy()
        {
            if (_inventory != null)
            {
                _inventory.Changed -= Refresh;
            }
        }

        public void SetInventory(MuckholdInventory inventory)
        {
            if (_inventory != null)
            {
                _inventory.Changed -= Refresh;
            }

            _inventory = inventory;

            if (_inventory != null)
            {
                _inventory.Changed += Refresh;
            }

            BindChildViews();
            Refresh();
        }

        public void SetViewReferences(
            Text promptText,
            Text statusText,
            Text miningText,
            Image miningFillImage,
            GameObject inventoryPanel,
            MuckholdInventoryItemDragView[] inventoryItemViews,
            MuckholdQuickSlotDropTarget[] quickSlotViews)
        {
            _promptText = promptText;
            _statusText = statusText;
            _miningText = miningText;
            _miningFillImage = miningFillImage;
            _inventoryPanel = inventoryPanel;
            _inventoryItemViews = inventoryItemViews;
            _quickSlotViews = quickSlotViews;
            BindChildViews();
        }

        public void ToggleInventory()
        {
            SetInventoryOpen(!_isInventoryOpen);
        }

        public void SetInventoryOpen(bool isOpen)
        {
            _isInventoryOpen = isOpen;
            if (_inventoryPanel != null)
            {
                _inventoryPanel.SetActive(isOpen);
            }
        }

        public void SetPrompt(string prompt)
        {
            if (_promptText != null)
            {
                _promptText.text = prompt;
            }
        }

        public void SetStatus(string status)
        {
            if (_statusText != null)
            {
                _statusText.text = status;
            }
        }

        public void SetSelectedQuickSlot(int slotIndex)
        {
            _selectedQuickSlotIndex = Mathf.Clamp(slotIndex, 0, MuckholdInventoryModel.QuickSlotCount - 1);
            RefreshQuickSlots();
        }

        public void SetMiningProgress(float progress)
        {
            float clampedProgress = Mathf.Clamp01(progress);
            if (_miningFillImage != null)
            {
                _miningFillImage.fillAmount = clampedProgress;
            }

            if (_miningText != null)
            {
                _miningText.text = clampedProgress > 0f && clampedProgress < 1f
                    ? $"Mining {Mathf.RoundToInt(clampedProgress * 100f)}%"
                    : string.Empty;
            }
        }

        private void Refresh()
        {
            RefreshInventoryRows();
            RefreshQuickSlots();
        }

        private void RefreshInventoryRows()
        {
            if (_inventoryItemViews == null || _inventory == null)
            {
                return;
            }

            for (int i = 0; i < _inventoryItemViews.Length && i < _inventoryOrder.Length; i++)
            {
                MuckholdItemId itemId = _inventoryOrder[i];
                _inventoryItemViews[i].SetInventory(_inventory);
                _inventoryItemViews[i].SetItem(itemId);
                _inventoryItemViews[i].Refresh(_inventory.GetItemCount(itemId));
            }
        }

        private void RefreshQuickSlots()
        {
            if (_quickSlotViews == null || _inventory == null)
            {
                return;
            }

            for (int i = 0; i < _quickSlotViews.Length; i++)
            {
                _quickSlotViews[i].SetInventory(_inventory);
                _quickSlotViews[i].SetHudView(this);
                _quickSlotViews[i].SetSlotIndex(i);
                _quickSlotViews[i].Refresh(_inventory.GetQuickSlotItem(i));
                _quickSlotViews[i].RefreshSelected(i == _selectedQuickSlotIndex);
            }
        }

        private void BindChildViews()
        {
            if (_inventoryItemViews != null)
            {
                for (int i = 0; i < _inventoryItemViews.Length; i++)
                {
                    if (_inventoryItemViews[i] != null)
                    {
                        _inventoryItemViews[i].SetInventory(_inventory);
                    }
                }
            }

            if (_quickSlotViews != null)
            {
                for (int i = 0; i < _quickSlotViews.Length; i++)
                {
                    if (_quickSlotViews[i] != null)
                    {
                        _quickSlotViews[i].SetInventory(_inventory);
                        _quickSlotViews[i].SetHudView(this);
                        _quickSlotViews[i].SetSlotIndex(i);
                    }
                }
            }
        }
    }
}
