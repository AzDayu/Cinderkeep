using UnityEngine;

using UnityEngine.Serialization;

namespace Cinderkeep.Gameplay
{
    // 게임 씬 전용 사운드 매니저입니다.
    // 현재는 게임 루프의 BGM과 전투/채집 효과음 재생 기반만 준비합니다.
    // BGM과 효과음 AudioSource는 Inspector에서 연결하고, 다른 스크립트는 이 매니저 함수만 호출합니다.
    public sealed class SoundManager : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private AudioSource _bgmAudioSource;
        [SerializeField] private AudioSource _effectAudioSource;
        [SerializeField] private float _defaultVolume = 0.3f;

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
    }
}
