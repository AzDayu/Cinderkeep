using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MainHub.CharacterSelect
{
    public sealed class MainHub_CharacterDetailPanel : MonoBehaviour
    {
        private const string SavePrefix = "MainHub.CharacterSelect.";

        [FormerlySerializedAs("_detailTitle")]
        [SerializeField] private Text Text_DetailTitle;
        [FormerlySerializedAs("_detailRole")]
        [SerializeField] private Text Text_DetailRole;
        [FormerlySerializedAs("_ownerInput")]
        [SerializeField] private InputField InputField_Owner;
        [FormerlySerializedAs("_introInput")]
        [SerializeField] private InputField InputField_Intro;
        [FormerlySerializedAs("_personalSpaceButton")]
        [SerializeField] private MainHub_SceneLoadButton CharacterSceneLoadButton_PersonalSpace;
        [SerializeField] private Button Button_RememberCharacter;
        [SerializeField] private Button Button_CancelRememberedCharacter;
        [SerializeField] private Text Text_RememberedStatus;
        [SerializeField] private bool _saveIntroductionsToPlayerPrefs = true;

        private MainHub_CharacterEntry _selectedEntry;

        public bool IsOpen
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

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
            MainHub_SceneLoadButton personalSpaceButton,
            Button rememberCharacterButton,
            Button cancelRememberedCharacterButton,
            Text rememberedStatus)
        {
            Text_DetailTitle = detailTitle;
            Text_DetailRole = detailRole;
            InputField_Owner = ownerInput;
            InputField_Intro = introInput;
            CharacterSceneLoadButton_PersonalSpace = personalSpaceButton;
            Button_RememberCharacter = rememberCharacterButton;
            Button_CancelRememberedCharacter = cancelRememberedCharacterButton;
            Text_RememberedStatus = rememberedStatus;
        }

        public void Show(MainHub_CharacterEntry entry)
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

            RememberSelectedCharacter();
            RefreshRememberedStatus();
        }

        public void Hide()
        {
            _selectedEntry = null;
            gameObject.SetActive(false);
        }

        public void RememberSelectedCharacter()
        {
            if (_selectedEntry == null)
            {
                return;
            }

            MainHub_CharacterSelectionStorage.SaveRememberedCharacter(_selectedEntry, GetPersonalSceneName(_selectedEntry));
            RefreshRememberedStatus();
        }

        public void CancelRememberedCharacter()
        {
            MainHub_CharacterSelectionStorage.ClearRememberedCharacter();
            RefreshRememberedStatus();
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

            if (Button_RememberCharacter != null)
            {
                Button_RememberCharacter.onClick.AddListener(RememberSelectedCharacter);
            }

            if (Button_CancelRememberedCharacter != null)
            {
                Button_CancelRememberedCharacter.onClick.AddListener(CancelRememberedCharacter);
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

            if (Button_RememberCharacter != null)
            {
                Button_RememberCharacter.onClick.RemoveListener(RememberSelectedCharacter);
            }

            if (Button_CancelRememberedCharacter != null)
            {
                Button_CancelRememberedCharacter.onClick.RemoveListener(CancelRememberedCharacter);
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

        private string ReadSavedValue(MainHub_CharacterEntry entry, string field, string fallback)
        {
            if (!_saveIntroductionsToPlayerPrefs)
            {
                return fallback;
            }

            string savedValue = PlayerPrefs.GetString(GetSaveKey(entry, field), fallback);
            return string.IsNullOrWhiteSpace(savedValue) ? fallback : savedValue;
        }

        private static string GetSaveKey(MainHub_CharacterEntry entry, string field)
        {
            return $"{SavePrefix}{entry.Id}.{field}";
        }

        private static string GetPersonalSceneName(MainHub_CharacterEntry entry)
        {
            return $"{entry.EnglishRole}_{entry.OwnerName}";
        }

        private void RefreshRememberedStatus()
        {
            if (Text_RememberedStatus == null)
            {
                return;
            }

            if (!MainHub_CharacterSelectionStorage.HasRememberedCharacter())
            {
                Text_RememberedStatus.text = "기억된 캐릭터: 없음";
                return;
            }

            Text_RememberedStatus.text = "기억된 캐릭터: "
                + MainHub_CharacterSelectionStorage.GetRememberedRoleName()
                + " / "
                + MainHub_CharacterSelectionStorage.GetRememberedOwnerName();
        }
    }
}
