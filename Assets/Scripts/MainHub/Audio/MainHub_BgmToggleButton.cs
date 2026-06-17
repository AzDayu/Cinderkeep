using UnityEngine;
using UnityEngine.UI;

namespace MainHub.Audio
{
    // Main_Lobby에서 BGM ON/OFF 버튼을 관리합니다.
    // 버튼 문구와 실제 BGM 컨트롤러 상태를 함께 맞춥니다.
    public sealed class MainHub_BgmToggleButton : MonoBehaviour
    {
        [SerializeField] private Button Button_Toggle;
        [SerializeField] private Text Text_Label;
        [SerializeField] private MainHub_BgmController MainHubBgmController_BgmController;

        private void Awake()
        {
            ResolveReferences();
            if (Button_Toggle != null)
            {
                Button_Toggle.onClick.AddListener(ToggleBgm);
            }
        }

        private void Start()
        {
            RefreshLabel();
        }

        private void OnDestroy()
        {
            if (Button_Toggle != null)
            {
                Button_Toggle.onClick.RemoveListener(ToggleBgm);
            }
        }

        public void SetReferences(Button toggleButton, Text label, MainHub_BgmController bgmController)
        {
            Button_Toggle = toggleButton;
            Text_Label = label;
            MainHubBgmController_BgmController = bgmController;
            RefreshLabel();
        }

        public void ToggleBgm()
        {
            ResolveReferences();
            MainHubBgmController_BgmController?.ToggleBgm();
            RefreshLabel();
        }

        public void RefreshLabel()
        {
            ResolveReferences();
            if (Text_Label == null)
            {
                return;
            }

            bool isEnabled = MainHubBgmController_BgmController == null || MainHubBgmController_BgmController.IsBgmEnabled;
            Text_Label.text = isEnabled ? "\u266a BGM ON" : "\u266a BGM OFF";
        }

        private void ResolveReferences()
        {
            if (Button_Toggle == null)
            {
                Button_Toggle = GetComponent<Button>();
            }

            if (Text_Label == null)
            {
                Text_Label = GetComponentInChildren<Text>(true);
            }

            if (MainHubBgmController_BgmController == null)
            {
                MainHubBgmController_BgmController = MainHub_BgmController.Instance;
            }
        }
    }
}
