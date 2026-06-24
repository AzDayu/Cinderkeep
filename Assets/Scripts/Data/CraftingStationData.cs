using System;
using System.Collections.Generic;
using UnityEngine;

// 5.00 direction: Defines static JSON-backed data for the 5.00 playable loop.
// 5.01+ note: Keep this file behavior-free; add gameplay logic in Systems or Managers and keep new fields data-driven.
namespace Cinderkeep.Gameplay
{
    // crafting_stations.json의 한 줄을 담는 Static Data입니다.
    // 제작대, 용광로, 모루처럼 제작 기능을 여는 시설의 기준값을 관리합니다.
    [Serializable]
    public sealed class CraftingStationData : GameDataBase
    {
        [SerializeField] private string _displayName;
        [SerializeField] private string _stationType;
        [SerializeField] private int _tier;
        [SerializeField] private string _description;
        [SerializeField] private string _prefabKey;
        [SerializeField] private string _uiKey;
        [SerializeField] private bool _canCraftItems;
        [SerializeField] private bool _canSmeltResources;

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        public string StationType
        {
            get
            {
                return _stationType;
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

        public string PrefabKey
        {
            get
            {
                return _prefabKey;
            }
        }

        public string UiKey
        {
            get
            {
                return _uiKey;
            }
        }

        public bool CanCraftItems
        {
            get
            {
                return _canCraftItems;
            }
        }

        public bool CanSmeltResources
        {
            get
            {
                return _canSmeltResources;
            }
        }
    }

    // JsonUtility가 읽기 쉬운 Items 감싸기 구조입니다.
    [Serializable]
    public sealed class CraftingStationDataCatalog
    {
        public List<CraftingStationData> Items = new List<CraftingStationData>();
    }
}
