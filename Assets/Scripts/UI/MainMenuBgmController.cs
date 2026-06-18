using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Cinderkeep.UI
{
    // 메인 메뉴에서 시작되는 공용 BGM을 담당합니다.
    // 이 오브젝트는 씬 전환 뒤에도 남아서 게임 작업 중에도 BGM 버튼을 사용할 수 있게 합니다.
    public sealed class MainMenuBgmController : MonoBehaviour
    {
        [FormerlySerializedAs("AudioSource_Bgm")]
        [SerializeField] private AudioSource _bgmAudioSource;
        [FormerlySerializedAs("Button_BgmToggle")]
        [SerializeField] private Button _bgmToggleButton;
        [FormerlySerializedAs("Toggle_VolumeMute")]
        [SerializeField] private Toggle _volumeMuteToggle;
        [FormerlySerializedAs("Slider_Volume")]
        [SerializeField] private Slider _volumeSlider;
        [FormerlySerializedAs("Text_BgmToggle")]
        [SerializeField] private Text _bgmToggleText;
        [FormerlySerializedAs("Text_VolumeMute")]
        [SerializeField] private Text _volumeMuteText;
        [FormerlySerializedAs("Text_VolumeValue")]
        [SerializeField] private Text _volumeValueText;
        [FormerlySerializedAs("AudioClips_Bgm")]
        [SerializeField] private AudioClip[] _bgmAudioClips;
        [SerializeField] private float _volume = 0.3f;

        private static MainMenuBgmController _activeController;

        private int _currentClipIndex;
        private bool _isBgmOn = true;
        private bool _isInitialized;

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            UpdateCursorState();
            UpdateBgmLoop();
        }

        private void OnDestroy()
        {
            ReleaseControls();

            if (_activeController == this)
            {
                _activeController = null;
            }
        }

        public void SetReferences(AudioSource audioSourceBgm, Button buttonBgmToggle, Toggle toggleVolumeMute, Slider sliderVolume, Text textBgmToggle, Text textVolumeMute, Text textVolumeValue, AudioClip[] audioClipsBgm, float volume)
        {
            _bgmAudioSource = audioSourceBgm;
            _bgmToggleButton = buttonBgmToggle;
            _volumeMuteToggle = toggleVolumeMute;
            _volumeSlider = sliderVolume;
            _bgmToggleText = textBgmToggle;
            _volumeMuteText = textVolumeMute;
            _volumeValueText = textVolumeValue;
            _bgmAudioClips = audioClipsBgm;
            _volume = volume;
        }

        public void ToggleBgm()
        {
            SetBgmOn(_isBgmOn == false);
        }

        public void SetVolumeMute(bool isMute)
        {
            SetBgmOn(isMute == false);
        }

        public void SetVolumeBySlider(float volumeLevel)
        {
            float safeVolumeLevel = Mathf.Clamp(volumeLevel, 0f, 10f);
            _volume = safeVolumeLevel / 10f;
            ApplyVolume();
            UpdateVolumeText();
        }

        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            if (_activeController != null && _activeController != this)
            {
                Destroy(gameObject);
                return;
            }

            _activeController = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSource();
            InitializeControls();
            PlayFirstBgm();
            UpdateBgmView();

            _isInitialized = true;
        }

        private void InitializeAudioSource()
        {
            if (_bgmAudioSource == null)
            {
                return;
            }

            _bgmAudioSource.playOnAwake = false;
            _bgmAudioSource.loop = false;
            _bgmAudioSource.spatialBlend = 0f;
            ApplyVolume();
        }

        private void InitializeControls()
        {
            if (_bgmToggleButton != null)
            {
                _bgmToggleButton.onClick.AddListener(ToggleBgm);
            }

            if (_volumeMuteToggle != null)
            {
                _volumeMuteToggle.onValueChanged.AddListener(SetVolumeMute);
            }

            if (_volumeSlider != null)
            {
                _volumeSlider.minValue = 0f;
                _volumeSlider.maxValue = 10f;
                _volumeSlider.wholeNumbers = true;
                _volumeSlider.value = Mathf.RoundToInt(Mathf.Clamp01(_volume) * 10f);
                _volumeSlider.onValueChanged.AddListener(SetVolumeBySlider);
            }
        }

        private void ReleaseControls()
        {
            if (_bgmToggleButton != null)
            {
                _bgmToggleButton.onClick.RemoveListener(ToggleBgm);
            }

            if (_volumeMuteToggle != null)
            {
                _volumeMuteToggle.onValueChanged.RemoveListener(SetVolumeMute);
            }

            if (_volumeSlider != null)
            {
                _volumeSlider.onValueChanged.RemoveListener(SetVolumeBySlider);
            }
        }

        private void SetBgmOn(bool isBgmOn)
        {
            _isBgmOn = isBgmOn;

            if (_isBgmOn)
            {
                PlayCurrentBgm();
            }
            else
            {
                StopBgm();
            }

            UpdateBgmView();
        }

        private void PlayFirstBgm()
        {
            if (CheckCanPlayBgm() == false)
            {
                return;
            }

            _currentClipIndex = Random.Range(0, _bgmAudioClips.Length);
            PlayCurrentBgm();
        }

        private void PlayCurrentBgm()
        {
            if (CheckCanPlayBgm() == false)
            {
                return;
            }

            _bgmAudioSource.clip = _bgmAudioClips[_currentClipIndex];
            ApplyVolume();
            _bgmAudioSource.Play();
        }

        private void PlayNextBgm()
        {
            if (CheckCanPlayBgm() == false)
            {
                return;
            }

            _currentClipIndex = SelectNextClipIndex();
            PlayCurrentBgm();
        }

        private void StopBgm()
        {
            if (_bgmAudioSource == null)
            {
                return;
            }

            _bgmAudioSource.Stop();
        }

        private void UpdateBgmLoop()
        {
            if (_isInitialized == false || _isBgmOn == false || _bgmAudioSource == null)
            {
                return;
            }

            if (_bgmAudioSource.isPlaying)
            {
                return;
            }

            PlayNextBgm();
        }

        private void UpdateCursorState()
        {
            if (SceneManager.GetActiveScene().name == "Main_Lobby")
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                return;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                return;
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void UpdateBgmView()
        {
            UpdateButtonText();
            UpdateMuteToggle();
            UpdateVolumeText();
        }

        private void UpdateButtonText()
        {
            if (_bgmToggleText == null)
            {
                return;
            }

            if (_isBgmOn)
            {
                _bgmToggleText.text = "♪ BGM ON";
            }
            else
            {
                _bgmToggleText.text = "♪ BGM OFF";
            }
        }

        private void UpdateMuteToggle()
        {
            if (_volumeMuteToggle != null)
            {
                _volumeMuteToggle.SetIsOnWithoutNotify(_isBgmOn == false);
            }

            if (_volumeMuteText != null)
            {
                _volumeMuteText.text = "볼륨 끄기";
            }
        }

        private void UpdateVolumeText()
        {
            if (_volumeValueText == null)
            {
                return;
            }

            int volumeLevel = Mathf.RoundToInt(Mathf.Clamp01(_volume) * 10f);
            _volumeValueText.text = volumeLevel.ToString() + " / 10";
        }

        private void ApplyVolume()
        {
            if (_bgmAudioSource == null)
            {
                return;
            }

            _bgmAudioSource.volume = Mathf.Clamp01(_volume);
        }

        private int SelectNextClipIndex()
        {
            if (_bgmAudioClips == null || _bgmAudioClips.Length <= 1)
            {
                return 0;
            }

            int nextClipIndex = Random.Range(0, _bgmAudioClips.Length);

            if (nextClipIndex == _currentClipIndex)
            {
                nextClipIndex++;
            }

            if (nextClipIndex >= _bgmAudioClips.Length)
            {
                nextClipIndex = 0;
            }

            return nextClipIndex;
        }

        private bool CheckCanPlayBgm()
        {
            if (_bgmAudioSource == null || _bgmAudioClips == null || _bgmAudioClips.Length == 0)
            {
                return false;
            }

            if (_bgmAudioClips[_currentClipIndex] == null)
            {
                return false;
            }

            return true;
        }
    }
}
