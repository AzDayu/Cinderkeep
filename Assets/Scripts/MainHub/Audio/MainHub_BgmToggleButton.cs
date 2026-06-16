using UnityEngine;
using UnityEngine.UI;

namespace MainHub.Audio
{
    // Main_Lobby?먯꽌 BGM ON/OFF瑜??대떦?섎뒗 ?뉗? UI 而댄룷?뚰듃?낅땲??
    // ?ㅼ젣 ?ъ깮 ?뺤콉? MainHub_BgmController媛 媛吏怨? 踰꾪듉? ?붿껌留??꾨떖?⑸땲??
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
            Text_Label.text = isEnabled ? "BGM ON" : "BGM OFF";
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
