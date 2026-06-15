using System;
using UnityEngine;

namespace OODong.Muckhold
{
    public sealed class MuckholdInventory : MonoBehaviour
    {
        [SerializeField] private bool _grantStarterPickaxe = true;
        [SerializeField] private bool _grantStarterArrows = true;
        [SerializeField] private int _starterPickaxeCount = 1;
        [SerializeField] private int _starterArrowCount = 30;
        [SerializeField] private int _starterArrowQuickSlotIndex = 0;
        [SerializeField] private int _starterPickaxeQuickSlotIndex = 1;

        private readonly MuckholdInventoryModel _model = new MuckholdInventoryModel();

        public MuckholdInventoryModel Model => _model;

        public event Action Changed
        {
            add => _model.Changed += value;
            remove => _model.Changed -= value;
        }

        private void Awake()
        {
            GrantStarterLoadout();
        }

        public void AddItem(MuckholdItemId itemId, int count)
        {
            _model.AddItem(itemId, count);
        }

        public bool TryRemoveItem(MuckholdItemId itemId, int count)
        {
            return _model.TryRemoveItem(itemId, count);
        }

        public bool HasItem(MuckholdItemId itemId)
        {
            return _model.HasItem(itemId);
        }

        public int GetItemCount(MuckholdItemId itemId)
        {
            return _model.GetItemCount(itemId);
        }

        public MuckholdItemId GetQuickSlotItem(int slotIndex)
        {
            return _model.GetQuickSlotItem(slotIndex);
        }

        public bool TryAssignQuickSlot(int slotIndex, MuckholdItemId itemId)
        {
            return _model.TryAssignQuickSlot(slotIndex, itemId);
        }

        private void GrantStarterLoadout()
        {
            if (_grantStarterArrows)
            {
                AddItem(MuckholdItemId.Arrow, Mathf.Max(1, _starterArrowCount));
                TryAssignQuickSlot(_starterArrowQuickSlotIndex, MuckholdItemId.Arrow);
            }

            if (!_grantStarterPickaxe)
            {
                return;
            }

            AddItem(MuckholdItemId.Pickaxe, Mathf.Max(1, _starterPickaxeCount));
            TryAssignQuickSlot(_starterPickaxeQuickSlotIndex, MuckholdItemId.Pickaxe);
        }
    }
}
