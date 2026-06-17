using UnityEngine;
using UnityEngine.Serialization;

namespace MainHub.CharacterSelect
{
    public sealed class MainHub_MenuCharacterPanel : MonoBehaviour
    {
        [FormerlySerializedAs("_characterScenePanel")]
        [SerializeField] private GameObject GameObject_CharacterScenePanel;

        private void Awake()
        {
            HideCharacterScenePanel();
        }

        public void SetCharacterScenePanel(GameObject characterScenePanel)
        {
            GameObject_CharacterScenePanel = characterScenePanel;
        }

        public void ShowCharacterScenePanel()
        {
            if (GameObject_CharacterScenePanel != null)
            {
                GameObject_CharacterScenePanel.SetActive(true);
            }
        }

        public void ToggleCharacterScenePanel()
        {
            if (GameObject_CharacterScenePanel != null)
            {
                GameObject_CharacterScenePanel.SetActive(!GameObject_CharacterScenePanel.activeSelf);
            }
        }

        public void HideCharacterScenePanel()
        {
            if (GameObject_CharacterScenePanel != null)
            {
                GameObject_CharacterScenePanel.SetActive(false);
            }
        }
    }
}
