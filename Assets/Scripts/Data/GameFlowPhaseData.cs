using System;
using System.Collections.Generic;
using UnityEngine;

// 5.00 direction: Defines static JSON-backed data for the 5.00 playable loop.
// 5.01+ note: Keep this file behavior-free; add gameplay logic in Systems or Managers and keep new fields data-driven.
namespace Cinderkeep.Gameplay
{
    // GameFlowPhase JSON 한 줄을 읽는 데이터 클래스입니다.
    // 낮, 밤, 아침 보상, 보스 접근 같은 페이즈 시간을 데이터로 관리합니다.
    [Serializable]
    public sealed class GameFlowPhaseData : GameDataBase
    {
        [SerializeField] private int _day;
        [SerializeField] private string _phase;
        [SerializeField] private float _durationSeconds;
        [SerializeField] private string _nextPhaseId;
        [SerializeField] private string _bgmKey;
        [SerializeField] private string _skyboxKey;
        [SerializeField] private string _lightPresetKey;

        public int Day
        {
            get
            {
                return _day;
            }
        }

        public string Phase
        {
            get
            {
                return _phase;
            }
        }

        public float DurationSeconds
        {
            get
            {
                return _durationSeconds;
            }
        }

        public string NextPhaseId
        {
            get
            {
                return _nextPhaseId;
            }
        }

        public string BgmKey
        {
            get
            {
                return _bgmKey;
            }
        }

        public string SkyboxKey
        {
            get
            {
                return _skyboxKey;
            }
        }

        public string LightPresetKey
        {
            get
            {
                return _lightPresetKey;
            }
        }
    }

    // JsonUtility가 읽기 쉬운 Items 감싸기 구조를 사용합니다.
    [Serializable]
    public sealed class GameFlowPhaseDataCatalog
    {
        public List<GameFlowPhaseData> Items = new List<GameFlowPhaseData>();
    }
}
