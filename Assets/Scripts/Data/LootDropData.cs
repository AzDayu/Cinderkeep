using System;
using System.Collections.Generic;
using UnityEngine;

// 5.00 direction: Defines static JSON-backed data for the 5.00 playable loop.
// 5.01+ note: Keep this file behavior-free; add gameplay logic in Systems or Managers and keep new fields data-driven.
namespace Cinderkeep.Gameplay
{
    // LootDrop JSON의 한 줄을 담는 데이터 클래스입니다.
    // 적 처치, 보스 처치, 오브젝트 파괴 보상을 같은 형식으로 관리합니다.
    [Serializable]
    public sealed class LootDropData : GameDataBase
    {
        [SerializeField] private string _ownerId;
        [SerializeField] private string _dropResourceId;
        [SerializeField] private int _minAmount;
        [SerializeField] private int _maxAmount;
        [SerializeField] private float _dropChance;
        [SerializeField] private string _dropPrefabKey;

        public string OwnerId
        {
            get
            {
                return _ownerId;
            }
        }

        public string DropResourceId
        {
            get
            {
                return _dropResourceId;
            }
        }

        public int MinAmount
        {
            get
            {
                return _minAmount;
            }
        }

        public int MaxAmount
        {
            get
            {
                return _maxAmount;
            }
        }

        public float DropChance
        {
            get
            {
                return _dropChance;
            }
        }

        public string DropPrefabKey
        {
            get
            {
                return _dropPrefabKey;
            }
        }
    }

    // LootDropData도 JsonUtility가 읽기 쉬운 Items 감싸기 구조를 사용합니다.
    [Serializable]
    public sealed class LootDropDataCatalog
    {
        public List<LootDropData> Items = new List<LootDropData>();
    }
}
