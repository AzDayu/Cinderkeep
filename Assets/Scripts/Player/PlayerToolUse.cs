using UnityEngine;

// 플레이어가 현재 들고 있는 도구로 자원을 채집하는 컴포넌트입니다.
// PlayerAttack은 적 공격만 담당하고, 도끼/곡괭이 좌클릭 채집은 이 클래스가 담당합니다.
public sealed class PlayerToolUse : MonoBehaviour
{
    [Header("Tool Use Settings")]
    [Tooltip("도구 채집이 닿는 최대 거리입니다.")]
    [SerializeField] private float _toolUseDistance = 2.5f;
    [Tooltip("도구 채집 판정의 두께입니다. 값이 클수록 조금 빗나가도 자원을 맞춥니다.")]
    [SerializeField] private float _toolUseRadius = 0.35f;
    [Tooltip("도구를 한 번 사용한 뒤 다시 사용할 수 있을 때까지 기다리는 시간입니다.")]
    [SerializeField] private float _toolUseInterval = 0.5f;
    [Tooltip("도구 채집 판정에 사용할 레이어입니다.")]
    [SerializeField] private LayerMask _toolUseLayerMask = ~0;

    [Header("Connected Objects")]
    [Tooltip("도구 사용 Ray가 시작되는 위치입니다. 비어 있으면 자식 카메라를 찾아 사용합니다.")]
    [SerializeField] private Transform _toolOrigin;
    [Tooltip("현재 장착한 도구 정보를 제공하는 컴포넌트입니다.")]
    [SerializeField] private PlayerToolController _playerToolController;
    [Tooltip("좌클릭 채집 시 1인칭 도구 휘두르기 연출을 담당합니다.")]
    [SerializeField] private FirstPersonToolView _firstPersonToolView;

    private float _lastToolUseTime;

    private void Start()
    {
        ConnectComponents();
    }

    private void Update()
    {
        ReadToolUseInput();
    }

    public void TryUseTool()
    {
        if (CanUseTool() == false)
        {
            return;
        }

        ResourceNode resourceNode = GetResourceNodeFromCast();
        if (resourceNode == null)
        {
            return;
        }

        if (resourceNode.TryGatherWithTool(gameObject, _playerToolController.CurrentToolType) == false)
        {
            return;
        }

        _lastToolUseTime = Time.time;
        PlayToolUseView();
    }

    private void ConnectComponents()
    {
        if (_playerToolController == null)
        {
            _playerToolController = GetComponent<PlayerToolController>();
        }

        if (_toolOrigin == null)
        {
            Camera camera = GetComponentInChildren<Camera>();
            if (camera != null)
            {
                _toolOrigin = camera.transform;
            }
        }

        if (_firstPersonToolView == null)
        {
            _firstPersonToolView = GetComponentInChildren<FirstPersonToolView>();
        }
    }

    private void ReadToolUseInput()
    {
        if (CinderkeepInput.WasLeftMousePressedThisFrame())
        {
            TryUseTool();
        }
    }

    private bool CanUseTool()
    {
        if (_playerToolController == null)
        {
            return false;
        }

        if (_playerToolController.CurrentToolType == GatherToolType.None)
        {
            return false;
        }

        return Time.time >= _lastToolUseTime + _toolUseInterval;
    }

    private ResourceNode GetResourceNodeFromCast()
    {
        if (_toolOrigin == null)
        {
            return null;
        }

        Ray toolRay = new Ray(_toolOrigin.position, _toolOrigin.forward);
        RaycastHit hitInfo;

        if (Physics.SphereCast(toolRay, _toolUseRadius, out hitInfo, _toolUseDistance, _toolUseLayerMask) == false)
        {
            return null;
        }

        return hitInfo.collider.GetComponentInParent<ResourceNode>();
    }

    private void PlayToolUseView()
    {
        if (_firstPersonToolView == null)
        {
            return;
        }

        _firstPersonToolView.PlaySwing();
    }
}
