using Cinderkeep.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cinderkeep.UI.Editor
{
    // Main_Lobby를 실제 게임용 메인 메뉴로 다시 만드는 편집기 도구입니다.
    // 런타임에서는 UI를 새로 만들지 않고, 이 도구가 미리 만들어 둔 씬 오브젝트를 사용합니다.
    public static class MainMenuSceneBuilder
    {
        private const string MainMenuScenePath = "Assets/Scenes/Main_Lobby.unity";
        private const string GameScenePath = "Assets/Scenes/MainGame/Cinderkeep_Game.unity";
        private const string BackgroundPath = "Assets/UI/MainMenu/MainMenu_Background.png";
        private const string KoreanFontPath = "Assets/Fonts/ChosunCentennial.ttf";

        [MenuItem("Cinderkeep/Main Menu/Rebuild Main Menu")]
        public static void RebuildMainMenuScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateEventSystem();
            CreateMainCamera();
            CreateMainMenuCanvas();
            SaveScene(scene);
            UpdateBuildSettings();
        }

        private static void CreateEventSystem()
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private static void CreateMainCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.02f, 0.025f, 0.03f, 1f);
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            cameraObject.tag = "MainCamera";
        }

        private static void CreateMainMenuCanvas()
        {
            GameObject canvasObject = new GameObject("Canvas_MainMenu", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasScaler.matchWidthOrHeight = 0.5f;

            RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
            Image background = CreateImage("Image_Background", canvasRect, new Color(0.04f, 0.05f, 0.065f, 1f));
            background.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
            background.preserveAspect = false;
            Stretch(background.rectTransform);

            RectTransform menuRoot = CreateRect("MainMenuRoot", canvasRect);
            menuRoot.anchorMin = new Vector2(0f, 0f);
            menuRoot.anchorMax = new Vector2(1f, 1f);
            menuRoot.offsetMin = Vector2.zero;
            menuRoot.offsetMax = Vector2.zero;

            Font font = AssetDatabase.LoadAssetAtPath<Font>(KoreanFontPath);
            CreateText("Text_Title", menuRoot, "Cinderkeep", 88, Color.white, TextAnchor.MiddleCenter, font, new Vector2(0.16f, 0.68f), new Vector2(0.84f, 0.84f));
            CreateText("Text_Subtitle", menuRoot, "불꽃 심장을 지키는 생존 방어 게임", 28, new Color(0.86f, 0.88f, 0.9f, 1f), TextAnchor.MiddleCenter, font, new Vector2(0.16f, 0.61f), new Vector2(0.84f, 0.68f));

            Button startButton = CreateButton("Button_StartGame", menuRoot, "게임 시작하기", new Color(0.2f, 0.48f, 0.32f, 0.96f), font, new Vector2(0.36f, 0.39f), new Vector2(0.64f, 0.47f));
            Button quitButton = CreateButton("Button_QuitGame", menuRoot, "게임 끝내기", new Color(0.46f, 0.16f, 0.16f, 0.96f), font, new Vector2(0.36f, 0.28f), new Vector2(0.64f, 0.36f));

            MainMenuController controller = menuRoot.gameObject.AddComponent<MainMenuController>();
            controller.SetReferences(startButton, quitButton);
            EditorUtility.SetDirty(controller);
        }

        private static RectTransform CreateRect(string objectName, Transform parent)
        {
            GameObject gameObject = new GameObject(objectName, typeof(RectTransform));
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.SetParent(parent, false);
            return rectTransform;
        }

        private static Image CreateImage(string objectName, Transform parent, Color color)
        {
            GameObject gameObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.SetParent(parent, false);

            Image image = gameObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string objectName, Transform parent, string text, int fontSize, Color color, TextAnchor alignment, Font font, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject gameObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.SetParent(parent, false);
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            Text label = gameObject.GetComponent<Text>();
            label.text = text;
            label.fontSize = fontSize;
            label.color = color;
            label.alignment = alignment;
            label.font = font;
            return label;
        }

        private static Button CreateButton(string objectName, Transform parent, string labelText, Color color, Font font, Vector2 anchorMin, Vector2 anchorMax)
        {
            Image image = CreateImage(objectName, parent, color);
            image.rectTransform.anchorMin = anchorMin;
            image.rectTransform.anchorMax = anchorMax;
            image.rectTransform.offsetMin = Vector2.zero;
            image.rectTransform.offsetMax = Vector2.zero;

            Button button = image.gameObject.AddComponent<Button>();
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = color;
            colorBlock.highlightedColor = new Color(color.r + 0.08f, color.g + 0.08f, color.b + 0.08f, color.a);
            colorBlock.pressedColor = new Color(color.r * 0.75f, color.g * 0.75f, color.b * 0.75f, color.a);
            button.colors = colorBlock;

            CreateText("Text_Label", image.transform, labelText, 34, Color.white, TextAnchor.MiddleCenter, font, Vector2.zero, Vector2.one);
            return button;
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static void SaveScene(Scene scene)
        {
            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
            AssetDatabase.SaveAssets();
        }

        private static void UpdateBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(GameScenePath, true)
            };
        }
    }
}
