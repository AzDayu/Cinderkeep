using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainHub.CharacterSelect
{
    public sealed class MainHub_RememberedCharacterWorkspaceButton : MonoBehaviour
    {
        [SerializeField] private string _fallbackSceneName = "PersonalWorkspaceRoom";

        public void SetFallbackSceneName(string sceneName)
        {
            _fallbackSceneName = sceneName;
        }

        public void LoadRememberedWorkspace()
        {
            string sceneName = MainHub_CharacterSelectionStorage.GetRememberedSceneName();
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                sceneName = _fallbackSceneName;
            }

            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogWarning("MainHub_RememberedCharacterWorkspaceButton: scene name is empty.");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
