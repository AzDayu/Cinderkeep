using UnityEngine;
using UnityEngine.UI;

namespace OODong.CharacterSelect
{
    public sealed class DeveloperWorkProgressProfile : MonoBehaviour
    {
        private const string SavePrefix = "OODong.DeveloperWorkProgress.";

        [SerializeField] private string _characterName;
        [SerializeField] private string _roleName;
        [SerializeField] private string _ownerEnglishName;
        [SerializeField] private float _experiencePerSecond = 6f;
        [SerializeField] private int _baseExperienceToLevelUp = 100;
        [SerializeField] private Text _profileTitleText;
        [SerializeField] private Text _levelText;
        [SerializeField] private Text _statusText;
        [SerializeField] private Text _experienceText;
        [SerializeField] private Image _experienceFillImage;

        private int _level = 1;
        private float _experience;
        private float _workTime;

        public int Level => _level;
        public float Experience => _experience;

        private void Awake()
        {
            LoadProgress();
            RefreshView();
        }

        private void Update()
        {
            AddExperience(_experiencePerSecond * Time.deltaTime);
            _workTime += Time.deltaTime;
            RefreshView();
        }

        private void OnDisable()
        {
            SaveProgress();
        }

        public void SetProfile(string characterName, string roleName, string ownerEnglishName)
        {
            _characterName = characterName;
            _roleName = roleName;
            _ownerEnglishName = ownerEnglishName;
        }

        public void SetViewReferences(
            Text profileTitleText,
            Text levelText,
            Text statusText,
            Text experienceText,
            Image experienceFillImage)
        {
            _profileTitleText = profileTitleText;
            _levelText = levelText;
            _statusText = statusText;
            _experienceText = experienceText;
            _experienceFillImage = experienceFillImage;
        }

        private void AddExperience(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            _experience += amount;
            while (_experience >= GetExperienceToLevelUp())
            {
                _experience -= GetExperienceToLevelUp();
                _level++;
            }
        }

        private void RefreshView()
        {
            int requiredExperience = GetExperienceToLevelUp();
            float progress = Mathf.Clamp01(_experience / requiredExperience);

            if (_profileTitleText != null)
            {
                _profileTitleText.text = $"{_roleName} {_ownerEnglishName}";
            }

            if (_levelText != null)
            {
                _levelText.text = $"DEV LEVEL {_level:00}";
            }

            if (_statusText != null)
            {
                _statusText.text = $"작업 경험치 쌓는 중 - {_ownerEnglishName} {GetWorkingDots()}";
            }

            if (_experienceText != null)
            {
                _experienceText.text = $"EXP {Mathf.FloorToInt(_experience):000} / {requiredExperience:000}";
            }

            if (_experienceFillImage != null)
            {
                _experienceFillImage.fillAmount = progress;
            }
        }

        private string GetWorkingDots()
        {
            int dotCount = 1 + (Mathf.FloorToInt(_workTime * 2f) % 3);
            return new string('.', dotCount);
        }

        private int GetExperienceToLevelUp()
        {
            return _baseExperienceToLevelUp + ((_level - 1) * 30);
        }

        private void LoadProgress()
        {
            string key = GetSaveKey();
            _level = Mathf.Max(1, PlayerPrefs.GetInt($"{key}.level", 1));
            _experience = Mathf.Max(0f, PlayerPrefs.GetFloat($"{key}.experience", 0f));
        }

        private void SaveProgress()
        {
            string key = GetSaveKey();
            PlayerPrefs.SetInt($"{key}.level", _level);
            PlayerPrefs.SetFloat($"{key}.experience", _experience);
            PlayerPrefs.Save();
        }

        private string GetSaveKey()
        {
            return $"{SavePrefix}{_characterName}";
        }
    }
}
