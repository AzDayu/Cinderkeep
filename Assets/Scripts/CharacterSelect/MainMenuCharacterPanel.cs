using UnityEngine;

namespace OODong.CharacterSelect
{
    public sealed class MainMenuCharacterPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _characterScenePanel;

        private void Awake()
        {
            HideCharacterScenePanel();
        }

        public void SetCharacterScenePanel(GameObject characterScenePanel)
        {
            _characterScenePanel = characterScenePanel;
        }

        public void ShowCharacterScenePanel()
        {
            if (_characterScenePanel != null)
            {
                _characterScenePanel.SetActive(true);
            }
        }

        public void ToggleCharacterScenePanel()
        {
            if (_characterScenePanel != null)
            {
                _characterScenePanel.SetActive(!_characterScenePanel.activeSelf);
            }
        }

        public void HideCharacterScenePanel()
        {
            if (_characterScenePanel != null)
            {
                _characterScenePanel.SetActive(false);
            }
        }
    }
}
