using Cinderkeep.Gameplay;
using UnityEngine;
using UnityEngine.Serialization;

// 플레이어의 건축 입력을 담당하는 컴포넌트입니다.
// 실제 건축물 생성은 BuildingManager가 담당하고, 이 클래스는 "어디에 무엇을 지을지"만 요청합니다.
public sealed class PlayerBuild : MonoBehaviour
{
    [Header("Build Input")]
    [Tooltip("건축을 시도하는 입력 키입니다.")]
    [SerializeField] private KeyCode _buildKey = KeyCode.B;

    [Header("Build Target")]
    [FormerlySerializedAs("Prefab_Fence")]
    [FormerlySerializedAs("GameObject_BuildingPrefab")]
    [Tooltip("생성할 건축물 프리팹입니다.")]
    [SerializeField] private GameObject _buildingPrefab;
    [Tooltip("고정 건축 지점입니다. 비어 있으면 플레이어 앞 위치 fallback을 사용합니다.")]
    [SerializeField] private BuildingSpot _targetBuildingSpot;
    [FormerlySerializedAs("SpawnDistance")]
    [Tooltip("고정 건축 지점이 없을 때 플레이어 앞에 건축물을 생성할 거리입니다.")]
    [SerializeField] private float _spawnDistance = 3f;
    [Tooltip("고정 건축 지점이 없을 때 플레이어 앞 위치에 건축할 수 있게 허용합니다.")]
    [SerializeField] private bool _usePositionFallback = true;

    [Header("Connected Manager")]
    [Tooltip("실제 건축물 생성을 맡는 매니저입니다. 비어 있으면 GameManager를 통해 연결합니다.")]
    [SerializeField] private BuildingManager _buildingManager;

    private void Start()
    {
        ConnectBuildingManager();
    }

    private void Update()
    {
        ReadBuildInput();
    }

    public void SetBuildingManager(BuildingManager buildingManager)
    {
        _buildingManager = buildingManager;
    }

    public void SetTargetBuildingSpot(BuildingSpot buildingSpot)
    {
        _targetBuildingSpot = buildingSpot;
    }

    private void ReadBuildInput()
    {
        if (CinderkeepInput.WasKeyPressedThisFrame(_buildKey))
        {
            TryBuild();
        }
    }

    private void TryBuild()
    {
        if (CanRequestBuild() == false)
        {
            return;
        }

        if (_targetBuildingSpot != null)
        {
            TryBuildAtSpot();
            return;
        }

        TryBuildAtFallbackPosition();
    }

    private bool CanRequestBuild()
    {
        ConnectBuildingManager();

        if (_buildingPrefab == null)
        {
            Debug.LogWarning("PlayerBuild: 건축 프리팹이 비어 있습니다.");
            return false;
        }

        if (_buildingManager == null)
        {
            Debug.LogWarning("PlayerBuild: BuildingManager 연결이 필요합니다.");
            return false;
        }

        return true;
    }

    private void TryBuildAtSpot()
    {
        bool isBuilt = _buildingManager.TryBuildAtSpot(_targetBuildingSpot, _buildingPrefab);
        if (isBuilt == true)
        {
            Debug.Log("PlayerBuild: 고정 건축 지점에 건축물을 설치했습니다.");
        }
    }

    private void TryBuildAtFallbackPosition()
    {
        if (_usePositionFallback == false)
        {
            Debug.LogWarning("PlayerBuild: 고정 건축 지점이 필요합니다.");
            return;
        }

        Vector3 buildPosition = transform.position + transform.forward * _spawnDistance;
        Quaternion buildRotation = transform.rotation;
        bool isBuilt = _buildingManager.TryBuildAtPosition(_buildingPrefab, buildPosition, buildRotation);
        if (isBuilt == true)
        {
            Debug.Log("PlayerBuild: 플레이어 앞 위치에 건축물을 설치했습니다.");
        }
    }

    private void ConnectBuildingManager()
    {
        if (_buildingManager != null)
        {
            return;
        }

        if (GameManager.Inst == null)
        {
            return;
        }

        _buildingManager = GameManager.Inst.GetBuildingManager();
    }
}
