using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainHub.CharacterSelect
{
    [Serializable]
    public sealed class MainHub_CharacterEntry
    {
        [SerializeField] private string _id;
        [SerializeField] private string _koreanRole;
        [SerializeField] private string _englishRole;
        [FormerlySerializedAs("_portrait")]
        [SerializeField] private Sprite Sprite_Portrait;
        [SerializeField] private string _ownerName;
        [SerializeField, TextArea(3, 8)] private string _introduction;

        public string Id
        {
            get
            {
                return _id;
            }
        }
        public string KoreanRole
        {
            get
            {
                return _koreanRole;
            }
        }
        public string EnglishRole
        {
            get
            {
                return _englishRole;
            }
        }
        public Sprite Portrait
        {
            get
            {
                return Sprite_Portrait;
            }
        }
        public string OwnerName
        {
            get
            {
                return _ownerName;
            }
        }
        public string Introduction
        {
            get
            {
                return _introduction;
            }
        }
        public bool HasId
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_id);
            }
        }

        public static MainHub_CharacterEntry Create(
            string id,
            string koreanRole,
            string englishRole,
            string ownerName = "",
            string introduction = "")
        {
            return new MainHub_CharacterEntry
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
