using OODong.Shared;
using UnityEngine;

namespace OODong.UI
{
    public sealed class UIManager : MonoBehaviour
    {
        [SerializeField] private SceneCameraController _cameraController;
        [SerializeField] private GameObject _cameraHelpPanel;
        [SerializeField] private bool _hideCameraHelpOnAwake = true;

        public SceneCameraController CameraController => _cameraController;

        private void Awake()
        {
            ResolveCameraController();

            if (_hideCameraHelpOnAwake && _cameraHelpPanel != null)
            {
                _cameraHelpPanel.SetActive(false);
            }
        }

        public void SetCameraController(SceneCameraController cameraController)
        {
            _cameraController = cameraController;
        }

        public void ShowCameraHelp()
        {
            if (_cameraHelpPanel != null)
            {
                _cameraHelpPanel.SetActive(true);
            }
        }

        public void HideCameraHelp()
        {
            if (_cameraHelpPanel != null)
            {
                _cameraHelpPanel.SetActive(false);
            }
        }

        public void ToggleCameraHelp()
        {
            if (_cameraHelpPanel != null)
            {
                _cameraHelpPanel.SetActive(!_cameraHelpPanel.activeSelf);
            }
        }

        public void MoveCameraUp()
        {
            ResolveCameraController();
            _cameraController?.MoveUp();
        }

        public void MoveCameraDown()
        {
            ResolveCameraController();
            _cameraController?.MoveDown();
        }

        public void MoveCameraLeft()
        {
            ResolveCameraController();
            _cameraController?.MoveLeft();
        }

        public void MoveCameraRight()
        {
            ResolveCameraController();
            _cameraController?.MoveRight();
        }

        public void ZoomCameraIn()
        {
            ResolveCameraController();
            _cameraController?.ZoomIn();
        }

        public void ZoomCameraOut()
        {
            ResolveCameraController();
            _cameraController?.ZoomOut();
        }

        public void ResetCameraView()
        {
            ResolveCameraController();
            _cameraController?.ResetView();
        }

        private void ResolveCameraController()
        {
            if (_cameraController != null)
            {
                return;
            }

            _cameraController = FindFirstObjectByType<SceneCameraController>();
        }
    }
}
