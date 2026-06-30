using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 플레이 상태를 화면에 표시하거나 사용자의 UI 요청을 전달합니다.
// UI는 규칙을 소유하지 않고 모델을 읽고 시스템에 요청을 보내는 계층으로 유지합니다.
namespace Cinderkeep.Gameplay
{
    // 용광로 UI를 갱신하는 컴포넌트입니다.
    // 왼쪽 입력 칸, 진행 게이지, 오른쪽 결과 칸을 코드에서 새로 만들지 않고 준비된 UI에 표시합니다.
    public sealed class FurnaceUI : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject _rootObject;

        [Header("Input Slot")]
        [SerializeField] private TMP_Text _inputResourceText;
        [SerializeField] private TMP_Text _inputAmountText;
        [SerializeField] private string _selectedInputResourceId = PlayerModel.ResourceIronOre;
        [SerializeField] private int _selectedInputAmount = 1;

        [Header("Recipe Slots")]
        [SerializeField] private FurnaceRecipeButtonView[] _recipeSlots;

        [Header("Progress UI")]
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TMP_Text _progressText;

        [Header("Output Slot")]
        [SerializeField] private TMP_Text _outputResourceText;
        [SerializeField] private TMP_Text _outputAmountText;

        [Header("Button UI")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _collectButton;

        [Header("Message UI")]
        [SerializeField] private TMP_Text _messageText;

        [Header("Crafting Embed")]
        [SerializeField] private InventoryUI _inventoryUI;

        [Header("Auto Close")]
        [Tooltip("용광로 UI가 열린 뒤 플레이어가 용광로에서 이 거리 이상 멀어지면 자동으로 닫습니다.")]
        [SerializeField] private float _autoCloseDistance = 4.5f;

        private readonly System.Collections.Generic.List<SmeltingRecipeData> _availableRecipes
            = new System.Collections.Generic.List<SmeltingRecipeData>();

        private FurnaceStation _currentFurnaceStation;
        private Transform _currentInteractorTransform;
        private PlayerModel _playerModel;

        private bool _isOpen;
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
        }

        public void SetInventoryUI(InventoryUI inventoryUI)
        {
            _inventoryUI = inventoryUI;
        }

        private void OnEnable()
        {
            ConnectButtons();
        }

        private void OnDisable()
        {
            DisconnectButtons();
            UnsubscribeFurnace();
        }

        private void Start()
        {
            Close();
        }

        private void Update()
        {
            RefreshUI();
        }

        public void OpenFurnace(FurnaceStation furnaceStation)
        {
            OpenFurnace(furnaceStation, null);
        }

        public void OpenFurnace(FurnaceStation furnaceStation, GameObject interactor)
        {
            if (furnaceStation == null)
            {
                return;
            }

            SetFurnaceStation(furnaceStation);
            _currentInteractorTransform = interactor == null ? null : interactor.transform;
            ConnectPlayerModel();
            ApplyReadableTextStyle();
            SetVisible(true);
            RefreshUI();

            if (_inventoryUI != null)
            {
                _inventoryUI.OpenEmbedded();
            }
        }

        public void Close()
        {
            UnsubscribeFurnace();
            SetVisible(false);
            _currentInteractorTransform = null;

            if (_inventoryUI != null)
            {
                _inventoryUI.Close();
            }
        }

        public bool ShouldCloseForDistance()
        {
            if (_isOpen == false || _currentFurnaceStation == null || _currentInteractorTransform == null)
            {
                return false;
            }

            float safeDistance = Mathf.Max(0.5f, _autoCloseDistance);
            return Vector3.Distance(_currentFurnaceStation.transform.position, _currentInteractorTransform.position) > safeDistance;
        }

        public void SetInputResource(string inputResourceId, int inputAmount)
        {
            _selectedInputResourceId = inputResourceId;
            _selectedInputAmount = inputAmount;
            RefreshUI();
        }

        // 레시피 버튼이 클릭되면 그 레시피의 투입물을 골라 바로 제련을 시작합니다.
        public void StartSmeltingByRecipe(string inputResourceId, int inputAmount)
        {
            SetInputResource(inputResourceId, inputAmount);
            TryStartSmelting();
        }

