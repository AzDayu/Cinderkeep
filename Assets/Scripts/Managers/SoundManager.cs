using UnityEngine;

using UnityEngine.Serialization;

namespace Cinderkeep.Gameplay
{
    // 게임 씬 전용 사운드 매니저입니다.
    // 현재는 게임 루프의 BGM과 전투/채집 효과음 재생 기반만 준비합니다.
    // BGM과 효과음 AudioSource는 Inspector에서 연결하고, 다른 스크립트는 이 매니저 함수만 호출합니다.
    public sealed class SoundManager : MonoBehaviour, IGameInitializable
    {
        private const string SfxRootPath = "Cinderkeep/audio/sfx/";

        private const string UiClickClipPath = SfxRootPath + "sfx_ui_click";
        private const string UiBackClipPath = SfxRootPath + "sfx_ui_back";
        private const string UiNotificationClipPath = SfxRootPath + "sfx_ui_notification";
        private const string UiSuccessClipPath = SfxRootPath + "sfx_ui_success";
        private const string UiFailClipPath = SfxRootPath + "sfx_ui_fail";
        private const string RewardSelectClipPath = SfxRootPath + "sfx_reward_select";
        private const string HealClipPath = SfxRootPath + "sfx_heal";
        private const string ResourcePickupClipPath = SfxRootPath + "sfx_resource_pickup";
        private const string ResourceOreHitClipPath = SfxRootPath + "sfx_resource_ore_hit";

        [SerializeField] private AudioSource _bgmAudioSource;
        [SerializeField] private AudioSource _effectAudioSource;
        [SerializeField] private float _defaultVolume = 0.7f;

        private AudioClip _uiClickClip;
        private AudioClip _uiBackClip;
        private AudioClip _uiNotificationClip;
        private AudioClip _uiSuccessClip;
        private AudioClip _uiFailClip;
        private AudioClip _rewardSelectClip;
        private AudioClip _healClip;
        private AudioClip _resourcePickupClip;
        private AudioClip _resourceOreHitClip;

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

            SetVolume(_defaultVolume);
            LoadDefaultSfxClips();
            _isInitialized = true;
        }

        public void PlayBgm(AudioClip bgmClip)
        {
            if (_bgmAudioSource == null || bgmClip == null)
            {
                return;
            }

            _bgmAudioSource.clip = bgmClip;
            _bgmAudioSource.loop = true;
            _bgmAudioSource.Play();
        }

        public void StopBgm()
        {
            if (_bgmAudioSource == null)
            {
                return;
            }

            _bgmAudioSource.Stop();
        }

        public void PlayEffect(AudioClip effectClip)
        {
            if (_effectAudioSource == null || effectClip == null)
            {
                return;
            }

            _effectAudioSource.PlayOneShot(effectClip);
        }

        public void PlayUiClick()
        {
            PlayEffect(_uiClickClip);
        }

        public void PlayUiBack()
        {
            PlayEffect(_uiBackClip);
        }

        public void PlayUiNotification()
        {
            PlayEffect(_uiNotificationClip);
        }

        public void PlayUiSuccess()
        {
            PlayEffect(_uiSuccessClip);
        }

        public void PlayUiFail()
        {
            PlayEffect(_uiFailClip);
        }

        public void PlayCraftSuccess()
        {
            PlayUiSuccess();
        }

        public void PlayCraftFail()
        {
            PlayUiFail();
        }

        public void PlayRewardSelect()
        {
            PlayEffect(_rewardSelectClip);
        }

        public void PlayHeal()
        {
            PlayEffect(_healClip);
        }

        public void PlayResourcePickup()
        {
            PlayEffect(_resourcePickupClip);
        }

        public void PlayResourceGather(string resourceId)
        {
            if (IsOreLikeResource(resourceId))
            {
                PlayEffect(_resourceOreHitClip);
                return;
            }

            PlayEffect(_resourcePickupClip);
        }

        public void SetVolume(float volume)
        {
            float safeVolume = Mathf.Clamp01(volume);

            if (_bgmAudioSource != null)
            {
                _bgmAudioSource.volume = safeVolume;
            }

            if (_effectAudioSource != null)
            {
                _effectAudioSource.volume = safeVolume;
            }
        }

        private void LoadDefaultSfxClips()
        {
            _uiClickClip = LoadSfxClip(UiClickClipPath);
            _uiBackClip = LoadSfxClip(UiBackClipPath);
            _uiNotificationClip = LoadSfxClip(UiNotificationClipPath);
            _uiSuccessClip = LoadSfxClip(UiSuccessClipPath);
            _uiFailClip = LoadSfxClip(UiFailClipPath);
            _rewardSelectClip = LoadSfxClip(RewardSelectClipPath);
            _healClip = LoadSfxClip(HealClipPath);
            _resourcePickupClip = LoadSfxClip(ResourcePickupClipPath);
            _resourceOreHitClip = LoadSfxClip(ResourceOreHitClipPath);
        }

        private AudioClip LoadSfxClip(string resourcePath)
        {
            AudioClip audioClip = Resources.Load<AudioClip>(resourcePath);
            if (audioClip == null)
            {
                Debug.LogWarning("SoundManager: SFX clip was not found. path=" + resourcePath);
            }

            return audioClip;
        }

        private bool IsOreLikeResource(string resourceId)
        {
            if (string.IsNullOrEmpty(resourceId))
            {
                return false;
            }

            return resourceId == PlayerModel.ResourceStone
                || resourceId == PlayerModel.ResourceIron
                || resourceId == PlayerModel.ResourceIronOre
                || resourceId == PlayerModel.ResourceGold
                || resourceId == PlayerModel.ResourceGoldOre
                || resourceId == PlayerModel.ResourceAdamantium
                || resourceId == PlayerModel.ResourceAdamantiumOre;
        }
    }
}
