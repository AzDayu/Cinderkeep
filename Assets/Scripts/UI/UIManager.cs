using OODong.Shared;
using UnityEngine;
using UnityEngine.Serialization;

namespace OODong.UI
{
    public sealed class UIManager : MonoBehaviour
    {
        [FormerlySerializedAs("_cameraController")]
        [SerializeField] private SceneCameraController SceneCameraController_CameraController;
        [FormerlySerializedAs("_cameraHelpPanel")]
        [SerializeField] private GameObject GameObject_CameraHelpPanel;
        [SerializeField] private bool _hideCameraHelpOnAwake = true;

        public SceneCameraController CameraController => SceneCameraController_CameraController;

        private void Awake()
        {
            ResolveCameraController();

            if (_hideCameraHelpOnAwake && GameObject_CameraHelpPanel != null)
            {
                GameObject_CameraHelpPanel.SetActive(false);
            }
        }

        public void SetCameraController(SceneCameraController cameraController)
        {
            SceneCameraController_CameraController = cameraController;
        }

        public void ShowCameraHelp()
        {
            if (GameObject_CameraHelpPanel != null)
            {
                GameObject_CameraHelpPanel.SetActive(true);
            }
        }

        public void HideCameraHelp()
        {
            if (GameObject_CameraHelpPanel != null)
            {
                GameObject_CameraHelpPanel.SetActive(false);
            }
        }

        public void ToggleCameraHelp()
        {
            if (GameObject_CameraHelpPanel != null)
            {
                GameObject_CameraHelpPanel.SetActive(!GameObject_CameraHelpPanel.activeSelf);
            }
        }

        public void MoveCameraUp()
        {
            ResolveCameraController();
            SceneCameraController_CameraController?.MoveUp();
        }

        public void MoveCameraDown()
        {
            ResolveCameraController();
            SceneCameraController_CameraController?.MoveDown();
        }

        public void MoveCameraLeft()
        {
            ResolveCameraController();
            SceneCameraController_CameraController?.MoveLeft();
        }

        public void MoveCameraRight()
        {
            ResolveCameraController();
            SceneCameraController_CameraController?.MoveRight();
        }

        public void ZoomCameraIn()
        {
            ResolveCameraController();
            SceneCameraController_CameraController?.ZoomIn();
        }

        public void ZoomCameraOut()
        {
            ResolveCameraController();
            SceneCameraController_CameraController?.ZoomOut();
        }

        public void ResetCameraView()
        {
            ResolveCameraController();
            SceneCameraController_CameraController?.ResetView();
        }

        private void ResolveCameraController()
        {
            if (SceneCameraController_CameraController != null)
            {
                return;
            }

            SceneCameraController_CameraController = FindFirstObjectByType<SceneCameraController>();
        }
    }
}
