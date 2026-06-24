using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Cinderkeep.UI
{
    // 메인 메뉴 화면 흐름을 담당하는 컴포넌트입니다.
    // BGM 조절은 MainMenuBgmController가 맡고, 이 클래스는 버튼 화면 전환만 담당합니다.
    public sealed class MainMenuController : MonoBehaviour
    {
        private const string UiClickClipPath = "Cinderkeep/audio/sfx/sfx_ui_click";
        private const string UiBackClipPath = "Cinderkeep/audio/sfx/sfx_ui_back";

        [SerializeField] private string _gameSceneName = "Cinderkeep_Game";
        [FormerlySerializedAs("Button_StartGame")]
        [SerializeField] private Button _startGameButton;
        [FormerlySerializedAs("Button_Settings")]
        [SerializeField] private Button _settingsButton;
        [FormerlySerializedAs("Button_QuitGame")]
        [SerializeField] private Button _quitGameButton;
        [FormerlySerializedAs("GameObject_SettingsPanel")]
        [SerializeField] private GameObject _settingsPanel;
        [FormerlySerializedAs("Button_CloseSettings")]
        [SerializeField] private Button _closeSettingsButton;

        private AudioSource _effectAudioSource;
        private AudioClip _uiClickClip;
        private AudioClip _uiBackClip;
        private bool _hasStarted;

        private void Start()
        {
            InitializeSfx();
            InitializeButtons();
            CloseSettings();
            _hasStarted = true;
        }

        private void OnDestroy()
        {
            ReleaseButtons();
        }

        public void SetReferences(Button buttonStartGame, Button buttonSettings, Button buttonQuitGame, GameObject gameObjectSettingsPanel, Button buttonCloseSettings)
        {
            _startGameButton = buttonStartGame;
            _settingsButton = buttonSettings;
            _quitGameButton = buttonQuitGame;
            _settingsPanel = gameObjectSettingsPanel;
            _closeSettingsButton = buttonCloseSettings;
        }

        public void StartGame()
        {
            if (string.IsNullOrEmpty(_gameSceneName))
            {
                Debug.LogWarning("MainMenuController: game scene name is empty.");
                return;
            }

            PlayUiClickSfx();
            SceneManager.LoadScene(_gameSceneName);
        }

        public void OpenSettings()
        {
            if (_settingsPanel == null)
            {
                return;
            }

            PlayUiClickSfx();
            _settingsPanel.SetActive(true);
        }

        public void CloseSettings()
        {
            if (_settingsPanel == null)
            {
                return;
            }

            if (_hasStarted)
            {
                PlayUiBackSfx();
            }

            _settingsPanel.SetActive(false);
        }

        public void QuitGame()
        {
            PlayUiClickSfx();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void InitializeButtons()
        {
            if (_startGameButton != null)
            {
                _startGameButton.onClick.AddListener(StartGame);
            }

            if (_settingsButton != null)
            {
                _settingsButton.onClick.AddListener(OpenSettings);
            }

            if (_quitGameButton != null)
            {
                _quitGameButton.onClick.AddListener(QuitGame);
            }

            if (_closeSettingsButton != null)
            {
                _closeSettingsButton.onClick.AddListener(CloseSettings);
            }
        }

        private void ReleaseButtons()
        {
            if (_startGameButton != null)
            {
                _startGameButton.onClick.RemoveListener(StartGame);
            }

            if (_settingsButton != null)
            {
                _settingsButton.onClick.RemoveListener(OpenSettings);
            }

            if (_quitGameButton != null)
            {
                _quitGameButton.onClick.RemoveListener(QuitGame);
            }

            if (_closeSettingsButton != null)
            {
                _closeSettingsButton.onClick.RemoveListener(CloseSettings);
            }
        }

        private void InitializeSfx()
        {
            _effectAudioSource = GetComponent<AudioSource>();
            if (_effectAudioSource == null)
            {
                _effectAudioSource = gameObject.AddComponent<AudioSource>();
            }

            _effectAudioSource.playOnAwake = false;
            _effectAudioSource.loop = false;
            _effectAudioSource.spatialBlend = 0f;
            _effectAudioSource.volume = 0.7f;

            _uiClickClip = Resources.Load<AudioClip>(UiClickClipPath);
            _uiBackClip = Resources.Load<AudioClip>(UiBackClipPath);
        }

        private void PlayUiClickSfx()
        {
            PlaySfx(_uiClickClip);
        }

        private void PlayUiBackSfx()
        {
            PlaySfx(_uiBackClip);
        }

        private void PlaySfx(AudioClip audioClip)
        {
            if (_effectAudioSource == null || audioClip == null)
            {
                return;
            }

            _effectAudioSource.PlayOneShot(audioClip);
        }
    }
}
