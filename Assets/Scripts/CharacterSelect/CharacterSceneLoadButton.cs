using UnityEngine;
using UnityEngine.SceneManagement;

namespace OODong.CharacterSelect
{
    public sealed class CharacterSceneLoadButton : MonoBehaviour
    {
        [SerializeField] private string _sceneName;

        public string SceneName => _sceneName;

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
                Debug.LogWarning($"{nameof(CharacterSceneLoadButton)}: scene name is empty.");
                return;
            }

            SceneManager.LoadScene(_sceneName);
        }
    }
}