        public void TryStartSmelting()
        {
            ConnectPlayerModel();

            if (_currentFurnaceStation == null || _playerModel == null)
            {
                PlayFailSfx();
                RefreshMessage("용광로 연결이 아직 준비되지 않았습니다.");
                return;
            }

            if (_currentFurnaceStation.TryStartSmeltingByInputResource(_selectedInputResourceId, _selectedInputAmount, _playerModel))
            {
                PlaySuccessSfx();
                RefreshMessage("제련을 시작했습니다.");
                RefreshUI();
                return;
            }

            PlayFailSfx();
            RefreshMessage("제련할 광석이 부족하거나 사용할 수 없는 재료입니다.");
            RefreshUI();
        }

        public void TryCollectOutput()
        {
            ConnectPlayerModel();

            if (_currentFurnaceStation == null || _playerModel == null)
            {
                PlayFailSfx();
                RefreshMessage("용광로 연결이 아직 준비되지 않았습니다.");
                return;
            }

            if (_currentFurnaceStation.TryCollectOutput(_playerModel))
            {
                PlaySuccessSfx();
                RefreshMessage("주괴를 회수했습니다.");
                RefreshUI();
                return;
            }

            PlayFailSfx();
            RefreshMessage("회수할 결과물이 없습니다.");
            RefreshUI();
        }

        private void SetFurnaceStation(FurnaceStation furnaceStation)
        {
            if (_currentFurnaceStation == furnaceStation)
            {
                return;
            }

            UnsubscribeFurnace();
            _currentFurnaceStation = furnaceStation;
            SubscribeFurnace();
        }

        private void RefreshUI()
        {
            RefreshInputSlot();
            RefreshProgress();
            RefreshOutputSlot();
            RefreshButtons();
            RefreshRecipes();
        }

        private void RefreshRecipes()
        {
            ClearRecipeSlots();

            if (_recipeSlots == null || _currentFurnaceStation == null)
            {
                return;
            }

            GameDataManager gameDataManager = GameManager.Inst == null ? null : GameManager.Inst.GetGameDataManager();
            if (gameDataManager == null)
            {
                return;
            }

            _currentFurnaceStation.GetAvailableSmeltingRecipes(gameDataManager, _availableRecipes);

            for (int i = 0; i < _availableRecipes.Count; i++)
            {
                if (i >= _recipeSlots.Length)
                {
                    RefreshMessage("표시 가능한 제련 슬롯이 부족합니다.");
                    return;
                }

                if (_recipeSlots[i] == null)
                {
                    continue;
                }

                SmeltingRecipeData recipeData = _availableRecipes[i];
                bool canSmelt = CanSmeltRecipe(recipeData);
                _recipeSlots[i].SetRecipe(recipeData, canSmelt, GetSmeltStateText(recipeData), this);
            }
        }

        private void ClearRecipeSlots()
        {
            if (_recipeSlots == null)
            {
                return;
            }

            for (int i = 0; i < _recipeSlots.Length; i++)
            {
                if (_recipeSlots[i] == null)
                {
                    continue;
                }

                _recipeSlots[i].Clear();
            }
        }

        private bool CanSmeltRecipe(SmeltingRecipeData recipeData)
        {
            if (recipeData == null || _playerModel == null)
            {
                return false;
            }

            if (_currentFurnaceStation != null && _currentFurnaceStation.IsSmelting)
            {
                return false;
            }

            return _playerModel.HasResource(recipeData.InputResourceId, recipeData.InputAmount);
        }

        private string GetSmeltStateText(SmeltingRecipeData recipeData)
        {
            if (_currentFurnaceStation != null && _currentFurnaceStation.IsSmelting)
            {
                return "제련 중";
            }

            if (recipeData == null || _playerModel == null)
            {
                return "재료 부족";
            }

            int owned = _playerModel.GetResourceAmount(recipeData.InputResourceId);
            if (owned >= recipeData.InputAmount)
            {
                return "제련 가능";
            }

            return "재료 부족 (" + owned + "/" + recipeData.InputAmount + ")";
        }

        private void RefreshInputSlot()
        {
            RefreshText(_inputResourceText, _selectedInputResourceId);
            RefreshText(_inputAmountText, _selectedInputAmount.ToString());
        }

