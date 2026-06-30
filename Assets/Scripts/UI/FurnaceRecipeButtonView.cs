using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 플레이 상태를 화면에 표시하거나 사용자의 UI 요청을 전달합니다.
// UI는 규칙을 소유하지 않고 모델을 읽고 시스템에 요청을 보내는 계층으로 유지합니다.
namespace Cinderkeep.Gameplay
{
    // 용광로 제련 목록의 한 줄을 표시하는 UI 컴포넌트입니다.
    // 실제 제련 처리는 FurnaceUI가 담당하고, 이 클래스는 버튼 표시와 클릭 전달만 맡습니다.
    public sealed class FurnaceRecipeButtonView : MonoBehaviour
    {
        [Header("Text UI")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private TMP_Text _stateText;

        [Header("Button UI")]
        [SerializeField] private Button _smeltButton;

        private FurnaceUI _ownerFurnaceUI;
        private string _inputResourceId;
        private int _inputAmount;

        private void OnEnable()
        {
            ConnectButton();
        }

        private void OnDisable()
        {
            DisconnectButton();
        }

        public void ApplyReadableTextStyle()
        {
            ApplyTextStyle(_nameText, 19f);
            ApplyTextStyle(_costText, 17f);
            ApplyTextStyle(_stateText, 17f);
        }

        public void SetRecipe(SmeltingRecipeData recipeData, bool canSmelt, string stateText, FurnaceUI ownerFurnaceUI)
        {
            if (recipeData == null)
            {
                Clear();
                return;
            }

            _ownerFurnaceUI = ownerFurnaceUI;
            _inputResourceId = recipeData.InputResourceId;
            _inputAmount = recipeData.InputAmount;

            SetVisible(true);
            RefreshName(recipeData);
            RefreshCost(recipeData);
            RefreshState(canSmelt, stateText);
            RefreshButton(canSmelt);
        }

        public void Clear()
        {
            _ownerFurnaceUI = null;
            _inputResourceId = string.Empty;
            _inputAmount = 0;

            RefreshText(_nameText, string.Empty);
            RefreshText(_costText, string.Empty);
            RefreshText(_stateText, string.Empty);
            RefreshButton(false);
            SetVisible(false);
        }

        private void HandleSmeltButtonClicked()
        {
            if (_ownerFurnaceUI == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_inputResourceId))
            {
                return;
            }

            _ownerFurnaceUI.StartSmeltingByRecipe(_inputResourceId, _inputAmount);
        }

        private void RefreshName(SmeltingRecipeData recipeData)
        {
            if (string.IsNullOrEmpty(recipeData.DisplayName))
            {
                RefreshText(_nameText, recipeData.OutputResourceId);
                return;
            }

            RefreshText(_nameText, recipeData.DisplayName);
        }

        private void RefreshCost(SmeltingRecipeData recipeData)
        {
            if (_costText == null)
            {
                return;
            }

            string inputName = UiItemDisplayFormatter.GetItemName(recipeData.InputResourceId, InventoryItemType.Resource);
            string outputName = UiItemDisplayFormatter.GetItemName(recipeData.OutputResourceId, InventoryItemType.Resource);
            _costText.text = inputName + " " + recipeData.InputAmount
                + " → " + outputName + " " + recipeData.OutputAmount;
        }

        private void RefreshState(bool canSmelt, string stateText)
        {
            string safeStateText = string.IsNullOrEmpty(stateText)
                ? (canSmelt ? "제련 가능" : "재료 부족")
                : stateText;

            RefreshText(_stateText, safeStateText);
            if (canSmelt)
            {
                RefreshStateColor(new Color(0.35f, 1f, 0.55f, 1f));
            }
            else
            {
                RefreshStateColor(new Color(1f, 0.45f, 0.32f, 1f));
            }
        }

        private void RefreshButton(bool canSmelt)
        {
            if (_smeltButton == null)
            {
                return;
            }

            _smeltButton.interactable = canSmelt;
        }

        private void RefreshStateColor(Color color)
        {
            if (_stateText == null)
            {
                return;
            }

            _stateText.color = color;
        }

        private void RefreshText(TMP_Text targetText, string text)
        {
            if (targetText == null)
            {
                return;
            }

            targetText.text = text;
        }

        private void ApplyTextStyle(TMP_Text targetText, float minFontSize)
        {
            if (targetText == null)
            {
                return;
            }

            targetText.textWrappingMode = TextWrappingModes.Normal;
            targetText.fontSize = Mathf.Max(targetText.fontSize, minFontSize);
        }

        private void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        private void ConnectButton()
        {
            if (_smeltButton == null)
            {
                return;
            }

            _smeltButton.onClick.RemoveListener(HandleSmeltButtonClicked);
            _smeltButton.onClick.AddListener(HandleSmeltButtonClicked);
        }

        private void DisconnectButton()
        {
            if (_smeltButton == null)
            {
                return;
            }

            _smeltButton.onClick.RemoveListener(HandleSmeltButtonClicked);
        }
    }
}
