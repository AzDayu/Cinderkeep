using UnityEngine;
using UnityEngine.Serialization;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace OODong.Shared
{
    [RequireComponent(typeof(Camera))]
    public sealed class SceneCameraController : MonoBehaviour
    {
        [FormerlySerializedAs("_camera")]
        [SerializeField] private Camera Camera_Camera;
        [SerializeField] private float _keyboardMoveSpeed = 12f;
        [SerializeField] private float _dragMoveScale = 1f;
        [SerializeField] private float _scrollZoomSpeed = 1.5f;
        [SerializeField] private float _buttonMoveDistance = 2f;
        [SerializeField] private float _buttonZoomDistance = 2f;
        [SerializeField] private float _minOrthographicSize = 3f;
        [SerializeField] private float _maxOrthographicSize = 24f;
        [SerializeField] private bool _clampPosition;
        [SerializeField] private Vector2 _minPosition = new Vector2(-40f, -40f);
        [SerializeField] private Vector2 _maxPosition = new Vector2(40f, 40f);

        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;
        private float _defaultOrthographicSize;
        private float _defaultFieldOfView;
        private Vector2 _previousPointerPosition;
        private bool _isDragging;

        public Camera Camera => Camera_Camera;

        private void Awake()
        {
            if (Camera_Camera == null)
            {
                Camera_Camera = GetComponent<Camera>();
            }

            StoreDefaultView();
        }

        private void Update()
        {
            MoveFromKeyboard();
            MoveFromPointerDrag();
            ZoomFromScroll();
            ClampPosition();
        }

        public void MoveUp()
        {
            MoveByScreenDirection(Vector2.up, _buttonMoveDistance);
        }

        public void MoveDown()
        {
            MoveByScreenDirection(Vector2.down, _buttonMoveDistance);
        }

        public void MoveLeft()
        {
            MoveByScreenDirection(Vector2.left, _buttonMoveDistance);
        }

        public void MoveRight()
        {
            MoveByScreenDirection(Vector2.right, _buttonMoveDistance);
        }

        public void ZoomIn()
        {
            ZoomBy(_buttonZoomDistance);
        }

        public void ZoomOut()
        {
            ZoomBy(-_buttonZoomDistance);
        }

        public void ResetView()
        {
            transform.position = _defaultPosition;
            transform.rotation = _defaultRotation;

            if (Camera_Camera.orthographic)
            {
                Camera_Camera.orthographicSize = _defaultOrthographicSize;
            }
            else
            {
                Camera_Camera.fieldOfView = _defaultFieldOfView;
            }
        }

        public void StoreDefaultView()
        {
            _defaultPosition = transform.position;
            _defaultRotation = transform.rotation;

            if (Camera_Camera == null)
            {
                Camera_Camera = GetComponent<Camera>();
            }

            _defaultOrthographicSize = Camera_Camera.orthographicSize;
            _defaultFieldOfView = Camera_Camera.fieldOfView;
        }

        public void SetCamera(Camera targetCamera)
        {
            Camera_Camera = targetCamera;
            StoreDefaultView();
        }

        private void MoveFromKeyboard()
        {
            Vector2 direction = ReadKeyboardDirection();
            if (direction.sqrMagnitude <= 0f)
            {
                return;
            }

            MoveByScreenDirection(direction.normalized, _keyboardMoveSpeed * Time.unscaledDeltaTime);
        }

        private void MoveFromPointerDrag()
        {
            Vector2 pointerPosition = ReadPointerPosition();
            bool isDragPressed = IsPointerDragPressed();

            if (!isDragPressed)
            {
                _isDragging = false;
                _previousPointerPosition = pointerPosition;
                return;
            }

            if (!_isDragging)
            {
                _isDragging = true;
                _previousPointerPosition = pointerPosition;
                return;
            }

            Vector2 delta = pointerPosition - _previousPointerPosition;
            _previousPointerPosition = pointerPosition;
            if (delta.sqrMagnitude <= 0f)
            {
                return;
            }

            float worldUnitsPerPixel = GetWorldUnitsPerPixel();
            MoveByScreenDelta(-delta, worldUnitsPerPixel * _dragMoveScale);
        }

        private void ZoomFromScroll()
        {
            float scroll = ReadScrollValue();
            if (Mathf.Abs(scroll) <= 0.001f)
            {
                return;
            }

            ZoomBy(scroll * _scrollZoomSpeed);
        }

        private void MoveByScreenDirection(Vector2 direction, float distance)
        {
            MoveByScreenDelta(direction.normalized, distance);
        }

        private void MoveByScreenDelta(Vector2 direction, float distance)
        {
            Vector3 right = transform.right;
            Vector3 up = GetCameraUpDirection();
            Vector3 movement = (right * direction.x) + (up * direction.y);
            transform.position += movement * distance;
        }

        private Vector3 GetCameraUpDirection()
        {
            Vector3 projectedUp = Vector3.ProjectOnPlane(transform.up, Vector3.up);
            if (projectedUp.sqrMagnitude > 0.0001f)
            {
                return projectedUp.normalized;
            }

            return transform.up;
        }

        private float GetWorldUnitsPerPixel()
        {
            if (Camera_Camera != null && Camera_Camera.orthographic)
            {
                return (Camera_Camera.orthographicSize * 2f) / Mathf.Max(1f, Screen.height);
            }

            return 0.02f;
        }

        private void ZoomBy(float amount)
        {
            if (Camera_Camera == null)
            {
                return;
            }

            if (Camera_Camera.orthographic)
            {
                Camera_Camera.orthographicSize = Mathf.Clamp(
                    Camera_Camera.orthographicSize - amount,
                    _minOrthographicSize,
                    _maxOrthographicSize);
                return;
            }

            Camera_Camera.fieldOfView = Mathf.Clamp(Camera_Camera.fieldOfView - amount * 4f, 20f, 90f);
        }

        private void ClampPosition()
        {
            if (!_clampPosition)
            {
                return;
            }

            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, _minPosition.x, _maxPosition.x);
            position.y = Mathf.Clamp(position.y, _minPosition.y, _maxPosition.y);
            position.z = Mathf.Clamp(position.z, _minPosition.x, _maxPosition.x);
            transform.position = position;
        }

        private Vector2 ReadKeyboardDirection()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            float horizontal = 0f;
            float vertical = 0f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                horizontal -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                horizontal += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                vertical -= 1f;
            }

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                vertical += 1f;
            }

            return new Vector2(horizontal, vertical);
#else
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
        }

        private Vector2 ReadPointerPosition()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
            return Input.mousePosition;
#endif
        }

        private bool IsPointerDragPressed()
        {
#if ENABLE_INPUT_SYSTEM
            Mouse mouse = Mouse.current;
            return mouse != null && (mouse.rightButton.isPressed || mouse.middleButton.isPressed);
#else
            return Input.GetMouseButton(1) || Input.GetMouseButton(2);
#endif
        }

        private float ReadScrollValue()
        {
#if ENABLE_INPUT_SYSTEM
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return 0f;
            }

            return mouse.scroll.ReadValue().y * 0.01f;
#else
            return Input.mouseScrollDelta.y;
#endif
        }
    }
}
