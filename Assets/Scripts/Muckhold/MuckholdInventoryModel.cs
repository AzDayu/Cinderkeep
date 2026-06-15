using System;
using System.Collections.Generic;

namespace OODong.Muckhold
{
    public struct MuckholdInventoryStack
    {
        public MuckholdInventoryStack(MuckholdItemId itemId, int count)
        {
            ItemId = itemId;
            Count = count;
        }

        public MuckholdItemId ItemId { get; }
        public int Count { get; }
    }

    public sealed class MuckholdInventoryModel
    {
        public const int QuickSlotCount = 7;

        private readonly Dictionary<MuckholdItemId, int> _items = new Dictionary<MuckholdItemId, int>();
        private readonly MuckholdItemId[] _quickSlots = new MuckholdItemId[QuickSlotCount];

        public event Action Changed;

        public void AddItem(MuckholdItemId itemId, int count)
        {
            if (itemId == MuckholdItemId.None || count <= 0)
            {
                return;
            }

            int currentCount = GetItemCount(itemId);
            _items[itemId] = currentCount + count;
            NotifyChanged();
        }

        public bool TryRemoveItem(MuckholdItemId itemId, int count)
        {
            if (itemId == MuckholdItemId.None || count <= 0)
            {
                return false;
            }

            int currentCount = GetItemCount(itemId);
            if (currentCount < count)
            {
                return false;
            }

            int nextCount = currentCount - count;
            if (nextCount <= 0)
            {
                _items.Remove(itemId);
            }
            else
            {
                _items[itemId] = nextCount;
            }

            NotifyChanged();
            return true;
        }

        public bool HasItem(MuckholdItemId itemId)
        {
            return GetItemCount(itemId) > 0;
        }

        public int GetItemCount(MuckholdItemId itemId)
        {
            return _items.TryGetValue(itemId, out int count) ? count : 0;
        }

        public MuckholdItemId GetQuickSlotItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _quickSlots.Length)
            {
                return MuckholdItemId.None;
            }

            return _quickSlots[slotIndex];
        }

        public bool TryAssignQuickSlot(int slotIndex, MuckholdItemId itemId)
        {
            if (slotIndex < 0 || slotIndex >= _quickSlots.Length)
            {
                return false;
            }

            if (!MuckholdItemCatalog.CanAssignQuickSlot(itemId) || !HasItem(itemId))
            {
                return false;
            }

            _quickSlots[slotIndex] = itemId;
            NotifyChanged();
            return true;
        }

        public IEnumerable<MuckholdInventoryStack> GetStacks(MuckholdItemId[] itemOrder)
        {
            for (int i = 0; i < itemOrder.Length; i++)
            {
                MuckholdItemId itemId = itemOrder[i];
                yield return new MuckholdInventoryStack(itemId, GetItemCount(itemId));
            }
        }

        private void NotifyChanged()
        {
            Changed?.Invoke();
        }
    }
}
