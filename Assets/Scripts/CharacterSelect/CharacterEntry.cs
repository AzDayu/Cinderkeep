using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace OODong.CharacterSelect
{
    [Serializable]
    public sealed class CharacterEntry
    {
        [SerializeField] private string _id;
        [SerializeField] private string _koreanRole;
        [SerializeField] private string _englishRole;
        [FormerlySerializedAs("_portrait")]
        [SerializeField] private Sprite Sprite_Portrait;
        [SerializeField] private string _ownerName;
        [SerializeField, TextArea(3, 8)] private string _introduction;

        public string Id => _id;
        public string KoreanRole => _koreanRole;
        public string EnglishRole => _englishRole;
        public Sprite Portrait => Sprite_Portrait;
        public string OwnerName => _ownerName;
        public string Introduction => _introduction;
        public bool HasId => !string.IsNullOrWhiteSpace(_id);

        public static CharacterEntry Create(
            string id,
            string koreanRole,
            string englishRole,
            string ownerName = "",
            string introduction = "")
        {
            return new CharacterEntry
            {
                _id = id,
                _koreanRole = koreanRole,
                _englishRole = englishRole,
                _ownerName = ownerName,
                _introduction = introduction
            };
        }

        public void SetPortrait(Sprite portrait)
        {
            Sprite_Portrait = portrait;
        }

        public void SetProfile(string ownerName, string introduction)
        {
            _ownerName = ownerName;
            _introduction = introduction;
        }
    }
}
