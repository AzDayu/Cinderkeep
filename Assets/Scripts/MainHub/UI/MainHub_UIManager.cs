using MainHub.Shared;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainHub.UI
{
    public sealed class MainHub_UIManager : MonoBehaviour
    {
        [FormerlySerializedAs("_cameraController")]
        [SerializeField] private MainHub_SceneCameraController SceneCameraController_CameraController;
        [FormerlySerializedAs("_cameraHelpPanel")]
        [SerializeField] private GameObject GameObject_CameraHelpPanel;
        [SerializeField] private bool _hideCameraHelpOnAwake = true;

        public MainHub_SceneCameraController CameraController
        {
            get
            {
                return SceneCameraController_CameraController;
            }
        }

        private void Awake()
        {
            if (_hideCameraHelpOnAwake && GameObject_CameraHelpPanel != null)
            {
                GameObject_CameraHelpPanel.SetActive(false);
            }
        }

        public void SetCameraController(MainHub_SceneCameraController cameraController)
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
            SceneCameraController_CameraController?.MoveUp();
        }

        public void MoveCameraDown()
        {
            SceneCameraController_CameraController?.MoveDown();
        }

        public void MoveCameraLeft()
        {
            SceneCameraController_CameraController?.MoveLeft();
        }

        public void MoveCameraRight()
        {
            SceneCameraController_CameraController?.MoveRight();
        }

        public void ZoomCameraIn()
        {
            SceneCameraController_CameraController?.ZoomIn();
        }

        public void ZoomCameraOut()
        {
            SceneCameraController_CameraController?.ZoomOut();
        }

        public void ResetCameraView()
        {
            SceneCameraController_CameraController?.ResetView();
        }
    }
}
