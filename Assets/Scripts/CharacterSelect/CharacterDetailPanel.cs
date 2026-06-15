using UnityEngine;
using UnityEngine.UI;

namespace OODong.CharacterSelect
{
    public sealed class CharacterDetailPanel : MonoBehaviour
    {
        private const string SavePrefix = "OODong.CharacterSelect.";

        [SerializeField] private Text _detailTitle;
        [SerializeField] private Text _detailRole;
        [SerializeField] private InputField _ownerInput;
        [SerializeField] private InputField _introInput;
        [SerializeField] private CharacterSceneLoadButton _personalSpaceButton;
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
            _detailTitle = detailTitle;
            _detailRole = detailRole;
            _ownerInput = ownerInput;
            _introInput = introInput;
            _personalSpaceButton = personalSpaceButton;
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

            if (_detailTitle != null)
            {
                _detailTitle.text = $"Member: {savedName}";
            }

            if (_detailRole != null)
            {
                _detailRole.text = $"Class Badge: {entry.EnglishRole}";
            }

            if (_ownerInput != null)
            {
                _ownerInput.text = savedName;
            }

            if (_introInput != null)
            {
                _introInput.text = savedIntro;
            }

            if (_personalSpaceButton != null)
            {
                _personalSpaceButton.SetSceneName(GetPersonalSceneName(entry));
                _personalSpaceButton.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            _selectedEntry = null;
            gameObject.SetActive(false);
        }

        private void RegisterInputEvents()
        {
            if (_ownerInput != null)
            {
                _ownerInput.onEndEdit.AddListener(HandleEditEnded);
            }

            if (_introInput != null)
            {
                _introInput.onEndEdit.AddListener(HandleEditEnded);
            }
        }

        private void UnregisterInputEvents()
        {
            if (_ownerInput != null)
            {
                _ownerInput.onEndEdit.RemoveListener(HandleEditEnded);
            }

            if (_introInput != null)
            {
                _introInput.onEndEdit.RemoveListener(HandleEditEnded);
            }
        }

        private void HandleEditEnded(string value)
        {
            SaveSelectedEntry();
        }

        private void SaveSelectedEntry()
        {
            if (!_saveIntroductionsToPlayerPrefs || _selectedEntry == null || _ownerInput == null || _introInput == null)
            {
                return;
            }

            PlayerPrefs.SetString(GetSaveKey(_selectedEntry, "name"), _ownerInput.text);
            PlayerPrefs.SetString(GetSaveKey(_selectedEntry, "intro"), _introInput.text);
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