        private void RefreshProgress()
        {
            float progress = 0f;
            if (_currentFurnaceStation != null)
            {
                progress = _currentFurnaceStation.GetProgress01();
            }

            if (_progressSlider != null)
            {
                _progressSlider.minValue = 0f;
                _progressSlider.maxValue = 1f;
                _progressSlider.value = progress;
            }

            RefreshText(_progressText, Mathf.RoundToInt(progress * 100f).ToString() + "%");
        }

        private void RefreshOutputSlot()
        {
            if (_currentFurnaceStation == null)
            {
                RefreshText(_outputResourceText, "");
                RefreshText(_outputAmountText, "0");
                return;
            }

            RefreshText(_outputResourceText, _currentFurnaceStation.CurrentOutputResourceId);
            RefreshText(_outputAmountText, _currentFurnaceStation.ReadyOutputAmount.ToString());
        }

        private void RefreshButtons()
        {
            if (_startButton != null)
            {
                _startButton.interactable = _currentFurnaceStation != null && _currentFurnaceStation.IsSmelting == false;
            }

            if (_collectButton != null)
            {
                _collectButton.interactable = _currentFurnaceStation != null && _currentFurnaceStation.ReadyOutputAmount > 0;
            }
        }

        private void RefreshMessage(string message)
        {
            RefreshText(_messageText, message);
        }

        private void ApplyReadableTextStyle()
        {
            ApplyTextStyle(_inputResourceText, 18f);
            ApplyTextStyle(_inputAmountText, 18f);
            ApplyTextStyle(_outputResourceText, 18f);
            ApplyTextStyle(_outputAmountText, 18f);
            ApplyTextStyle(_progressText, 18f);
            ApplyTextStyle(_messageText, 20f);

            if (_recipeSlots == null)
            {
                return;
            }

            for (int i = 0; i < _recipeSlots.Length; i++)
            {
                if (_recipeSlots[i] != null)
                {
                    _recipeSlots[i].ApplyReadableTextStyle();
                }
            }
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

        private void RefreshText(TMP_Text targetText, string text)
        {
            if (targetText == null)
            {
                return;
            }

            targetText.text = text;
        }

        private void ConnectPlayerModel()
        {
            if (_playerModel != null)
            {
                return;
            }

            if (GameManager.Inst == null)
            {
                return;
            }

            _playerModel = GameManager.Inst.PlayerModel;
        }

        private void SubscribeFurnace()
        {
            if (_currentFurnaceStation == null)
            {
                return;
            }

            _currentFurnaceStation.OnFurnaceStateChanged += RefreshUI;
        }

        private void UnsubscribeFurnace()
        {
            if (_currentFurnaceStation == null)
            {
                return;
            }

            _currentFurnaceStation.OnFurnaceStateChanged -= RefreshUI;
        }

        private void ConnectButtons()
        {
            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(TryStartSmelting);
                _startButton.onClick.AddListener(TryStartSmelting);
            }

            if (_collectButton != null)
            {
                _collectButton.onClick.RemoveListener(TryCollectOutput);
                _collectButton.onClick.AddListener(TryCollectOutput);
            }
        }

        private void DisconnectButtons()
        {
            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(TryStartSmelting);
            }

            if (_collectButton != null)
            {
                _collectButton.onClick.RemoveListener(TryCollectOutput);
            }
        }

        private void SetVisible(bool isVisible)
        {
            _isOpen = isVisible;

            if (_rootObject == null)
            {
                gameObject.SetActive(isVisible);
                return;
            }

            _rootObject.SetActive(isVisible);
        }

        private void PlaySuccessSfx()
        {
            SoundManager soundManager = GetSoundManager();
            if (soundManager == null)
            {
                return;
            }

            soundManager.PlayUiSuccess();
        }

        private void PlayFailSfx()
        {
            SoundManager soundManager = GetSoundManager();
            if (soundManager == null)
            {
                return;
            }

            soundManager.PlayUiFail();
        }

        private SoundManager GetSoundManager()
        {
            if (GameManager.Inst == null)
            {
                return null;
            }

            return GameManager.Inst.GetSoundManager();
        }
    }
}
