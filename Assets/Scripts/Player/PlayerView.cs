using UnityEngine;
using UnityEngine.Serialization;

public sealed class PlayerView : MonoBehaviour
{
    [FormerlySerializedAs("mouseSensitivity")]
    [SerializeField] private float _mouseSensitivity = 260f;
    [FormerlySerializedAs("playerBody")]
    [FormerlySerializedAs("Transform_PlayerBody")]
    [SerializeField] private Transform _playerBody;
    [FormerlySerializedAs("Transform_Camera")]
    [SerializeField] private Transform _cameraTransform;

    private float _xRotation;

    private void Start()
    {
        ResolveReferences();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        RotateView();
    }

    private void ResolveReferences()
    {
        if (_playerBody == null)
        {
            _playerBody = transform;
        }

        if (_cameraTransform == null)
        {
            Camera camera = GetComponentInChildren<Camera>(true);
            if (camera != null)
            {
                _cameraTransform = camera.transform;
            }
        }
    }

    private void RotateView()
    {
        if (_cameraTransform == null || _playerBody == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -45f, 45f);

        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _playerBody.Rotate(Vector3.up * mouseX);
    }
}
