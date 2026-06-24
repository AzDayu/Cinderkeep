using System;
using UnityEngine;

namespace Cinderkeep.Gameplay
{
    // 제작법 실행을 담당하는 컴포넌트입니다.
    // 비용 확인, 자원 차감, 결과 지급만 담당하고 UI 표시는 다른 컴포넌트가 맡습니다.
    public sealed class CraftingRecipeExecutor : MonoBehaviour
    {
        public bool CanCraft(CraftingRecipeData recipeData, PlayerModel playerModel)
        {
            if (recipeData == null || playerModel == null)
            {
                return false;
            }

            if (CanGrantRecipeResult(recipeData) == false)
            {
                return false;
            }

            return CanPayRecipeCost(recipeData, playerModel);
        }

        public bool TryCraft(CraftingRecipeData recipeData, PlayerModel playerModel)
        {
            if (CanCraft(recipeData, playerModel) == false)
            {
                return false;
            }

            if (TryPayRecipeCost(recipeData, playerModel) == false)
            {
                return false;
            }

            return TryGrantRecipeResult(recipeData, playerModel);
        }

        private bool CanPayRecipeCost(CraftingRecipeData recipeData, PlayerModel playerModel)
        {
            for (int i = 0; i < recipeData.Costs.Count; i++)
            {
                CraftingCostData costData = recipeData.Costs[i];
                if (costData == null)
                {
                    continue;
                }

                if (playerModel.HasResource(costData.ResourceId, costData.Amount) == false)
                {

                    return false;
                }

            }

            return true;
        }

        private bool TryPayRecipeCost(CraftingRecipeData recipeData, PlayerModel playerModel)
        {
            for (int i = 0; i < recipeData.Costs.Count; i++)
            {
                CraftingCostData costData = recipeData.Costs[i];
                if (costData == null)
                {
                    continue;
                }

                if (playerModel.UseResource(costData.ResourceId, costData.Amount) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryGrantRecipeResult(CraftingRecipeData recipeData, PlayerModel playerModel)
        {
            if (IsResourceResult(recipeData) == true)
            {
                playerModel.AddResource(recipeData.ResultItemId, recipeData.ResultCount);
                return true;
            }

            InventoryItemType itemType;
            if (TryConvertToItemType(recipeData.ResultDataType, out itemType) == true)
            {
                if (GameManager.Inst == null)
                {
                    return false;
                }

                PlayerInventoryModel inventoryModel = GameManager.Inst.PlayerInventoryModel;
                if (inventoryModel == null)
                {
                    return false;
                }

                bool isAdded = inventoryModel.TryAddItem(recipeData.ResultItemId, itemType, recipeData.ResultCount);
                if (isAdded == false)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool CanGrantRecipeResult(CraftingRecipeData recipeData)
        {
            if (IsResourceResult(recipeData) == true)
            {
                return true;
            }

            InventoryItemType itemType;
            bool result = TryConvertToItemType(recipeData.ResultDataType, out itemType);
            return result;
        }

        private bool IsResourceResult(CraftingRecipeData recipeData)
        {
            if (recipeData == null)
            {
                return false;
            }

            return string.Equals(recipeData.ResultDataType, "Resource", StringComparison.OrdinalIgnoreCase);
        }

        private bool TryConvertToItemType(string resultDataType, out InventoryItemType itemType)
        {
            itemType = InventoryItemType.Tool;

            if (string.Equals(resultDataType, "Tool", StringComparison.OrdinalIgnoreCase))
            {
                itemType = InventoryItemType.Tool;
                return true;
            }

            // Weapon, Armor는 다음 단계에서 여기에 추가
            return false;
        }
    }
}
