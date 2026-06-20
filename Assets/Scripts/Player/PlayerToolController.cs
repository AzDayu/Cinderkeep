using Cinderkeep.Gameplay;
using UnityEngine;

// 플레이어가 현재 들고 있는 도구를 관리하는 컴포넌트입니다.
// 현재 단계에서는 1번 도끼, 2번 곡괭이, 3번 맨손으로 연결합니다.
// 도구의 세부 수치는 tools.json에서 가져오고, 데이터가 없으면 기존 타입만 사용합니다.
public sealed class PlayerToolController : MonoBehaviour
{
    [Header("Tool Keys")]
    [Tooltip("도끼를 장착하는 입력 키입니다.")]
    [SerializeField] private KeyCode _axeKey = KeyCode.Alpha1;
    [Tooltip("곡괭이를 장착하는 입력 키입니다.")]
    [SerializeField] private KeyCode _pickaxeKey = KeyCode.Alpha2;
    [Tooltip("맨손으로 전환하는 입력 키입니다.")]
    [SerializeField] private KeyCode _emptyHandKey = KeyCode.Alpha3;

    [Header("Current Tool")]
    [Tooltip("현재 플레이어가 장착한 도구입니다.")]
    [SerializeField] private GatherToolType _currentToolType = GatherToolType.Axe;

    [Header("Tool Data")]
    [Tooltip("true이면 tools.json의 도구 데이터를 현재 장착 도구 정보로 사용합니다.")]
    [SerializeField] private bool _useToolData = true;
    [Tooltip("도끼 장착 시 사용할 tools.json의 _id입니다.")]
    [SerializeField] private string _axeToolDataId = "stone_axe";
    [Tooltip("곡괭이 장착 시 사용할 tools.json의 _id입니다.")]
    [SerializeField] private string _pickaxeToolDataId = "stone_pickaxe";
    [Tooltip("맨손 장착 시 사용할 tools.json의 _id입니다.")]
    [SerializeField] private string _emptyHandToolDataId = "hand";
    [Tooltip("현재 장착 중인 tools.json의 _id입니다. 플레이 중 디버깅용으로 확인합니다.")]
    [SerializeField] private string _currentToolDataId = "stone_axe";

    public GatherToolType CurrentToolType
    {
        get
        {
            return _currentToolType;
        }
    }

    public string CurrentToolDataId
    {
        get
        {
            return _currentToolDataId;
        }
    }

    public int CurrentToolTier
    {
        get
        {
            ToolData toolData = GetCurrentToolData();
            if (toolData == null)
            {
                return GetFallbackToolTier();
            }

            return toolData.Tier;
        }
    }

    private void Start()
    {
        SetCurrentToolDataIdByType(_currentToolType);
    }

    private void Update()
    {
        ReadToolInput();
    }

    public void EquipTool(GatherToolType toolType)
    {
        _currentToolType = toolType;
        SetCurrentToolDataIdByType(toolType);
    }

    public bool HasRequiredTool(GatherToolType requiredToolType)
    {
        if (requiredToolType == GatherToolType.None)
        {
            return true;
        }

        return _currentToolType == requiredToolType;
    }

    public ToolData GetCurrentToolData()
    {
        if (_useToolData == false)
        {
            return null;
        }

        if (GameManager.Inst == null)
        {
            return null;
        }

        GameDataManager gameDataManager = GameManager.Inst.GetGameDataManager();
        if (gameDataManager == null)
        {
            return null;
        }

        return gameDataManager.GetTool(_currentToolDataId);
    }

    private void ReadToolInput()
    {
        if (CinderkeepInput.WasKeyPressedThisFrame(_axeKey))
        {
            EquipTool(GatherToolType.Axe);
            return;
        }

        if (CinderkeepInput.WasKeyPressedThisFrame(_pickaxeKey))
        {
            EquipTool(GatherToolType.Pickaxe);
            return;
        }

        if (CinderkeepInput.WasKeyPressedThisFrame(_emptyHandKey))
        {
            EquipTool(GatherToolType.None);
        }
    }

    private void SetCurrentToolDataIdByType(GatherToolType toolType)
    {
        if (toolType == GatherToolType.Axe)
        {
            _currentToolDataId = _axeToolDataId;
            return;
        }

        if (toolType == GatherToolType.Pickaxe)
        {
            _currentToolDataId = _pickaxeToolDataId;
            return;
        }

        _currentToolDataId = _emptyHandToolDataId;
    }

    private int GetFallbackToolTier()
    {
        if (_currentToolType == GatherToolType.None)
        {
            return 0;
        }

        return 1;
    }
}
