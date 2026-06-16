using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainHub.CharacterSelect
{
    public sealed class MainHub_SceneLoadButton : MonoBehaviour
    {
        [SerializeField] private string _sceneName;

        public string SceneName
        {
            get
            {
                return _sceneName;
            }
        }

        public void SetSceneName(string sceneName)
        {
            _sceneName = sceneName;
        }

        public void SetEditorAssetPathsToOpen(params string[] assetPaths)
        {
            // Scene load buttons must never open scripts or editor assets.
        }

        public void LoadScene()
        {
            if (string.IsNullOrWhiteSpace(_sceneName))
            {
                Debug.LogWarning($"{nameof(MainHub_SceneLoadButton)}: scene name is empty.");
                return;
            }

            SceneManager.LoadScene(_sceneName);
        }
    }
}
