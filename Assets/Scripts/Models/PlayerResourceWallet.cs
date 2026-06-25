using System;
using System.Collections.Generic;

namespace Cinderkeep.Gameplay
{
    // 플레이어가 들고 있는 자원 수량만 관리하는 런타임 지갑입니다.
    // PlayerModel은 외부 진입점을 유지하고, 실제 자원 증감 규칙은 이 클래스에 맡깁니다.
    [Serializable]
    public sealed class PlayerResourceWallet
    {
        private readonly Dictionary<string, int> _resourceAmounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public void InitializeDefault()
        {
            _resourceAmounts.Clear();
            SetResource(PlayerModel.ResourceWood, 0);
            SetResource(PlayerModel.ResourceStone, 0);
            SetResource(PlayerModel.ResourceIron, 0);
            SetResource(PlayerModel.ResourceGold, 0);
            SetResource(PlayerModel.ResourceMithril, 0);
            SetResource(PlayerModel.ResourceAdamantium, 0);
            SetResource(PlayerModel.ResourceIronOre, 0);
            SetResource(PlayerModel.ResourceGoldOre, 0);
            SetResource(PlayerModel.ResourceAdamantiumOre, 0);
            SetResource(PlayerModel.ResourceIronIngot, 0);
            SetResource(PlayerModel.ResourceGoldIngot, 0);
            SetResource(PlayerModel.ResourceAdamantiumIngot, 0);
        }

        public int GetResourceAmount(string resourceType)
        {
            string safeResourceType = NormalizeResourceType(resourceType);
            if (string.IsNullOrEmpty(safeResourceType))
            {
                return 0;
            }

            int amount;
            if (_resourceAmounts.TryGetValue(safeResourceType, out amount) == false)
            {
                return 0;
            }

            return Math.Max(0, amount);
        }

        public bool HasResource(string resourceType, int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            return GetResourceAmount(resourceType) >= amount;
        }

        public bool TryAddResource(string resourceType, int amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            string safeResourceType = NormalizeResourceType(resourceType);
            if (string.IsNullOrEmpty(safeResourceType))
            {
                return false;
            }

            SetResource(safeResourceType, GetResourceAmount(safeResourceType) + amount);
            return true;
        }

        public bool TryUseResource(string resourceType, int amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            string safeResourceType = NormalizeResourceType(resourceType);
            int currentAmount = GetResourceAmount(safeResourceType);
            if (currentAmount < amount)
            {
                return false;
            }

            SetResource(safeResourceType, currentAmount - amount);
            return true;
        }

        private void SetResource(string resourceType, int amount)
        {
            if (string.IsNullOrEmpty(resourceType))
            {
                return;
            }

            _resourceAmounts[resourceType] = Math.Max(0, amount);
        }

        private string NormalizeResourceType(string resourceType)
        {
            if (string.IsNullOrEmpty(resourceType))
            {
                return string.Empty;
            }

            string loweredType = resourceType.ToLowerInvariant();
            switch (loweredType)
            {
                case "wood":
                    return PlayerModel.ResourceWood;
                case "stone":
                    return PlayerModel.ResourceStone;
                case "iron":
                    return PlayerModel.ResourceIron;
                case "gold":
                    return PlayerModel.ResourceGold;
                case "mithril":
                    return PlayerModel.ResourceMithril;
                case "adamantium":
                    return PlayerModel.ResourceAdamantium;
                case "ironore":
                case "iron_ore":
                    return PlayerModel.ResourceIronOre;
                case "goldore":
                case "gold_ore":
                    return PlayerModel.ResourceGoldOre;
                case "adamantiumore":
                case "adamantium_ore":
                    return PlayerModel.ResourceAdamantiumOre;
                case "ironingot":
                case "iron_ingot":
                    return PlayerModel.ResourceIronIngot;
                case "goldingot":
                case "gold_ingot":
                    return PlayerModel.ResourceGoldIngot;
                case "adamantiumingot":
                case "adamantium_ingot":
                    return PlayerModel.ResourceAdamantiumIngot;
            }

            return resourceType;
        }
    }
}
