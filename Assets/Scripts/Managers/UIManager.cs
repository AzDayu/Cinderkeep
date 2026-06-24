using UnityEngine;

namespace Cinderkeep.Gameplay
{
    // 게임 씬 전용 UI 매니저입니다.
    // 현재는 3일 게임 루프에 필요한 HUD, 인벤토리, 게임오버 UI부터 관리합니다.
    // UI는 코드에서 새로 만들지 않고, 씬이나 프리팹에 준비된 오브젝트를 켜고 끄는 방식으로 관리합니다.
    public sealed class UIManager : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private GameObject _hudRoot;
        [SerializeField] private GameObject _inventoryRoot;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private InventoryUI _inventoryUI;
        [SerializeField] private CraftingUI _craftingUI;
        [SerializeField] private FurnaceUI _furnaceUI;
        [SerializeField] private CinderHeartSkillSelectionUI _cinderHeartSkillSelectionUI;

        private bool _isInitialized;

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            CloseHud();
            CloseInventory();
            CloseGameOverPanel();
            CloseCraftingUI();
            CloseFurnaceUI();
            CloseCinderHeartSkillSelectionUI();
            RefreshCursorState();
            _isInitialized = true;
        }

        private void Update()
        {
            if (CinderkeepInput.WasKeyPressedThisFrame(KeyCode.Tab))
            {
                ToggleInventory();
            }
        }

        public void OpenHud()
        {
            SetActive(_hudRoot, true);
        }

        public void CloseHud()
        {
            SetActive(_hudRoot, false);
        }

        public void OpenInventory()
        {
            if (_inventoryUI != null)
            {
                _inventoryUI.Open();
                RefreshCursorState();
                return;
            }

            SetActive(_inventoryRoot, true);
            RefreshCursorState();
        }

        public void CloseInventory()
        {
            if (_inventoryUI != null)
            {
                _inventoryUI.Close();
                RefreshCursorState();

                return;
            }

            SetActive(_inventoryRoot, false);
            RefreshCursorState();
        }

        public void ToggleInventory()
        {
            if (_inventoryUI != null)
            {
                bool wasOpen = _inventoryUI.IsOpen;
                _inventoryUI.Toggle();
                RefreshCursorState();
                PlayUiToggleSfx(wasOpen);

                return;
            }

            if (_inventoryRoot == null)
            {
                return;
            }

            bool wasRootOpen = _inventoryRoot.activeSelf;
            SetActive(_inventoryRoot, wasRootOpen == false);
            RefreshCursorState();
            PlayUiToggleSfx(wasRootOpen);
        }

        public void OpenGameOverPanel()
        {
            SetActive(_gameOverPanel, true);
            PlayUiFailSfx();
        }

        public void CloseGameOverPanel()
        {
            SetActive(_gameOverPanel, false);
        }

        public void OpenCraftingUI(CraftingStation craftingStation, GameObject interactor)
        {
            if (_craftingUI == null)
            {
                return;
            }

            CloseFurnaceUI();
            _craftingUI.OpenStation(craftingStation, interactor);
            RefreshCursorState();
            PlayUiClickSfx();
        }

        public void CloseCraftingUI()
        {
            if (_craftingUI == null)
            {
                return;
            }

            _craftingUI.Close();
            RefreshCursorState();
        }

        public void ToggleCraftingUI(CraftingStation craftingStation, GameObject interactor)
        {
            if (_craftingUI == null)
            {
                return;
            }

            CloseFurnaceUI();
            bool wasOpen = _craftingUI.IsOpen;
            _craftingUI.Toggle(craftingStation, interactor);
            RefreshCursorState();
            PlayUiToggleSfx(wasOpen);
        }

        public void OpenFurnaceUI(FurnaceStation furnaceStation, GameObject interactor)
        {
            if (_furnaceUI == null)
            {
                return;
            }

            CloseCraftingUI();
            _furnaceUI.OpenFurnace(furnaceStation);
            RefreshCursorState();
            PlayUiClickSfx();
        }

        public void CloseFurnaceUI()
        {
            if (_furnaceUI == null)
            {
                return;
            }

            _furnaceUI.Close();
            RefreshCursorState();
        }
        public void OpenCinderHeartSkillSelectionUI(
            System.Collections.Generic.IReadOnlyList<CinderHeartSkillData> skillOptions,
            System.Action onClosed)
        {
            if (_cinderHeartSkillSelectionUI == null)
            {
                if (onClosed != null)
                {
                    onClosed.Invoke();
                }

                return;
            }

            CloseInventory();
            CloseCraftingUI();
            CloseFurnaceUI();
            _cinderHeartSkillSelectionUI.Open(skillOptions, onClosed);
            PlayUiNotificationSfx();
        }

        public void CloseCinderHeartSkillSelectionUI()
        {
            if (_cinderHeartSkillSelectionUI == null)
            {
                return;
            }

            _cinderHeartSkillSelectionUI.Close();
        }

        private void SetActive(GameObject targetObject, bool isActive)
        {
            if (targetObject == null)
            {
                return;
            }

            targetObject.SetActive(isActive);
        }

        private void RefreshCursorState()
        {
            bool anyUIOpen = IsAnyBlockingUIOpen();

            if (anyUIOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private bool IsAnyBlockingUIOpen()
        {
            if (_inventoryUI != null && _inventoryUI.IsOpen)
            {
                return true;
            }

            if (_inventoryUI == null && _inventoryRoot != null && _inventoryRoot.activeSelf)
            {
                return true;
            }

            if (_craftingUI != null && _craftingUI.IsOpen)
            {
                return true;
            }

            if (_furnaceUI != null && _furnaceUI.IsOpen)
            {
                return true;
            }

            return false;
        }

        private void PlayUiToggleSfx(bool wasOpen)
        {
            if (wasOpen)
            {
                PlayUiBackSfx();
                return;
            }

            PlayUiClickSfx();
        }

        private void PlayUiClickSfx()
        {
            SoundManager soundManager = GetSoundManager();
            if (soundManager == null)
            {
                return;
            }

            soundManager.PlayUiClick();
        }

        private void PlayUiBackSfx()
        {
            SoundManager soundManager = GetSoundManager();
            if (soundManager == null)
            {
                return;
            }

            soundManager.PlayUiBack();
        }

        private void PlayUiNotificationSfx()
        {
            SoundManager soundManager = GetSoundManager();
            if (soundManager == null)
            {
                return;
            }

            soundManager.PlayUiNotification();
        }

        private void PlayUiFailSfx()
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
