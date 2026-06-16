using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OODong.CharacterSelect
{
    public sealed class CharacterDetailPanel : MonoBehaviour
    {
        private const string SavePrefix = "OODong.CharacterSelect.";

        [FormerlySerializedAs("_detailTitle")]
        [SerializeField] private Text Text_DetailTitle;
        [FormerlySerializedAs("_detailRole")]
        [SerializeField] private Text Text_DetailRole;
        [FormerlySerializedAs("_ownerInput")]
        [SerializeField] private InputField InputField_Owner;
        [FormerlySerializedAs("_introInput")]
        [SerializeField] private InputField InputField_Intro;
        [FormerlySerializedAs("_personalSpaceButton")]
        [SerializeField] private CharacterSceneLoadButton CharacterSceneLoadButton_PersonalSpace;
        [SerializeField] private bool _saveIntroductionsToPlayerPrefs = true;

        private CharacterEntry _selectedEntry;

        public bool IsOpen => gameObject.activeSelf;

        private void OnEnable()
        {
            RegisterInputEvents();
        }

        private void OnDisable()
        {
            UnregisterInputEvents();
        }

        public void SetViewReferences(
            Text detailTitle,
            Text detailRole,
            InputField ownerInput,
            InputField introInput,
            CharacterSceneLoadButton personalSpaceButton)
        {
            Text_DetailTitle = detailTitle;
            Text_DetailRole = detailRole;
            InputField_Owner = ownerInput;
            InputField_Intro = introInput;
            CharacterSceneLoadButton_PersonalSpace = personalSpaceButton;
        }

        public void Show(CharacterEntry entry)
        {
            if (entry == null)
            {
                Hide();
                return;
            }

            _selectedEntry = entry;
            gameObject.SetActive(true);

            string savedName = ReadSavedValue(entry, "name", entry.OwnerName);
            string savedIntro = ReadSavedValue(entry, "intro", entry.Introduction);

            if (Text_DetailTitle != null)
            {
                Text_DetailTitle.text = $"Member: {savedName}";
            }

            if (Text_DetailRole != null)
            {
                Text_DetailRole.text = $"Class Badge: {entry.EnglishRole}";
            }

            if (InputField_Owner != null)
            {
                InputField_Owner.text = savedName;
            }

            if (InputField_Intro != null)
            {
                InputField_Intro.text = savedIntro;
            }

            if (CharacterSceneLoadButton_PersonalSpace != null)
            {
                CharacterSceneLoadButton_PersonalSpace.SetSceneName(GetPersonalSceneName(entry));
                CharacterSceneLoadButton_PersonalSpace.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            _selectedEntry = null;
            gameObject.SetActive(false);
        }

        private void RegisterInputEvents()
        {
            if (InputField_Owner != null)
            {
                InputField_Owner.onEndEdit.AddListener(HandleEditEnded);
            }

            if (InputField_Intro != null)
            {
                InputField_Intro.onEndEdit.AddListener(HandleEditEnded);
            }
        }

        private void UnregisterInputEvents()
        {
            if (InputField_Owner != null)
            {
                InputField_Owner.onEndEdit.RemoveListener(HandleEditEnded);
            }

            if (InputField_Intro != null)
            {
                InputField_Intro.onEndEdit.RemoveListener(HandleEditEnded);
            }
        }

        private void HandleEditEnded(string value)
        {
            SaveSelectedEntry();
        }

        private void SaveSelectedEntry()
        {
            if (!_saveIntroductionsToPlayerPrefs || _selectedEntry == null || InputField_Owner == null || InputField_Intro == null)
            {
                return;
            }

            PlayerPrefs.SetString(GetSaveKey(_selectedEntry, "name"), InputField_Owner.text);
            PlayerPrefs.SetString(GetSaveKey(_selectedEntry, "intro"), InputField_Intro.text);
            PlayerPrefs.Save();
        }

        private string ReadSavedValue(CharacterEntry entry, string field, string fallback)
        {
            if (!_saveIntroductionsToPlayerPrefs)
            {
                return fallback;
            }

            string savedValue = PlayerPrefs.GetString(GetSaveKey(entry, field), fallback);
            return string.IsNullOrWhiteSpace(savedValue) ? fallback : savedValue;
        }

        private static string GetSaveKey(CharacterEntry entry, string field)
        {
            return $"{SavePrefix}{entry.Id}.{field}";
        }

        private static string GetPersonalSceneName(CharacterEntry entry)
        {
            return $"{entry.EnglishRole}_{entry.OwnerName}";
        }
    }
}
