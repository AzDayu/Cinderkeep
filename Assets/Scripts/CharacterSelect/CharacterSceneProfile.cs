using UnityEngine;

namespace OODong.CharacterSelect
{
    public sealed class CharacterSceneProfile : MonoBehaviour
    {
        [SerializeField] private string _characterName;
        [SerializeField] private string _roleName;
        [SerializeField] private string _ownerEnglishName;

        public string CharacterName => _characterName;
        public string RoleName => _roleName;
        public string OwnerEnglishName => _ownerEnglishName;

        public void SetProfile(string characterName, string roleName, string ownerEnglishName)
        {
            _characterName = characterName;
            _roleName = roleName;
            _ownerEnglishName = ownerEnglishName;
        }
    }
}
