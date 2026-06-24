using UnityEngine;

// GameFlowController가 사용하는 시간 fallback 설정입니다.
// game_flow_phases.json 데이터가 없거나 잘못되면 이 Inspector 값을 사용합니다.
[System.Serializable]
public sealed class GameFlowSettings
{
    [Tooltip("낮 페이즈 지속 시간입니다. 180이면 3분입니다.")]
    [SerializeField] private float _dayDuration = 180f;

    [Tooltip("밤 페이즈 지속 시간입니다. 120이면 2분입니다.")]
    [SerializeField] private float _nightDuration = 120f;

    [Tooltip("밤이 끝난 뒤 다음 날로 넘어가기 전에 보여줄 보상 시간입니다.")]
    [SerializeField] private float _morningRewardDuration = 30f;

    [Tooltip("마지막 밤 이후 보스가 접근하는 페이즈의 지속 시간입니다.")]
    [SerializeField] private float _bossApproachDuration = 180f;

    public float DayDuration
    {
        get
        {
            return _dayDuration;
        }
    }

    public float NightDuration
    {
        get
        {
            return _nightDuration;
        }
    }

    public float MorningRewardDuration
    {
        get
        {
            return _morningRewardDuration;
        }
    }

    public float BossApproachDuration
    {
        get
        {
            return _bossApproachDuration;
        }
    }

    public void ClampValues()
    {
        if (_dayDuration < 1f)
        {
            _dayDuration = 1f;
        }

        if (_nightDuration < 1f)
        {
            _nightDuration = 1f;
        }

        if (_morningRewardDuration < 1f)
        {
            _morningRewardDuration = 1f;
        }

        if (_bossApproachDuration < 1f)
        {
            _bossApproachDuration = 1f;
        }
    }
}
