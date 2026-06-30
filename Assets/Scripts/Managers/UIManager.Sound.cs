namespace Cinderkeep.Gameplay
{
    public sealed partial class UIManager
    {
        // UI 동작음 호출만 담당합니다.
        // 실제 클립 선택과 재생은 SoundManager가 소유합니다.
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
