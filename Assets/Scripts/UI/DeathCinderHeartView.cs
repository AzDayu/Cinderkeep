using UnityEngine;

// 플레이어 사망 후 CinderHeart를 바라보는 관전 시점을 담당합니다.
// PlayerStatus는 죽음만 판단하고, 카메라 전환은 이 컴포넌트가 맡습니다.
public sealed class DeathCinderHeartView : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _targetCamera;

    [Header("Target")]
    [SerializeField] private Transform _cinderHeartTarget;

    [Header("View")]
    [SerializeField] private Vector3 _viewOffset = new Vector3(0f, 5f, -8f);
    [SerializeField] private float _lookHeight = 1.6f;

    public void ShowCinderHeartView()
    {
        if (_targetCamera == null)
        {
            return;
        }

        if (_cinderHeartTarget == null)
        {
            return;
        }

        Transform cameraTransform = _targetCamera.transform;
        cameraTransform.position = _cinderHeartTarget.position + _viewOffset;
        cameraTransform.LookAt(GetLookPosition());
    }

    public void SetCamera(Camera targetCamera)
    {
        _targetCamera = targetCamera;
    }

    public void SetCinderHeartTarget(Transform cinderHeartTarget)
    {
        _cinderHeartTarget = cinderHeartTarget;
    }

    private Vector3 GetLookPosition()
    {
        Vector3 lookPosition = _cinderHeartTarget.position;
        lookPosition.y += _lookHeight;
        return lookPosition;
    }
}
