using System;
using System.Collections.Generic;
using UnityEngine;

// 5.00 direction: Defines static JSON-backed data for the 5.00 playable loop.
// 5.01+ note: Keep this file behavior-free; add gameplay logic in Systems or Managers and keep new fields data-driven.
namespace Cinderkeep.Gameplay
{
    // Armor JSON의 한 줄을 담는 데이터 클래스입니다.
    // 헬멧, 갑옷, 장화는 같은 방어 장비이므로 ArmorSlot 값으로 구분합니다.
    [Serializable]
    public sealed class ArmorData : GameDataBase
    {
        [SerializeField] private string _displayName;
        [SerializeField] private string _armorSlot;
        [SerializeField] private int _tier;
        [SerializeField] private string _description;
        [SerializeField] private float _defense;
        [SerializeField] private float _maxHealthBonus;
        [SerializeField] private float _staminaBonus;
        [SerializeField] private float _moveSpeedBonus;
        [SerializeField] private string _prefabKey;
        [SerializeField] private string _craftingRecipeId;

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        public string ArmorSlot
        {
            get
            {
                return _armorSlot;
            }
        }

        public int Tier
        {
            get
            {
                return _tier;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public float Defense
        {
            get
            {
                return _defense;
            }
        }

        public float MaxHealthBonus
        {
            get
            {
                return _maxHealthBonus;
            }
        }

        public float StaminaBonus
        {
            get
            {
                return _staminaBonus;
            }
        }

        public float MoveSpeedBonus
        {
            get
            {
                return _moveSpeedBonus;
            }
        }

        public string PrefabKey
        {
            get
            {
                return _prefabKey;
            }
        }

        public string CraftingRecipeId
        {
            get
            {
                return _craftingRecipeId;
            }
        }
    }

    // ArmorData도 JsonUtility가 읽기 쉬운 Items 감싸기 구조를 사용합니다.
    [Serializable]
    public sealed class ArmorDataCatalog
    {
        public List<ArmorData> Items = new List<ArmorData>();
    }
}
